using System;
using System.Collections.Generic;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// Audio-visual equipment such as projectors, speakers and smart displays.
    /// These are borrowed for a few HOURS and usually come with accessories.
    /// </summary>
    public class AVEquipment : SchoolEquipment
    {
        // Fine rules for AV items (in PHP)
        public const double FirstLateHourFine = 50.00;
        public const double ExtraLateHourFine = 25.00;
        public const int DefaultHours = 4;

        public bool IncludesRemote { get; set; }
        public bool IncludesPowerCord { get; set; }
        public bool IncludesHDMI { get; set; }
        public int BorrowDurationHours { get; set; }

        public AVEquipment()
            : base()
        {
            IncludesRemote = false;
            IncludesPowerCord = true;
            IncludesHDMI = true;
            BorrowDurationHours = DefaultHours;
        }

        public AVEquipment(string propertyTag, string itemName, bool includesRemote, bool includesPowerCord, bool includesHDMI, int durationHours = DefaultHours, string condition = ConditionGood, bool isAvailable = true)
            : base(propertyTag, itemName, condition, isAvailable)
        {
            IncludesRemote = includesRemote;
            IncludesPowerCord = includesPowerCord;
            IncludesHDMI = includesHDMI;

            // A loan of 0 or fewer hours makes no sense, so fall back to the default
            if (durationHours > 0)
            {
                BorrowDurationHours = durationHours;
            }
            else
            {
                BorrowDurationHours = DefaultHours;
            }
        }

        public int GetBorrowDurationHours()
        {
            return BorrowDurationHours;
        }

        /// <summary>
        /// AV loans are counted in hours, so this only returns whole days (usually 0).
        /// </summary>
        public override int GetBorrowDurationDays()
        {
            return BorrowDurationHours / 24;
        }

        public override DateTime CalculateDueTime(DateTime timeBorrowed)
        {
            return timeBorrowed.AddHours(BorrowDurationHours);
        }

        /// <summary>
        /// PHP 50 for the first late hour, then PHP 25 for every hour after that.
        /// A part of an hour counts as a full hour.
        /// </summary>
        public override double CalculateLateFine(TimeSpan lateBy)
        {
            if (lateBy.TotalMinutes <= 0)
            {
                return 0;
            }

            double lateHours = Math.Ceiling(lateBy.TotalHours);
            return FirstLateHourFine + ((lateHours - 1) * ExtraLateHourFine);
        }

        public override List<string> GetReturnChecklist()
        {
            List<string> checklist = new List<string>();
            checklist.Add(string.Format("Main AV Unit ({0})", ItemName));

            if (IncludesPowerCord)
            {
                checklist.Add("AC Power Adapter / Power Cord");
            }

            if (IncludesRemote)
            {
                checklist.Add("Remote Controller (with functional batteries)");
            }

            if (IncludesHDMI)
            {
                checklist.Add("High-Speed HDMI / Display Cable");
            }

            checklist.Add("Lens and Outer Shell Physical Integrity");
            return checklist;
        }

        public override string GetCategoryName()
        {
            return "AV Equipment";
        }

        public override string GetLoanPeriodText()
        {
            if (BorrowDurationHours == 1)
            {
                return "1 hour";
            }
            return BorrowDurationHours + " hours";
        }

        public override string GetDetailsText()
        {
            List<string> parts = new List<string>();
            if (IncludesRemote) parts.Add("Remote");
            if (IncludesPowerCord) parts.Add("Power cord");
            if (IncludesHDMI) parts.Add("HDMI");

            if (parts.Count == 0)
            {
                return "No accessories";
            }
            return string.Join(", ", parts);
        }

        public override string ToString()
        {
            return string.Format("{0} [AV: {1}h max, Remote:{2}, Cable:{3}, HDMI:{4}]",
                base.ToString(), BorrowDurationHours, IncludesRemote ? "Yes" : "No", IncludesPowerCord ? "Yes" : "No", IncludesHDMI ? "Yes" : "No");
        }
    }
}
