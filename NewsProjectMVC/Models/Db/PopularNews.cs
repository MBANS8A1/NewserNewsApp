using System;
using System.Collections.Generic;

namespace NewsProjectMVC.Models.Db;

public partial class PopularNews
{
    public string? Title { get; set; }

    public int? Id { get; set; }

    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ViewCount { get; set; }

    public string? Status { get; set; }

    public string? ImageName { get; set; }

    public int? CategoryId { get; set; }

    public string? Tags { get; set; }

    public int? UserId { get; set; }

    public int? CommentCount { get; set; }
}
