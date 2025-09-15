using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class KetBan
{
    public int Ma { get; set; }

    public int? MaNguoi1 { get; set; }

    public int? MaNguoi2 { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual NguoiDung? MaNguoi1Navigation { get; set; }

    public virtual NguoiDung? MaNguoi2Navigation { get; set; }
}
