
using CourseRegistration.Core.Models;
using CourseRegistration.Core.Repositories;
using CourseRegistration.Core.Services;
using Xunit;

namespace CourseRegistration.Tests;

public class EnrollmentServiceTests
{
    private static (
        InMemoryRegistrationStore Store,
        EnrollmentService Service) CreateSetup(
            int capacity = 2,
            string? prerequisite = null)
    {
        var store = new InMemoryRegistrationStore();

        store.AddCourse(new CourseOffering
        {
            Id = 1,
            CourseName = "Advanced C#",
            Capacity = capacity,
            PrerequisiteCourseId = prerequisite
        });

        return (store, new EnrollmentService(store));
    }

    private static Learner CreateLearner(
        int id,
        bool completedPrerequisite = true)
    {
        var learner = new Learner
        {
            Id = id,
            Name = $"Learner {id}"
        };

        if (completedPrerequisite)
        {
            learner.CompletedCourseIds.Add("BASIC-CSHARP");
        }

        return learner;
    }

    // TEST 1: Enrollment succeeds when capacity is available.
    [Fact]
    public void Enroll_WhenCapacityAvailable_ShouldEnrollLearner()
    {
        var (store, service) = CreateSetup();

        store.AddLearner(CreateLearner(1));

        var result = service.Enroll(1, 1);

        Assert.Equal(RegistrationStatus.Enrolled, result.Status);
        Assert.Single(store.GetEnrollments(1));
    }

    // TEST 2: When course is full, learner joins waitlist.
    [Fact]
    public void Enroll_WhenCourseFull_ShouldAddToWaitlist()
    {
        var (store, service) = CreateSetup(capacity: 1);

        store.AddLearner(CreateLearner(1));
        store.AddLearner(CreateLearner(2));

        service.Enroll(1, 1);

        var result = service.Enroll(2, 1);

        Assert.Equal(RegistrationStatus.Waitlisted, result.Status);
        Assert.Single(store.GetWaitlist(1));
    }

    // TEST 3: Duplicate enrollment should be rejected.
    [Fact]
    public void Enroll_WhenAlreadyEnrolled_ShouldRejectDuplicate()
    {
        var (store, service) = CreateSetup();

        store.AddLearner(CreateLearner(1));

        service.Enroll(1, 1);

        var result = service.Enroll(1, 1);

        Assert.Equal(
            RegistrationStatus.AlreadyEnrolled,
            result.Status);

        Assert.Single(store.GetEnrollments(1));
    }

    // TEST 4: Duplicate waitlist request should be rejected.
    [Fact]
    public void Enroll_WhenAlreadyWaitlisted_ShouldRejectDuplicate()
    {
        var (store, service) = CreateSetup(capacity: 1);

        store.AddLearner(CreateLearner(1));
        store.AddLearner(CreateLearner(2));

        service.Enroll(1, 1);
        service.Enroll(2, 1);

        var result = service.Enroll(2, 1);

        Assert.Equal(
            RegistrationStatus.AlreadyWaitlisted,
            result.Status);

        Assert.Single(store.GetWaitlist(1));
    }

    // TEST 5: Dropping a learner should promote FIFO learner.
    [Fact]
    public void Drop_ShouldPromoteFirstWaitlistedLearner()
    {
        var (store, service) = CreateSetup(capacity: 2);

        for (int i = 1; i <= 4; i++)
        {
            store.AddLearner(CreateLearner(i));
        }

        service.Enroll(1, 1);
        service.Enroll(2, 1);
        service.Enroll(3, 1);
        service.Enroll(4, 1);

        var result = service.Drop(1, 1);

        Assert.Equal(RegistrationStatus.Dropped, result.Status);
        Assert.Equal(3, result.PromotedLearnerId);

        var enrolled = store.GetEnrollments(1);

        Assert.Contains(enrolled, x => x.LearnerId == 3);
        Assert.DoesNotContain(enrolled, x => x.LearnerId == 1);
    }

    // TEST 6: Learner without prerequisite cannot enroll.
    [Fact]
    public void Enroll_WhenPrerequisiteNotCompleted_ShouldReject()
    {
        var (store, service) = CreateSetup(
            capacity: 2,
            prerequisite: "BASIC-CSHARP");

        store.AddLearner(CreateLearner(
            1,
            completedPrerequisite: false));

        var result = service.Enroll(1, 1);

        Assert.Equal(
            RegistrationStatus.PrerequisiteNotMet,
            result.Status);

        Assert.Empty(store.GetEnrollments(1));
        Assert.Empty(store.GetWaitlist(1));
    }

    // TEST 7: Ineligible first learner should be skipped.
    [Fact]
    public void Drop_WhenFirstWaitlistedLearnerIneligible_ShouldSkipAndPromoteNext()
    {
        var (store, service) = CreateSetup(
            capacity: 2,
            prerequisite: "BASIC-CSHARP");

        var learner1 = CreateLearner(1);
        var learner2 = CreateLearner(2);
        var learner3 = CreateLearner(3);
        var learner4 = CreateLearner(4);

        store.AddLearner(learner1);
        store.AddLearner(learner2);
        store.AddLearner(learner3);
        store.AddLearner(learner4);

        service.Enroll(1, 1);
        service.Enroll(2, 1);
        service.Enroll(3, 1);
        service.Enroll(4, 1);

        // Learner 3 loses the prerequisite after joining waitlist.
        learner3.CompletedCourseIds.Clear();

        var result = service.Drop(1, 1);

        Assert.Equal(4, result.PromotedLearnerId);

        var enrolled = store.GetEnrollments(1);

        Assert.Contains(enrolled, x => x.LearnerId == 4);
        Assert.DoesNotContain(enrolled, x => x.LearnerId == 3);
    }

    // TEST 8: Invalid learner should return clear result.
    [Fact]
    public void Enroll_WhenLearnerDoesNotExist_ShouldReturnNotFound()
    {
        var (_, service) = CreateSetup();

        var result = service.Enroll(999, 1);

        Assert.Equal(
            RegistrationStatus.LearnerNotFound,
            result.Status);
    }

    // TEST 9: Repeated drop should be rejected.
    [Fact]
    public void Drop_WhenAlreadyDropped_ShouldReturnNotEnrolled()
    {
        var (store, service) = CreateSetup();

        store.AddLearner(CreateLearner(1));

        service.Enroll(1, 1);

        service.Drop(1, 1);

        var result = service.Drop(1, 1);

        Assert.Equal(
            RegistrationStatus.NotEnrolled,
            result.Status);

        Assert.Empty(store.GetEnrollments(1));
    }

    // TEST 10: Concurrent requests must not exceed capacity.
    [Fact]
    public void Enroll_WhenConcurrentRequests_ShouldNotExceedCapacity()
    {
        const int capacity = 5;
        const int totalLearners = 20;

        var (store, service) = CreateSetup(capacity);

        for (int i = 1; i <= totalLearners; i++)
        {
            store.AddLearner(CreateLearner(i));
        }

        Parallel.For(1, totalLearners + 1, learnerId =>
        {
            service.Enroll(learnerId, 1);
        });

        var enrolled = store.GetEnrollments(1);
        var waitlist = store.GetWaitlist(1);

        Assert.Equal(capacity, enrolled.Count);
        Assert.Equal(totalLearners - capacity, waitlist.Count);

        Assert.Equal(
            totalLearners,
            enrolled.Count + waitlist.Count);
    }

    // TEST 11: Invalid course should return clear result.
    [Fact]
    public void Enroll_WhenCourseDoesNotExist_ShouldReturnNotFound()
    {
        var (store, service) = CreateSetup();

        store.AddLearner(CreateLearner(1));

        var result = service.Enroll(1, 999);

        Assert.Equal(
            RegistrationStatus.CourseNotFound,
            result.Status);
    }

    // TEST 12: Course capacity is zero.
    [Fact]
    public void AddCourse_WhenCapacityIsZero_ShouldThrowException()
    {
        var store = new InMemoryRegistrationStore();

        var course = new CourseOffering
        {
            Id = 10,
            CourseName = "C# Programming",
            Capacity = 0
        };

        var exception = Assert.Throws<ArgumentException>(
            () => store.AddCourse(course));

        Assert.Equal(
            "Course capacity must be greater than zero.",
            exception.Message);
    }

    // TEST 13: Course capacity is negative.
    [Fact]
    public void AddCourse_WhenCapacityIsNegative_ShouldThrowException()
    {
        var store = new InMemoryRegistrationStore();

        var course = new CourseOffering
        {
            Id = 11,
            CourseName = "C# Programming",
            Capacity = -5
        };

        Assert.Throws<ArgumentException>(
            () => store.AddCourse(course));
    }

    // TEST 14: All waitlisted learners are ineligible.
    [Fact]
    public void Drop_WhenAllWaitlistedLearnersIneligible_ShouldNotPromoteAnyone()
    {
        var (store, service) = CreateSetup(
            capacity: 2,
            prerequisite: "BASIC-CSHARP");

        var learner1 = CreateLearner(1);
        var learner2 = CreateLearner(2);
        var learner3 = CreateLearner(3);
        var learner4 = CreateLearner(4);

        store.AddLearner(learner1);
        store.AddLearner(learner2);
        store.AddLearner(learner3);
        store.AddLearner(learner4);

        service.Enroll(1, 1);
        service.Enroll(2, 1);
        service.Enroll(3, 1);
        service.Enroll(4, 1);

        // Both waitlisted learners lose their prerequisites.
        learner3.CompletedCourseIds.Clear();
        learner4.CompletedCourseIds.Clear();

        var result = service.Drop(1, 1);

        Assert.Equal(RegistrationStatus.Dropped, result.Status);
        Assert.Null(result.PromotedLearnerId);

        Assert.Single(store.GetEnrollments(1));
        Assert.Empty(store.GetWaitlist(1));
    }
}
