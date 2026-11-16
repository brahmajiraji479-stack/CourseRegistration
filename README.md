\# Course Registration – AI-Assisted Engineering Assessment



\## Overview



This project implements a bounded course-registration workflow using C# and .NET 10.



The application supports:

\- Learner enrollment.

\- Course prerequisite validation.

\- Course capacity management.

\- FIFO waitlist management.

\- Automatic promotion when an enrolled learner drops.

\- Duplicate enrollment and waitlist prevention.

\- Thread-safe in-memory operations.



\## Technology Stack



\- C#

\- .NET 10

\- xUnit

\- In-memory collections

\- Visual Studio 2022



No database is required. The application uses in-memory storage as permitted by the assessment.



\## Solution Structure



```text

CourseRegistration/

│

├── CourseRegistration.Core/

│   ├── Models/

│   ├── Services/

│   └── Repositories/

│

├── CourseRegistration.Console/

│

├── CourseRegistration.Tests/

│

├── docs/

│   ├── DesignSpecification.md

│   ├── AI-Usage-Record.md

│   └── Limitations.md

│

├── CourseRegistration.slnx

└── README.md

```



\## Business Rules



1\. A learner can enroll only once in a course offering.

2\. Prerequisites must be completed before enrollment.

3\. If capacity is available, enrollment succeeds.

4\. If the course is full, eligible learners join the waitlist.

5\. Duplicate waitlist entries are prevented.

6\. When an enrolled learner drops, the earliest eligible waitlisted learner is promoted.

7\. Ineligible waitlisted learners are skipped and removed.

8\. Invalid requests return meaningful status messages.

9\. Concurrent operations are protected using synchronization.



\## Architecture



The application follows a simple layered design:



\- \*\*Models:\*\* Represent learners, course offerings, enrollments and waitlist entries.

\- \*\*Repositories:\*\* Maintain in-memory registration data.

\- \*\*Services:\*\* Implement enrollment, drop and promotion business rules.

\- \*\*Console:\*\* Application entry point.

\- \*\*Tests:\*\* Verify business requirements using xUnit.



\## Setup Instructions



Prerequisites:

\- .NET 10 SDK

\- Visual Studio 2022 or compatible development environment



Clone the repository:



```bash

git clone <YOUR-GITHUB-REPOSITORY-URL>

```



Navigate to the project directory:



```bash

cd CourseRegistration

```



Restore dependencies:



```bash

dotnet restore

```



Build the solution:



```bash

dotnet build

```



Execute automated tests:



```bash

dotnet test

```



\## Testing



Automated tests cover:



\- Enrollment with available capacity.

\- Enrollment when course is full.

\- Duplicate enrollment.

\- Duplicate waitlist requests.

\- FIFO waitlist promotion.

\- Prerequisite validation.

\- Ineligible learner skipping.

\- Invalid learner and course requests.

\- Repeated drop requests.

\- Concurrent enrollment requests.



\## AI-Assisted Engineering



ChatGPT was used as an engineering assistant for:

\- Project decomposition and architecture.

\- Domain model design.

\- Business logic implementation.

\- Test case design.

\- Documentation preparation.



AI-generated suggestions were reviewed and verified through compilation and automated testing.



Refer to `docs/AI-Usage-Record.md` for details.



\## Design Documentation



Additional documentation is available in the `docs` directory:



\- DesignSpecification.md

\- AI-Usage-Record.md

\- Limitations.md



\## Limitations



\- Data is stored in memory and is lost when the application terminates.

\- Synchronization protects only a single application instance.

\- No persistent database or distributed locking is implemented.

\- The implementation is intentionally limited to one course-registration workflow.



\## Author



Course Registration – AI-Assisted Engineering Assessment



