using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project1.Models;

public partial class WebTaContext : DbContext
{
    public WebTaContext()
    {
    }

    public WebTaContext(DbContextOptions<WebTaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaiHoc> BaiHocs { get; set; }

    public virtual DbSet<CauHoi> CauHois { get; set; }

    public virtual DbSet<DeThiMau> DeThiMaus { get; set; }

    public virtual DbSet<DoanVan> DoanVans { get; set; }

    public virtual DbSet<GhiAm> GhiAms { get; set; }

    public virtual DbSet<KetBan> KetBans { get; set; }

    public virtual DbSet<KetQuaBaiThi> KetQuaBaiThis { get; set; }

    public virtual DbSet<LuaChon> LuaChons { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiDungBaiHoc> NguoiDungBaiHocs { get; set; }

    public virtual DbSet<NguoiDungTuVung> NguoiDungTuVungs { get; set; }

    public virtual DbSet<NhatKyHoatDong> NhatKyHoatDongs { get; set; }

    public virtual DbSet<TuVung> TuVungs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-P9RRDLP\\SQLEXPRESS;Initial Catalog=WebTA;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiHoc>(entity =>
        {
            entity.HasKey(e => e.MaBai).HasName("PK__BaiHoc__03FFDF7033CBE4A7");

            entity.ToTable("BaiHoc");

            entity.Property(e => e.MaBai).HasColumnName("ma_bai");
            entity.Property(e => e.DuongDan)
                .HasMaxLength(255)
                .HasColumnName("duong_dan");
            entity.Property(e => e.LoaiNoiDung)
                .HasMaxLength(50)
                .HasColumnName("loai_noi_dung");
            entity.Property(e => e.MoTa).HasColumnName("mo_ta");
            entity.Property(e => e.TieuDe)
                .HasMaxLength(200)
                .HasColumnName("tieu_de");
        });

        modelBuilder.Entity<CauHoi>(entity =>
        {
            entity.HasKey(e => e.MaCau).HasName("PK__CauHoi__043FED5DF5C14A3D");

            entity.ToTable("CauHoi");

            entity.Property(e => e.MaCau).HasColumnName("ma_cau");
            entity.Property(e => e.MaDoan).HasColumnName("ma_doan");
            entity.Property(e => e.NoiDung).HasColumnName("noi_dung");

            entity.HasOne(d => d.MaDoanNavigation).WithMany(p => p.CauHois)
                .HasForeignKey(d => d.MaDoan)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CauHoi__ma_doan__5070F446");
        });

        modelBuilder.Entity<DeThiMau>(entity =>
        {
            entity.HasKey(e => e.MaDe).HasName("PK__DeThiMau__0FE14F02BD6445BE");

            entity.ToTable("DeThiMau");

            entity.Property(e => e.MaDe).HasColumnName("ma_de");
            entity.Property(e => e.Loai)
                .HasMaxLength(50)
                .HasColumnName("loai");
            entity.Property(e => e.MoTa).HasColumnName("mo_ta");
            entity.Property(e => e.TieuDe)
                .HasMaxLength(200)
                .HasColumnName("tieu_de");

            entity.HasMany(d => d.MaCaus).WithMany(p => p.MaDes)
                .UsingEntity<Dictionary<string, object>>(
                    "DeThiMauCauHoi",
                    r => r.HasOne<CauHoi>().WithMany()
                        .HasForeignKey("MaCau")
                        .HasConstraintName("FK__DeThiMau___ma_ca__59FA5E80"),
                    l => l.HasOne<DeThiMau>().WithMany()
                        .HasForeignKey("MaDe")
                        .HasConstraintName("FK__DeThiMau___ma_de__59063A47"),
                    j =>
                    {
                        j.HasKey("MaDe", "MaCau").HasName("PK__DeThiMau__CFA2B1D7E2DB2083");
                        j.ToTable("DeThiMau_CauHoi");
                        j.IndexerProperty<int>("MaDe").HasColumnName("ma_de");
                        j.IndexerProperty<int>("MaCau").HasColumnName("ma_cau");
                    });
        });

        modelBuilder.Entity<DoanVan>(entity =>
        {
            entity.HasKey(e => e.MaDoan).HasName("PK__DoanVan__99090DA573447039");

            entity.ToTable("DoanVan");

            entity.Property(e => e.MaDoan).HasColumnName("ma_doan");
            entity.Property(e => e.NoiDung).HasColumnName("noi_dung");
            entity.Property(e => e.TieuDe)
                .HasMaxLength(200)
                .HasColumnName("tieu_de");
            entity.Property(e => e.TomTat).HasColumnName("tom_tat");
            entity.Property(e => e.TuKhoa).HasColumnName("tu_khoa");
        });

        modelBuilder.Entity<GhiAm>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__GhiAm__3213C8B760ACF60F");

            entity.ToTable("GhiAm");

            entity.Property(e => e.Ma).HasColumnName("ma");
            entity.Property(e => e.Diem).HasColumnName("diem");
            entity.Property(e => e.DuongDanAudio)
                .HasMaxLength(255)
                .HasColumnName("duong_dan_audio");
            entity.Property(e => e.LienKetId).HasColumnName("lien_ket_id");
            entity.Property(e => e.LoaiLienKet)
                .HasMaxLength(50)
                .HasColumnName("loai_lien_ket");
            entity.Property(e => e.MaNguoiDung).HasColumnName("ma_nguoi_dung");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.NhanXet).HasColumnName("nhan_xet");
            entity.Property(e => e.VanBan).HasColumnName("van_ban");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GhiAms)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GhiAm__ma_nguoi___628FA481");
        });

        modelBuilder.Entity<KetBan>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__KetBan__3213C8B768656821");

            entity.ToTable("KetBan");

            entity.HasIndex(e => new { e.MaNguoi1, e.MaNguoi2 }, "UQ_KetBan").IsUnique();

            entity.Property(e => e.Ma).HasColumnName("ma");
            entity.Property(e => e.MaNguoi1).HasColumnName("ma_nguoi_1");
            entity.Property(e => e.MaNguoi2).HasColumnName("ma_nguoi_2");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("cho")
                .HasColumnName("trang_thai");

            entity.HasOne(d => d.MaNguoi1Navigation).WithMany(p => p.KetBanMaNguoi1Navigations)
                .HasForeignKey(d => d.MaNguoi1)
                .HasConstraintName("FK__KetBan__ma_nguoi__6754599E");

            entity.HasOne(d => d.MaNguoi2Navigation).WithMany(p => p.KetBanMaNguoi2Navigations)
                .HasForeignKey(d => d.MaNguoi2)
                .HasConstraintName("FK__KetBan__ma_nguoi__68487DD7");
        });

        modelBuilder.Entity<KetQuaBaiThi>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__KetQuaBa__3213C8B7B1A6B463");

            entity.ToTable("KetQuaBaiThi");

            entity.Property(e => e.Ma).HasColumnName("ma");
            entity.Property(e => e.ChiTiet).HasColumnName("chi_tiet");
            entity.Property(e => e.Diem).HasColumnName("diem");
            entity.Property(e => e.MaDe).HasColumnName("ma_de");
            entity.Property(e => e.MaNguoiDung).HasColumnName("ma_nguoi_dung");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("ngay_tao");

            entity.HasOne(d => d.MaDeNavigation).WithMany(p => p.KetQuaBaiThis)
                .HasForeignKey(d => d.MaDe)
                .HasConstraintName("FK__KetQuaBai__ma_de__5DCAEF64");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.KetQuaBaiThis)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__KetQuaBai__ma_ng__5CD6CB2B");
        });

        modelBuilder.Entity<LuaChon>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__LuaChon__3213C8B701CE516C");

            entity.ToTable("LuaChon");

            entity.Property(e => e.Ma).HasColumnName("ma");
            entity.Property(e => e.LaDapAn).HasColumnName("la_dap_an");
            entity.Property(e => e.MaCau).HasColumnName("ma_cau");
            entity.Property(e => e.NoiDung).HasColumnName("noi_dung");

            entity.HasOne(d => d.MaCauNavigation).WithMany(p => p.LuaChons)
                .HasForeignKey(d => d.MaCau)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__LuaChon__ma_cau__534D60F1");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__19C32CF7874D3E46");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "UQ__NguoiDun__363698B33605E88F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__AB6E616459BB182D").IsUnique();

            entity.Property(e => e.MaNguoiDung).HasColumnName("ma_nguoi_dung");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.MatKhau).HasColumnName("mat_khau");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.NgonNguUaThich)
                .HasMaxLength(10)
                .HasColumnName("ngon_ngu_ua_thich");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .HasColumnName("ten_dang_nhap");
        });

        modelBuilder.Entity<NguoiDungBaiHoc>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__NguoiDun__3213C8B7579B0F96");

            entity.ToTable("NguoiDung_BaiHoc");

            entity.HasIndex(e => new { e.MaNguoiDung, e.MaBai }, "UQ_NguoiDung_BaiHoc").IsUnique();

            entity.Property(e => e.Ma).HasColumnName("ma");
            entity.Property(e => e.Diem).HasColumnName("diem");
            entity.Property(e => e.MaBai).HasColumnName("ma_bai");
            entity.Property(e => e.MaNguoiDung).HasColumnName("ma_nguoi_dung");
            entity.Property(e => e.NgayHoanThanh).HasColumnName("ngay_hoan_thanh");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("chua_bat_dau")
                .HasColumnName("trang_thai");

            entity.HasOne(d => d.MaBaiNavigation).WithMany(p => p.NguoiDungBaiHocs)
                .HasForeignKey(d => d.MaBai)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__NguoiDung__ma_ba__48CFD27E");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.NguoiDungBaiHocs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__NguoiDung__ma_ng__47DBAE45");
        });

        modelBuilder.Entity<NguoiDungTuVung>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__NguoiDun__3213C8B73A313E59");

            entity.ToTable("NguoiDung_TuVung");

            entity.HasIndex(e => new { e.MaNguoiDung, e.MaTu }, "UQ_NguoiDung_TuVung").IsUnique();

            entity.Property(e => e.Ma).HasColumnName("ma");
            entity.Property(e => e.DaThanhThao)
                .HasDefaultValue(false)
                .HasColumnName("da_thanh_thao");
            entity.Property(e => e.HeSoDe)
                .HasDefaultValue(2.5)
                .HasColumnName("he_so_de");
            entity.Property(e => e.MaNguoiDung).HasColumnName("ma_nguoi_dung");
            entity.Property(e => e.MaTu).HasColumnName("ma_tu");
            entity.Property(e => e.NgayOnTapCuoi).HasColumnName("ngay_on_tap_cuoi");
            entity.Property(e => e.NgayOnTapTiep).HasColumnName("ngay_on_tap_tiep");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.NguoiDungTuVungs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__NguoiDung__ma_ng__3F466844");

            entity.HasOne(d => d.MaTuNavigation).WithMany(p => p.NguoiDungTuVungs)
                .HasForeignKey(d => d.MaTu)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__NguoiDung__ma_tu__403A8C7D");
        });

        modelBuilder.Entity<NhatKyHoatDong>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__NhatKyHo__3213C8B768518CCB");

            entity.ToTable("NhatKyHoatDong");

            entity.Property(e => e.Ma).HasColumnName("ma");
            entity.Property(e => e.DuLieu).HasColumnName("du_lieu");
            entity.Property(e => e.HanhDong)
                .HasMaxLength(50)
                .HasColumnName("hanh_dong");
            entity.Property(e => e.MaNguoiDung).HasColumnName("ma_nguoi_dung");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("ngay_tao");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.NhatKyHoatDongs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__NhatKyHoa__ma_ng__6E01572D");
        });

        modelBuilder.Entity<TuVung>(entity =>
        {
            entity.HasKey(e => e.MaTu).HasName("PK__TuVung__0FE0CD2E9C437D01");

            entity.ToTable("TuVung");

            entity.Property(e => e.MaTu).HasColumnName("ma_tu");
            entity.Property(e => e.DuongDanAnh)
                .HasMaxLength(255)
                .HasColumnName("duong_dan_anh");
            entity.Property(e => e.Nghia).HasColumnName("nghia");
            entity.Property(e => e.PhienAm)
                .HasMaxLength(100)
                .HasColumnName("phien_am");
            entity.Property(e => e.TrinhDo)
                .HasMaxLength(10)
                .HasColumnName("trinh_do");
            entity.Property(e => e.Tu)
                .HasMaxLength(100)
                .HasColumnName("tu");
            entity.Property(e => e.TuLoai)
                .HasMaxLength(50)
                .HasColumnName("tu_loai");
            entity.Property(e => e.ViDu).HasColumnName("vi_du");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
