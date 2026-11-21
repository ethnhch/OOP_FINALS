using System;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// Represents a school borrower (student, faculty, or staff member).
    /// Encapsulates identification credentials and clearance hold eligibility.
    /// </summary>
    public class SchoolBorrower
    {
        public string SchoolIDNumber { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public bool HasClearanceHold { get; set; }

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
        /// Validates borrower eligibility for acquiring school equipment loans.
        /// An individual is ineligible if a clearance hold is active or their ID is invalid.
        /// </summary>
        /// <returns>True if eligible; false otherwise.</returns>
        public bool CheckEligibility()
        {
            if (string.IsNullOrEmpty(SchoolIDNumber) || string.IsNullOrEmpty(SchoolIDNumber.Trim()))
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
