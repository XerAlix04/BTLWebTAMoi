using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class GhiAm
{
    public int Ma { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? LienKetId { get; set; }

    public string? LoaiLienKet { get; set; }

    public string DuongDanAudio { get; set; } = null!;

    public string? VanBan { get; set; }

    public double? Diem { get; set; }

    public string? NhanXet { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
