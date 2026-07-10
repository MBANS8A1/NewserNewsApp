using System;
using System.Collections.Generic;

namespace NewsProjectMVC.Models.Db;

public partial class Menu
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Link { get; set; } = null!;

    public int? ParentId { get; set; }

    public short? Priority { get; set; }
}
