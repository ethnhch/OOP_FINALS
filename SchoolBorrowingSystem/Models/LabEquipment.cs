using System;
using System.Collections.Generic;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// Represents scientific or technical laboratory equipment (e.g., compound microscopes, digital oscilloscopes).
    /// Typically loaned for multiple days and requires rigorous calibration verification and component tracking.
    /// </summary>
    public class LabEquipment : SchoolEquipment
    {
        public DateTime CalibrationDate { get; set; }
        public int ComponentCount { get; set; }
        public int BorrowDurationDays { get; set; }

        public LabEquipment()
            : base()
        {
            CalibrationDate = DateTime.Now;
            ComponentCount = 1;
            BorrowDurationDays = 7; // Default loan window: 7 days
        }

        public LabEquipment(string propertyTag, string itemName, DateTime calibrationDate, int componentCount, int durationDays = 7, string condition = "Good", bool isAvailable = true)
            : base(propertyTag, itemName, condition, isAvailable)
        {
            CalibrationDate = calibrationDate;
            ComponentCount = componentCount > 0 ? componentCount : 1;
            BorrowDurationDays = durationDays > 0 ? durationDays : 7;
        }

        /// <summary>
        /// Fulfills base contract: returns multi-day loan allowance for lab equipment.
        /// </summary>
        public override int GetBorrowDurationDays()
        {
            return BorrowDurationDays;
        }

        /// <summary>
        /// Generates a laboratory-grade inspection checklist, verifying calibration validity,
        /// all modular components, and biological/chemical contamination safety.
        /// Demonstrates runtime polymorphism.
        /// </summary>
        public override List<string> GetReturnChecklist()
        {
            List<string> checklist = new List<string>();
            checklist.Add(string.Format("Main Apparatus ({0})", ItemName));
            checklist.Add(string.Format("{0} Discrete Component(s) / Glassware / Accessories verified present", ComponentCount));
            checklist.Add(string.Format("Calibration Seal Valid (Calibration Date: {0})", CalibrationDate.ToString("yyyy-MM-dd")));
            checklist.Add("Biological / Chemical Residue Inspection and Decontamination Check");
            return checklist;
        }

        public override string ToString()
        {
            return string.Format("{0} [Lab: {1} days max, Components: {2}, Calibrated: {3}]",
                base.ToString(), BorrowDurationDays, ComponentCount, CalibrationDate.ToString("yyyy-MM-dd"));
        }
    }
}
