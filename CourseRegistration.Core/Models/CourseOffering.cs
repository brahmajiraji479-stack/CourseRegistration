
namespace CourseRegistration.Core.Models;

public class CourseOffering
{
    public int Id { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public string? PrerequisiteCourseId { get; set; }
}
