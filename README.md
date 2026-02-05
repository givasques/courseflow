# CourseFlow API

REST API built with **.NET 8** for managing an online learning platform. The system handles **courses, students, and enrollments**, with authentication, role-based access control, and backend best practices.

---

## Project Idea

CourseFlow simulates the backend of an online education platform where users can access courses and students can enroll in them. The API focuses on secure authentication, authorization, and a clean, scalable architecture.

---

## Roles

- **Admin** — Full access  
- **Instructor** — Manage courses  
- **Student** — Enroll and view own enrollments  

---

## Functional Requirements

### Authentication
- User registration and login  
- JWT token issuance  
- Role-based access control (Admin, Instructor, Student)

### Courses
- Create course (Admin / Instructor)  
- List courses with pagination and filters (Public)  
- View course details (Public)  
- Update course (Admin / Instructor)  
- Remove course (Admin)

**Validation**
- Course title must have at least 3 characters

### Students
- Create student profile linked to a user (Admin)  
- List students (Admin)  
- View and update student profile (Admin or the student)  
- Deactivate or remove student (Admin)

**Validation**
- Student email must be valid and unique

### Enrollments
- Authenticated student can enroll in a course  
- Duplicate enrollments are prevented  
- Students can list their own enrollments  
- Admin can list enrollments

### Error Handling
- Standardized error responses  
- Clear HTTP status codes and messages

### Documentation
- Swagger with Bearer authentication  
- Example requests  
- README with setup and usage instructions  

---

## Technical Requirements

- .NET 8 + ASP.NET Core Web API  
- Entity Framework Core  
  - SQLite for development  
  - SQL Server or PostgreSQL ready  
- ASP.NET Core Identity + JWT Bearer  
- Configuration via environment variables and user-secrets  
- No secrets stored in repository  
- Migrations applied automatically  
- Initial seed:
  - Roles (Admin, Instructor, Student)  
  - Default Admin user  
- Database constraints:
  - Unique student email  
  - Unique enrollment (student + course)  
- DTOs separated from entities  
- Pagination and filters via query string  
- Swagger/OpenAPI with Bearer security scheme  
- HTTPS enabled  
- CORS restricted to required origins  

---

## How to Run Locally

### Clone the repository

```bash
git clone https://github.com/your-username/courseflow-api.git
cd courseflow-api
```

### Restore dependencies
```bash
dotnet restore
```

### Configure secrets
```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-super-secret-key"
dotnet user-secrets set "Jwt:Issuer" "CourseFlow"
dotnet user-secrets set "Jwt:Audience" "CourseFlowUsers"
```

### Apply migrations and seed database
```bash
dotnet ef database update
```

### Run the API
```bash
dotnet run
```

API base URL:
- https://localhost:<port>

### Access Swagger
- https://localhost:<port>/swagger

### Authentication Flow
- Register or login

- Receive JWT token

- Click Authorize in Swagger

- Paste token as:
  Bearer YOUR_TOKEN_HERE
