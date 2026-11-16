
namespace CourseRegistration.Core.Models;

public class Enrollment
{
    public int LearnerId { get; set; }

    public int CourseOfferingId { get; set; }

    public DateTime EnrolledAt { get; set; }
}
