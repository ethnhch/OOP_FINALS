using System;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.Transactions
{
    /// <summary>
    /// One borrowing record: who borrowed what, where it is used, and when it is due.
    /// After the item is returned, the slip also remembers the condition and the fine.
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

        // Filled in when the item is returned
        public string ReturnCondition { get; set; }
        public bool ChecklistComplete { get; set; }
        public double PenaltyCharged { get; set; }

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
            ReturnCondition = string.Empty;
            ChecklistComplete = false;
            PenaltyCharged = 0;
        }

        public BorrowSlip(string slipID, SchoolBorrower borrower, SchoolEquipment equipment, string roomToBeUsed, DateTime timeBorrowed, DateTime dueTime, string releasedByStaff)
            : this()
        {
            SlipID = slipID;
            Borrower = borrower;
            Equipment = equipment;
            RoomToBeUsed = roomToBeUsed;
            TimeBorrowed = timeBorrowed;
            DueTime = dueTime;
            ReleasedByStaff = releasedByStaff;
        }

        public bool IsReturned()
        {
            return TimeReturned.HasValue;
        }

        /// <summary>
        /// Returned slips: was it returned after the due time?
        /// Open slips: is it already past the due time right now?
        /// </summary>
        public bool IsOverdue()
        {
            if (TimeReturned.HasValue)
            {
                return TimeReturned.Value > DueTime;
            }

            return DateTime.Now > DueTime;
        }

        /// <summary>
        /// Text for the status column, e.g. "Due in 2h 15m" or "Overdue by 1d 3h".
        /// </summary>
        public string GetTimeLeftText()
        {
            if (TimeReturned.HasValue)
            {
                return "Returned";
            }

            TimeSpan difference = DueTime - DateTime.Now;
            if (difference.TotalSeconds >= 0)
            {
                return "Due in " + FormatTimeSpan(difference);
            }

            return "Overdue by " + FormatTimeSpan(difference.Negate());
        }

        /// <summary>
        /// Turns a time span into short text like "3d 4h", "2h 5m" or "12m".
        /// </summary>
        public static string FormatTimeSpan(TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                return string.Format("{0}d {1}h", (int)span.TotalDays, span.Hours);
            }
            if (span.TotalHours >= 1)
            {
                return string.Format("{0}h {1}m", (int)span.TotalHours, span.Minutes);
            }
            return string.Format("{0}m", Math.Max(1, (int)span.TotalMinutes));
        }

        public override string ToString()
        {
            string borrowerName = Borrower != null ? Borrower.FullName : "Unknown Borrower";
            string itemName = Equipment != null ? Equipment.ItemName : "Unknown Equipment";

            string status;
            if (TimeReturned.HasValue)
            {
                status = string.Format("RETURNED ({0:yyyy-MM-dd HH:mm})", TimeReturned.Value);
            }
            else if (IsOverdue())
            {
                status = "OVERDUE";
            }
            else
            {
                status = "ACTIVE";
            }

            return string.Format("[Slip #{0}] {1} borrowed by {2} | Room: {3} | Due: {4:yyyy-MM-dd HH:mm} | Status: {5}",
                SlipID, itemName, borrowerName, RoomToBeUsed, DueTime, status);
        }
    }
}
