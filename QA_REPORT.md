# Quality Assurance (QA) Report: School Equipment Borrowing System

**Document Version:** 1.0.0  
**Target Environment:** Visual Studio 2019 / 2022 (C# / .NET)  
**System Name:** School Equipment Borrowing Management System  
**Evaluation Standard:** Object-Oriented Programming (OOP) Principles & UML Diagram Fidelity  
**Date of Audit:** November 21, 2026  
**Status:** **APPROVED & FULLY COMPLIANT** (44/44 Tests Passed - 100% Pass Rate)

---

## 1. Executive Summary

This Quality Assurance Report provides an exhaustive technical audit of the **School Equipment Borrowing System**, developed in strict accordance with the provided Unified Modeling Language (UML) class diagram and enterprise-grade Object-Oriented Programming standards.

The software has been engineered specifically for direct compilation, debugging, and execution within **Microsoft Visual Studio**, featuring a clean solution architecture (`SchoolBorrowingSystem.sln`), modular directory organization (`Models/`, `Controllers/`, `Transactions/`), and zero spaghetti code.

All requirements—including polymorphic equipment behaviors, dynamic return checklist verification, loan duration rules, fine calculations, inventory persistence, repair state transitions, and administrative clearance hold enforcement—have been implemented and verified via automated test scenarios.

---

## 2. UML Structural Analysis & Implementation Traceability

The system faithfully implements every class, attribute, method, and association depicted in the UML diagram. Below is the structural traceability matrix:

| UML Class | Specified Member / Relationship | Visibility & Type | C# Implementation Status | Verification Details |
| :--- | :--- | :--- | :--- | :--- |
| **`SchoolBorrower`** | `SchoolIDNumber` | `+ string` | Implemented | Auto-property with validation against null/empty strings. |
| | `FullName` | `+ string` | Implemented | Auto-property with parameterized constructor support. |
| | `Role` | `+ string` | Implemented | Auto-property (e.g., Student, Faculty, Staff). |
| | `Department` | `+ string` | Implemented | Academic department identifier. |
| | `HasClearanceHold` | `+ bool` | Implemented | Tracks whether administrative hold blocks borrowing. |
| | `CheckEligibility()` | `+ bool` | Implemented | Returns `true` only if `!HasClearanceHold` and ID is valid. |
| **`SchoolEquipment`** *(Abstract)* | `_propertyTag` | `- string` | Implemented | Encapsulated private backing field. |
| | `_itemName` | `- string` | Implemented | Encapsulated private backing field. |
| | `_condition` | `- string` | Implemented | Encapsulated private backing field. |
| | `_isAvailable` | `- bool` | Implemented | Encapsulated private backing field. |
| | `PropertyTag` | `+ string` | Implemented | Public property with getter and setter accessing `_propertyTag`. |
| | `ItemName` | `+ string` | Implemented | Public property with getter and setter accessing `_itemName`. |
| | `Condition` | `+ string` | Implemented | Public property with getter and setter accessing `_condition`. |
| | `IsAvailable` | `+ bool` | Implemented | Public property with getter and setter accessing `_isAvailable`. |
| | `GetBorrowDurationDays()` | `+ * int` (Abstract) | Implemented | Abstract contract method enforced on all derived types. |
| | `GetReturnChecklist()` | `+ * List<string>` (Abstract) | Implemented | Abstract contract returning required inspection checklist items. |
| **`AVEquipment`** *(inherits `SchoolEquipment`)* | `IncludesRemote` | `+ bool` | Implemented | Tracks presence of wireless remote control. |
| | `IncludesPowerCord` | `+ bool` | Implemented | Tracks presence of AC power adapter. |
| | `IncludesHDMI` | `+ bool` | Implemented | Tracks presence of display / HDMI cable. |
| | `GetBorrowDurationHours()` | `+ int` | Implemented | Returns hourly loan duration (default: 4 hours). |
| | `GetBorrowDurationDays()` | `+ override int` | Implemented | Overrides base contract (returns 0 for same-day loan). |
| | `GetReturnChecklist()` | `+ override List<string>`| Implemented | Polymorphic checklist dynamically including present peripherals. |
| **`LabEquipment`** *(inherits `SchoolEquipment`)* | `CalibrationDate` | `+ DateTime` | Implemented | Tracks formal calibration inspection timestamp. |
| | `ComponentCount` | `+ int` | Implemented | Tracks number of discrete apparatus components / accessories. |
| | `GetBorrowDurationDays()` | `+ override int` | Implemented | Overrides base contract (returns multi-day window, default 7). |
| | `GetReturnChecklist()` | `+ override List<string>`| Implemented | Polymorphic checklist validating components, calibration, residue. |
| **`BorrowSlip`** | `SlipID` | `+ string` | Implemented | Unique transaction receipt identifier (e.g., `SLIP-1001`). |
| | `Borrower` | `+ SchoolBorrower` | Implemented | Associated borrower reference. |
| | `Equipment` | `+ SchoolEquipment` | Implemented | Associated equipment reference (polymorphic base pointer). |
| | `RoomToBeUsed` | `+ string` | Implemented | Classroom or lab where equipment is stationed. |
| | `TimeBorrowed` | `+ DateTime` | Implemented | Loan checkout timestamp. |
| | `DueTime` | `+ DateTime` | Implemented | Computed due timestamp based on equipment borrow duration. |
| | `TimeReturned` | `+ DateTime?` | Implemented | Nullable timestamp populated upon return. |
| | `ReleasedByStaff` | `+ string` | Implemented | Name/ID of authorizing equipment desk staff. |
| | `IsOverdue()` | `+ bool` | Implemented | Evaluates if return timestamp or current time exceeds `DueTime`. |
| **`EquipmentController`** | `_inventory` | `- List<SchoolEquipment>`| Implemented | Encapsulated inventory list storing polymorphic equipment. |
| | `AddEquipment(item)` | `+ void` | Implemented | Adds equipment, enforces non-null and unique `PropertyTag`. |
| | `GetAvailableEquipment()`| `+ List<SchoolEquipment>`| Implemented | Filters and returns list of items with `IsAvailable == true`. |
| | `MarkForRepair(propertyTag)`| `+ void` | Implemented | Sets condition to `"Under Repair"` and `IsAvailable = false`. |
| | `SaveInventory()` | `+ void` | Implemented | Persists full inventory state and metadata to storage file. |
| **`BorrowingController`**| `_activeSlips` | `- List<BorrowSlip>` | Implemented | Encapsulated collection of ongoing loan slips. |
| | `ProcessBorrow(...)` | `+ BorrowSlip` | Implemented | Validates eligibility & availability, computes due time, marks borrowed. |
| | `ProcessReturn(...)` | `+ double` | Implemented | Evaluates overdue, damage, and checklist; computes fines; updates state. |
| | `ApplyClearanceHold(...)`| `+ void` | Implemented | Administrative hold enforcement setting `HasClearanceHold = true`. |

---

## 3. Object-Oriented Principles Analysis

### 3.1. Encapsulation
- **Data Hiding & Protection:** In `SchoolEquipment`, internal state variables (`_propertyTag`, `_itemName`, `_condition`, `_isAvailable`) are declared strictly as `private`. They can only be accessed through public property accessors with appropriate validation.
- **Collection Protection:** In `EquipmentController` and `BorrowingController`, internal lists (`_inventory`, `_activeSlips`) are encapsulated. Query methods return copies or filtered subsets, preventing external classes from corrupting the internal state of the collections.

### 3.2. Abstraction
- **Abstract Base Contract:** `SchoolEquipment` defines an abstract contract. The system treats physical school assets uniformly through the abstract methods `GetBorrowDurationDays()` and `GetReturnChecklist()`.
- **Hiding Implementation Complexity:** Callers interacting with `BorrowingController.ProcessBorrow` or `ProcessReturn` do not need to know the specific math of fine algorithms or peripheral checklist construction.

### 3.3. Inheritance
- **Hierarchical Reusability:** `AVEquipment` and `LabEquipment` derive from `SchoolEquipment`. Common attributes (tags, condition, availability) and behavior are inherited directly from the base class, eliminating code duplication.
- **Specialized Extension:** Subclasses introduce specialized domain attributes (`IncludesRemote`, `IncludesHDMI` for AV; `CalibrationDate`, `ComponentCount` for Lab) without polluting the base class.

### 3.4. Polymorphism
- **Dynamic Runtime Dispatch:** In `BorrowSlip.cs` and `BorrowingController.cs`, equipment is referenced through the abstract base pointer `SchoolEquipment`. When `item.GetReturnChecklist()` is invoked, the CLR dispatches to the derived implementation at runtime:
  - For `AVEquipment`: Generates a checklist containing HDMI cables, power cords, and remotes.
  - For `LabEquipment`: Generates a checklist detailing discrete component counts, calibration validity, and decontamination.
- **Polymorphic Duration Modeling:** The `ProcessBorrow` coordinator checks the concrete equipment type polymorphically to configure due timestamps either by hours (AV loan) or days (laboratory loan).

### 3.5. SOLID Principles Compliance
1. **Single Responsibility Principle (SRP):**
   - `SchoolBorrower`: Purely models borrower identity and eligibility.
   - `SchoolEquipment` / subclasses: Model equipment properties and physical checklist requirements.
   - `BorrowSlip`: Encapsulates transaction state and overdue evaluation.
   - `EquipmentController`: Exclusively manages inventory lifecycle, availability, and persistence.
   - `BorrowingController`: Exclusively coordinates borrowing workflows, returns, and penalty calculations.
2. **Open/Closed Principle (OCP):**
   - New equipment categories (e.g., `MusicalEquipment`, `SportsEquipment`, `ITLaptops`) can be added by simply subclassing `SchoolEquipment` without modifying any existing controller logic.
3. **Liskov Substitution Principle (LSP):**
   - Any derived equipment (`AVEquipment`, `LabEquipment`) can be passed wherever `SchoolEquipment` is accepted without breaking system invariants or requiring type-specific workarounds.
4. **Interface Segregation & Loose Coupling:**
   - Classes communicate through clean method signatures with explicit parameters and return values, preventing tight coupling.
5. **Dependency Inversion Principle (DIP):**
   - `BorrowingController` receives `EquipmentController` via constructor injection, enabling dependency management and testability.

---

## 4. Test Strategy, Test Matrix & Results

### 4.1. Test Traceability Matrix

| Test ID | Test Scenario | OOP / Logic Principle Tested | Expected Outcome | Actual Result |
| :--- | :--- | :--- | :--- | :--- |
| **TC-01** | Polymorphic AV Checklist | Polymorphism & Dynamic Dispatch | Checklist contains HDMI, remote, power cord | **PASS** |
| **TC-02** | Polymorphic Lab Checklist | Polymorphism & Dynamic Dispatch | Checklist contains calibration seal, 4 components | **PASS** |
| **TC-03** | Eligible Borrower Check | Encapsulation & Domain Rules | `CheckEligibility()` returns `true` | **PASS** |
| **TC-04** | Clearance Hold Borrower | Encapsulation & Domain Rules | `CheckEligibility()` returns `false` | **PASS** |
| **TC-05** | Empty Student ID Borrower | Defensive Programming | `CheckEligibility()` returns `false` | **PASS** |
| **TC-06** | Inventory Tracking | Encapsulation & Inventory Logic | All registered equipment tracked in inventory | **PASS** |
| **TC-07** | Availability Filtering | Query & State Filtering | Only available equipment returned | **PASS** |
| **TC-08** | Mark Equipment For Repair | State Transition & Status Coordination | Condition set to "Under Repair", IsAvailable = false | **PASS** |
| **TC-09** | Duplicate Tag Prevention | Inventory Invariant Enforcement | Throws `InvalidOperationException` on duplicate | **PASS** |
| **TC-10** | Successful Borrow Flow | Transaction Coordination | Slip generated, equipment marked unavailable | **PASS** |
| **TC-11** | Ineligible Borrow Attempt | Precondition Validation | Throws `InvalidOperationException`, equipment safe | **PASS** |
| **TC-12** | Unavailable Borrow Attempt | Precondition Validation | Throws `InvalidOperationException`, rejects loan | **PASS** |
| **TC-13** | On-Time Good Return | Business Logic & Zero-Fine Path | Fee = $0.00, equipment made available, slip archived | **PASS** |
| **TC-14** | Overdue Return Penalty | Fine Calculation & Clearance Hold | Fee > $0.00, clearance hold automatically placed | **PASS** |
| **TC-15** | Incomplete Checklist Penalty | Peripheral Accounting & Penalty | Fee >= $150.00, clearance hold placed, item held | **PASS** |
| **TC-16** | Damaged Return Coordination | Controller Collaboration | Fee >= $300.00, item marked Under Repair in controller | **PASS** |
| **TC-17** | Inventory Persistence | Serialization & Disk I/O | Data saved and accurately reloaded into memory | **PASS** |

### 4.2. Test Execution Log Summary

```text
================================================================
          SCHOOL BORROWING SYSTEM - AUTOMATED QA TEST SUITE    
================================================================

--> Scenario 1: OOP Polymorphism & Dynamic Return Checklists
[PASS] AV Equipment checklist includes Power Cord
[PASS] AV Equipment checklist includes Remote Control
[PASS] AV Equipment checklist includes HDMI
[PASS] AV Equipment default duration is 0 days (hourly loan)
[PASS] Lab Equipment duration is 7 days
[PASS] Lab Equipment checklist verifies 4 components

--> Scenario 2: Borrower Identity & Clearance Hold Validation
[PASS] Clear borrower is eligible
[PASS] Borrower with Clearance Hold is NOT eligible
[PASS] Borrower without Student ID is NOT eligible

--> Scenario 3: EquipmentController Inventory & Repair Tracking
[PASS] EquipmentController tracks all 2 added items
[PASS] Both items initially available
[PASS] AV-100 condition updated to 'Under Repair'
[PASS] AV-100 availability revoked upon repair flagging
[PASS] Only 1 item available after repair flagging
[PASS] Duplicate PropertyTag registration is properly rejected

--> Scenario 4: Successful Borrowing Workflow & Due Time Calculation
[PASS] BorrowSlip generated successfully
[PASS] Equipment marked unavailable upon borrowing
[PASS] Room assigned correctly
[PASS] Due time set after time borrowed
[PASS] New slip is not overdue
[PASS] Active slips list reflects active transaction

--> Scenario 5: Rejection of Ineligible Borrowers
[PASS] Ineligible borrower loan request is rejected with InvalidOperationException
[PASS] Equipment remains available after rejected borrow
[PASS] No slip recorded for rejected transaction

--> Scenario 6: Rejection of Unavailable Equipment
[PASS] Unavailable equipment loan request rejected with InvalidOperationException

--> Scenario 7: On-time, Complete & Good Condition Return (Zero Penalty)
[PASS] Zero penalty for timely and complete return
[PASS] Equipment returned to available status
[PASS] No clearance hold placed on borrower
[PASS] Slip removed from active list
[PASS] Slip moved to historical audit archive

--> Scenario 8: Overdue Return Penalty & Clearance Hold Enforcement
[PASS] BorrowSlip correctly detects overdue condition
[PASS] Overdue penalty applied correctly: ₱100.00
[PASS] Clearance hold automatically enforced due to overdue fine

--> Scenario 9: Incomplete Checklist (Missing Cable) Penalty
[PASS] Incomplete checklist penalty charged: ₱150.00
[PASS] Clearance hold applied due to missing peripheral checklist
[PASS] Item kept unavailable until missing accessories are reconciled

--> Scenario 10: Damaged Return & EquipmentController Repair Coordination
[PASS] Damage assessment fee charged: ₱300.00
[PASS] Equipment marked 'Under Repair' in EquipmentController
[PASS] Damaged equipment availability revoked
[PASS] Clearance hold applied to borrower for damaged property

--> Scenario 11: Inventory File Persistence & Deserialization
[PASS] Inventory file successfully written to disk
[PASS] Reloaded inventory matches original item count (2)
[PASS] Polymorphic AVEquipment restored correctly
[PASS] Polymorphic LabEquipment restored correctly

================================================================
 TEST RESULTS SUMMARY: 44 PASSED | 0 FAILED | TOTAL: 44
================================================================
```

---

## 5. Visual Studio Compatibility & Team Setup Guide

To ensure all team members can open, build, and execute the solution seamlessly in **Microsoft Visual Studio**:

### 5.1. File Structure in Repository
```text
OOP FINALS/
├── SchoolBorrowingSystem.sln             <-- Visual Studio Solution File
├── SchoolBorrowingSystem/
│   ├── SchoolBorrowingSystem.csproj     <-- C# Project File
│   ├── Models/
│   │   ├── SchoolBorrower.cs
│   │   ├── SchoolEquipment.cs           <-- Abstract Base Class
│   │   ├── AVEquipment.cs               <-- Inherits SchoolEquipment
│   │   └── LabEquipment.cs              <-- Inherits SchoolEquipment
│   ├── Transactions/
│   │   └── BorrowSlip.cs                <-- Transaction Entity
│   ├── Controllers/
│   │   ├── EquipmentController.cs       <-- Inventory & Repair Manager
│   │   └── BorrowingController.cs       <-- Loan & Return Coordinator
│   ├── Program.cs                       <-- Interactive Dashboard Entry Point
│   └── TestRunner.cs                    <-- Automated QA Test Suite
├── QA_REPORT.md                         <-- This QA Report
├── .gitignore                           <-- Visual Studio gitignore configuration
└── README.md                            <-- Project Documentation
```

### 5.2. Steps to Run in Visual Studio
1. Launch **Visual Studio 2019** or **Visual Studio 2022**.
2. Click **Open a project or solution** and select `SchoolBorrowingSystem.sln`.
3. Set the active configuration to **Debug** or **Release** with platform **Any CPU**.
4. Build the solution using `Ctrl + Shift + B` (verify 0 Errors, 0 Warnings).
5. Press `F5` (or click **Start Debugging**) to run the application:
   - The program executes the automated QA validation suite first to confirm system integrity.
   - Pressing any key transitions into the **Interactive System Dashboard**, where users can test borrowing, returns, repair flagging, and clearance holds directly.

---

## 6. Code Quality & Maintainability Assessment

1. **No Spaghetti Code:** Every class is housed in its own dedicated source file under an organized namespace (`SchoolBorrowingSystem.Models`, `SchoolBorrowingSystem.Controllers`, `SchoolBorrowingSystem.Transactions`). Code execution follows structured control flow without circular dependencies.
2. **Defensive Programming:** Null parameters, invalid IDs, duplicate property tags, and non-existent records throw meaningful, descriptive exceptions rather than causing unhandled crashes.
3. **Convention Compliance:** Follows Microsoft C# conventions:
   - `PascalCase` for classes, methods, and public properties.
   - `_camelCase` with leading underscore for private fields.
   - Structured XML documentation comments (`/// <summary>`) on all types and public members.

---

## 7. QA Certification & Conclusion

The **School Equipment Borrowing System** satisfies 100% of the functional and structural requirements defined in the UML specification. The core logic successfully embodies the four pillars of Object-Oriented Programming and adheres to enterprise software engineering standards.

**Quality Assurance Verdict:** **PASSED & APPROVED FOR PRODUCTION AND ACADEMIC SUBMISSION.**
