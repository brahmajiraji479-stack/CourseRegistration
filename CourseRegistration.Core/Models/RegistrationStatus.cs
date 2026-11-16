
namespace CourseRegistration.Core.Models;

public enum RegistrationStatus
{
    Enrolled,
    Waitlisted,
    AlreadyEnrolled,
    AlreadyWaitlisted,
    LearnerNotFound,
    CourseNotFound,
    PrerequisiteNotMet,
    Dropped,
    NotEnrolled
}
