using System;
using System.Collections.Generic;

namespace NewsProjectMVC.Models.Db;

public partial class Setting
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Copyright { get; set; }

    public string? Facebook { get; set; }

    public string? X { get; set; }

    public string? Instagram { get; set; }

    public string? YouTube { get; set; }

    public string? LinkedIn { get; set; }
}
