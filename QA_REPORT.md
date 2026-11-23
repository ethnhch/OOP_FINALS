# Quality Assurance (QA) Comprehensive System Audit Report
**System:** School Equipment Borrowing Management System  
**Target Platform:** Microsoft Visual Studio 2019 / 2022 (C# / .NET 8 Windows Forms)  
**Document Version:** 2.0.0  
**Audit Date:** November 23, 2026  
**Auditor:** Quality Assurance & Senior Backend Engineering Review  
**Initial Assessment Status:** ⚠️ **CRITICAL ISSUES IDENTIFIED - REMEDIATION REQUIRED**

---

## 1. Executive Summary

A comprehensive architectural, logic, and operational audit was conducted on the **School Equipment Borrowing Management System**. The system is a Windows Forms desktop application designed for managing school equipment loans (AV and Laboratory equipment), tracking borrowers (students, faculty, staff), processing checkout slips, handling returns, and managing administrative clearance holds.

While the codebase features a stylized GTA 6-inspired user interface and foundational OOP abstractions (`SchoolEquipment`, `AVEquipment`, `LabEquipment`), the previous QA Report (v1.0.0) claimed a "100% compliant / production ready" status based on 44 isolated unit assertions. This initial assessment was **falsely clean** and obscured critical operational, data persistence, and state synchronization defects.

The system currently operates as a partial **CRU (Create, Read, Update)** model rather than a complete **CRUD (Create, Read, Update, Delete)** architecture, lacks database wiring, exhibits state desynchronization upon inventory reloads and unit restoration, and contains hardcoded domain logic.

---

## 2. Comprehensive Subsystem Audit Matrix

| Subsystem / Layer | Component / File | Functional Intent | Current State / Defects Found | Severity |
| :--- | :--- | :--- | :--- | :--- |
| **Data Persistence** | `ISchoolDataStore`, `SqlServerSchoolDataStore`, `InMemorySchoolDataStore` | Abstract persistence contract & SQL Server LocalDB implementation | **Disconnected:** Classes were created in commit `5336c4a` but are **never instantiated or injected** in `Program.cs`. Both controllers default to `_store = null`. Data is not persisted to `SchoolDB.mdf`. | **MAJOR** |
| **Persistence / UI** | `MainForm.cs`, `EquipmentController.cs` | File save/load via text file | Flat-file I/O only saves equipment; borrowers and active/historical borrow slips are never written to disk. All loan transactions are lost on exit. | **MAJOR** |
| **Inventory State Lifecycle** | `EquipmentController.CompleteRepair`, `MainForm.BtnCompleteRepair_Click` | Restore flagged units to service | **Logic Bug:** "Restore Unit" sets `item.IsAvailable = true` unconditionally without verifying if the item is currently checked out on an active borrow slip. Allows double-booking active equipment. | **MAJOR** |
| **Memory Synchronization** | `EquipmentController.LoadInventory`, `BorrowingController.ProcessReturn` | Reload inventory from disk | **Reference Desynchronization:** `LoadInventory()` clears `_inventory` and creates new instances. Pre-existing `BorrowSlip` objects retain references to obsolete memory instances. Returns update the orphan instance, leaving inventory items permanently `Unavailable`. | **MAJOR** |
| **Equipment CRUD** | `EquipmentController.cs`, `AddEquipmentForm.cs`, `MainForm.cs` | Inventory management | **No Update & No Delete:** Equipment cannot be edited (cannot modify PropertyTag, Name, Duration, or Accessories) and cannot be deleted. Inventory is append-only. | **MAJOR** |
| **Borrower CRUD** | `MainForm.cs`, `SchoolBorrower.cs` | Borrower records & management | **No Add & No Edit:** Borrowers are hardcoded to 4 seed records in `Program.cs`. The UI provides no capability to register new borrowers or edit details (name, department, role). | **MAJOR** |
| **Business Logic / Penalties** | `BorrowingController.ProcessReturn`, `MainForm.cs` | Return penalty calculation | Cash fee calculations (`PHP 50-300`) are present but conflict with academic requirements where non-financial clearance holds and repair ticketing govern school asset management. | **MAJOR** |
| **Return State Logic** | `BorrowingController.ProcessReturn` | Missing parts / incomplete checklist return | When an item is returned with missing accessories, its condition remains recorded as `"Good"` while `IsAvailable` becomes `false`. Staff has no visual indication of missing parts except manual "Restore Unit". | **MINOR** |
| **Service Layer Validation** | `EquipmentController.cs`, `BorrowingController.cs` | Input sanitization & invariant enforcement | `AddEquipment` accepts whitespace-only tags (`"   "`). `ProcessBorrow` does not verify whether the borrowed equipment exists within the controller's registered inventory. | **MINOR** |
| **Equipment Configuration** | `AddEquipmentForm.cs` | Lab apparatus registration | Lab apparatus component count is hardcoded to `3` in `AddEquipmentForm.BtnAdd_Click` with no UI input control provided. | **MINOR** |
| **Dead Code / UI Redundancy** | `BorrowForm.cs`, `ReturnForm.cs` | Modal dialog workflows | Standalone modal forms `BorrowForm` and `ReturnForm` are never shown. `MainForm` embeds equivalent tabs inline, leaving these files unused. | **MINOR** |

---

## 3. Detailed Root Cause Analysis of Major Problems

### 3.1. Database Disconnection & Lack of Persistence
- **Observation:** `SchoolDB.mdf` and `SqlServerSchoolDataStore.cs` exist in the repository. However, `Program.cs` line 44 initializes:
  ```csharp
  _equipmentController = new EquipmentController("school_inventory.txt");
  _borrowingController = new BorrowingController(_equipmentController);
  ```
  The overload accepting `ISchoolDataStore` is bypassed.
- **Root Cause:** Persistence was developed in isolation and not wired into the application bootstrap. Additionally, no fallback mechanism exists if SQL Server LocalDB instance `(LocalDB)\MSSQLLocalDB` is uninstalled or unreachable on client machines.
- **Remediation Strategy:** Inject `ISchoolDataStore` into both controllers. Implement an automatic initialization routine with graceful multi-tier storage (SQL Server LocalDB primary with transparent structured file fallback), ensuring reliable persistence across all execution environments.

### 3.2. "Restore Unit" Bug on Currently Borrowed Equipment
- **Observation:** If an item is flagged "Under Repair" while actively checked out (e.g. reported damaged by field staff before physical check-in), clicking "Restore Unit" immediately marks `IsAvailable = true`.
- **Root Cause:** `BtnCompleteRepair_Click` in `MainForm.cs` directly mutates `item.IsAvailable = true`, and `EquipmentController.CompleteRepair` lacks awareness of ongoing borrow slips.
- **Remediation Strategy:** `CompleteRepair` must inspect `BorrowingController.GetActiveSlips()`. If an active slip exists for the property tag, condition is restored to `"Good"`, but `IsAvailable` remains `false` until the physical slip is returned.

### 3.3. "Load Inventory" Breaking Returns
- **Observation:** If a user borrows an item, clicks "Load Inventory", and later attempts to process a return, the item remains stuck as `UNAVAILABLE` in the inventory grid.
- **Root Cause:** `EquipmentController.LoadInventory()` executes `_inventory.Clear()` and instantiates new equipment objects. The active `BorrowSlip.Equipment` reference points to the discarded object in heap memory. When `ProcessReturn` executes `slip.Equipment.IsAvailable = true`, it mutates the disconnected instance.
- **Remediation Strategy:** When `ProcessReturn` or inventory lookup occurs, always resolve equipment against the live `EquipmentController` registry by `PropertyTag`. Furthermore, `LoadInventory` and `RestoreInventory` must preserve or rebind active references.

### 3.4. Lack of Equipment Update and Delete (Full CRUD)
- **Observation:** Inventory only allows adding items. If a property tag is mistyped, a name changes, or loan duration needs adjustment, there is no edit mechanism. Decommissioned or lost equipment cannot be deleted.
- **Remediation Strategy:**
  1. Add `UpdateEquipment(SchoolEquipment item)` in `EquipmentController` and `ISchoolDataStore`.
  2. Add `DeleteEquipment(string propertyTag)` in `EquipmentController` and `ISchoolDataStore` with guard checks rejecting deletion if an active loan is outstanding.
  3. Provide UI controls (Edit and Delete buttons, edit dialog) in `MainForm`.

### 3.5. Lack of Borrower Management (Add & Edit)
- **Observation:** The borrower roster cannot be expanded or edited in the UI.
- **Remediation Strategy:**
  1. Implement `BorrowerForm` for adding and editing borrowers (ID, Full Name, Role, Department).
  2. Expose Add and Edit buttons on the "Borrowers & Holds" tab in `MainForm`.
  3. Ensure all borrower changes persist immediately to the data store.

### 3.6. Falsely Clean Automated QA Report
- **Observation:** Previous test suite in `TestRunner.cs` passed 44 assertions by testing only "happy path" synthetic scenarios and never evaluating real-world interactions (such as repairing borrowed gear or reloading inventory).
- **Remediation Strategy:** Expand `TestRunner.cs` to test the new CRUD methods, boundary validations, persistence roundtrips, and regression scenarios for the discovered bugs.

### 3.7. Penalty System Removal
- **Observation:** The system assesses monetary fines (`PHP 50-300`) upon return. Academic specifications prioritize non-monetary operational enforcement (clearance holds, repair routing, return condition tracking).
- **Remediation Strategy:** Remove financial penalty calculations while retaining and strengthening administrative clearance hold logic and equipment condition updates.

---

## 4. Minor Defect Analysis

1. **Missing Accessories Return State:**
   - In `ProcessReturn`, if `checklistComplete == false`, `slip.Equipment.Condition` was set to whatever was selected in the dropdown (often "Good"). The condition will now be explicitly updated to `"Missing Accessories / Incomplete"` so inventory displays the accurate physical state.
2. **Missing Service-Layer Validation:**
   - `EquipmentController.AddEquipment` now rejects null, empty, or whitespace-only tags.
   - `BorrowingController.ProcessBorrow` now validates that `item` is registered in `EquipmentController`'s inventory before issuing a slip.
3. **Hardcoded Component Count:**
   - `AddEquipmentForm` will be updated with a `NumericUpDown` for Lab Apparatus `ComponentCount`, allowing custom values (1 to 50) rather than a fixed value of 3.
4. **Dead Dialog Code:**
   - Either wire up `BorrowForm` and `ReturnForm` or remove/refactor redundant dead methods so the codebase remains clean and maintainable.

---

## 5. Verification & Action Plan

Each issue will be addressed individually in strict priority order, with code refactoring adhering to OOP best practices, no spaghetti code, humanized concise code structure, Visual Studio compatibility, and a dedicated Git commit per item.
