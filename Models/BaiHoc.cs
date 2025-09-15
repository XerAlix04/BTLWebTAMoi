using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class BaiHoc
{
    public int MaBai { get; set; }

    public string TieuDe { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? LoaiNoiDung { get; set; }

    public string? DuongDan { get; set; }

    public virtual ICollection<NguoiDungBaiHoc> NguoiDungBaiHocs { get; set; } = new List<NguoiDungBaiHoc>();
}
