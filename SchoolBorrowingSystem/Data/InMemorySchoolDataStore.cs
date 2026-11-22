using System;
using System.Collections.Generic;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.Data
{
    /// <summary>
    /// A fake data store that keeps everything in memory. Used by the automated tests so the
    /// save/restore logic can be checked without needing a real database.
    /// Like a real database, it stores COPIES, so loading always returns brand-new objects.
    /// </summary>
    public class InMemorySchoolDataStore : ISchoolDataStore
    {
        private class SlipRecord
        {
            public string SlipID;
            public string BorrowerID;
            public string PropertyTag;
            public string Room;
            public DateTime Borrowed;
            public DateTime Due;
            public DateTime? Returned;
            public string Staff;
        }

        private readonly List<SchoolEquipment> _equipment = new List<SchoolEquipment>();
        private readonly List<SchoolBorrower> _borrowers = new List<SchoolBorrower>();
        private readonly List<SlipRecord> _slips = new List<SlipRecord>();

        public void Initialize()
        {
        }

        public bool IsEmpty()
        {
            return _equipment.Count == 0 && _borrowers.Count == 0;
        }

        public void SaveEquipment(SchoolEquipment item)
        {
            SchoolEquipment copy = CloneEquipment(item);
            int index = _equipment.FindIndex(e => string.Equals(e.PropertyTag, copy.PropertyTag, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                _equipment[index] = copy;
            }
            else
            {
                _equipment.Add(copy);
            }
        }

        public List<SchoolEquipment> LoadEquipment()
        {
            List<SchoolEquipment> result = new List<SchoolEquipment>();
            foreach (SchoolEquipment e in _equipment)
            {
                result.Add(CloneEquipment(e));
            }
            return result;
        }

        public void SaveBorrower(SchoolBorrower borrower)
        {
            SchoolBorrower copy = new SchoolBorrower(
                borrower.SchoolIDNumber, borrower.FullName, borrower.Role, borrower.Department, borrower.HasClearanceHold);
            int index = _borrowers.FindIndex(b => string.Equals(b.SchoolIDNumber, copy.SchoolIDNumber, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                _borrowers[index] = copy;
            }
            else
            {
                _borrowers.Add(copy);
            }
        }

        public List<SchoolBorrower> LoadBorrowers()
        {
            List<SchoolBorrower> result = new List<SchoolBorrower>();
            foreach (SchoolBorrower b in _borrowers)
            {
                result.Add(new SchoolBorrower(b.SchoolIDNumber, b.FullName, b.Role, b.Department, b.HasClearanceHold));
            }
            return result;
        }

        public void SaveSlip(BorrowSlip slip)
        {
            SaveBorrower(slip.Borrower);
            SaveEquipment(slip.Equipment);

            SlipRecord record = new SlipRecord();
            record.SlipID = slip.SlipID;
            record.BorrowerID = slip.Borrower.SchoolIDNumber;
            record.PropertyTag = slip.Equipment.PropertyTag;
            record.Room = slip.RoomToBeUsed;
            record.Borrowed = slip.TimeBorrowed;
            record.Due = slip.DueTime;
            record.Returned = slip.TimeReturned;
            record.Staff = slip.ReleasedByStaff;

            int index = _slips.FindIndex(s => string.Equals(s.SlipID, record.SlipID, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                _slips[index] = record;
            }
            else
            {
                _slips.Add(record);
            }
        }

        public List<BorrowSlip> LoadSlips(IList<SchoolBorrower> borrowers, IList<SchoolEquipment> equipment)
        {
            List<BorrowSlip> result = new List<BorrowSlip>();

            foreach (SlipRecord record in _slips)
            {
                SchoolBorrower borrower = null;
                foreach (SchoolBorrower b in borrowers)
                {
                    if (string.Equals(b.SchoolIDNumber, record.BorrowerID, StringComparison.OrdinalIgnoreCase))
                    {
                        borrower = b;
                        break;
                    }
                }

                SchoolEquipment item = null;
                foreach (SchoolEquipment e in equipment)
                {
                    if (string.Equals(e.PropertyTag, record.PropertyTag, StringComparison.OrdinalIgnoreCase))
                    {
                        item = e;
                        break;
                    }
                }

                if (borrower == null || item == null)
                {
                    continue;
                }

                BorrowSlip slip = new BorrowSlip(record.SlipID, borrower, item, record.Room, record.Borrowed, record.Due, record.Staff);
                slip.TimeReturned = record.Returned;
                result.Add(slip);
            }

            return result;
        }

        private static SchoolEquipment CloneEquipment(SchoolEquipment item)
        {
            AVEquipment av = item as AVEquipment;
            if (av != null)
            {
                return new AVEquipment(av.PropertyTag, av.ItemName, av.IncludesRemote, av.IncludesPowerCord,
                    av.IncludesHDMI, av.BorrowDurationHours, av.Condition, av.IsAvailable);
            }

            LabEquipment lab = item as LabEquipment;
            if (lab != null)
            {
                return new LabEquipment(lab.PropertyTag, lab.ItemName, lab.CalibrationDate, lab.ComponentCount,
                    lab.BorrowDurationDays, lab.Condition, lab.IsAvailable);
            }

            throw new NotSupportedException("Unknown equipment type: " + item.GetType().Name);
        }
    }
}
