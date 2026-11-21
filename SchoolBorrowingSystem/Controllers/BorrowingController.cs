using System;
using System.Collections.Generic;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.Controllers
{
    /// <summary>
    /// Coordinates equipment borrowing transactions, return processing, penalty calculations,
    /// and clearance hold administration.
    /// Fulfills the Controller pattern in GRASP/MVC and coordinates with EquipmentController.
    /// </summary>
    public class BorrowingController
    {
        // UML-specified private field
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
                throw new ArgumentNullException("equipmentController", "EquipmentController coordinator cannot be null.");
            }

            _activeSlips = new List<BorrowSlip>();
            _historicalSlips = new List<BorrowSlip>();
            _equipmentController = equipmentController;
            _slipSequenceCounter = 1000;
        }

        /// <summary>
        /// Gets the current active slips in the system.
        /// </summary>
        public List<BorrowSlip> GetActiveSlips()
        {
            return new List<BorrowSlip>(_activeSlips);
        }

        /// <summary>
        /// Gets the historical (returned) slips for auditing.
        /// </summary>
        public List<BorrowSlip> GetHistoricalSlips()
        {
            return new List<BorrowSlip>(_historicalSlips);
        }

        /// <summary>
        /// Processes an equipment borrow request for an eligible borrower.
        /// Updates equipment availability and coordinates loan durations.
        /// </summary>
        /// <param name="borrower">The borrower requesting the item.</param>
        /// <param name="item">The equipment requested.</param>
        /// <param name="room">Designated classroom or laboratory.</param>
        /// <returns>A validated BorrowSlip recording the loan transaction.</returns>
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

            // Verify borrower clearance and identity
            if (!borrower.CheckEligibility())
            {
                throw new InvalidOperationException(
                    string.Format("Borrower '{0}' (ID: {1}) is ineligible to borrow equipment. Clearance hold: {2}.",
                        borrower.FullName, borrower.SchoolIDNumber, borrower.HasClearanceHold ? "ACTIVE" : "NONE"));
            }

            // Verify equipment availability
            if (!item.IsAvailable)
            {
                throw new InvalidOperationException(
                    string.Format("Equipment '{0}' (PropertyTag: {1}) is not currently available for borrowing (Condition: {2}).",
                        item.ItemName, item.PropertyTag, item.Condition));
            }

            // Calculate loan due time polymorphically
            DateTime now = DateTime.Now;
            DateTime dueTime;

            if (item is AVEquipment)
            {
                AVEquipment av = (AVEquipment)item;
                dueTime = now.AddHours(av.GetBorrowDurationHours());
            }
            else
            {
                int durationDays = item.GetBorrowDurationDays();
                dueTime = durationDays > 0 ? now.AddDays(durationDays) : now.AddHours(4);
            }

            // Update equipment availability status
            item.IsAvailable = false;

            // Generate unique slip ID
            _slipSequenceCounter++;
            string slipID = string.Format("SLIP-{0}", _slipSequenceCounter);

            string assignedRoom = string.IsNullOrEmpty(room) ? "General Campus / Lecture Hall" : room.Trim();
            BorrowSlip slip = new BorrowSlip(slipID, borrower, item, assignedRoom, now, dueTime, "Staff Desk");

            _activeSlips.Add(slip);
            return slip;
        }

        /// <summary>
        /// Processes the return of borrowed equipment, evaluating condition, completeness,
        /// overdue penalties, and clearance hold requirements.
        /// </summary>
        /// <param name="slipID">Transaction slip identifier.</param>
        /// <param name="condition">Condition assessment upon return (e.g. Good, Fair, Damaged).</param>
        /// <param name="checklistComplete">Whether all accessories in the return checklist are verified.</param>
        /// <returns>Total financial penalty / fine incurred (double).</returns>
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

            slip.TimeReturned = DateTime.Now;
            double totalPenalty = 0.0;

            // 1. Calculate Overdue Fine
            if (slip.IsOverdue())
            {
                TimeSpan overdueSpan = slip.TimeReturned.Value - slip.DueTime;
                if (slip.Equipment is AVEquipment)
                {
                    // AV equipment: 50.00 base + 25.00 per overdue hour
                    double overdueHours = Math.Max(1.0, Math.Ceiling(overdueSpan.TotalHours));
                    totalPenalty += 50.00 + ((overdueHours - 1) * 25.00);
                }
                else
                {
                    // Lab equipment: 100.00 base + 50.00 per overdue day
                    double overdueDays = Math.Max(1.0, Math.Ceiling(overdueSpan.TotalDays));
                    totalPenalty += 100.00 + ((overdueDays - 1) * 50.00);
                }
            }

            // 2. Incomplete Return Checklist Penalty
            if (!checklistComplete)
            {
                // Fine for missing cables, remotes, or incomplete component count
                totalPenalty += 150.00;
            }

            // 3. Equipment Condition Assessment & Repair Coordination
            string finalCondition = string.IsNullOrEmpty(condition) ? "Good" : condition.Trim();
            slip.Equipment.Condition = finalCondition;

            bool isDamaged = string.Equals(finalCondition, "Damaged", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(finalCondition, "Needs Repair", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(finalCondition, "Broken", StringComparison.OrdinalIgnoreCase);

            if (isDamaged)
            {
                totalPenalty += 300.00; // Damage restoration fee
                _equipmentController.MarkForRepair(slip.Equipment.PropertyTag);
            }
            else if (checklistComplete)
            {
                // Return item back to active circulation
                slip.Equipment.IsAvailable = true;
            }
            else
            {
                // Incomplete checklist retains item as unavailable until accessories are retrieved
                slip.Equipment.IsAvailable = false;
            }

            // 4. Apply clearance hold if violations occur
            if (totalPenalty > 0 || isDamaged || !checklistComplete)
            {
                ApplyClearanceHold(slip.Borrower);
            }

            // Move slip from active to historical records
            _activeSlips.Remove(slip);
            _historicalSlips.Add(slip);

            return totalPenalty;
        }

        /// <summary>
        /// Applies an administrative clearance hold to the specified borrower.
        /// Prevents future loan transactions until the hold is resolved.
        /// </summary>
        /// <param name="borrower">The borrower to place on clearance hold.</param>
        public void ApplyClearanceHold(SchoolBorrower borrower)
        {
            if (borrower == null)
            {
                throw new ArgumentNullException("borrower", "Borrower cannot be null.");
            }

            borrower.HasClearanceHold = true;
        }

        /// <summary>
        /// Removes an administrative clearance hold from a borrower.
        /// </summary>
        public void LiftClearanceHold(SchoolBorrower borrower)
        {
            if (borrower == null)
            {
                throw new ArgumentNullException("borrower", "Borrower cannot be null.");
            }

            borrower.HasClearanceHold = false;
        }

        /// <summary>
        /// Locates an active slip by its ID.
        /// </summary>
        public BorrowSlip FindActiveSlip(string slipID)
        {
            for (int i = 0; i < _activeSlips.Count; i++)
            {
                if (string.Equals(_activeSlips[i].SlipID, slipID, StringComparison.OrdinalIgnoreCase))
                {
                    return _activeSlips[i];
                }
            }

            return null;
        }
    }
}
