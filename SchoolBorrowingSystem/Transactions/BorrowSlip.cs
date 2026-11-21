using System;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.Transactions
{
    /// <summary>
    /// Represents an active or historical borrowing transaction slip.
    /// Tracks loan duration, room location, staff release credentials, and overdue state.
    /// </summary>
    public class BorrowSlip
    {
        public string SlipID { get; set; }
        public SchoolBorrower Borrower { get; set; }
        public SchoolEquipment Equipment { get; set; }
        public string RoomToBeUsed { get; set; }
        public DateTime TimeBorrowed { get; set; }
        public DateTime DueTime { get; set; }
        public DateTime? TimeReturned { get; set; }
        public string ReleasedByStaff { get; set; }

        public BorrowSlip()
        {
            SlipID = string.Empty;
            Borrower = null;
            Equipment = null;
            RoomToBeUsed = string.Empty;
            TimeBorrowed = DateTime.Now;
            DueTime = DateTime.Now;
            TimeReturned = null;
            ReleasedByStaff = "Admin";
        }

        public BorrowSlip(string slipID, SchoolBorrower borrower, SchoolEquipment equipment, string roomToBeUsed, DateTime timeBorrowed, DateTime dueTime, string releasedByStaff)
        {
            SlipID = slipID;
            Borrower = borrower;
            Equipment = equipment;
            RoomToBeUsed = roomToBeUsed;
            TimeBorrowed = timeBorrowed;
            DueTime = dueTime;
            TimeReturned = null;
            ReleasedByStaff = releasedByStaff;
        }

        /// <summary>
        /// Evaluates whether the loan is overdue.
        /// If already returned, checks if return timestamp exceeded due time.
        /// If currently outstanding, checks if current time exceeds due time.
        /// </summary>
        /// <returns>True if returned late or currently past due; false otherwise.</returns>
        public bool IsOverdue()
        {
            if (TimeReturned.HasValue)
            {
                return TimeReturned.Value > DueTime;
            }

            return DateTime.Now > DueTime;
        }

        public override string ToString()
        {
            string borrowerName = Borrower != null ? Borrower.FullName : "Unknown Borrower";
            string itemName = Equipment != null ? Equipment.ItemName : "Unknown Equipment";
            string status = TimeReturned.HasValue
                ? string.Format("RETURNED ({0:yyyy-MM-dd HH:mm})", TimeReturned.Value)
                : (IsOverdue() ? "OVERDUE" : "ACTIVE");

            return string.Format("[Slip #{0}] {1} borrowed by {2} | Room: {3} | Due: {4:yyyy-MM-dd HH:mm} | Status: {5}",
                SlipID, itemName, borrowerName, RoomToBeUsed, DueTime, status);
        }
    }
}
