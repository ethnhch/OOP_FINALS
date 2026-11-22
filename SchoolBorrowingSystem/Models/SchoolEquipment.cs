using System;
using System.Collections.Generic;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// Base class for every item the school lends out.
    /// It holds the shared data (tag, name, condition, availability) and forces
    /// each kind of equipment to decide its own loan rules (due time, late fine, checklist).
    /// </summary>
    public abstract class SchoolEquipment
    {
        // Condition values used across the whole program.
        // Keeping them here means we never mistype "Under Repair" somewhere else.
        public const string ConditionGood = "Good";
        public const string ConditionFair = "Fair";
        public const string ConditionDamaged = "Damaged";
        public const string ConditionNeedsRepair = "Needs Repair";
        public const string ConditionUnderRepair = "Under Repair";
        public const string ConditionMissingParts = "Missing Parts";

        // Private fields (from the UML diagram)
        private string _propertyTag;
        private string _itemName;
        private string _condition;
        private bool _isAvailable;

        public string PropertyTag
        {
            get { return _propertyTag; }
            set { _propertyTag = value; }
        }

        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public string Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set { _isAvailable = value; }
        }

        /// <summary>
        /// Short text used in drop-down lists, for example "AV-101 - Epson Projector".
        /// </summary>
        public string DisplayName
        {
            get { return _propertyTag + " - " + _itemName; }
        }

        protected SchoolEquipment()
        {
            _propertyTag = string.Empty;
            _itemName = string.Empty;
            _condition = ConditionGood;
            _isAvailable = true;
        }

        protected SchoolEquipment(string propertyTag, string itemName, string condition = ConditionGood, bool isAvailable = true)
        {
            _propertyTag = propertyTag;
            _itemName = itemName;
            _condition = condition;
            _isAvailable = isAvailable;
        }

        // ---------------------------------------------------------------
        // Abstract methods: every subclass MUST write its own version.
        // ---------------------------------------------------------------

        /// <summary>How many days the item can be borrowed (0 means same-day loan).</summary>
        public abstract int GetBorrowDurationDays();

        /// <summary>The things staff must check when the item comes back.</summary>
        public abstract List<string> GetReturnChecklist();

        /// <summary>Works out when the item has to be returned.</summary>
        public abstract DateTime CalculateDueTime(DateTime timeBorrowed);

        /// <summary>Works out the late fine (in PHP) for an item returned late by the given amount of time.</summary>
        public abstract double CalculateLateFine(TimeSpan lateBy);

        /// <summary>Friendly category name shown in the tables, e.g. "AV Equipment".</summary>
        public abstract string GetCategoryName();

        /// <summary>Human readable loan length, e.g. "4 hours" or "7 days".</summary>
        public abstract string GetLoanPeriodText();

        /// <summary>Short list of extra details for the inventory table, e.g. "Remote, HDMI".</summary>
        public abstract string GetDetailsText();

        public override string ToString()
        {
            return string.Format("[{0}] {1} | Condition: {2} | Available: {3}",
                _propertyTag, _itemName, _condition, _isAvailable ? "YES" : "NO");
        }
    }
}
