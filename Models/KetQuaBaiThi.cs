using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class KetQuaBaiThi
{
    public int Ma { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaDe { get; set; }

    public double? Diem { get; set; }

    public string? ChiTiet { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual DeThiMau? MaDeNavigation { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
