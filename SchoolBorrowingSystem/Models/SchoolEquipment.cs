using System;
using System.Collections.Generic;

namespace SchoolBorrowingSystem.Models
{
    /// <summary>
    /// Abstract base class defining common state and operations for all school equipment.
    /// Demonstrates encapsulation through private fields and public properties,
    /// and abstraction via contract methods for borrowing duration and return checklists.
    /// </summary>
    public abstract class SchoolEquipment
    {
        // UML-specified private fields
        private string _propertyTag;
        private string _itemName;
        private string _condition;
        private bool _isAvailable;

        // Public properties encapsulating private fields
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

        protected SchoolEquipment()
        {
            _propertyTag = string.Empty;
            _itemName = string.Empty;
            _condition = "Good";
            _isAvailable = true;
        }

        protected SchoolEquipment(string propertyTag, string itemName, string condition = "Good", bool isAvailable = true)
        {
            _propertyTag = propertyTag;
            _itemName = itemName;
            _condition = condition;
            _isAvailable = isAvailable;
        }

        /// <summary>
        /// Abstract contract specifying loan duration in days.
        /// Derived classes must provide domain-specific implementation.
        /// </summary>
        public abstract int GetBorrowDurationDays();

        /// <summary>
        /// Abstract contract defining the physical and operational inspection checklist
        /// that must be validated upon equipment return.
        /// </summary>
        public abstract List<string> GetReturnChecklist();

        public override string ToString()
        {
            return string.Format("[{0}] {1} | Condition: {2} | Available: {3}",
                _propertyTag, _itemName, _condition, _isAvailable ? "YES" : "NO");
        }
    }
}
