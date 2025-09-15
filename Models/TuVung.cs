using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class TuVung
{
    public int MaTu { get; set; }

    public string Tu { get; set; } = null!;

    public string? PhienAm { get; set; }

    public string Nghia { get; set; } = null!;

    public string? TuLoai { get; set; }

    public string? ViDu { get; set; }

    public string? DuongDanAnh { get; set; }

    public string? TrinhDo { get; set; }

    public virtual ICollection<NguoiDungTuVung> NguoiDungTuVungs { get; set; } = new List<NguoiDungTuVung>();
}
