using System;
using System.Collections.Generic;

namespace NewsProjectMVC.Models.Db;

public partial class Category
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
}
