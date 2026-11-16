
using CourseRegistration.Core.Models;
using CourseRegistration.Core.Repositories;

namespace CourseRegistration.Core.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly InMemoryRegistrationStore _store;

    public EnrollmentService(InMemoryRegistrationStore store)
    {
        _store = store;
    }

    public RegistrationResult Enroll(
        int learnerId,
        int courseOfferingId)
    {
        lock (_store.SyncRoot)
        {
            // 1. Validate learner
            if (!_store.Learners.TryGetValue(
                learnerId, out var learner))
            {
                return new(
                    RegistrationStatus.LearnerNotFound,
                    "Learner not found.");
            }

            // 2. Validate course
            if (!_store.Courses.TryGetValue(
                courseOfferingId, out var course))
            {
                return new(
                    RegistrationStatus.CourseNotFound,
                    "Course not found.");
            }

            // 3. Check duplicate enrollment
            if (_store.Enrollments.Any(x =>
                x.LearnerId == learnerId &&
                x.CourseOfferingId == courseOfferingId))
            {
                return new(
                    RegistrationStatus.AlreadyEnrolled,
                    "Learner is already enrolled.");
            }

            // 4. Check duplicate waitlist entry
            if (_store.Waitlist.Any(x =>
                x.LearnerId == learnerId &&
                x.CourseOfferingId == courseOfferingId))
            {
                return new(
                    RegistrationStatus.AlreadyWaitlisted,
                    "Learner is already in the waitlist.");
            }

            // 5. Validate prerequisite
            if (!IsEligible(learner, course))
            {
                return new(
                    RegistrationStatus.PrerequisiteNotMet,
                    "Required prerequisite is not completed.");
            }

            // 6. Check available capacity
            int enrolledCount = _store.Enrollments.Count(x =>
                x.CourseOfferingId == courseOfferingId);

            if (enrolledCount < course.Capacity)
            {
                AddEnrollment(learnerId, courseOfferingId);

                return new(
                    RegistrationStatus.Enrolled,
                    "Enrollment successful.");
            }

            // 7. Course is full. Add to waitlist.
            _store.Waitlist.Add(new WaitlistEntry
            {
                LearnerId = learnerId,
                CourseOfferingId = courseOfferingId,
                QueuePosition = _store.NextQueuePosition++,
                AddedAt = DateTime.UtcNow
            });

            return new(
                RegistrationStatus.Waitlisted,
                "Course is full. Learner added to waitlist.");
        }
    }

    public RegistrationResult Drop(
        int learnerId,
        int courseOfferingId)
    {
        lock (_store.SyncRoot)
        {
            // 1. Validate learner
            if (!_store.Learners.ContainsKey(learnerId))
            {
                return new(
                    RegistrationStatus.LearnerNotFound,
                    "Learner not found.");
            }

            // 2. Validate course
            if (!_store.Courses.ContainsKey(courseOfferingId))
            {
                return new(
                    RegistrationStatus.CourseNotFound,
                    "Course not found.");
            }

            // 3. Find existing enrollment
            var enrollment = _store.Enrollments.FirstOrDefault(x =>
                x.LearnerId == learnerId &&
                x.CourseOfferingId == courseOfferingId);

            if (enrollment == null)
            {
                return new(
                    RegistrationStatus.NotEnrolled,
                    "Learner is not enrolled in this course.");
            }

            // 4. Remove enrollment
            _store.Enrollments.Remove(enrollment);

            // 5. Promote earliest eligible learner
            int? promotedLearnerId =
                PromoteNextLearner(courseOfferingId);


            string message = promotedLearnerId.HasValue
                ? $"Learner {learnerId} dropped successfully. " +
                  $"Learner {promotedLearnerId} promoted from waitlist."
                : $"Learner {learnerId} dropped successfully. " +
                  "No eligible waitlisted learner found.";


            return new(
                RegistrationStatus.Dropped,
                message,
                promotedLearnerId);
        }
    }

    private bool IsEligible(
        Learner learner,
        CourseOffering course)
    {
        // No prerequisite means everyone is eligible.
        if (string.IsNullOrWhiteSpace(
            course.PrerequisiteCourseId))
        {
            return true;
        }

        return learner.CompletedCourseIds.Contains(
            course.PrerequisiteCourseId);
    }

    private void AddEnrollment(
        int learnerId,
        int courseOfferingId)
    {
        _store.Enrollments.Add(new Enrollment
        {
            LearnerId = learnerId,
            CourseOfferingId = courseOfferingId,
            EnrolledAt = DateTime.UtcNow
        });
    }

    private int? PromoteNextLearner(int courseOfferingId)
    {
        var waitingLearners = _store.Waitlist
            .Where(x => x.CourseOfferingId == courseOfferingId)
            .OrderBy(x => x.QueuePosition)
            .ToList();

        foreach (var entry in waitingLearners)
        {
            // Remove the candidate from the queue.
            _store.Waitlist.Remove(entry);

            var learner = _store.Learners[entry.LearnerId];

            // Skip learners who are no longer eligible.
            var course = _store.Courses[courseOfferingId];

            if (!IsEligible(learner, course))
            {
                continue;
            }

            // Promote the first eligible learner.
            AddEnrollment(entry.LearnerId, courseOfferingId);

            // Stop after promoting the first eligible learner.
            return entry.LearnerId;
        }

        return null;
    }
}
