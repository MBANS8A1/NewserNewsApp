using System;
using System.Collections.Generic;

namespace NewsProjectMVC.Models.Db;

public partial class Tag
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
}
