using System.Collections.Generic;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// Represents audiovisual equipment such as digital projectors, smart displays, and audio monitors.
    /// Typically loaned out for short-term durations (hours during a school day) with specific peripheral accessories.
    /// </summary>
    public class AVEquipment : SchoolEquipment
    {
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
            BorrowDurationHours = 4; // Default loan window: 4 hours
        }

        public AVEquipment(string propertyTag, string itemName, bool includesRemote, bool includesPowerCord, bool includesHDMI, int durationHours = 4, string condition = "Good", bool isAvailable = true)
            : base(propertyTag, itemName, condition, isAvailable)
        {
            IncludesRemote = includesRemote;
            IncludesPowerCord = includesPowerCord;
            IncludesHDMI = includesHDMI;
            BorrowDurationHours = durationHours > 0 ? durationHours : 4;
        }

        /// <summary>
        /// Returns specific borrow window in hours for AV devices.
        /// </summary>
        public int GetBorrowDurationHours()
        {
            return BorrowDurationHours;
        }

        /// <summary>
        /// Fulfills abstract base contract: AV equipment is measured primarily in hours for same-day operations,
        /// returning 0 days for same-day loans or days equivalent.
        /// </summary>
        public override int GetBorrowDurationDays()
        {
            return BorrowDurationHours >= 24 ? BorrowDurationHours / 24 : 0;
        }

        /// <summary>
        /// Generates a customized return checklist based on the peripherals bundled with this AV device.
        /// Demonstrates runtime polymorphism.
        /// </summary>
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

        public override string ToString()
        {
            return string.Format("{0} [AV: {1}h max, Remote:{2}, Cable:{3}, HDMI:{4}]",
                base.ToString(), BorrowDurationHours, IncludesRemote ? "Yes" : "No", IncludesPowerCord ? "Yes" : "No", IncludesHDMI ? "Yes" : "No");
        }
    }
}
