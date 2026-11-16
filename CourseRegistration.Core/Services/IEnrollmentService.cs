
using CourseRegistration.Core.Models;

namespace CourseRegistration.Core.Services;

public interface IEnrollmentService
{
    RegistrationResult Enroll(int learnerId, int courseOfferingId);

    RegistrationResult Drop(int learnerId, int courseOfferingId);
}
