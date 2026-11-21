using System;
using System.Collections.Generic;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem
{
    /// <summary>
    /// Visual Studio Console Application Entry Point.
    /// Provides an interactive management dashboard and integration test execution.
    /// </summary>
    public class Program
    {
        private static EquipmentController _equipmentController;
        private static BorrowingController _borrowingController;
        private static List<SchoolBorrower> _borrowers;

        public static int Main(string[] args)
        {
            Console.Title = "School Equipment Borrowing System - Visual Studio";

            // If command line asks specifically for tests or non-interactive mode
            if (args != null && args.Length > 0 && (args[0] == "--test" || args[0] == "-t"))
            {
                bool success = TestRunner.RunAllTests();
                return success ? 0 : 1;
            }

            InitializeSeedData();

            // Run automated QA suite first to demonstrate core logic integrity
            bool qaSuccess = TestRunner.RunAllTests();
            Console.WriteLine();
            Console.WriteLine("Press any key to enter the Interactive System Dashboard (or Ctrl+C to exit)...");
            
            // If in non-interactive environment, exit cleanly with QA result
            if (Console.IsInputRedirected)
            {
                return qaSuccess ? 0 : 1;
            }

            try
            {
                Console.ReadKey(true);
            }
            catch
            {
                return qaSuccess ? 0 : 1;
            }

            RunInteractiveMenu();
            return 0;
        }

        private static void InitializeSeedData()
        {
            _equipmentController = new EquipmentController("school_inventory.txt");
            _borrowingController = new BorrowingController(_equipmentController);
            _borrowers = new List<SchoolBorrower>();

            // Seed Sample Borrowers
            _borrowers.Add(new SchoolBorrower("2024-10001", "Emma Watson", "Student", "Computer Science", false));
            _borrowers.Add(new SchoolBorrower("2024-10002", "Liam Hemsworth", "Student", "Biology", false));
            _borrowers.Add(new SchoolBorrower("FAC-5001", "Prof. Charles Xavier", "Faculty", "Physics", false));
            _borrowers.Add(new SchoolBorrower("2024-10099", "Bad Standing Student", "Student", "Engineering", true));

            // Seed Sample Equipment (AV & Lab)
            _equipmentController.AddEquipment(new AVEquipment("AV-101", "Epson Full-HD Laser Projector", true, true, true, 4));
            _equipmentController.AddEquipment(new AVEquipment("AV-102", "Sony Conference Speakerphone", false, true, false, 2));
            _equipmentController.AddEquipment(new LabEquipment("LAB-201", "Olympus CX23 Microscope", DateTime.Now.AddMonths(-1), 4, 7));
            _equipmentController.AddEquipment(new LabEquipment("LAB-202", "Tektronix Digital Oscilloscope", DateTime.Now.AddMonths(-3), 3, 5));
        }

        private static void RunInteractiveMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("====================================================================");
                Console.WriteLine("         SCHOOL EQUIPMENT BORROWING MANAGEMENT SYSTEM (C#)          ");
                Console.WriteLine("              Visual Studio Ready | Object-Oriented                 ");
                Console.WriteLine("====================================================================");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine(" [1] View Available Equipment");
                Console.WriteLine(" [2] View All Equipment (with Conditions & Availability)");
                Console.WriteLine(" [3] View Registered School Borrowers");
                Console.WriteLine(" [4] Process New Borrow Request");
                Console.WriteLine(" [5] Process Equipment Return (Condition & Inspection Check)");
                Console.WriteLine(" [6] View Active Borrow Slips");
                Console.WriteLine(" [7] Mark Equipment for Repair");
                Console.WriteLine(" [8] Apply / Lift Borrower Clearance Hold");
                Console.WriteLine(" [9] Save Inventory to File");
                Console.WriteLine(" [T] Run Automated QA Unit & Integration Tests");
                Console.WriteLine(" [0] Exit Application");
                Console.WriteLine();
                Console.Write("Enter your selection: ");

                string choice = Console.ReadLine();
                if (choice == null) break;
                choice = choice.Trim().ToUpper();

                Console.WriteLine();
                switch (choice)
                {
                    case "1":
                        ShowAvailableEquipment();
                        break;
                    case "2":
                        ShowAllEquipment();
                        break;
                    case "3":
                        ShowBorrowers();
                        break;
                    case "4":
                        InteractiveBorrow();
                        break;
                    case "5":
                        InteractiveReturn();
                        break;
                    case "6":
                        ShowActiveSlips();
                        break;
                    case "7":
                        InteractiveMarkRepair();
                        break;
                    case "8":
                        InteractiveClearanceHold();
                        break;
                    case "9":
                        _equipmentController.SaveInventory();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(">> Inventory successfully saved to 'school_inventory.txt'.");
                        Console.ResetColor();
                        break;
                    case "T":
                        TestRunner.RunAllTests();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Exiting School Equipment Borrowing System. Goodbye!");
                        continue;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">> Invalid selection. Please choose an option from the menu.");
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Press Enter to return to main menu...");
                Console.ReadLine();
            }
        }

        private static void ShowAvailableEquipment()
        {
            Console.WriteLine("--- Available School Equipment ---");
            List<SchoolEquipment> available = _equipmentController.GetAvailableEquipment();
            if (available.Count == 0)
            {
                Console.WriteLine("No equipment is currently available for borrowing.");
                return;
            }

            for (int i = 0; i < available.Count; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, available[i]));
            }
        }

        private static void ShowAllEquipment()
        {
            Console.WriteLine("--- Full School Inventory ---");
            List<SchoolEquipment> all = _equipmentController.GetAllEquipment();
            for (int i = 0; i < all.Count; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, all[i]));
            }
        }

        private static void ShowBorrowers()
        {
            Console.WriteLine("--- Registered Borrowers ---");
            for (int i = 0; i < _borrowers.Count; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, _borrowers[i]));
            }
        }

        private static void InteractiveBorrow()
        {
            Console.WriteLine("--- Process Borrow Request ---");
            Console.Write("Enter Borrower School ID: ");
            string id = Console.ReadLine();
            SchoolBorrower borrower = _borrowers.Find(b => string.Equals(b.SchoolIDNumber, id, StringComparison.OrdinalIgnoreCase));
            if (borrower == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(">> Error: Borrower not found.");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter Equipment Property Tag: ");
            string tag = Console.ReadLine();
            SchoolEquipment item = _equipmentController.FindEquipmentByTag(tag);
            if (item == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(">> Error: Equipment not found.");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter Room where equipment will be used: ");
            string room = Console.ReadLine();

            try
            {
                BorrowSlip slip = _borrowingController.ProcessBorrow(borrower, item, room);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(">> SUCCESS: BorrowSlip Issued!");
                Console.ResetColor();
                Console.WriteLine(slip);
                Console.WriteLine("Expected Checklist on Return:");
                List<string> checklist = item.GetReturnChecklist();
                for (int i = 0; i < checklist.Count; i++)
                {
                    Console.WriteLine(string.Format("   [ ] {0}", checklist[i]));
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format(">> Borrowing Rejected: {0}", ex.Message));
                Console.ResetColor();
            }
        }

        private static void InteractiveReturn()
        {
            Console.WriteLine("--- Process Equipment Return ---");
            Console.Write("Enter Borrow Slip ID (e.g. SLIP-1001): ");
            string slipId = Console.ReadLine();

            BorrowSlip slip = _borrowingController.FindActiveSlip(slipId);
            if (slip == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(">> Error: Active slip not found.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine(string.Format("Processing Return for: {0} ({1})", slip.Equipment.ItemName, slip.Equipment.PropertyTag));
            Console.WriteLine("Verifying Return Checklist:");
            List<string> checklist = slip.Equipment.GetReturnChecklist();
            for (int i = 0; i < checklist.Count; i++)
            {
                Console.WriteLine(string.Format("  - {0}", checklist[i]));
            }

            Console.Write("Are all checklist components present and intact? (Y/N): ");
            string checkAns = Console.ReadLine();
            bool checklistComplete = !string.IsNullOrEmpty(checkAns) && checkAns.Trim().ToUpper() == "Y";

            Console.Write("Enter Return Condition (Good / Fair / Damaged / Needs Repair): ");
            string condition = Console.ReadLine();

            try
            {
                double penalty = _borrowingController.ProcessReturn(slipId, condition, checklistComplete);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(">> Return processed successfully.");
                Console.ResetColor();

                if (penalty > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format(">> PENALTY INCURRED: {0:C}", penalty));
                    Console.WriteLine(string.Format(">> Notice: Borrower {0} has been placed on CLEARANCE HOLD.", slip.Borrower.FullName));
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(">> Equipment returned in good standing with $0.00 penalty.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format(">> Error processing return: {0}", ex.Message));
                Console.ResetColor();
            }
        }

        private static void ShowActiveSlips()
        {
            Console.WriteLine("--- Active Borrow Slips ---");
            List<BorrowSlip> slips = _borrowingController.GetActiveSlips();
            if (slips.Count == 0)
            {
                Console.WriteLine("There are currently no active borrow slips.");
                return;
            }

            for (int i = 0; i < slips.Count; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, slips[i]));
            }
        }

        private static void InteractiveMarkRepair()
        {
            Console.Write("Enter Equipment Property Tag to mark for repair: ");
            string tag = Console.ReadLine();

            try
            {
                _equipmentController.MarkForRepair(tag);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format(">> Equipment '{0}' marked as Under Repair and removed from circulation.", tag));
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format(">> Error: {0}", ex.Message));
                Console.ResetColor();
            }
        }

        private static void InteractiveClearanceHold()
        {
            Console.Write("Enter Borrower School ID: ");
            string id = Console.ReadLine();
            SchoolBorrower borrower = _borrowers.Find(b => string.Equals(b.SchoolIDNumber, id, StringComparison.OrdinalIgnoreCase));
            if (borrower == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(">> Error: Borrower not found.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine(string.Format("Current Status: {0}", borrower));
            Console.Write("Action: [A]pply Hold or [L]ift Hold?: ");
            string action = Console.ReadLine();
            if (!string.IsNullOrEmpty(action) && action.Trim().ToUpper() == "A")
            {
                _borrowingController.ApplyClearanceHold(borrower);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(">> Clearance hold applied.");
                Console.ResetColor();
            }
            else if (!string.IsNullOrEmpty(action) && action.Trim().ToUpper() == "L")
            {
                _borrowingController.LiftClearanceHold(borrower);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(">> Clearance hold lifted.");
                Console.ResetColor();
            }
        }
    }
}
