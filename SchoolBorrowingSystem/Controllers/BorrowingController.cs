using System;
using System.Collections.Generic;
using SchoolBorrowingSystem.Data;
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
        private readonly ISchoolDataStore _store; // optional: null = no database

        public BorrowingController()
            : this(new EquipmentController())
        {
        }

        public BorrowingController(EquipmentController equipmentController)
            : this(equipmentController, null)
        {
        }

        public BorrowingController(EquipmentController equipmentController, ISchoolDataStore store)
        {
            if (equipmentController == null)
            {
                throw new ArgumentNullException("equipmentController", "EquipmentController coordinator cannot be null.");
            }

            _activeSlips = new List<BorrowSlip>();
            _historicalSlips = new List<BorrowSlip>();
            _equipmentController = equipmentController;
            _store = store;
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
        /// Checks whether the equipment with the specified property tag is currently on an active loan.
        /// </summary>
        /// <param name="propertyTag">The property tag to check.</param>
        /// <returns>True if an active (unreturned) slip exists for the equipment; false otherwise.</returns>
        public bool IsEquipmentBorrowed(string propertyTag)
        {
            if (string.IsNullOrWhiteSpace(propertyTag))
            {
                return false;
            }

            for (int i = 0; i < _activeSlips.Count; i++)
            {
                BorrowSlip slip = _activeSlips[i];
                if (slip.Equipment != null &&
                    string.Equals(slip.Equipment.PropertyTag, propertyTag.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Re-binds active and historical slip equipment references to the live items in EquipmentController.
        /// Ensures reference integrity after inventory reloads from disk or storage.
        /// </summary>
        public void RebindEquipmentReferences()
        {
            foreach (var slip in _activeSlips)
            {
                if (slip.Equipment != null)
                {
                    var live = _equipmentController.FindEquipmentByTag(slip.Equipment.PropertyTag);
                    if (live != null)
                    {
                        slip.Equipment = live;
                    }
                }
            }

            foreach (var slip in _historicalSlips)
            {
                if (slip.Equipment != null)
                {
                    var live = _equipmentController.FindEquipmentByTag(slip.Equipment.PropertyTag);
                    if (live != null)
                    {
                        slip.Equipment = live;
                    }
                }
            }
        }

        /// <summary>
        /// Synchronizes inventory availability flags with currently active loans after an inventory reload.
        /// Re-binds slip references and guarantees that loaned equipment is not falsely shown as available.
        /// </summary>
        public void SyncAvailabilityWithActiveLoans()
        {
            RebindEquipmentReferences();
            foreach (var slip in _activeSlips)
            {
                if (slip.Equipment != null)
                {
                    slip.Equipment.IsAvailable = false;
                }
            }
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

            // Verify equipment belongs to registered school inventory
            SchoolEquipment inventoryItem = _equipmentController.FindEquipmentByTag(item.PropertyTag);
            if (inventoryItem == null)
            {
                throw new InvalidOperationException(
                    string.Format("Equipment '{0}' (PropertyTag: {1}) does not exist in school inventory.",
                        item.ItemName, item.PropertyTag));
            }

            // Verify borrower clearance and identity
            if (!borrower.CheckEligibility())
            {
                throw new InvalidOperationException(
                    string.Format("Borrower '{0}' (ID: {1}) is ineligible to borrow equipment. Clearance hold: {2}.",
                        borrower.FullName, borrower.SchoolIDNumber, borrower.HasClearanceHold ? "ACTIVE" : "NONE"));
            }

            // Verify equipment availability
            if (!inventoryItem.IsAvailable)
            {
                throw new InvalidOperationException(
                    string.Format("Equipment '{0}' (PropertyTag: {1}) is not currently available for borrowing (Condition: {2}).",
                        inventoryItem.ItemName, inventoryItem.PropertyTag, inventoryItem.Condition));
            }

            // Calculate loan due time polymorphically
            DateTime now = DateTime.Now;
            DateTime dueTime;

            if (inventoryItem is AVEquipment)
            {
                AVEquipment av = (AVEquipment)inventoryItem;
                dueTime = now.AddHours(av.GetBorrowDurationHours());
            }
            else
            {
                int durationDays = inventoryItem.GetBorrowDurationDays();
                dueTime = durationDays > 0 ? now.AddDays(durationDays) : now.AddHours(4);
            }

            // Update equipment availability status
            inventoryItem.IsAvailable = false;
            item.IsAvailable = false;

            // Generate unique slip ID
            _slipSequenceCounter++;
            string slipID = string.Format("SLIP-{0}", _slipSequenceCounter);

            string assignedRoom = string.IsNullOrEmpty(room) ? "General Campus / Lecture Hall" : room.Trim();
            BorrowSlip slip = new BorrowSlip(slipID, borrower, inventoryItem, assignedRoom, now, dueTime, "Staff Desk");

            _activeSlips.Add(slip);

            if (_store != null)
            {
                _store.SaveSlip(slip);
            }

            return slip;
        }

        /// <summary>
        /// Processes the return of borrowed equipment, evaluating condition, checklist completeness,
        /// overdue duration, and administrative clearance hold enforcement.
        /// Fines / penalties are removed per academic specifications; discipline is governed by clearance holds.
        /// </summary>
        /// <param name="slipID">Transaction slip identifier.</param>
        /// <param name="condition">Condition assessment upon return (e.g. Good, Fair, Damaged).</param>
        /// <param name="checklistComplete">Whether all accessories in the return checklist are verified.</param>
        /// <returns>True if an administrative clearance hold was enforced; false if cleared in good standing.</returns>
        public bool ProcessReturn(string slipID, string condition, bool checklistComplete)
        {
            if (string.IsNullOrWhiteSpace(slipID))
            {
                throw new ArgumentException("Slip ID cannot be null or empty.", "slipID");
            }

            BorrowSlip slip = FindActiveSlip(slipID);
            if (slip == null)
            {
                throw new KeyNotFoundException(string.Format("Active borrow slip '{0}' was not found.", slipID));
            }

            slip.TimeReturned = DateTime.Now;
            bool isOverdue = slip.IsOverdue();

            // Re-resolve live equipment instance from EquipmentController to prevent stale/orphan reference desynchronization
            SchoolEquipment liveItem = (slip.Equipment != null)
                ? (_equipmentController.FindEquipmentByTag(slip.Equipment.PropertyTag) ?? slip.Equipment)
                : null;

            string finalCondition = string.IsNullOrWhiteSpace(condition) ? "Good" : condition.Trim();
            bool isDamaged = string.Equals(finalCondition, "Damaged", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(finalCondition, "Needs Repair", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(finalCondition, "Broken", StringComparison.OrdinalIgnoreCase);

            if (isDamaged)
            {
                if (liveItem != null)
                {
                    liveItem.Condition = "Under Repair";
                    _equipmentController.MarkForRepair(liveItem.PropertyTag);
                }
            }
            else if (!checklistComplete)
            {
                // Incomplete checklist retains item as unavailable and updates condition accurately
                if (liveItem != null)
                {
                    liveItem.Condition = "Missing Accessories / Incomplete";
                    liveItem.IsAvailable = false;
                    if (_store != null)
                    {
                        _store.SaveEquipment(liveItem);
                    }
                }
            }
            else
            {
                // Return item back to active circulation in Good standing
                if (liveItem != null)
                {
                    liveItem.Condition = finalCondition;
                    liveItem.IsAvailable = true;
                    if (_store != null)
                    {
                        _store.SaveEquipment(liveItem);
                    }
                }
            }

            if (liveItem != null)
            {
                slip.Equipment = liveItem;
            }

            // Apply administrative clearance hold if violations occur (overdue, damaged, or missing parts)
            bool holdRequired = isOverdue || isDamaged || !checklistComplete;
            if (holdRequired && slip.Borrower != null)
            {
                ApplyClearanceHold(slip.Borrower);
            }

            // Move slip from active to historical records
            _activeSlips.Remove(slip);
            _historicalSlips.Add(slip);

            if (_store != null)
            {
                _store.SaveSlip(slip); // slip + equipment state + borrower hold
            }

            return holdRequired;
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

            if (_store != null)
            {
                _store.SaveBorrower(borrower);
            }
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

            if (_store != null)
            {
                _store.SaveBorrower(borrower);
            }
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

        /// <summary>
        /// Rebuilds the slip lists from the database at startup. A slip without a return time
        /// is active; otherwise it is history. Also continues the SLIP-#### numbering.
        /// Does not write anything back to the database.
        /// </summary>
        public void RestoreSlips(IEnumerable<BorrowSlip> slips)
        {
            _activeSlips.Clear();
            _historicalSlips.Clear();

            foreach (BorrowSlip slip in slips)
            {
                if (slip.TimeReturned.HasValue)
                {
                    _historicalSlips.Add(slip);
                }
                else
                {
                    _activeSlips.Add(slip);
                }

                int number;
                if (slip.SlipID != null &&
                    slip.SlipID.StartsWith("SLIP-", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(slip.SlipID.Substring(5), out number) &&
                    number > _slipSequenceCounter)
                {
                    _slipSequenceCounter = number;
                }
            }
        }
    }
}
