namespace FeedbackFusionAPI.DTOs;

public class FeedbackDto
{
    public long Id { get; set; }
    public  bool RatingSuccessful { get; set; }
    public string FeedbackComment { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
}

public class CreateFeedbackDto
{
    public required string UserId { get; set; }
    public bool RatingSuccessful { get; set; }
    public string FeedbackComment { get; set; } = "";
}