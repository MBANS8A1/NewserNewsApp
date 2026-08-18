using System;
using System.Collections.Generic;

namespace NewsProjectMVC.Models.Db;

public partial class Comment
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string CommentText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsApproved { get; set; }

    public int NewsId { get; set; }
}
