using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Data;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Models.UI;
using SchoolBorrowingSystem.UI;

namespace SchoolBorrowingSystem
{
    public class Program
    {
        private static EquipmentController _equipmentController;
        private static BorrowingController _borrowingController;
        private static List<SchoolBorrower> _borrowers;
        private static ISchoolDataStore _dataStore;

        [STAThread]
        public static int Main(string[] args)
        {
            string firstArg = args != null ? args.FirstOrDefault() : null;
            if (firstArg == "--test" || firstArg == "-t")
            {
                bool success = TestRunner.RunAllTests();
                return success ? 0 : 1;
            }

            InitializeSeedData();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var loadingScreen = new GtaLoadingForm())
            {
                loadingScreen.ShowDialog();
            }

            Application.Run(new MainForm(_equipmentController, _borrowingController, _borrowers, _dataStore));
            return 0;
        }

        private static void InitializeSeedData()
        {
            _dataStore = CreateDataStore();
            _equipmentController = new EquipmentController("school_inventory.txt", _dataStore);
            _borrowingController = new BorrowingController(_equipmentController, _dataStore);
            _borrowers = new List<SchoolBorrower>();

            if (_dataStore != null)
            {
                try
                {
                    if (_dataStore.IsEmpty())
                    {
                        SeedInitialData();
                    }
                    else
                    {
                        var persistedEquipment = _dataStore.LoadEquipment();
                        _equipmentController.RestoreInventory(persistedEquipment);

                        var persistedBorrowers = _dataStore.LoadBorrowers();
                        foreach (var b in persistedBorrowers)
                        {
                            _borrowers.Add(b);
                        }

                        var persistedSlips = _dataStore.LoadSlips(_borrowers, _equipmentController.GetAllEquipment());
                        _borrowingController.RestoreSlips(persistedSlips);
                    }
                }
                catch
                {
                    SeedInitialData();
                }
            }
            else
            {
                SeedInitialData();
            }
        }

        private static ISchoolDataStore CreateDataStore()
        {
            try
            {
                var sqlStore = new SqlServerSchoolDataStore();
                sqlStore.Initialize();
                return sqlStore;
            }
            catch
            {
                try
                {
                    var fileStore = new FileSchoolDataStore("school_database.json");
                    fileStore.Initialize();
                    return fileStore;
                }
                catch
                {
                    return new InMemorySchoolDataStore();
                }
            }
        }

        private static void SeedInitialData()
        {
            _borrowers.Clear();
            _borrowers.Add(new SchoolBorrower("2024-10001", "Emma Watson", "Student", "Computer Science", false));
            _borrowers.Add(new SchoolBorrower("2024-10002", "Liam Hemsworth", "Student", "Biology", false));
            _borrowers.Add(new SchoolBorrower("FAC-5001", "Prof. Charles Xavier", "Faculty", "Physics", false));
            _borrowers.Add(new SchoolBorrower("2024-10099", "Bad Standing Student", "Student", "Engineering", true));

            if (_dataStore != null)
            {
                foreach (var b in _borrowers)
                {
                    _dataStore.SaveBorrower(b);
                }
            }

            var eq1 = new AVEquipment("AV-101", "Epson Full-HD Laser Projector", true, true, true, 4);
            var eq2 = new AVEquipment("AV-102", "Sony Conference Speakerphone", false, true, false, 2);
            var eq3 = new LabEquipment("LAB-201", "Olympus CX23 Microscope", DateTime.Now.AddMonths(-1), 4, 7);
            var eq4 = new LabEquipment("LAB-202", "Tektronix Digital Oscilloscope", DateTime.Now.AddMonths(-3), 3, 5);

            _equipmentController.AddEquipment(eq1);
            _equipmentController.AddEquipment(eq2);
            _equipmentController.AddEquipment(eq3);
            _equipmentController.AddEquipment(eq4);
        }
    }
}
