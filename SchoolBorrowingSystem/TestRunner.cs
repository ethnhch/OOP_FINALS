using System;
using System.Collections.Generic;
using SchoolBorrowingSystem.Controllers;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem
{
    /// <summary>
    /// Automated Quality Assurance Test Suite.
    /// Rigorously validates all core logic, OOP principles, boundary conditions,
    /// and business rules outlined in the UML specification.
    /// </summary>
    public static class TestRunner
    {
        private static int _passedTests = 0;
        private static int _failedTests = 0;

        // So the Tests page can show the numbers without parsing text
        public static int PassedCount { get { return _passedTests; } }
        public static int FailedCount { get { return _failedTests; } }

        public static bool RunAllTests()
        {
            _passedTests = 0;
            _failedTests = 0;

            Console.WriteLine("================================================================");
            Console.WriteLine("          SCHOOL BORROWING SYSTEM - AUTOMATED QA TEST SUITE    ");
            Console.WriteLine("================================================================");
            Console.WriteLine();

            TestPolymorphismAndChecklists();
            TestBorrowerEligibility();
            TestEquipmentControllerInventory();
            TestSuccessfulBorrowWorkflow();
            TestIneligibleBorrowerRejection();
            TestUnavailableEquipmentRejection();
            TestCleanReturnNoFees();
            TestOverdueReturnPenalty();
            TestIncompleteChecklistPenalty();
            TestDamagedReturnAndAutomaticRepairWorkflow();
            TestInventoryPersistence();
            TestExactFineAmountsAndPreview();
            TestRestoreAfterRepairOrMissingParts();
            TestBorrowerRegistry();

            Console.WriteLine();
            Console.WriteLine("================================================================");
            Console.WriteLine(string.Format(" TEST RESULTS SUMMARY: {0} PASSED | {1} FAILED | TOTAL: {2}",
                _passedTests, _failedTests, _passedTests + _failedTests));
            Console.WriteLine("================================================================");

            return _failedTests == 0;
        }

        private static void Assert(bool condition, string testName, string details = "")
        {
            if (condition)
            {
                _passedTests++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[PASS] ");
                Console.ResetColor();
                Console.WriteLine(testName);
            }
            else
            {
                _failedTests++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[FAIL] ");
                Console.ResetColor();
                Console.WriteLine(string.Format("{0} - Details: {1}", testName, details));
            }
        }

        private static void TestPolymorphismAndChecklists()
        {
            Console.WriteLine("--> Scenario 1: OOP Polymorphism & Dynamic Return Checklists");

            SchoolEquipment av = new AVEquipment("AV-001", "Epson HD Projector", true, true, true, 4);
            SchoolEquipment lab = new LabEquipment("LAB-001", "Binocular Microscope", DateTime.Now.AddMonths(-2), 4, 7);

            // Test polymorphic method calls on SchoolEquipment reference
            List<string> avChecklist = av.GetReturnChecklist();
            List<string> labChecklist = lab.GetReturnChecklist();

            Assert(avChecklist.Contains("AC Power Adapter / Power Cord"), "AV Equipment checklist includes Power Cord");
            Assert(avChecklist.Contains("Remote Controller (with functional batteries)"), "AV Equipment checklist includes Remote Control");
            Assert(avChecklist.Contains("High-Speed HDMI / Display Cable"), "AV Equipment checklist includes HDMI");
            Assert(av.GetBorrowDurationDays() == 0, "AV Equipment default duration is 0 days (hourly loan)");

            Assert(lab.GetBorrowDurationDays() == 7, "Lab Equipment duration is 7 days");
            bool hasComponentCheck = false;
            for (int i = 0; i < labChecklist.Count; i++)
            {
                if (labChecklist[i].Contains("4 Discrete Component(s)")) hasComponentCheck = true;
            }
            Assert(hasComponentCheck, "Lab Equipment checklist verifies 4 components");
        }

        private static void TestBorrowerEligibility()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 2: Borrower Identity & Clearance Hold Validation");

            SchoolBorrower studentClear = new SchoolBorrower("2024-00123", "Alice Johnson", "Student", "Computer Science", false);
            SchoolBorrower studentHold = new SchoolBorrower("2024-00456", "Bob Smith", "Student", "Engineering", true);
            SchoolBorrower studentNoId = new SchoolBorrower("", "Charlie Brown", "Student", "Arts", false);

            Assert(studentClear.CheckEligibility() == true, "Clear borrower is eligible");
            Assert(studentHold.CheckEligibility() == false, "Borrower with Clearance Hold is NOT eligible");
            Assert(studentNoId.CheckEligibility() == false, "Borrower without Student ID is NOT eligible");
        }

        private static void TestEquipmentControllerInventory()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 3: EquipmentController Inventory & Repair Tracking");

            EquipmentController controller = new EquipmentController("test_temp_inventory.txt");
            AVEquipment av1 = new AVEquipment("AV-100", "Sony Bravia Display", false, true, true, 3);
            LabEquipment lab1 = new LabEquipment("LAB-200", "Digital Centrifuge", DateTime.Now, 2, 5);

            controller.AddEquipment(av1);
            controller.AddEquipment(lab1);

            Assert(controller.GetAllEquipment().Count == 2, "EquipmentController tracks all 2 added items");
            Assert(controller.GetAvailableEquipment().Count == 2, "Both items initially available");

            // Mark for repair
            controller.MarkForRepair("AV-100");
            Assert(av1.Condition == "Under Repair", "AV-100 condition updated to 'Under Repair'");
            Assert(av1.IsAvailable == false, "AV-100 availability revoked upon repair flagging");
            Assert(controller.GetAvailableEquipment().Count == 1, "Only 1 item available after repair flagging");

            // Duplicate prevention
            bool duplicateCaught = false;
            try
            {
                controller.AddEquipment(new AVEquipment("AV-100", "Duplicate Unit", true, true, true));
            }
            catch (InvalidOperationException)
            {
                duplicateCaught = true;
            }
            Assert(duplicateCaught, "Duplicate PropertyTag registration is properly rejected");
        }

        private static void TestSuccessfulBorrowWorkflow()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 4: Successful Borrowing Workflow & Due Time Calculation");

            EquipmentController eqCtrl = new EquipmentController();
            AVEquipment projector = new AVEquipment("AV-301", "Optoma Projector", true, true, true, 4);
            eqCtrl.AddEquipment(projector);

            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower faculty = new SchoolBorrower("FAC-8899", "Dr. Alan Turing", "Faculty", "Computer Science");

            BorrowSlip slip = borrowCtrl.ProcessBorrow(faculty, projector, "Room 402");

            Assert(slip != null, "BorrowSlip generated successfully");
            Assert(slip.Equipment.IsAvailable == false, "Equipment marked unavailable upon borrowing");
            Assert(slip.RoomToBeUsed == "Room 402", "Room assigned correctly");
            Assert(slip.DueTime > slip.TimeBorrowed, "Due time set after time borrowed");
            Assert(slip.IsOverdue() == false, "New slip is not overdue");
            Assert(borrowCtrl.GetActiveSlips().Count == 1, "Active slips list reflects active transaction");
        }

        private static void TestIneligibleBorrowerRejection()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 5: Rejection of Ineligible Borrowers");

            EquipmentController eqCtrl = new EquipmentController();
            AVEquipment av = new AVEquipment("AV-401", "Document Camera", false, true, true);
            eqCtrl.AddEquipment(av);

            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower suspendedBorrower = new SchoolBorrower("STU-9999", "Jane Doe", "Student", "Business", true);

            bool exceptionCaught = false;
            try
            {
                borrowCtrl.ProcessBorrow(suspendedBorrower, av, "Lab 101");
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            Assert(exceptionCaught, "Ineligible borrower loan request is rejected with InvalidOperationException");
            Assert(av.IsAvailable == true, "Equipment remains available after rejected borrow");
            Assert(borrowCtrl.GetActiveSlips().Count == 0, "No slip recorded for rejected transaction");
        }

        private static void TestUnavailableEquipmentRejection()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 6: Rejection of Unavailable Equipment");

            EquipmentController eqCtrl = new EquipmentController();
            AVEquipment av = new AVEquipment("AV-501", "Microphone Set", true, true, false, 2, "Under Repair", false);
            eqCtrl.AddEquipment(av);

            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower student = new SchoolBorrower("STU-1111", "Mark Evans", "Student", "Engineering");

            bool exceptionCaught = false;
            try
            {
                borrowCtrl.ProcessBorrow(student, av, "Auditorium");
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }

            Assert(exceptionCaught, "Unavailable equipment loan request rejected with InvalidOperationException");
        }

        private static void TestCleanReturnNoFees()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 7: On-time, Complete & Good Condition Return (Zero Penalty)");

            EquipmentController eqCtrl = new EquipmentController();
            LabEquipment microscope = new LabEquipment("LAB-301", "Stereo Microscope", DateTime.Now, 3, 5);
            eqCtrl.AddEquipment(microscope);

            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower student = new SchoolBorrower("STU-2222", "Grace Hopper", "Student", "Biology");

            BorrowSlip slip = borrowCtrl.ProcessBorrow(student, microscope, "Bio Lab A");
            double penalty = borrowCtrl.ProcessReturn(slip.SlipID, "Good", true);

            Assert(penalty == 0.0, "Zero penalty for timely and complete return");
            Assert(microscope.IsAvailable == true, "Equipment returned to available status");
            Assert(student.HasClearanceHold == false, "No clearance hold placed on borrower");
            Assert(borrowCtrl.GetActiveSlips().Count == 0, "Slip removed from active list");
            Assert(borrowCtrl.GetHistoricalSlips().Count == 1, "Slip moved to historical audit archive");
        }

        private static void TestOverdueReturnPenalty()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 8: Overdue Return Penalty & Clearance Hold Enforcement");

            EquipmentController eqCtrl = new EquipmentController();
            AVEquipment projector = new AVEquipment("AV-601", "Epson Portable Projector", true, true, true, 2);
            eqCtrl.AddEquipment(projector);

            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower student = new SchoolBorrower("STU-3333", "John von Neumann", "Student", "Physics");

            BorrowSlip slip = borrowCtrl.ProcessBorrow(student, projector, "Lecture Hall 1");

            // Artificially simulate overdue loan by backdating DueTime
            slip.DueTime = DateTime.Now.AddHours(-3); // 3 hours overdue
            Assert(slip.IsOverdue() == true, "BorrowSlip correctly detects overdue condition");

            double penalty = borrowCtrl.ProcessReturn(slip.SlipID, "Good", true);

            Assert(penalty > 0.0, string.Format("Overdue penalty applied correctly: {0:C}", penalty));
            Assert(student.HasClearanceHold == true, "Clearance hold automatically enforced due to overdue fine");
        }

        private static void TestIncompleteChecklistPenalty()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 9: Incomplete Checklist (Missing Cable) Penalty");

            EquipmentController eqCtrl = new EquipmentController();
            AVEquipment av = new AVEquipment("AV-701", "ViewSonic Smart Board", true, true, true, 4);
            eqCtrl.AddEquipment(av);

            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower student = new SchoolBorrower("STU-4444", "Ada Lovelace", "Student", "Computer Science");

            BorrowSlip slip = borrowCtrl.ProcessBorrow(student, av, "Lab 203");
            double penalty = borrowCtrl.ProcessReturn(slip.SlipID, "Good", false); // Checklist incomplete!

            Assert(penalty >= 150.00, string.Format("Incomplete checklist penalty charged: {0:C}", penalty));
            Assert(student.HasClearanceHold == true, "Clearance hold applied due to missing peripheral checklist");
            Assert(av.IsAvailable == false, "Item kept unavailable until missing accessories are reconciled");
        }

        private static void TestDamagedReturnAndAutomaticRepairWorkflow()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 10: Damaged Return & EquipmentController Repair Coordination");

            EquipmentController eqCtrl = new EquipmentController();
            LabEquipment balance = new LabEquipment("LAB-401", "Analytical Balance", DateTime.Now, 2, 3);
            eqCtrl.AddEquipment(balance);

            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower student = new SchoolBorrower("STU-5555", "Dennis Ritchie", "Student", "Chemistry");

            BorrowSlip slip = borrowCtrl.ProcessBorrow(student, balance, "Chem Lab 4");
            double penalty = borrowCtrl.ProcessReturn(slip.SlipID, "Damaged", true);

            Assert(penalty >= 300.00, string.Format("Damage assessment fee charged: {0:C}", penalty));
            Assert(balance.Condition == "Under Repair", "Equipment marked 'Under Repair' in EquipmentController");
            Assert(balance.IsAvailable == false, "Damaged equipment availability revoked");
            Assert(student.HasClearanceHold == true, "Clearance hold applied to borrower for damaged property");
        }

        private static void TestInventoryPersistence()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 11: Inventory File Persistence & Deserialization");

            string testFile = "qa_inventory_test.txt";
            if (System.IO.File.Exists(testFile)) System.IO.File.Delete(testFile);

            EquipmentController originalCtrl = new EquipmentController(testFile);
            originalCtrl.AddEquipment(new AVEquipment("AV-999", "Panasonic 4K Camcorder", true, true, true, 6));
            originalCtrl.AddEquipment(new LabEquipment("LAB-888", "Digital Refractometer", new DateTime(2025, 5, 20), 3, 10));

            originalCtrl.SaveInventory();
            Assert(System.IO.File.Exists(testFile), "Inventory file successfully written to disk");

            EquipmentController reloadedCtrl = new EquipmentController(testFile);
            reloadedCtrl.LoadInventory();

            Assert(reloadedCtrl.GetAllEquipment().Count == 2, "Reloaded inventory matches original item count (2)");
            SchoolEquipment reloadedAV = reloadedCtrl.FindEquipmentByTag("AV-999");
            Assert(reloadedAV != null && reloadedAV is AVEquipment, "Polymorphic AVEquipment restored correctly");
            SchoolEquipment reloadedLab = reloadedCtrl.FindEquipmentByTag("LAB-888");
            Assert(reloadedLab != null && reloadedLab is LabEquipment, "Polymorphic LabEquipment restored correctly");

            // Clean up test file
            if (System.IO.File.Exists(testFile)) System.IO.File.Delete(testFile);
        }
            private static void TestExactFineAmountsAndPreview()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 12: Exact Fine Amounts & Live Fine Preview");

            AVEquipment av = new AVEquipment("AV-950", "Wireless Mic", false, true, false, 2);
            LabEquipment lab = new LabEquipment("LAB-950", "pH Meter", DateTime.Now, 2, 3);

            Assert(av.CalculateLateFine(TimeSpan.Zero) == 0, "AV: no late fine when returned on time");
            Assert(av.CalculateLateFine(TimeSpan.FromMinutes(10)) == 50, "AV: 10 minutes late counts as 1 hour (PHP 50)");
            Assert(av.CalculateLateFine(TimeSpan.FromHours(3)) == 100, "AV: 3 hours late = 50 + 25 + 25 (PHP 100)");
            Assert(lab.CalculateLateFine(TimeSpan.FromDays(2)) == 150, "Lab: 2 days late = 100 + 50 (PHP 150)");

            EquipmentController eqCtrl = new EquipmentController();
            eqCtrl.AddEquipment(av);
            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower student = new SchoolBorrower("STU-9500", "Barbara Liskov", "Student", "Computer Science");
            BorrowSlip slip = borrowCtrl.ProcessBorrow(student, av, "Room 101");

            Assert(slip.DueTime == slip.TimeBorrowed.AddHours(2), "Due time comes from AVEquipment.CalculateDueTime (2 hours)");

            double preview = borrowCtrl.CalculatePenalty(slip, "Damaged", false, slip.DueTime.AddMinutes(30));
            Assert(preview == 50 + 150 + 300, "Preview adds late + missing parts + damage fees (PHP 500)");
            Assert(av.IsAvailable == false && student.HasClearanceHold == false, "Preview does not change any data");
        }

        private static void TestRestoreAfterRepairOrMissingParts()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 13: Restoring Equipment After Repair / Missing Parts");

            EquipmentController eqCtrl = new EquipmentController();
            AVEquipment av = new AVEquipment("AV-960", "Speaker Set", false, true, false, 4);
            eqCtrl.AddEquipment(av);
            BorrowingController borrowCtrl = new BorrowingController(eqCtrl);
            SchoolBorrower student = new SchoolBorrower("STU-9600", "Ken Thompson", "Student", "IT");

            BorrowSlip slip = borrowCtrl.ProcessBorrow(student, av, "Gym");
            borrowCtrl.ProcessReturn(slip.SlipID, "Good", false);
            Assert(av.Condition == "Missing Parts", "Incomplete return marks item as 'Missing Parts'");
            Assert(borrowCtrl.GetHistoricalSlips()[0].PenaltyCharged == 150, "Slip remembers the fine that was charged");

            eqCtrl.RestoreToService("AV-960");
            Assert(av.IsAvailable == true && av.Condition == "Good", "RestoreToService puts item back as Good / Available");
        }

        private static void TestBorrowerRegistry()
        {
            Console.WriteLine();
            Console.WriteLine("--> Scenario 14: BorrowerController Registration Rules");

            BorrowerController borrowers = new BorrowerController();
            borrowers.AddBorrower(new SchoolBorrower("2025-0001", "Linus Torvalds", "Student", "IT"));

            bool duplicateCaught = false;
            try
            {
                borrowers.AddBorrower(new SchoolBorrower("2025-0001", "Someone Else", "Student", "IT"));
            }
            catch (InvalidOperationException)
            {
                duplicateCaught = true;
            }

            bool emptyNameCaught = false;
            try
            {
                borrowers.AddBorrower(new SchoolBorrower("2025-0002", "  ", "Student", "IT"));
            }
            catch (ArgumentException)
            {
                emptyNameCaught = true;
            }

            Assert(duplicateCaught, "Duplicate school ID is rejected");
            Assert(emptyNameCaught, "Borrower without a name is rejected");
            Assert(borrowers.FindBorrowerById("2025-0001") != null, "Borrower can be found by school ID");
        }
    }
}
