using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.UI;

namespace SchoolBorrowingSystem
{
    public class Program
    {
        private static EquipmentController _equipmentController;
        private static BorrowingController _borrowingController;
        private static List<SchoolBorrower> _borrowers;

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
            Application.Run(new MainForm(_equipmentController, _borrowingController, _borrowers));
            return 0;
        }

        private static void InitializeSeedData()
        {
            _equipmentController = new EquipmentController("school_inventory.txt");
            _borrowingController = new BorrowingController(_equipmentController);
            _borrowers = new List<SchoolBorrower>();

            _borrowers.Add(new SchoolBorrower("2024-10001", "Emma Watson", "Student", "Computer Science", false));
            _borrowers.Add(new SchoolBorrower("2024-10002", "Liam Hemsworth", "Student", "Biology", false));
            _borrowers.Add(new SchoolBorrower("FAC-5001", "Prof. Charles Xavier", "Faculty", "Physics", false));
            _borrowers.Add(new SchoolBorrower("2024-10099", "Bad Standing Student", "Student", "Engineering", true));

            _equipmentController.AddEquipment(new AVEquipment("AV-101", "Epson Full-HD Laser Projector", true, true, true, 4));
            _equipmentController.AddEquipment(new AVEquipment("AV-102", "Sony Conference Speakerphone", false, true, false, 2));
            _equipmentController.AddEquipment(new LabEquipment("LAB-201", "Olympus CX23 Microscope", DateTime.Now.AddMonths(-1), 4, 7));
            _equipmentController.AddEquipment(new LabEquipment("LAB-202", "Tektronix Digital Oscilloscope", DateTime.Now.AddMonths(-3), 3, 5));
        }
    }
}
