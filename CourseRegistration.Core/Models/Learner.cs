
namespace CourseRegistration.Core.Models;

public class Learner
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public HashSet<string> CompletedCourseIds { get; set; }
        = new(StringComparer.OrdinalIgnoreCase);
}
