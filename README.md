Cumulative Project – Part 3: Update Teacher Functionality
Overview
This part of the MVP adds Update capabilities to the Teachers table using ASP.NET Core Web API and MVC. It includes both an API endpoint and a form-based UI to edit teacher data.
GitHub Repository: https://github.com/camoyphillips/Cumulative.git
________________________________________
MVP Features (CRUD on Teachers Table)
•	Create: Add a new teacher (Part 2)
•	 Read: View list of teachers (Part 1)
•	Update: Edit existing teacher information (Part 3)
•	Delete: Remove a teacher from the database (Part 2)
________________________________________
Features Implemented
•	API Endpoint: HTTP PUT to update an existing teacher.
•	MVC View: A form at /TeacherPage/Edit/{id} for editing a teacher.
•	Client-side Validation:
o	Name must not be empty
o	Hire date cannot be in the future
o	Salary must be >= 0
•	Server-side Validation:
o	Check for empty name
o	Check for invalid hire date
o	Check for negative salary
o	Error when updating a non-existent teacher
________________________________________
File Structure
Type	Description	Path
Database Context	Connects to MySQL	/Models/SchoolDbContext.cs
Web API Controller	Logic to update teacher	/Controllers/TeacherAPIController.cs
MVC Controller	Routes to edit view	/Controllers/TeacherPageController.cs
Model	Represents a Teacher	/Models/Teacher.cs
View (Edit)	Form to update a teacher	/Views/Teacher/Edit.cshtml
________________________________________
How to Use
1.	Clone the repository.
2.	Open in Visual Studio.
3.	Update your appsettings.json with your MySQL connection string.
4.	Run the project and navigate to /TeacherPage/Edit/{id} to update a teacher (replace {id} with the teacher's ID).
________________________________________
Developed By
Camoy Phillips
HTTP5125 – Web Development
