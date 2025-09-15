using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class NhatKyHoatDong
{
    public int Ma { get; set; }

    public int? MaNguoiDung { get; set; }

    public string? HanhDong { get; set; }

    public string? DuLieu { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
