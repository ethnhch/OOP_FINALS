using System;
using System.Collections.Generic;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.Controllers
{
    /// <summary>
    /// Keeps the list of registered borrowers (students, faculty and staff).
    /// Before this class existed, the list just lived inside Program.cs.
    /// </summary>
    public class BorrowerController
    {
        private readonly List<SchoolBorrower> _borrowers;

        public BorrowerController()
        {
            _borrowers = new List<SchoolBorrower>();
        }

        public void AddBorrower(SchoolBorrower borrower)
        {
            if (borrower == null)
            {
                throw new ArgumentNullException("borrower", "Borrower cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(borrower.SchoolIDNumber))
            {
                throw new ArgumentException("School ID number is required.", "borrower");
            }

            if (string.IsNullOrWhiteSpace(borrower.FullName))
            {
                throw new ArgumentException("Full name is required.", "borrower");
            }

            if (FindBorrowerById(borrower.SchoolIDNumber) != null)
            {
                throw new InvalidOperationException(string.Format("A borrower with ID '{0}' is already registered.", borrower.SchoolIDNumber));
            }

            _borrowers.Add(borrower);
        }

        public List<SchoolBorrower> GetAllBorrowers()
        {
            return new List<SchoolBorrower>(_borrowers);
        }

        public SchoolBorrower FindBorrowerById(string schoolIDNumber)
        {
            if (string.IsNullOrEmpty(schoolIDNumber))
            {
                return null;
            }

            foreach (SchoolBorrower borrower in _borrowers)
            {
                if (string.Equals(borrower.SchoolIDNumber, schoolIDNumber.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return borrower;
                }
            }
            return null;
        }

        public int CountBorrowersOnHold()
        {
            int count = 0;
            foreach (SchoolBorrower borrower in _borrowers)
            {
                if (borrower.HasClearanceHold)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
