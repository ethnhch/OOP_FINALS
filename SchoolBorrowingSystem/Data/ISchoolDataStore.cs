using System.Collections.Generic;
using SchoolBorrowingSystem.Models;
using SchoolBorrowingSystem.Transactions;

namespace SchoolBorrowingSystem.Data
{
    /// <summary>
    /// Contract for saving and loading the system's data.
    /// The controllers depend on this interface, not on a specific database,
    /// so SQLite can be swapped for SQL Server (or anything else) without touching them.
    /// </summary>
    public interface ISchoolDataStore
    {
        /// <summary>Creates the database file and tables if they do not exist yet.</summary>
        void Initialize();

        /// <summary>True when the database has no equipment and no borrowers (first run).</summary>
        bool IsEmpty();

        /// <summary>Inserts the equipment, or updates it if the PropertyTag already exists.</summary>
        void SaveEquipment(SchoolEquipment item);

        /// <summary>Deletes the equipment record with the specified property tag.</summary>
        void DeleteEquipment(string propertyTag);

        /// <summary>Loads all equipment as AVEquipment / LabEquipment objects.</summary>
        List<SchoolEquipment> LoadEquipment();

        /// <summary>Inserts the borrower, or updates it if the school ID already exists.</summary>
        void SaveBorrower(SchoolBorrower borrower);

        /// <summary>Deletes the borrower record with the specified school ID.</summary>
        void DeleteBorrower(string schoolIdNumber);

        /// <summary>Loads all borrowers.</summary>
        List<SchoolBorrower> LoadBorrowers();

        /// <summary>
        /// Saves a slip together with its borrower and equipment in one transaction,
        /// so a borrow or return is stored completely or not at all.
        /// </summary>
        void SaveSlip(BorrowSlip slip);

        /// <summary>
        /// Loads every slip (active and returned), linking each one to the SAME borrower and
        /// equipment objects that are already in memory.
        /// </summary>
        List<BorrowSlip> LoadSlips(IList<SchoolBorrower> borrowers, IList<SchoolEquipment> equipment);
    }
}
