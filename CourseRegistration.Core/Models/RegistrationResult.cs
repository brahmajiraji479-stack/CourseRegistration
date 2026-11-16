
namespace CourseRegistration.Core.Models;

public sealed record RegistrationResult(
    RegistrationStatus Status,
    string Message,
    int? PromotedLearnerId = null)
{
    public bool Success =>
        Status == RegistrationStatus.Enrolled ||
        Status == RegistrationStatus.Waitlisted ||
        Status == RegistrationStatus.Dropped;
}
