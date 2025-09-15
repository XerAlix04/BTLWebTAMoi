using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class DoanVan
{
    public int MaDoan { get; set; }

    public string? TieuDe { get; set; }

    public string NoiDung { get; set; } = null!;

    public string? TomTat { get; set; }

    public string? TuKhoa { get; set; }

    public virtual ICollection<CauHoi> CauHois { get; set; } = new List<CauHoi>();
}
