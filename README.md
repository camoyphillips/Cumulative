# Cumulative Project - Part 2: Add and Delete Teacher Functionality

## Overview

This project builds upon Part 1 and focuses on adding and deleting teachers within the **School Database** using **ASP.NET Core Web API** and **MVC**. It implements the **Create** and **Delete** operations for the `Teachers` table.

GitHub Repository: [https://github.com/camoyphillips/Cumulative.git]

---

**MVP Features (CRUD on Teachers Table)**

- **Create**: Add a new teacher
- **Read**: View list of teachers (from Part 1)
- **Update**: *(Coming in Part 3)*
- **Delete**: Remove a teacher from the database

---

**File Structure**

| Type                 | Description                            | Path                                                     |
| :------------------- | :------------------------------------- | :------------------------------------------------------- |
| Database Context     | Connects to MySQL                      | `/Models/SchoolDbContext.cs`                            |
| Web API Controller   | Add & Delete API logic               | `/Controllers/TeacherAPIController.cs`                   |
| MVC Controller       | Routes to dynamic views                | `/Controllers/TeacherPageController.cs`                  |
| Model                | Represents a Teacher                   | `/Models/Teacher.cs`                                    |
| View (Add)           | Form to enter a new teacher            | `/Views/Teacher/New.cshtml`                             |
| View (Delete)        | Confirmation page for deleting a teacher | `/Views/Teacher/DeleteConfirm.cshtml`                   |

---

**How to Use**

1. Clone the repository.
2. Open in Visual Studio.
3. Update your `appsettings.json` with your MySQL connection string.
4. Run the project and navigate to `/TeacherPage/New` to add a teacher or `/TeacherPage/Delete/{id}` to delete a teacher (replace `{id}` with the teacher's ID).

---

**Features Implemented**

- **Validation**: Employee number format (`T` + digits)
- **Validation**: Hire date must not be in the future
- **Validation**: Names must not be empty
- **Validation**: Duplicate employee number check
- **Error Handling**: Graceful error when deleting a non-existent teacher

---

Developed By

Camoy Phillips
HTTP5125 – Web Development
