using System;
using System.Collections.Generic;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// Laboratory equipment such as microscopes and oscilloscopes.
    /// These are borrowed for several DAYS and have parts that must all come back.
    /// </summary>
    public class LabEquipment : SchoolEquipment
    {
        // Fine rules for lab items (in PHP)
        public const double FirstLateDayFine = 100.00;
        public const double ExtraLateDayFine = 50.00;
        public const int DefaultDays = 7;

        public DateTime CalibrationDate { get; set; }
        public int ComponentCount { get; set; }
        public int BorrowDurationDays { get; set; }

        public LabEquipment()
            : base()
        {
            CalibrationDate = DateTime.Now;
            ComponentCount = 1;
            BorrowDurationDays = DefaultDays;
        }

        public LabEquipment(string propertyTag, string itemName, DateTime calibrationDate, int componentCount, int durationDays = DefaultDays, string condition = ConditionGood, bool isAvailable = true)
            : base(propertyTag, itemName, condition, isAvailable)
        {
            CalibrationDate = calibrationDate;

            // Every item has at least 1 part (the item itself)
            if (componentCount > 0)
            {
                ComponentCount = componentCount;
            }
            else
            {
                ComponentCount = 1;
            }

            if (durationDays > 0)
            {
                BorrowDurationDays = durationDays;
            }
            else
            {
                BorrowDurationDays = DefaultDays;
            }
        }

        public override int GetBorrowDurationDays()
        {
            return BorrowDurationDays;
        }

        public override DateTime CalculateDueTime(DateTime timeBorrowed)
        {
            return timeBorrowed.AddDays(BorrowDurationDays);
        }

        /// <summary>
        /// PHP 100 for the first late day, then PHP 50 for every day after that.
        /// A part of a day counts as a full day.
        /// </summary>
        public override double CalculateLateFine(TimeSpan lateBy)
        {
            if (lateBy.TotalMinutes <= 0)
            {
                return 0;
            }

            double lateDays = Math.Ceiling(lateBy.TotalDays);
            return FirstLateDayFine + ((lateDays - 1) * ExtraLateDayFine);
        }

        public override List<string> GetReturnChecklist()
        {
            List<string> checklist = new List<string>();
            checklist.Add(string.Format("Main Apparatus ({0})", ItemName));
            checklist.Add(string.Format("{0} Discrete Component(s) / Glassware / Accessories verified present", ComponentCount));
            checklist.Add(string.Format("Calibration Seal Valid (Calibration Date: {0})", CalibrationDate.ToString("yyyy-MM-dd")));
            checklist.Add("Biological / Chemical Residue Inspection and Decontamination Check");
            return checklist;
        }

        public override string GetCategoryName()
        {
            return "Lab Equipment";
        }

        public override string GetLoanPeriodText()
        {
            if (BorrowDurationDays == 1)
            {
                return "1 day";
            }
            return BorrowDurationDays + " days";
        }

        public override string GetDetailsText()
        {
            return string.Format("{0} part(s), calibrated {1:MMM d, yyyy}", ComponentCount, CalibrationDate);
        }

        public override string ToString()
        {
            return string.Format("{0} [Lab: {1} days max, Components: {2}, Calibrated: {3}]",
                base.ToString(), BorrowDurationDays, ComponentCount, CalibrationDate.ToString("yyyy-MM-dd"));
        }
    }
}
