using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SchoolBorrowingSystem.Data;
using SchoolBorrowingSystem.Models;

namespace SchoolBorrowingSystem.Controllers
{
    /// <summary>
    /// Controller managing school equipment inventory, availability queries,
    /// maintenance lifecycle, and disk persistence.
    /// Demonstrates encapsulation of the inventory list and Single Responsibility Principle (SRP).
    /// </summary>
    public class EquipmentController
    {
        // UML-specified private field
        private readonly List<SchoolEquipment> _inventory;
        private readonly string _storageFilePath;
        private readonly ISchoolDataStore _store; // optional: null = no database

        public EquipmentController()
            : this("equipment_inventory.txt")
        {
        }

        public EquipmentController(string storageFilePath)
            : this(storageFilePath, null)
        {
        }

        public EquipmentController(string storageFilePath, ISchoolDataStore store)
        {
            _inventory = new List<SchoolEquipment>();
            _storageFilePath = storageFilePath;
            _store = store;
        }

        /// <summary>
        /// Adds a new equipment item to the school inventory.
        /// </summary>
        /// <param name="item">Equipment instance to register.</param>
        public void AddEquipment(SchoolEquipment item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Equipment item cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(item.PropertyTag))
            {
                throw new ArgumentException("Equipment PropertyTag cannot be empty or whitespace.", "item");
            }

            if (string.IsNullOrWhiteSpace(item.ItemName))
            {
                throw new ArgumentException("Equipment ItemName cannot be empty or whitespace.", "item");
            }

            // Ensure uniqueness of PropertyTag
            SchoolEquipment existing = FindEquipmentByTag(item.PropertyTag.Trim());
            if (existing != null)
            {
                throw new InvalidOperationException(string.Format("Equipment with PropertyTag '{0}' is already registered.", item.PropertyTag));
            }

            item.PropertyTag = item.PropertyTag.Trim();
            item.ItemName = item.ItemName.Trim();
            _inventory.Add(item);

            if (_store != null)
            {
                _store.SaveEquipment(item);
            }
        }

        /// <summary>
        /// Updates an existing equipment item with new specifications (name, tag, borrow time, accessories).
        /// </summary>
        /// <param name="originalTag">The current property tag of the item to update.</param>
        /// <param name="updatedItem">The updated equipment instance containing new values.</param>
        public void UpdateEquipment(string originalTag, SchoolEquipment updatedItem)
        {
            if (string.IsNullOrWhiteSpace(originalTag))
            {
                throw new ArgumentException("Original property tag cannot be empty.", "originalTag");
            }

            if (updatedItem == null)
            {
                throw new ArgumentNullException("updatedItem", "Updated equipment item cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(updatedItem.PropertyTag))
            {
                throw new ArgumentException("Updated property tag cannot be empty or whitespace.", "updatedItem");
            }

            if (string.IsNullOrWhiteSpace(updatedItem.ItemName))
            {
                throw new ArgumentException("Updated item name cannot be empty or whitespace.", "updatedItem");
            }

            SchoolEquipment existing = FindEquipmentByTag(originalTag);
            if (existing == null)
            {
                throw new KeyNotFoundException(string.Format("No equipment found with PropertyTag '{0}'.", originalTag));
            }

            string newTag = updatedItem.PropertyTag.Trim();
            bool tagChanged = !string.Equals(originalTag.Trim(), newTag, StringComparison.OrdinalIgnoreCase);

            if (tagChanged)
            {
                SchoolEquipment tagConflict = FindEquipmentByTag(newTag);
                if (tagConflict != null)
                {
                    throw new InvalidOperationException(string.Format("Equipment with PropertyTag '{0}' is already registered.", newTag));
                }
            }

            // Update common properties
            existing.PropertyTag = newTag;
            existing.ItemName = updatedItem.ItemName.Trim();
            existing.Condition = updatedItem.Condition;

            // Update polymorphic subtype properties
            if (existing is AVEquipment avExisting && updatedItem is AVEquipment avUpdated)
            {
                avExisting.IncludesRemote = avUpdated.IncludesRemote;
                avExisting.IncludesPowerCord = avUpdated.IncludesPowerCord;
                avExisting.IncludesHDMI = avUpdated.IncludesHDMI;
                avExisting.BorrowDurationHours = avUpdated.BorrowDurationHours;
            }
            else if (existing is LabEquipment labExisting && updatedItem is LabEquipment labUpdated)
            {
                labExisting.CalibrationDate = labUpdated.CalibrationDate;
                labExisting.ComponentCount = labUpdated.ComponentCount;
                labExisting.BorrowDurationDays = labUpdated.BorrowDurationDays;
            }

            if (_store != null)
            {
                if (tagChanged)
                {
                    _store.DeleteEquipment(originalTag);
                }
                _store.SaveEquipment(existing);
            }
        }

        /// <summary>
        /// Deletes an equipment item from the school inventory.
        /// Rejects deletion if the item is currently on loan.
        /// </summary>
        /// <param name="propertyTag">Property tag of the equipment to delete.</param>
        /// <param name="isCurrentlyBorrowed">Whether the equipment is currently held by an active loan.</param>
        public void DeleteEquipment(string propertyTag, bool isCurrentlyBorrowed = false)
        {
            if (string.IsNullOrWhiteSpace(propertyTag))
            {
                throw new ArgumentException("Property tag cannot be empty.", "propertyTag");
            }

            SchoolEquipment existing = FindEquipmentByTag(propertyTag);
            if (existing == null)
            {
                throw new KeyNotFoundException(string.Format("No equipment found with PropertyTag '{0}'.", propertyTag));
            }

            if (isCurrentlyBorrowed)
            {
                throw new InvalidOperationException(string.Format("Cannot delete equipment '{0}' while it is actively borrowed.", propertyTag));
            }

            _inventory.Remove(existing);

            if (_store != null)
            {
                _store.DeleteEquipment(propertyTag);
            }
        }

        /// <summary>
        /// Retrieves all equipment currently marked as available for borrowing.
        /// </summary>
        /// <returns>List of available equipment items.</returns>
        public List<SchoolEquipment> GetAvailableEquipment()
        {
            List<SchoolEquipment> available = new List<SchoolEquipment>();
            for (int i = 0; i < _inventory.Count; i++)
            {
                SchoolEquipment item = _inventory[i];
                if (item.IsAvailable)
                {
                    available.Add(item);
                }
            }
            return available;
        }

        /// <summary>
        /// Retrieves a copy of the entire equipment inventory.
        /// </summary>
        public List<SchoolEquipment> GetAllEquipment()
        {
            return new List<SchoolEquipment>(_inventory);
        }

        /// <summary>
        /// Locates an equipment item by its unique property tag.
        /// </summary>
        /// <param name="propertyTag">The unique property identifier.</param>
        /// <returns>Matching SchoolEquipment if found; otherwise null.</returns>
        public SchoolEquipment FindEquipmentByTag(string propertyTag)
        {
            if (string.IsNullOrEmpty(propertyTag))
            {
                return null;
            }

            for (int i = 0; i < _inventory.Count; i++)
            {
                SchoolEquipment item = _inventory[i];
                if (string.Equals(item.PropertyTag, propertyTag, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Marks an equipment item as requiring maintenance or repair,
        /// automatically revoking its availability for future loans.
        /// </summary>
        /// <param name="propertyTag">Property tag of the equipment needing repair.</param>
        public void MarkForRepair(string propertyTag)
        {
            SchoolEquipment item = FindEquipmentByTag(propertyTag);
            if (item == null)
            {
                throw new KeyNotFoundException(string.Format("No equipment found with PropertyTag '{0}'.", propertyTag));
            }

            item.Condition = "Under Repair";
            item.IsAvailable = false;

            if (_store != null)
            {
                _store.SaveEquipment(item);
            }
        }

        /// <summary>
        /// Completes a repair: the item goes back to Good condition.
        /// If the item is currently on loan to a borrower, it remains unavailable until physically returned.
        /// </summary>
        /// <param name="propertyTag">Property tag of the repaired equipment.</param>
        /// <param name="isCurrentlyBorrowed">True if an active loan slip currently holds this unit.</param>
        public void CompleteRepair(string propertyTag, bool isCurrentlyBorrowed = false)
        {
            SchoolEquipment item = FindEquipmentByTag(propertyTag);
            if (item == null)
            {
                throw new KeyNotFoundException(string.Format("No equipment found with PropertyTag '{0}'.", propertyTag));
            }

            item.Condition = "Good";
            item.IsAvailable = !isCurrentlyBorrowed;

            if (_store != null)
            {
                _store.SaveEquipment(item);
            }
        }

        /// <summary>
        /// Replaces the in-memory inventory with items loaded from the database at startup.
        /// Does not write anything back to the database.
        /// </summary>
        public void RestoreInventory(IEnumerable<SchoolEquipment> items)
        {
            _inventory.Clear();
            _inventory.AddRange(items);
        }

        /// <summary>
        /// Persists the current inventory snapshot to storage.
        /// Demonstrates structured, reliable file I/O.
        /// </summary>
        public void SaveInventory()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("# PropertyTag|Type|ItemName|Condition|IsAvailable|ExtraData");

                for (int i = 0; i < _inventory.Count; i++)
                {
                    SchoolEquipment item = _inventory[i];
                    string typeName = item.GetType().Name;
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
                        item.PropertyTag, typeName, item.ItemName, item.Condition, item.IsAvailable, extraData));
                }

                File.WriteAllText(_storageFilePath, sb.ToString());
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("Failed to save inventory to '{0}': {1}", _storageFilePath, ex.Message), ex);
            }
        }

        /// <summary>
        /// Loads inventory from the storage file if it exists.
        /// </summary>
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

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
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
                    bool isAvailable = false;
                    bool.TryParse(parts[4], out isAvailable);
                    string extra = parts.Length > 5 ? parts[5] : string.Empty;

                    if (type == "AVEquipment")
                    {
                        bool remote = false, cord = true, hdmi = true;
                        int hours = 4;

                        string[] extras = extra.Split(';');
                        for (int e = 0; e < extras.Length; e++)
                        {
                            string[] kv = extras[e].Split('=');
                            if (kv.Length == 2)
                            {
                                if (kv[0] == "Remote") bool.TryParse(kv[1], out remote);
                                else if (kv[0] == "Cord") bool.TryParse(kv[1], out cord);
                                else if (kv[0] == "HDMI") bool.TryParse(kv[1], out hdmi);
                                else if (kv[0] == "Hours") int.TryParse(kv[1], out hours);
                            }
                        }

                        AVEquipment av = new AVEquipment(tag, name, remote, cord, hdmi, hours, condition, isAvailable);
                        _inventory.Add(av);
                    }
                    else if (type == "LabEquipment")
                    {
                        DateTime calDate = DateTime.Now;
                        int compCount = 1;
                        int days = 7;

                        string[] extras = extra.Split(';');
                        for (int e = 0; e < extras.Length; e++)
                        {
                            string[] kv = extras[e].Split('=');
                            if (kv.Length == 2)
                            {
                                if (kv[0] == "Calibration") DateTime.TryParse(kv[1], out calDate);
                                else if (kv[0] == "Components") int.TryParse(kv[1], out compCount);
                                else if (kv[0] == "Days") int.TryParse(kv[1], out days);
                            }
                        }

                        LabEquipment lab = new LabEquipment(tag, name, calDate, compCount, days, condition, isAvailable);
                        _inventory.Add(lab);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException(string.Format("Failed to load inventory from '{0}': {1}", _storageFilePath, ex.Message), ex);
            }
        }
    }
}
