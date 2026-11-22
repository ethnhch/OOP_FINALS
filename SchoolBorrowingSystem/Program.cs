using System;
using System.Windows.Forms;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;
using SchoolBorrowingSystem.UI;

namespace SchoolBorrowingSystem
{
    public class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            // "SchoolBorrowingSystem.exe --test" runs the automated tests instead of the window
            if (args.Length > 0 && (args[0] == "--test" || args[0] == "-t"))
            {
                bool success = TestRunner.RunAllTests();
                return success ? 0 : 1;
            }

            EquipmentController equipment = new EquipmentController("school_inventory.txt");
            BorrowingController borrowing = new BorrowingController(equipment);
            BorrowerController borrowers = new BorrowerController();

            AddSampleData(equipment, borrowing, borrowers);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(equipment, borrowing, borrowers));
            return 0;
        }

        /// <summary>
        /// Some starting data so the app isn't empty when you open it.
        /// </summary>
        private static void AddSampleData(EquipmentController equipment, BorrowingController borrowing, BorrowerController borrowers)
        {
            SchoolBorrower juan = new SchoolBorrower("2024-10001", "Juan Dela Cruz", "Student", "Computer Science");
            SchoolBorrower maria = new SchoolBorrower("2024-10002", "Maria Santos", "Student", "Biology");
            SchoolBorrower reyes = new SchoolBorrower("FAC-5001", "Prof. Jose Reyes", "Faculty", "Physics");
            SchoolBorrower andres = new SchoolBorrower("2024-10099", "Andres Bautista", "Student", "Engineering", true);
            SchoolBorrower ana = new SchoolBorrower("STF-3001", "Ana Villanueva", "Staff", "Registrar");

            borrowers.AddBorrower(juan);
            borrowers.AddBorrower(maria);
            borrowers.AddBorrower(reyes);
            borrowers.AddBorrower(andres);
            borrowers.AddBorrower(ana);

            equipment.AddEquipment(new AVEquipment("AV-101", "Epson Full-HD Laser Projector", true, true, true, 4));
            equipment.AddEquipment(new AVEquipment("AV-102", "Sony Conference Speakerphone", false, true, false, 2));
            equipment.AddEquipment(new AVEquipment("AV-103", "Logitech Wireless Presenter", false, false, false, 3));
            equipment.AddEquipment(new AVEquipment("AV-104", "Samsung 55\" Smart Display", true, true, true, 6));
            equipment.AddEquipment(new LabEquipment("LAB-201", "Olympus CX23 Microscope", DateTime.Now.AddMonths(-1), 4, 7));
            equipment.AddEquipment(new LabEquipment("LAB-202", "Tektronix Digital Oscilloscope", DateTime.Now.AddMonths(-3), 3, 5));
            equipment.AddEquipment(new LabEquipment("LAB-203", "Vernier pH Sensor Kit", DateTime.Now.AddMonths(-2), 5, 3));

            // Two loans that are already out, so the Dashboard and Return pages have something to show
            borrowing.ProcessBorrow(reyes, equipment.FindEquipmentByTag("LAB-202"), "Physics Lab 2");

            BorrowSlip lateSlip = borrowing.ProcessBorrow(maria, equipment.FindEquipmentByTag("AV-102"), "Room 210");
            lateSlip.TimeBorrowed = DateTime.Now.AddHours(-5);
            lateSlip.DueTime = DateTime.Now.AddHours(-3); // pretend it was due 3 hours ago
        }
    }
}
