using System;
using System.Collections.Generic;

namespace Project1.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? NgonNguUaThich { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<GhiAm> GhiAms { get; set; } = new List<GhiAm>();

    public virtual ICollection<KetBan> KetBanMaNguoi1Navigations { get; set; } = new List<KetBan>();

    public virtual ICollection<KetBan> KetBanMaNguoi2Navigations { get; set; } = new List<KetBan>();

    public virtual ICollection<KetQuaBaiThi> KetQuaBaiThis { get; set; } = new List<KetQuaBaiThi>();

    public virtual ICollection<NguoiDungBaiHoc> NguoiDungBaiHocs { get; set; } = new List<NguoiDungBaiHoc>();

    public virtual ICollection<NguoiDungTuVung> NguoiDungTuVungs { get; set; } = new List<NguoiDungTuVung>();

    public virtual ICollection<NhatKyHoatDong> NhatKyHoatDongs { get; set; } = new List<NhatKyHoatDong>();
}
