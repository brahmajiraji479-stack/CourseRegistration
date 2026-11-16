
# AI-Assisted Engineering Usage Record

## 1. AI Tool Used

ChatGPT was used as an engineering assistant.

## 2. Activities

### Activity 1: Project Architecture

Prompt:
"Help me design a .NET application for course
registration with enrollment, dropping and FIFO
waitlist promotion."

AI suggestion:
Separate the application into Core, Console and Tests
projects.

Decision: Accepted.

Verification:
Created the solution and successfully executed
dotnet build and dotnet test.

### Activity 2: Domain Models

Prompt:
"Suggest C# domain models for learners, course offerings,
enrollments and waitlist entries."

AI suggestion:
Create separate models for each business entity.

Decision: Accepted.

Verification:
Compiled the application successfully.

### Activity 3: Business Logic

Prompt:
"Implement enrollment, prerequisite validation,
duplicate prevention, dropping and FIFO waitlist
promotion in C#."

AI suggestion:
Use an enrollment service and in-memory repository.

Decision: Accepted and reviewed.

Verification:
Created xUnit tests for enrollment, waitlisting,
prerequisites, duplicate requests and FIFO promotion.

### Activity 4: Concurrency

Prompt:
"How can we prevent concurrent enrollment requests
from exceeding course capacity?"

AI suggestion:
Use synchronization around state-changing operations.

Decision: Accepted.

Verification:
Added a concurrent enrollment test using Parallel.For.

### Activity 5: Automated Testing

Prompt:
"Create xUnit tests covering the course registration
acceptance criteria."

AI suggestion:
Use focused tests for capacity, duplicates,
prerequisites, promotion and invalid requests.

Decision: Reviewed and adapted.

Verification:
Executed dotnet test successfully.

## 3. Independent Review

AI-generated suggestions were reviewed against
the assessment requirements.

The implementation was verified through compilation
and automated test execution.

## 4. Limitations

The implementation uses in-memory storage.
Concurrency protection applies within one application
instance only.
