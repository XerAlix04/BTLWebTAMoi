using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class NguoiDungBaiHoc
{
    public int Ma { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaBai { get; set; }

    public string? TrangThai { get; set; }

    public double? Diem { get; set; }

    public DateTime? NgayHoanThanh { get; set; }

    public virtual BaiHoc? MaBaiNavigation { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
