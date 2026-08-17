using System;
using System.Collections.Generic;

namespace NewsProjectMVC.Models.Db;

public partial class PopularCategory
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? NewsCount { get; set; }
}
