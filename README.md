# School Equipment Borrowing System

A robust, object-oriented School Equipment Borrowing Management System implemented in C# for Visual Studio.

## Architecture & Design Patterns
- **Language**: C# (.NET / Visual Studio compatible)
- **Object-Oriented Core**: Full adherence to Abstraction, Encapsulation, Inheritance, Polymorphism, and SOLID principles.
- **Visual Studio Ready**: Includes solution (`.sln`) and project (`.csproj`) files ready to open and run in Visual Studio 2022/2019.

## UML Class Structure
- `SchoolBorrower`: Borrower entity managing credentials, role, department, and clearance holds.
- `SchoolEquipment` (Abstract): Base abstraction for borrowable equipment with encapsulated fields and abstract checklists/durations.
- `AVEquipment`: Subclass for audiovisual equipment (projectors, remotes, cables) with hourly borrow duration and checklist logic.
- `LabEquipment`: Subclass for laboratory equipment (microscopes, kits) with calibration dates, component counts, and multi-day duration.
- `BorrowSlip`: Transaction record coordinating loan duration, due times, overdue tracking, and assigned parties.
- `EquipmentController`: Manages school inventory, availability filtering, repair workflows, and persistence.
- `BorrowingController`: Central coordinator processing borrowing transactions, return evaluations, fine calculations, and clearance hold enforcement.

## Getting Started in Visual Studio
1. Open `SchoolBorrowingSystem.sln` in Visual Studio 2019/2022.
2. Build the solution (`Ctrl + Shift + B`).
3. Press `F5` to run either the interactive management console or automated QA test scenarios.
