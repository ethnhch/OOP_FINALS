using System;
using System.Collections.Generic;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.Controllers
{
    /// <summary>
    /// Handles borrowing and returning: creates borrow slips, computes fines,
    /// and puts borrowers on clearance hold when something goes wrong.
    /// </summary>
    public class BorrowingController
    {
        // Fees that are the same for every kind of equipment (in PHP)
        public const double MissingPartsFee = 150.00;
        public const double DamageFee = 300.00;

        private readonly List<BorrowSlip> _activeSlips;
        private readonly List<BorrowSlip> _historicalSlips;
        private readonly EquipmentController _equipmentController;
        private int _slipSequenceCounter;

        public BorrowingController()
            : this(new EquipmentController())
        {
        }

        public BorrowingController(EquipmentController equipmentController)
        {
            if (equipmentController == null)
            {
                throw new ArgumentNullException("equipmentController", "EquipmentController cannot be null.");
            }

            _activeSlips = new List<BorrowSlip>();
            _historicalSlips = new List<BorrowSlip>();
            _equipmentController = equipmentController;
            _slipSequenceCounter = 1000;
        }

        // ---------------------------------------------------------------
        // Reading data
        // ---------------------------------------------------------------

        /// <summary>Returns a copy so other classes can't change our list by accident.</summary>
        public List<BorrowSlip> GetActiveSlips()
        {
            return new List<BorrowSlip>(_activeSlips);
        }

        public List<BorrowSlip> GetHistoricalSlips()
        {
            return new List<BorrowSlip>(_historicalSlips);
        }

        public List<BorrowSlip> GetOverdueSlips()
        {
            List<BorrowSlip> overdue = new List<BorrowSlip>();
            foreach (BorrowSlip slip in _activeSlips)
            {
                if (slip.IsOverdue())
                {
                    overdue.Add(slip);
                }
            }
            return overdue;
        }

        public BorrowSlip FindActiveSlip(string slipID)
        {
            foreach (BorrowSlip slip in _activeSlips)
            {
                if (string.Equals(slip.SlipID, slipID, StringComparison.OrdinalIgnoreCase))
                {
                    return slip;
                }
            }
            return null;
        }

        /// <summary>Finds the open slip for an item, or null if the item is not on loan.</summary>
        public BorrowSlip FindActiveSlipForEquipment(string propertyTag)
        {
            foreach (BorrowSlip slip in _activeSlips)
            {
                if (slip.Equipment != null && string.Equals(slip.Equipment.PropertyTag, propertyTag, StringComparison.OrdinalIgnoreCase))
                {
                    return slip;
                }
            }
            return null;
        }

        public int CountActiveLoans(SchoolBorrower borrower)
        {
            int count = 0;
            foreach (BorrowSlip slip in _activeSlips)
            {
                if (slip.Borrower == borrower)
                {
                    count++;
                }
            }
            return count;
        }

        public double GetTotalFinesCollected()
        {
            double total = 0;
            foreach (BorrowSlip slip in _historicalSlips)
            {
                total += slip.PenaltyCharged;
            }
            return total;
        }

        // ---------------------------------------------------------------
        // Borrowing
        // ---------------------------------------------------------------

        public BorrowSlip ProcessBorrow(SchoolBorrower borrower, SchoolEquipment item, string room)
        {
            if (borrower == null)
            {
                throw new ArgumentNullException("borrower", "Borrower cannot be null.");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item", "Equipment item cannot be null.");
            }

            if (!borrower.CheckEligibility())
            {
                throw new InvalidOperationException(string.Format(
                    "{0} cannot borrow right now because they have a clearance hold. Lift the hold first in the Borrowers page.",
                    borrower.FullName));
            }

            if (!item.IsAvailable)
            {
                throw new InvalidOperationException(string.Format(
                    "'{0}' ({1}) is not available right now (Condition: {2}).",
                    item.ItemName, item.PropertyTag, item.Condition));
            }

            // Each equipment type knows its own loan period (polymorphism)
            DateTime now = DateTime.Now;
            DateTime dueTime = item.CalculateDueTime(now);

            item.IsAvailable = false;

            _slipSequenceCounter++;
            string slipID = "SLIP-" + _slipSequenceCounter;

            string assignedRoom = "General Campus / Lecture Hall";
            if (!string.IsNullOrWhiteSpace(room))
            {
                assignedRoom = room.Trim();
            }

            BorrowSlip slip = new BorrowSlip(slipID, borrower, item, assignedRoom, now, dueTime, "Staff Desk");
            _activeSlips.Add(slip);
            return slip;
        }

        // ---------------------------------------------------------------
        // Returning
        // ---------------------------------------------------------------

        /// <summary>
        /// True if the condition means the item must go to repair.
        /// </summary>
        public static bool IsDamagedCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
            {
                return false;
            }

            return string.Equals(condition, SchoolEquipment.ConditionDamaged, StringComparison.OrdinalIgnoreCase)
                || string.Equals(condition, SchoolEquipment.ConditionNeedsRepair, StringComparison.OrdinalIgnoreCase)
                || string.Equals(condition, "Broken", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Works out the fine WITHOUT changing anything.
        /// The Return page uses this to show a live preview before staff confirm.
        /// </summary>
        public double CalculatePenalty(BorrowSlip slip, string condition, bool checklistComplete, DateTime returnTime)
        {
            if (slip == null || slip.Equipment == null)
            {
                return 0;
            }

            double total = 0;

            // 1. Late fine (each equipment type has its own rule)
            if (returnTime > slip.DueTime)
            {
                total += slip.Equipment.CalculateLateFine(returnTime - slip.DueTime);
            }

            // 2. Missing cables / parts
            if (!checklistComplete)
            {
                total += MissingPartsFee;
            }

            // 3. Damage
            if (IsDamagedCondition(condition))
            {
                total += DamageFee;
            }

            return total;
        }

        /// <summary>
        /// Checks the item back in. Returns the fine in PHP (0 means a clean return).
        /// </summary>
        public double ProcessReturn(string slipID, string condition, bool checklistComplete)
        {
            if (string.IsNullOrEmpty(slipID))
            {
                throw new ArgumentException("Slip ID cannot be null or empty.", "slipID");
            }

            BorrowSlip slip = FindActiveSlip(slipID);
            if (slip == null)
            {
                throw new KeyNotFoundException(string.Format("Active borrow slip '{0}' was not found.", slipID));
            }

            string finalCondition = SchoolEquipment.ConditionGood;
            if (!string.IsNullOrWhiteSpace(condition))
            {
                finalCondition = condition.Trim();
            }

            DateTime now = DateTime.Now;
            double penalty = CalculatePenalty(slip, finalCondition, checklistComplete, now);
            bool isDamaged = IsDamagedCondition(finalCondition);

            // Update the equipment
            slip.Equipment.Condition = finalCondition;
            if (isDamaged)
            {
                // Sends it to repair and makes it unavailable
                _equipmentController.MarkForRepair(slip.Equipment.PropertyTag);
            }
            else if (!checklistComplete)
            {
                // Keep it out of circulation until the missing parts are found
                slip.Equipment.Condition = SchoolEquipment.ConditionMissingParts;
                slip.Equipment.IsAvailable = false;
            }
            else
            {
                slip.Equipment.IsAvailable = true;
            }

            // Any problem at all = clearance hold
            if (penalty > 0)
            {
                ApplyClearanceHold(slip.Borrower);
            }

            // Save the return details on the slip and move it to history
            slip.TimeReturned = now;
            slip.ReturnCondition = finalCondition;
            slip.ChecklistComplete = checklistComplete;
            slip.PenaltyCharged = penalty;

            _activeSlips.Remove(slip);
            _historicalSlips.Add(slip);

            return penalty;
        }

        // ---------------------------------------------------------------
        // Clearance holds
        // ---------------------------------------------------------------

        public void ApplyClearanceHold(SchoolBorrower borrower)
        {
            if (borrower == null)
            {
                throw new ArgumentNullException("borrower", "Borrower cannot be null.");
            }

            borrower.HasClearanceHold = true;
        }

        public void LiftClearanceHold(SchoolBorrower borrower)
        {
            if (borrower == null)
            {
                throw new ArgumentNullException("borrower", "Borrower cannot be null.");
            }

            borrower.HasClearanceHold = false;
        }
    }
}
