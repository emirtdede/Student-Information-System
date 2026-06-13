# Database Schema

## Tables and Fields

### `Users` (Mapped via Table-per-Hierarchy inheritance)
- `Id` (int, PK)
- `TcKimlikNo` (string)
- `Password` (string)
- `FirstName` (string)
- `LastName` (string)
- `Email` (string)
- `Role` (string: "Student", "Advisor", "Admin")
- `UserType` (Discriminator string)
- `EmailNotificationsEnabled` (bool)
- `SmsNotificationsEnabled` (bool)
- `ThemePreference` (string)
- `LanguagePreference` (string)
- **Student Extra Fields:**
  - `StudentNumber` (string)
  - `Department` (string)
  - `EnrollmentYear` (int)
  - `Gpa` (double)
  - `IsInternship1Completed` (bool)
  - `IsInternship2Completed` (bool)
  - `IsDoubleMajorActive` (bool)
  - `DoubleMajorDepartment` (string)
  - `DoubleMajorGpa` (double)
  - `AdvisorId` (int, FK)
  - `TuitionDebt` (decimal)
  - `DiningBalance` (decimal)
- **Advisor Extra Fields:**
  - `Department` (string)
  - `Title` (string)

### `Courses`
- `Id` (int, PK)
- `Code` (string)
- `Name` (string)
- `Credits` (int)
- `Ects` (int)
- `Instructor` (string)
- `ClassDay` (string)
- `StartTime` (string)
- `EndTime` (string)
- `Capacity` (int)
- `PrerequisiteCourseId` (int, Nullable FK)

### `Enrollments`
- `StudentId` (int, FK, Composite PK)
- `CourseId` (int, FK, Composite PK)
- `Status` (string: "Approved", "Pending", "Rejected")
- `MidtermGrade` (double, Nullable)
- `FinalGrade` (double, Nullable)
- `LetterGrade` (string)
- `EnrollmentType` (string: "Major", "DoubleMajor")
- `IsSurveyCompleted` (bool)

### `Announcements`
- `Id` (int, PK)
- `Title` (string)
- `Content` (string)
- `DatePosted` (string)
- `Category` (string)

### `Messages`
- `Id` (int, PK)
- `SenderId` (int, FK)
- `ReceiverId` (int, FK)
- `Subject` (string)
- `Content` (string)
- `SentDate` (DateTime)
- `IsRead` (bool)

### `SubstitutionRequests`
- `Id` (int, PK)
- `StudentId` (int, FK)
- `OldCourseId` (int, FK)
- `NewCourseId` (int, FK)
- `Reason` (string)
- `Status` (string: "Pending", "Approved", "Rejected")
- `RequestDate` (DateTime)

### `GradeObjections`
- `Id` (int, PK)
- `StudentId` (int, FK)
- `CourseId` (int, FK)
- `ExamType` (string)
- `CurrentGrade` (double)
- `ProposedGrade` (double, Nullable)
- `Explanation` (string)
- `Status` (string)
- `RequestDate` (DateTime)

### `DocumentRequests`
- `Id` (int, PK)
- `StudentId` (int, FK)
- `DocumentType` (string)
- `Purpose` (string)
- `CopyCount` (int)
- `Status` (string)
- `RequestDate` (DateTime)
- `CompletedDate` (DateTime, Nullable)
- `AdminNote` (string)
