using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.Data
{
    /// <summary>
    /// SQL Server LocalDB implementation of ISchoolDataStore, using a Visual Studio
    /// "Service-based Database" (the SchoolDB.mdf file added to the project).
    /// </summary>
    public class SqlServerSchoolDataStore : ISchoolDataStore
    {
        public const string DefaultFileName = "SchoolDB.mdf";

        private static readonly string[] CreateTableSql = new string[]
        {
@"IF OBJECT_ID(N'dbo.Equipment', N'U') IS NULL
CREATE TABLE dbo.Equipment (
    SeqNo               INT IDENTITY(1,1) NOT NULL,
    PropertyTag         NVARCHAR(50)  NOT NULL PRIMARY KEY,
    EquipmentType       NVARCHAR(3)   NOT NULL CHECK (EquipmentType IN (N'AV', N'LAB')),
    ItemName            NVARCHAR(200) NOT NULL,
    ItemCondition       NVARCHAR(50)  NOT NULL,
    IsAvailable         BIT           NOT NULL,
    IncludesRemote      BIT           NULL,
    IncludesPowerCord   BIT           NULL,
    IncludesHDMI        BIT           NULL,
    BorrowDurationHours INT           NULL,
    CalibrationDate     DATETIME2(0)  NULL,
    ComponentCount      INT           NULL,
    BorrowDurationDays  INT           NULL
);",
@"IF OBJECT_ID(N'dbo.Borrowers', N'U') IS NULL
CREATE TABLE dbo.Borrowers (
    SeqNo            INT IDENTITY(1,1) NOT NULL,
    SchoolIDNumber   NVARCHAR(50)  NOT NULL PRIMARY KEY,
    FullName         NVARCHAR(200) NOT NULL,
    Role             NVARCHAR(50)  NOT NULL,
    Department       NVARCHAR(200) NOT NULL,
    HasClearanceHold BIT           NOT NULL
);",
@"IF OBJECT_ID(N'dbo.BorrowSlips', N'U') IS NULL
CREATE TABLE dbo.BorrowSlips (
    SeqNo           INT IDENTITY(1,1) NOT NULL,
    SlipID          NVARCHAR(30)  NOT NULL PRIMARY KEY,
    BorrowerID      NVARCHAR(50)  NOT NULL REFERENCES dbo.Borrowers(SchoolIDNumber),
    PropertyTag     NVARCHAR(50)  NOT NULL REFERENCES dbo.Equipment(PropertyTag),
    RoomToBeUsed    NVARCHAR(200) NOT NULL,
    TimeBorrowed    DATETIME2(0)  NOT NULL,
    DueTime         DATETIME2(0)  NOT NULL,
    TimeReturned    DATETIME2(0)  NULL,
    ReleasedByStaff NVARCHAR(100) NOT NULL
);"
        };

        // "Upsert" = update the row if it exists, otherwise insert it.
        private const string UpsertEquipmentSql = @"
UPDATE dbo.Equipment SET
    EquipmentType = @type, ItemName = @name, ItemCondition = @cond, IsAvailable = @avail,
    IncludesRemote = @remote, IncludesPowerCord = @cord, IncludesHDMI = @hdmi, BorrowDurationHours = @hours,
    CalibrationDate = @cal, ComponentCount = @comp, BorrowDurationDays = @days
WHERE PropertyTag = @tag;
IF @@ROWCOUNT = 0
    INSERT INTO dbo.Equipment (PropertyTag, EquipmentType, ItemName, ItemCondition, IsAvailable,
        IncludesRemote, IncludesPowerCord, IncludesHDMI, BorrowDurationHours,
        CalibrationDate, ComponentCount, BorrowDurationDays)
    VALUES (@tag, @type, @name, @cond, @avail, @remote, @cord, @hdmi, @hours, @cal, @comp, @days);";

        private const string UpsertBorrowerSql = @"
UPDATE dbo.Borrowers SET
    FullName = @name, Role = @role, Department = @dept, HasClearanceHold = @hold
WHERE SchoolIDNumber = @id;
IF @@ROWCOUNT = 0
    INSERT INTO dbo.Borrowers (SchoolIDNumber, FullName, Role, Department, HasClearanceHold)
    VALUES (@id, @name, @role, @dept, @hold);";

        private const string UpsertSlipSql = @"
UPDATE dbo.BorrowSlips SET
    BorrowerID = @bid, PropertyTag = @tag, RoomToBeUsed = @room, TimeBorrowed = @borrowed,
    DueTime = @due, TimeReturned = @returned, ReleasedByStaff = @staff
WHERE SlipID = @id;
IF @@ROWCOUNT = 0
    INSERT INTO dbo.BorrowSlips (SlipID, BorrowerID, PropertyTag, RoomToBeUsed, TimeBorrowed, DueTime, TimeReturned, ReleasedByStaff)
    VALUES (@id, @bid, @tag, @room, @borrowed, @due, @returned, @staff);";

        private readonly string _connectionString;

        /// <summary>Full path of the .mdf file this store connects to.</summary>
        public string DatabaseFilePath { get; private set; }

        /// <summary>Uses SchoolDB.mdf next to the running program (bin\Debug\...).</summary>
        public SqlServerSchoolDataStore()
            : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultFileName))
        {
        }

        public SqlServerSchoolDataStore(string mdfFilePath)
        {
            if (string.IsNullOrEmpty(mdfFilePath))
            {
                throw new ArgumentException("Database file path cannot be empty.", "mdfFilePath");
            }

            DatabaseFilePath = Path.GetFullPath(mdfFilePath);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(LocalDB)\MSSQLLocalDB";
            builder.AttachDBFilename = DatabaseFilePath;
            builder.IntegratedSecurity = true;
            builder.ConnectTimeout = 30;

            // LocalDB has no certificate, so encryption is turned off for the local connection.
            _connectionString = builder.ConnectionString + ";Encrypt=False";
        }

        public void Initialize()
        {
            if (!File.Exists(DatabaseFilePath))
            {
                throw new FileNotFoundException(
                    "The service-based database file was not found. Add a 'Service-based Database' named '" +
                    DefaultFileName + "' to the project and set 'Copy to Output Directory' to 'Copy if newer'.",
                    DatabaseFilePath);
            }

            using (SqlConnection conn = OpenConnection())
            {
                for (int i = 0; i < CreateTableSql.Length; i++)
                {
                    using (SqlCommand cmd = CreateCommand(conn, null, CreateTableSql[i]))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public bool IsEmpty()
        {
            using (SqlConnection conn = OpenConnection())
            using (SqlCommand cmd = CreateCommand(conn, null,
                "SELECT (SELECT COUNT(*) FROM dbo.Equipment) + (SELECT COUNT(*) FROM dbo.Borrowers);"))
            {
                return Convert.ToInt64(cmd.ExecuteScalar()) == 0;
            }
        }

        // ---------------------------------------------------------------- Equipment

        public void SaveEquipment(SchoolEquipment item)
        {
            using (SqlConnection conn = OpenConnection())
            {
                UpsertEquipment(conn, null, item);
            }
        }

        public List<SchoolEquipment> LoadEquipment()
        {
            List<SchoolEquipment> items = new List<SchoolEquipment>();

            using (SqlConnection conn = OpenConnection())
            using (SqlCommand cmd = CreateCommand(conn, null, "SELECT * FROM dbo.Equipment ORDER BY SeqNo;"))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    string tag = GetText(r, "PropertyTag");
                    string type = GetText(r, "EquipmentType");
                    string name = GetText(r, "ItemName");
                    string condition = GetText(r, "ItemCondition");
                    bool available = GetBool(r, "IsAvailable", true);

                    if (type == "AV")
                    {
                        items.Add(new AVEquipment(
                            tag, name,
                            GetBool(r, "IncludesRemote", false),
                            GetBool(r, "IncludesPowerCord", true),
                            GetBool(r, "IncludesHDMI", true),
                            GetInt(r, "BorrowDurationHours", 4),
                            condition, available));
                    }
                    else if (type == "LAB")
                    {
                        int calOrdinal = r.GetOrdinal("CalibrationDate");
                        DateTime calibration = r.IsDBNull(calOrdinal) ? DateTime.Now : r.GetDateTime(calOrdinal);

                        items.Add(new LabEquipment(
                            tag, name, calibration,
                            GetInt(r, "ComponentCount", 1),
                            GetInt(r, "BorrowDurationDays", 7),
                            condition, available));
                    }
                }
            }

            return items;
        }

        // ---------------------------------------------------------------- Borrowers

        public void SaveBorrower(SchoolBorrower borrower)
        {
            using (SqlConnection conn = OpenConnection())
            {
                UpsertBorrower(conn, null, borrower);
            }
        }

        public List<SchoolBorrower> LoadBorrowers()
        {
            List<SchoolBorrower> borrowers = new List<SchoolBorrower>();

            using (SqlConnection conn = OpenConnection())
            using (SqlCommand cmd = CreateCommand(conn, null, "SELECT * FROM dbo.Borrowers ORDER BY SeqNo;"))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    borrowers.Add(new SchoolBorrower(
                        GetText(r, "SchoolIDNumber"),
                        GetText(r, "FullName"),
                        GetText(r, "Role"),
                        GetText(r, "Department"),
                        GetBool(r, "HasClearanceHold", false)));
                }
            }

            return borrowers;
        }

        // ---------------------------------------------------------------- Slips

        public void SaveSlip(BorrowSlip slip)
        {
            if (slip == null)
            {
                throw new ArgumentNullException("slip", "Borrow slip cannot be null.");
            }

            using (SqlConnection conn = OpenConnection())
            using (SqlTransaction tx = conn.BeginTransaction())
            {
                // Parents first (foreign keys), then the slip. All or nothing.
                UpsertBorrower(conn, tx, slip.Borrower);
                UpsertEquipment(conn, tx, slip.Equipment);

                using (SqlCommand cmd = CreateCommand(conn, tx, UpsertSlipSql))
                {
                    Bind(cmd, "@id", slip.SlipID);
                    Bind(cmd, "@bid", slip.Borrower.SchoolIDNumber);
                    Bind(cmd, "@tag", slip.Equipment.PropertyTag);
                    Bind(cmd, "@room", slip.RoomToBeUsed ?? string.Empty);
                    Bind(cmd, "@borrowed", slip.TimeBorrowed);
                    Bind(cmd, "@due", slip.DueTime);
                    Bind(cmd, "@returned", slip.TimeReturned.HasValue ? (object)slip.TimeReturned.Value : null);
                    Bind(cmd, "@staff", slip.ReleasedByStaff ?? string.Empty);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
        }

        public List<BorrowSlip> LoadSlips(IList<SchoolBorrower> borrowers, IList<SchoolEquipment> equipment)
        {
            Dictionary<string, SchoolBorrower> borrowerById =
                new Dictionary<string, SchoolBorrower>(StringComparer.OrdinalIgnoreCase);
            foreach (SchoolBorrower b in borrowers)
            {
                borrowerById[b.SchoolIDNumber] = b;
            }

            Dictionary<string, SchoolEquipment> equipmentByTag =
                new Dictionary<string, SchoolEquipment>(StringComparer.OrdinalIgnoreCase);
            foreach (SchoolEquipment e in equipment)
            {
                equipmentByTag[e.PropertyTag] = e;
            }

            List<BorrowSlip> slips = new List<BorrowSlip>();

            using (SqlConnection conn = OpenConnection())
            using (SqlCommand cmd = CreateCommand(conn, null, "SELECT * FROM dbo.BorrowSlips ORDER BY SeqNo;"))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    SchoolBorrower borrower;
                    SchoolEquipment item;

                    if (!borrowerById.TryGetValue(GetText(r, "BorrowerID"), out borrower) ||
                        !equipmentByTag.TryGetValue(GetText(r, "PropertyTag"), out item))
                    {
                        continue; // orphan row, skip it
                    }

                    BorrowSlip slip = new BorrowSlip(
                        GetText(r, "SlipID"),
                        borrower,
                        item,
                        GetText(r, "RoomToBeUsed"),
                        r.GetDateTime(r.GetOrdinal("TimeBorrowed")),
                        r.GetDateTime(r.GetOrdinal("DueTime")),
                        GetText(r, "ReleasedByStaff"));

                    int returnedOrdinal = r.GetOrdinal("TimeReturned");
                    if (!r.IsDBNull(returnedOrdinal))
                    {
                        slip.TimeReturned = r.GetDateTime(returnedOrdinal);
                    }

                    slips.Add(slip);
                }
            }

            return slips;
        }

        // ---------------------------------------------------------------- Helpers

        private SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private static SqlCommand CreateCommand(SqlConnection conn, SqlTransaction tx, string sql)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = tx;
            return cmd;
        }

        private static void Bind(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private static void UpsertEquipment(SqlConnection conn, SqlTransaction tx, SchoolEquipment item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Equipment cannot be null.");
            }

            string type;
            object remote = null, cord = null, hdmi = null, hours = null;
            object calibration = null, components = null, days = null;

            // The only place that must know about each equipment subclass.
            // Adding a new subclass later = add one more branch here (and in LoadEquipment).
            AVEquipment av = item as AVEquipment;
            LabEquipment lab = item as LabEquipment;

            if (av != null)
            {
                type = "AV";
                remote = av.IncludesRemote;
                cord = av.IncludesPowerCord;
                hdmi = av.IncludesHDMI;
                hours = av.BorrowDurationHours;
            }
            else if (lab != null)
            {
                type = "LAB";
                calibration = lab.CalibrationDate;
                components = lab.ComponentCount;
                days = lab.BorrowDurationDays;
            }
            else
            {
                throw new NotSupportedException(
                    "Equipment type '" + item.GetType().Name + "' has no database mapping yet.");
            }

            using (SqlCommand cmd = CreateCommand(conn, tx, UpsertEquipmentSql))
            {
                Bind(cmd, "@tag", item.PropertyTag);
                Bind(cmd, "@type", type);
                Bind(cmd, "@name", item.ItemName ?? string.Empty);
                Bind(cmd, "@cond", item.Condition ?? "Good");
                Bind(cmd, "@avail", item.IsAvailable);
                Bind(cmd, "@remote", remote);
                Bind(cmd, "@cord", cord);
                Bind(cmd, "@hdmi", hdmi);
                Bind(cmd, "@hours", hours);
                Bind(cmd, "@cal", calibration);
                Bind(cmd, "@comp", components);
                Bind(cmd, "@days", days);
                cmd.ExecuteNonQuery();
            }
        }

        private static void UpsertBorrower(SqlConnection conn, SqlTransaction tx, SchoolBorrower borrower)
        {
            if (borrower == null)
            {
                throw new ArgumentNullException("borrower", "Borrower cannot be null.");
            }

            using (SqlCommand cmd = CreateCommand(conn, tx, UpsertBorrowerSql))
            {
                Bind(cmd, "@id", borrower.SchoolIDNumber);
                Bind(cmd, "@name", borrower.FullName ?? string.Empty);
                Bind(cmd, "@role", borrower.Role ?? "Student");
                Bind(cmd, "@dept", borrower.Department ?? string.Empty);
                Bind(cmd, "@hold", borrower.HasClearanceHold);
                cmd.ExecuteNonQuery();
            }
        }

        private static string GetText(SqlDataReader r, string column)
        {
            return r.GetString(r.GetOrdinal(column));
        }

        private static bool GetBool(SqlDataReader r, string column, bool fallback)
        {
            int ordinal = r.GetOrdinal(column);
            return r.IsDBNull(ordinal) ? fallback : r.GetBoolean(ordinal);
        }

        private static int GetInt(SqlDataReader r, string column, int fallback)
        {
            int ordinal = r.GetOrdinal(column);
            return r.IsDBNull(ordinal) ? fallback : r.GetInt32(ordinal);
        }
    }
}
