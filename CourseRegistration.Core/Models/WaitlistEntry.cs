
namespace CourseRegistration.Core.Models;

public class WaitlistEntry
{
    public int LearnerId { get; set; }

    public int CourseOfferingId { get; set; }

    // Determines FIFO ordering
    public long QueuePosition { get; set; }

    public DateTime AddedAt { get; set; }
}
