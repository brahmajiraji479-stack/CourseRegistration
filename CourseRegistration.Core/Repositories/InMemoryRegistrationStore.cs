
using CourseRegistration.Core.Models;

namespace CourseRegistration.Core.Repositories;

public class InMemoryRegistrationStore
{
    internal object SyncRoot { get; } = new();

    internal Dictionary<int, Learner> Learners { get; } = new();

    internal Dictionary<int, CourseOffering> Courses { get; } = new();

    internal List<Enrollment> Enrollments { get; } = new();

    internal List<WaitlistEntry> Waitlist { get; } = new();

    internal long NextQueuePosition { get; set; } = 1;

    public void AddLearner(Learner learner)
    {
        lock (SyncRoot)
        {
            if (!Learners.TryAdd(learner.Id, learner))
                throw new InvalidOperationException(
                    "Learner already exists.");
        }
    }


    public void AddCourse(CourseOffering course)
    {
        lock (SyncRoot)
        {
            // 1. Validate course object
            ArgumentNullException.ThrowIfNull(course);

            // 2. Validate course capacity
            if (course.Capacity <= 0)
            {
                throw new ArgumentException(
                    "Course capacity must be greater than zero.");
            }

            // 3. Prevent duplicate course
            if (!Courses.TryAdd(course.Id, course))
            {
                throw new InvalidOperationException(
                    "Course already exists.");
            }
        }
    }


    public IReadOnlyList<Enrollment> GetEnrollments(int courseId)
    {
        lock (SyncRoot)
        {
            return Enrollments
                .Where(x => x.CourseOfferingId == courseId)
                .Select(x => new Enrollment
                {
                    LearnerId = x.LearnerId,
                    CourseOfferingId = x.CourseOfferingId,
                    EnrolledAt = x.EnrolledAt
                })
                .ToList();
        }
    }

    public IReadOnlyList<WaitlistEntry> GetWaitlist(int courseId)
    {
        lock (SyncRoot)
        {
            return Waitlist
                .Where(x => x.CourseOfferingId == courseId)
                .OrderBy(x => x.QueuePosition)
                .Select(x => new WaitlistEntry
                {
                    LearnerId = x.LearnerId,
                    CourseOfferingId = x.CourseOfferingId,
                    QueuePosition = x.QueuePosition,
                    AddedAt = x.AddedAt
                })
                .ToList();
        }
    }
}
