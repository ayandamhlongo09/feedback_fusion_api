using System;
using System.Collections.Generic;

namespace FeedbackFusionAPI.Model;

public partial class User
{
    public string UserId { get; set; } = null!;

    public int Role { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

}
