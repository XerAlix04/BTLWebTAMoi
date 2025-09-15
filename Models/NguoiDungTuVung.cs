using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class NguoiDungTuVung
{
    public int Ma { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaTu { get; set; }

    public double? HeSoDe { get; set; }

    public DateTime? NgayOnTapCuoi { get; set; }

    public DateTime? NgayOnTapTiep { get; set; }

    public bool? DaThanhThao { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual TuVung? MaTuNavigation { get; set; }
}
