using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.Controllers
{
    /// <summary>
    /// Keeps the list of all school equipment.
    /// Handles adding items, repairs, and saving/loading the inventory file.
    /// </summary>
    public class EquipmentController
    {
        private readonly List<SchoolEquipment> _inventory;
        private readonly string _storageFilePath;

        public EquipmentController()
            : this("equipment_inventory.txt")
        {
        }

        public EquipmentController(string storageFilePath)
        {
            _inventory = new List<SchoolEquipment>();
            _storageFilePath = storageFilePath;
        }

        public string StorageFilePath
        {
            get { return _storageFilePath; }
        }

        // ---------------------------------------------------------------
        // Adding and finding equipment
        // ---------------------------------------------------------------

        public void AddEquipment(SchoolEquipment item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Equipment item cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(item.PropertyTag))
            {
                throw new ArgumentException("Property tag cannot be empty.", "item");
            }

            if (string.IsNullOrWhiteSpace(item.ItemName))
            {
                throw new ArgumentException("Equipment name cannot be empty.", "item");
            }

            // The save file uses | to separate columns, so it can't be inside the text
            if (item.PropertyTag.Contains("|") || item.ItemName.Contains("|"))
            {
                throw new ArgumentException("Property tag and name cannot contain the '|' character.", "item");
            }

            if (FindEquipmentByTag(item.PropertyTag) != null)
            {
                throw new InvalidOperationException(string.Format("Equipment with PropertyTag '{0}' is already registered.", item.PropertyTag));
            }

            _inventory.Add(item);
        }

        public List<SchoolEquipment> GetAllEquipment()
        {
            return new List<SchoolEquipment>(_inventory);
        }

        public List<SchoolEquipment> GetAvailableEquipment()
        {
            List<SchoolEquipment> available = new List<SchoolEquipment>();
            foreach (SchoolEquipment item in _inventory)
            {
                if (item.IsAvailable)
                {
                    available.Add(item);
                }
            }
            return available;
        }

        public SchoolEquipment FindEquipmentByTag(string propertyTag)
        {
            if (string.IsNullOrEmpty(propertyTag))
            {
                return null;
            }

            foreach (SchoolEquipment item in _inventory)
            {
                if (string.Equals(item.PropertyTag, propertyTag, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        // ---------------------------------------------------------------
        // Repairs
        // ---------------------------------------------------------------

        public void MarkForRepair(string propertyTag)
        {
            SchoolEquipment item = FindEquipmentByTag(propertyTag);
            if (item == null)
            {
                throw new KeyNotFoundException(string.Format("No equipment found with PropertyTag '{0}'.", propertyTag));
            }

            item.Condition = SchoolEquipment.ConditionUnderRepair;
            item.IsAvailable = false;
        }

        /// <summary>
        /// Puts an item back into circulation after a repair (or after missing parts are found).
        /// </summary>
        public void RestoreToService(string propertyTag)
        {
            SchoolEquipment item = FindEquipmentByTag(propertyTag);
            if (item == null)
            {
                throw new KeyNotFoundException(string.Format("No equipment found with PropertyTag '{0}'.", propertyTag));
            }

            item.Condition = SchoolEquipment.ConditionGood;
            item.IsAvailable = true;
        }

        // ---------------------------------------------------------------
        // Saving and loading
        // File format (one item per line):
        //   PropertyTag|Type|ItemName|Condition|IsAvailable|Key=Value;Key=Value
        // ---------------------------------------------------------------

        public void SaveInventory()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("# PropertyTag|Type|ItemName|Condition|IsAvailable|ExtraData");

                foreach (SchoolEquipment item in _inventory)
                {
                    string extraData = string.Empty;

                    if (item is AVEquipment)
                    {
                        AVEquipment av = (AVEquipment)item;
                        extraData = string.Format("Remote={0};Cord={1};HDMI={2};Hours={3}",
                            av.IncludesRemote, av.IncludesPowerCord, av.IncludesHDMI, av.BorrowDurationHours);
                    }
                    else if (item is LabEquipment)
                    {
                        LabEquipment lab = (LabEquipment)item;
                        extraData = string.Format("Calibration={0:yyyy-MM-dd};Components={1};Days={2}",
                            lab.CalibrationDate, lab.ComponentCount, lab.BorrowDurationDays);
                    }

                    sb.AppendLine(string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                        item.PropertyTag, item.GetType().Name, item.ItemName, item.Condition, item.IsAvailable, extraData));
                }

                File.WriteAllText(_storageFilePath, sb.ToString());
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("Failed to save inventory to '{0}': {1}", _storageFilePath, ex.Message), ex);
            }
        }

        public void LoadInventory()
        {
            if (!File.Exists(_storageFilePath))
            {
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(_storageFilePath);
                _inventory.Clear();

                foreach (string rawLine in lines)
                {
                    string line = rawLine.Trim();

                    // Skip empty lines and the header comment
                    if (line.Length == 0 || line.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] parts = line.Split('|');
                    if (parts.Length < 5)
                    {
                        continue;
                    }

                    string tag = parts[0];
                    string type = parts[1];
                    string name = parts[2];
                    string condition = parts[3];
                    bool isAvailable;
                    bool.TryParse(parts[4], out isAvailable);

                    Dictionary<string, string> extras = new Dictionary<string, string>();
                    if (parts.Length > 5)
                    {
                        extras = ReadExtraValues(parts[5]);
                    }

                    if (type == "AVEquipment")
                    {
                        bool remote = ReadBool(extras, "Remote", false);
                        bool cord = ReadBool(extras, "Cord", true);
                        bool hdmi = ReadBool(extras, "HDMI", true);
                        int hours = ReadInt(extras, "Hours", AVEquipment.DefaultHours);

                        _inventory.Add(new AVEquipment(tag, name, remote, cord, hdmi, hours, condition, isAvailable));
                    }
                    else if (type == "LabEquipment")
                    {
                        DateTime calibration = DateTime.Now;
                        if (extras.ContainsKey("Calibration"))
                        {
                            DateTime.TryParse(extras["Calibration"], out calibration);
                        }
                        int components = ReadInt(extras, "Components", 1);
                        int days = ReadInt(extras, "Days", LabEquipment.DefaultDays);

                        _inventory.Add(new LabEquipment(tag, name, calibration, components, days, condition, isAvailable));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("Failed to load inventory from '{0}': {1}", _storageFilePath, ex.Message), ex);
            }
        }

        // Turns "Remote=True;Hours=4" into a dictionary { Remote: True, Hours: 4 }
        private static Dictionary<string, string> ReadExtraValues(string text)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            string[] pairs = text.Split(';');

            foreach (string pair in pairs)
            {
                string[] keyAndValue = pair.Split('=');
                if (keyAndValue.Length == 2)
                {
                    values[keyAndValue[0]] = keyAndValue[1];
                }
            }
            return values;
        }

        private static bool ReadBool(Dictionary<string, string> values, string key, bool defaultValue)
        {
            bool result;
            if (values.ContainsKey(key) && bool.TryParse(values[key], out result))
            {
                return result;
            }
            return defaultValue;
        }

        private static int ReadInt(Dictionary<string, string> values, string key, int defaultValue)
        {
            int result;
            if (values.ContainsKey(key) && int.TryParse(values[key], out result))
            {
                return result;
            }
            return defaultValue;
        }
    }
}
