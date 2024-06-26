
namespace FeedbackFusionAPI.Model;

public partial class Feedback
{
    public long Id { get; set; }

    public string UserId { get; set; } = null!;

    public bool RatingSuccessful { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
