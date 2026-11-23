using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.Data
{
    /// <summary>
    /// File-backed implementation of ISchoolDataStore.
    /// Provides persistence for all equipment, borrowers, and slips via JSON storage.
    /// Serves as the primary persistence layer or a seamless fallback when SQL Server LocalDB is unavailable.
    /// </summary>
    public class FileSchoolDataStore : ISchoolDataStore
    {
        private class DataSchema
        {
            public List<EquipmentRecord> Equipment { get; set; } = new List<EquipmentRecord>();
            public List<BorrowerRecord> Borrowers { get; set; } = new List<BorrowerRecord>();
            public List<SlipRecord> Slips { get; set; } = new List<SlipRecord>();
        }

        private class EquipmentRecord
        {
            public string PropertyTag { get; set; }
            public string Type { get; set; }
            public string ItemName { get; set; }
            public string Condition { get; set; }
            public bool IsAvailable { get; set; }
            public bool IncludesRemote { get; set; }
            public bool IncludesPowerCord { get; set; }
            public bool IncludesHDMI { get; set; }
            public int BorrowDurationHours { get; set; }
            public DateTime? CalibrationDate { get; set; }
            public int ComponentCount { get; set; }
            public int BorrowDurationDays { get; set; }
        }

        private class BorrowerRecord
        {
            public string SchoolIDNumber { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string Department { get; set; }
            public bool HasClearanceHold { get; set; }
        }

        private class SlipRecord
        {
            public string SlipID { get; set; }
            public string BorrowerID { get; set; }
            public string PropertyTag { get; set; }
            public string RoomToBeUsed { get; set; }
            public DateTime TimeBorrowed { get; set; }
            public DateTime DueTime { get; set; }
            public DateTime? TimeReturned { get; set; }
            public string ReleasedByStaff { get; set; }
        }

        private readonly string _filePath;
        private DataSchema _cache = new DataSchema();
        private readonly object _lock = new object();

        public FileSchoolDataStore()
            : this("school_db_store.json")
        {
        }

        public FileSchoolDataStore(string filePath)
        {
            _filePath = string.IsNullOrEmpty(filePath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "school_db_store.json")
                : Path.GetFullPath(filePath);
        }

        public void Initialize()
        {
            lock (_lock)
            {
                if (File.Exists(_filePath))
                {
                    try
                    {
                        string json = File.ReadAllText(_filePath);
                        var data = JsonSerializer.Deserialize<DataSchema>(json);
                        if (data != null)
                        {
                            _cache = data;
                        }
                    }
                    catch
                    {
                        _cache = new DataSchema();
                    }
                }
                else
                {
                    _cache = new DataSchema();
                    PersistToFile();
                }
            }
        }

        public bool IsEmpty()
        {
            lock (_lock)
            {
                return _cache.Equipment.Count == 0 && _cache.Borrowers.Count == 0;
            }
        }

        public void SaveEquipment(SchoolEquipment item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            lock (_lock)
            {
                var record = new EquipmentRecord
                {
                    PropertyTag = item.PropertyTag,
                    ItemName = item.ItemName,
                    Condition = item.Condition,
                    IsAvailable = item.IsAvailable
                };

                if (item is AVEquipment av)
                {
                    record.Type = "AV";
                    record.IncludesRemote = av.IncludesRemote;
                    record.IncludesPowerCord = av.IncludesPowerCord;
                    record.IncludesHDMI = av.IncludesHDMI;
                    record.BorrowDurationHours = av.BorrowDurationHours;
                }
                else if (item is LabEquipment lab)
                {
                    record.Type = "LAB";
                    record.CalibrationDate = lab.CalibrationDate;
                    record.ComponentCount = lab.ComponentCount;
                    record.BorrowDurationDays = lab.BorrowDurationDays;
                }

                int idx = _cache.Equipment.FindIndex(e => string.Equals(e.PropertyTag, item.PropertyTag, StringComparison.OrdinalIgnoreCase));
                if (idx >= 0)
                {
                    _cache.Equipment[idx] = record;
                }
                else
                {
                    _cache.Equipment.Add(record);
                }

                PersistToFile();
            }
        }

        public void DeleteEquipment(string propertyTag)
        {
            if (string.IsNullOrWhiteSpace(propertyTag)) return;

            lock (_lock)
            {
                int count = _cache.Equipment.RemoveAll(e => string.Equals(e.PropertyTag, propertyTag, StringComparison.OrdinalIgnoreCase));
                if (count > 0)
                {
                    PersistToFile();
                }
            }
        }

        public List<SchoolEquipment> LoadEquipment()
        {
            lock (_lock)
            {
                var list = new List<SchoolEquipment>();
                foreach (var rec in _cache.Equipment)
                {
                    if (rec.Type == "AV")
                    {
                        list.Add(new AVEquipment(
                            rec.PropertyTag, rec.ItemName,
                            rec.IncludesRemote, rec.IncludesPowerCord, rec.IncludesHDMI,
                            rec.BorrowDurationHours > 0 ? rec.BorrowDurationHours : 4,
                            rec.Condition ?? "Good", rec.IsAvailable));
                    }
                    else if (rec.Type == "LAB")
                    {
                        list.Add(new LabEquipment(
                            rec.PropertyTag, rec.ItemName,
                            rec.CalibrationDate ?? DateTime.Now,
                            rec.ComponentCount > 0 ? rec.ComponentCount : 1,
                            rec.BorrowDurationDays > 0 ? rec.BorrowDurationDays : 7,
                            rec.Condition ?? "Good", rec.IsAvailable));
                    }
                }
                return list;
            }
        }

        public void SaveBorrower(SchoolBorrower borrower)
        {
            if (borrower == null) throw new ArgumentNullException(nameof(borrower));

            lock (_lock)
            {
                var record = new BorrowerRecord
                {
                    SchoolIDNumber = borrower.SchoolIDNumber,
                    FullName = borrower.FullName,
                    Role = borrower.Role,
                    Department = borrower.Department,
                    HasClearanceHold = borrower.HasClearanceHold
                };

                int idx = _cache.Borrowers.FindIndex(b => string.Equals(b.SchoolIDNumber, borrower.SchoolIDNumber, StringComparison.OrdinalIgnoreCase));
                if (idx >= 0)
                {
                    _cache.Borrowers[idx] = record;
                }
                else
                {
                    _cache.Borrowers.Add(record);
                }

                PersistToFile();
            }
        }

        public void DeleteBorrower(string schoolIdNumber)
        {
            if (string.IsNullOrWhiteSpace(schoolIdNumber)) return;

            lock (_lock)
            {
                int count = _cache.Borrowers.RemoveAll(b => string.Equals(b.SchoolIDNumber, schoolIdNumber, StringComparison.OrdinalIgnoreCase));
                if (count > 0)
                {
                    PersistToFile();
                }
            }
        }

        public List<SchoolBorrower> LoadBorrowers()
        {
            lock (_lock)
            {
                var list = new List<SchoolBorrower>();
                foreach (var b in _cache.Borrowers)
                {
                    list.Add(new SchoolBorrower(b.SchoolIDNumber, b.FullName, b.Role, b.Department, b.HasClearanceHold));
                }
                return list;
            }
        }

        public void SaveSlip(BorrowSlip slip)
        {
            if (slip == null) throw new ArgumentNullException(nameof(slip));

            lock (_lock)
            {
                SaveBorrower(slip.Borrower);
                SaveEquipment(slip.Equipment);

                var record = new SlipRecord
                {
                    SlipID = slip.SlipID,
                    BorrowerID = slip.Borrower.SchoolIDNumber,
                    PropertyTag = slip.Equipment.PropertyTag,
                    RoomToBeUsed = slip.RoomToBeUsed,
                    TimeBorrowed = slip.TimeBorrowed,
                    DueTime = slip.DueTime,
                    TimeReturned = slip.TimeReturned,
                    ReleasedByStaff = slip.ReleasedByStaff
                };

                int idx = _cache.Slips.FindIndex(s => string.Equals(s.SlipID, slip.SlipID, StringComparison.OrdinalIgnoreCase));
                if (idx >= 0)
                {
                    _cache.Slips[idx] = record;
                }
                else
                {
                    _cache.Slips.Add(record);
                }

                PersistToFile();
            }
        }

        public List<BorrowSlip> LoadSlips(IList<SchoolBorrower> borrowers, IList<SchoolEquipment> equipment)
        {
            lock (_lock)
            {
                var borrowerMap = new Dictionary<string, SchoolBorrower>(StringComparer.OrdinalIgnoreCase);
                foreach (var b in borrowers) borrowerMap[b.SchoolIDNumber] = b;

                var equipMap = new Dictionary<string, SchoolEquipment>(StringComparer.OrdinalIgnoreCase);
                foreach (var e in equipment) equipMap[e.PropertyTag] = e;

                var list = new List<BorrowSlip>();
                foreach (var rec in _cache.Slips)
                {
                    if (borrowerMap.TryGetValue(rec.BorrowerID, out var b) && equipMap.TryGetValue(rec.PropertyTag, out var e))
                    {
                        var slip = new BorrowSlip(rec.SlipID, b, e, rec.RoomToBeUsed, rec.TimeBorrowed, rec.DueTime, rec.ReleasedByStaff)
                        {
                            TimeReturned = rec.TimeReturned
                        };
                        list.Add(slip);
                    }
                }
                return list;
            }
        }

        private void PersistToFile()
        {
            try
            {
                string dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_cache, options);
                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Suppress background disk I/O errors
            }
        }
    }
}
