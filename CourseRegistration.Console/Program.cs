
using CourseRegistration.Core.Models;
using CourseRegistration.Core.Repositories;
using CourseRegistration.Core.Services;

// 1. Initialize in-memory storage
var store = new InMemoryRegistrationStore();

// 2. Create course offering
var course = new CourseOffering
{
    Id = 1,
    CourseName = "C# Programming",
    Capacity = 2,
    PrerequisiteCourseId = "BASIC-CSHARP"
};

store.AddCourse(course);

// 3. Create learners
for (int i = 1; i <= 4; i++)
{
    var learner = new Learner
    {
        Id = i,
        Name = $"Learner {i}"
    };

    learner.CompletedCourseIds.Add("BASIC-CSHARP");

    store.AddLearner(learner);
}

// 4. Initialize enrollment service
var enrollmentService = new EnrollmentService(store);

Console.WriteLine("===== COURSE REGISTRATION SYSTEM =====");
Console.WriteLine($"Course: {course.CourseName}");
Console.WriteLine($"Capacity: {course.Capacity}");
Console.WriteLine();

// 5. Enroll learners
for (int i = 1; i <= 4; i++)
{
    var result = enrollmentService.Enroll(i, 1);

    Console.WriteLine(
        $"Learner {i}: {result.Status} - {result.Message}");
}

Console.WriteLine();

// 6. Display enrolled learners
Console.WriteLine("===== ENROLLED LEARNERS =====");

foreach (var enrollment in store.GetEnrollments(1))
{
    Console.WriteLine($"Learner {enrollment.LearnerId}");
}

Console.WriteLine();

// 7. Display waitlist
Console.WriteLine("===== WAITLIST =====");

foreach (var entry in store.GetWaitlist(1))
{
    Console.WriteLine(
        $"Position {entry.QueuePosition}: Learner {entry.LearnerId}");
}

Console.WriteLine();

// 8. Drop an enrolled learner
Console.WriteLine("===== DROP COURSE =====");

var dropResult = enrollmentService.Drop(1, 1);

Console.WriteLine(dropResult.Message);

Console.WriteLine();

// 9. Display final enrollment
Console.WriteLine("===== FINAL ENROLLMENT =====");

foreach (var enrollment in store.GetEnrollments(1))
{
    Console.WriteLine($"Learner {enrollment.LearnerId}");
}

Console.WriteLine();

// 10. Display remaining waitlist
Console.WriteLine("===== FINAL WAITLIST =====");

//foreach (var entry in store.GetWaitlist(1))
//{
//    Console.WriteLine(
//        $"Position {entry.QueuePosition}: Learner {entry.LearnerId}");
//}

var finalWaitlist = store.GetWaitlist(1);

for (int i = 0; i < finalWaitlist.Count; i++)
{
    Console.WriteLine(
        $"Position {i + 1}: Learner {finalWaitlist[i].LearnerId}");
}

Console.WriteLine();
Console.WriteLine("===== PROCESS COMPLETED =====");
