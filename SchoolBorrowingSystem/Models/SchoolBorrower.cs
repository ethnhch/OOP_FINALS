using System;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// A person who can borrow equipment (student, faculty or staff).
    /// </summary>
    public class SchoolBorrower
    {
        public string SchoolIDNumber { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public bool HasClearanceHold { get; set; }

        /// <summary>
        /// Short text used in drop-down lists, for example "2024-10001 - Emma Watson (Student)".
        /// </summary>
        public string DisplayName
        {
            get { return SchoolIDNumber + " - " + FullName + " (" + Role + ")"; }
        }

        public SchoolBorrower()
        {
            SchoolIDNumber = string.Empty;
            FullName = string.Empty;
            Role = "Student";
            Department = string.Empty;
            HasClearanceHold = false;
        }

        public SchoolBorrower(string schoolIDNumber, string fullName, string role, string department, bool hasClearanceHold = false)
        {
            SchoolIDNumber = schoolIDNumber;
            FullName = fullName;
            Role = role;
            Department = department;
            HasClearanceHold = hasClearanceHold;
        }

        /// <summary>
        /// A borrower can only borrow if they have a school ID and no clearance hold.
        /// </summary>
        public bool CheckEligibility()
        {
            if (string.IsNullOrWhiteSpace(SchoolIDNumber))
            {
                return false;
            }

            return !HasClearanceHold;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} ({2} - {3}) | Clearance Hold: {4}",
                SchoolIDNumber, FullName, Role, Department, HasClearanceHold ? "ACTIVE (HOLD)" : "CLEAR");
        }
    }
}
