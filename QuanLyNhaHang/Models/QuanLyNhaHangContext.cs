using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class QuanLyNhaHangContext : DbContext
    {
        public QuanLyNhaHangContext()
        {
        }

        public QuanLyNhaHangContext(DbContextOptions<QuanLyNhaHangContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ban> Ban { get; set; }
        public virtual DbSet<Ca> Ca { get; set; }
        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDon { get; set; }
        public virtual DbSet<ChiTietHoaDonTam> ChiTietHoaDonTam { get; set; }
        public virtual DbSet<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
        public virtual DbSet<ChiTietPhieuXuat> ChiTietPhieuXuat { get; set; }
        public virtual DbSet<DinhMuc> DinhMuc { get; set; }
        public virtual DbSet<DonViTinh> DonViTinh { get; set; }
        public virtual DbSet<Gia> Gia { get; set; }
        public virtual DbSet<HangHoa> HangHoa { get; set; }
        public virtual DbSet<HoaDon> HoaDon { get; set; }
        public virtual DbSet<Khu> Khu { get; set; }
        public virtual DbSet<LichLamViec> LichLamViec { get; set; }
        public virtual DbSet<NhaCungCap> NhaCungCap { get; set; }
        public virtual DbSet<NhanVien> NhanVien { get; set; }
        public virtual DbSet<NhomHangHoa> NhomHangHoa { get; set; }
        public virtual DbSet<NhomNhanVien> NhomNhanVien { get; set; }
        public virtual DbSet<NhomThucAn> NhomThucAn { get; set; }
        public virtual DbSet<PhieuNhap> PhieuNhap { get; set; }
        public virtual DbSet<PhieuXuat> PhieuXuat { get; set; }
        public virtual DbSet<QlDanhGia> QlDanhGia { get; set; }
        public virtual DbSet<Sanh> Sanh { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoan { get; set; }
        public virtual DbSet<ThucDon> ThucDon { get; set; }
        public virtual DbSet<TonKho> TonKho { get; set; }
        public virtual DbSet<VaiTro> VaiTro { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=TRAN-UY\\MSSQLSERVER22;Database=QuanLyNhaHang;User Id=sa;Password=123456;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ban>(entity =>
            {
                entity.HasKey(e => e.Idban);

                entity.Property(e => e.Idban).HasColumnName("IDBan");

                entity.Property(e => e.Idkhu).HasColumnName("IDKhu");

                entity.Property(e => e.Ipmac)
                    .HasColumnName("IPMac")
                    .HasMaxLength(100);

                entity.Property(e => e.MaBan).HasMaxLength(50);

                entity.Property(e => e.TenBan).HasMaxLength(200);

                entity.HasOne(d => d.IdkhuNavigation)
                    .WithMany(p => p.Ban)
                    .HasForeignKey(d => d.Idkhu)
                    .HasConstraintName("FK_Ban_Khu");
            });

            modelBuilder.Entity<Ca>(entity =>
            {
                entity.HasKey(e => e.Idca);

                entity.Property(e => e.Idca).HasColumnName("IDCa");

                entity.Property(e => e.MaCa).HasMaxLength(50);

                entity.Property(e => e.TenCa).HasMaxLength(200);

                entity.Property(e => e.TgbatDau).HasColumnName("TGBatDau");

                entity.Property(e => e.TgketThuc).HasColumnName("TGKetThuc");

                entity.Property(e => e.Thu).HasMaxLength(27);
            });

            modelBuilder.Entity<ChiTietHoaDon>(entity =>
            {
                entity.HasKey(e => e.Idcthd);

                entity.Property(e => e.Idcthd).HasColumnName("IDCTHD");

                entity.Property(e => e.Idca).HasColumnName("IDCa");

                entity.Property(e => e.Idhd).HasColumnName("IDHD");

                entity.Property(e => e.Idtd).HasColumnName("IDTD");

                entity.Property(e => e.Sl).HasColumnName("SL");

                entity.Property(e => e.Tgbep)
                    .HasColumnName("TGBep")
                    .HasColumnType("datetime");

                entity.Property(e => e.TghoanThanh)
                    .HasColumnName("TGHoanThanh")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tgorder)
                    .HasColumnName("TGOrder")
                    .HasColumnType("datetime");

                entity.Property(e => e.TgphucVu)
                    .HasColumnName("TGPhucVu")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdcaNavigation)
                    .WithMany(p => p.ChiTietHoaDon)
                    .HasForeignKey(d => d.Idca)
                    .HasConstraintName("FK_ChiTietHoaDon_Ca");

                entity.HasOne(d => d.IdhdNavigation)
                    .WithMany(p => p.ChiTietHoaDon)
                    .HasForeignKey(d => d.Idhd)
                    .HasConstraintName("FK_ChiTietHoaDon_HoaDon");

                entity.HasOne(d => d.IdtdNavigation)
                    .WithMany(p => p.ChiTietHoaDon)
                    .HasForeignKey(d => d.Idtd)
                    .HasConstraintName("FK_ChiTietHoaDon_ThucDon");
            });

            modelBuilder.Entity<ChiTietHoaDonTam>(entity =>
            {
                entity.HasKey(e => e.Idcthdt);

                entity.Property(e => e.Idcthdt).HasColumnName("IDCTHDT");

                entity.Property(e => e.Idhd).HasColumnName("IDHD");

                entity.Property(e => e.Idtd).HasColumnName("IDTD");

                entity.Property(e => e.Ipmac)
                    .HasColumnName("IPMAC")
                    .HasMaxLength(50);

                entity.Property(e => e.Sl).HasColumnName("SL");
            });

            modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
            {
                entity.HasKey(e => e.Idctpn);

                entity.Property(e => e.Idctpn).HasColumnName("IDCTPN");

                entity.Property(e => e.Hsd)
                    .HasColumnName("HSD")
                    .HasColumnType("datetime");

                entity.Property(e => e.Idhh).HasColumnName("IDHH");

                entity.Property(e => e.Idpn).HasColumnName("IDPN");

                entity.Property(e => e.Nsx)
                    .HasColumnName("NSX")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdhhNavigation)
                    .WithMany(p => p.ChiTietPhieuNhap)
                    .HasForeignKey(d => d.Idhh)
                    .HasConstraintName("FK_ChiTietPhieuNhap_HangHoa");

                entity.HasOne(d => d.IdpnNavigation)
                    .WithMany(p => p.ChiTietPhieuNhap)
                    .HasForeignKey(d => d.Idpn)
                    .HasConstraintName("FK_ChiTietPhieuNhap_PhieuNhap");
            });

            modelBuilder.Entity<ChiTietPhieuXuat>(entity =>
            {
                entity.HasKey(e => e.Idctpx);

                entity.Property(e => e.Idctpx).HasColumnName("IDCTPX");

                entity.Property(e => e.Idctpn).HasColumnName("IDCTPN");

                entity.Property(e => e.Idhh).HasColumnName("IDHH");

                entity.Property(e => e.Idpx).HasColumnName("IDPX");

                entity.HasOne(d => d.IdctpnNavigation)
                    .WithMany(p => p.ChiTietPhieuXuat)
                    .HasForeignKey(d => d.Idctpn)
                    .HasConstraintName("FK_ChiTietPhieuXuat_ChiTietPhieuNhap");

                entity.HasOne(d => d.IdhhNavigation)
                    .WithMany(p => p.ChiTietPhieuXuat)
                    .HasForeignKey(d => d.Idhh)
                    .HasConstraintName("FK_ChiTietPhieuXuat_HangHoa");

                entity.HasOne(d => d.IdpxNavigation)
                    .WithMany(p => p.ChiTietPhieuXuat)
                    .HasForeignKey(d => d.Idpx)
                    .HasConstraintName("FK_ChiTietPhieuXuat_PhieuXuat");
            });

            modelBuilder.Entity<DinhMuc>(entity =>
            {
                entity.HasKey(e => e.Iddm);

                entity.Property(e => e.Iddm).HasColumnName("IDDM");

                entity.Property(e => e.Idhh).HasColumnName("IDHH");

                entity.Property(e => e.Idtd).HasColumnName("IDTD");

                entity.HasOne(d => d.IdhhNavigation)
                    .WithMany(p => p.DinhMuc)
                    .HasForeignKey(d => d.Idhh)
                    .HasConstraintName("FK_DinhMuc_HangHoa");

                entity.HasOne(d => d.IdtdNavigation)
                    .WithMany(p => p.DinhMuc)
                    .HasForeignKey(d => d.Idtd)
                    .HasConstraintName("FK_DinhMuc_ThucDon");
            });

            modelBuilder.Entity<DonViTinh>(entity =>
            {
                entity.HasKey(e => e.Iddvt);

                entity.Property(e => e.Iddvt).HasColumnName("IDDVT");

                entity.Property(e => e.MaDvt)
                    .HasColumnName("MaDVT")
                    .HasMaxLength(50);

                entity.Property(e => e.TenDvt)
                    .HasColumnName("TenDVT")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Gia>(entity =>
            {
                entity.HasKey(e => e.Idgia);

                entity.Property(e => e.Idgia).HasColumnName("IDGia");

                entity.Property(e => e.Gia1).HasColumnName("Gia");

                entity.Property(e => e.Idsanh).HasColumnName("IDSanh");

                entity.Property(e => e.Idtd).HasColumnName("IDTD");

                entity.HasOne(d => d.IdsanhNavigation)
                    .WithMany(p => p.Gia)
                    .HasForeignKey(d => d.Idsanh)
                    .HasConstraintName("FK_Gia_Sanh");

                entity.HasOne(d => d.IdtdNavigation)
                    .WithMany(p => p.Gia)
                    .HasForeignKey(d => d.Idtd)
                    .HasConstraintName("FK_Gia_ThucDon");
            });

            modelBuilder.Entity<HangHoa>(entity =>
            {
                entity.HasKey(e => e.Idhh);

                entity.Property(e => e.Idhh).HasColumnName("IDHH");

                entity.Property(e => e.Avatar).HasMaxLength(500);

                entity.Property(e => e.Iddvt).HasColumnName("IDDVT");

                entity.Property(e => e.Idnhh).HasColumnName("IDNHH");

                entity.Property(e => e.MaHh)
                    .HasColumnName("MaHH")
                    .HasMaxLength(100);

                entity.Property(e => e.TenHh)
                    .HasColumnName("TenHH")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IddvtNavigation)
                    .WithMany(p => p.HangHoa)
                    .HasForeignKey(d => d.Iddvt)
                    .HasConstraintName("FK_HangHoa_DonViTinh");

                entity.HasOne(d => d.IdnhhNavigation)
                    .WithMany(p => p.HangHoaNavigation)
                    .HasForeignKey(d => d.Idnhh)
                    .HasConstraintName("FK_HangHoa_NhomHangHoa");
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.Idhd);

                entity.Property(e => e.Idhd).HasColumnName("IDHD");

                entity.Property(e => e.Idban).HasColumnName("IDBan");

                entity.Property(e => e.Idnv).HasColumnName("IDNV");

                entity.Property(e => e.MaHd)
                    .HasColumnName("MaHD")
                    .HasMaxLength(50);

                entity.Property(e => e.Tgxuat)
                    .HasColumnName("TGXuat")
                    .HasColumnType("datetime");

                entity.Property(e => e.TinhTrangTt).HasColumnName("TinhTrangTT");

                entity.HasOne(d => d.IdbanNavigation)
                    .WithMany(p => p.HoaDon)
                    .HasForeignKey(d => d.Idban)
                    .HasConstraintName("FK_HoaDon_Ban");

                entity.HasOne(d => d.IdnvNavigation)
                    .WithMany(p => p.HoaDon)
                    .HasForeignKey(d => d.Idnv)
                    .HasConstraintName("FK_HoaDon_NhanVien");
            });

            modelBuilder.Entity<Khu>(entity =>
            {
                entity.HasKey(e => e.Idkhu);

                entity.Property(e => e.Idkhu).HasColumnName("IDKhu");

                entity.Property(e => e.Idsanh).HasColumnName("IDSanh");

                entity.Property(e => e.MaKhu).HasMaxLength(50);

                entity.Property(e => e.Ngay).HasColumnType("datetime");

                entity.Property(e => e.TenKhu).HasMaxLength(200);

                entity.HasOne(d => d.IdsanhNavigation)
                    .WithMany(p => p.Khu)
                    .HasForeignKey(d => d.Idsanh)
                    .HasConstraintName("FK_Khu_Sanh");
            });

            modelBuilder.Entity<LichLamViec>(entity =>
            {
                entity.HasKey(e => e.Idllv);

                entity.Property(e => e.Idllv).HasColumnName("IDLLV");

                entity.Property(e => e.Idca).HasColumnName("IDCa");

                entity.Property(e => e.Idkhu).HasColumnName("IDKhu");

                entity.Property(e => e.Idnv).HasColumnName("IDNV");

                entity.HasOne(d => d.IdcaNavigation)
                    .WithMany(p => p.LichLamViec)
                    .HasForeignKey(d => d.Idca)
                    .HasConstraintName("FK_LichLamViec_Ca");

                entity.HasOne(d => d.IdkhuNavigation)
                    .WithMany(p => p.LichLamViec)
                    .HasForeignKey(d => d.Idkhu)
                    .HasConstraintName("FK_LichLamViec_Khu");

                entity.HasOne(d => d.IdnvNavigation)
                    .WithMany(p => p.LichLamViec)
                    .HasForeignKey(d => d.Idnv)
                    .HasConstraintName("FK_LichLamViec_NhanVien");
            });

            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.HasKey(e => e.Idncc);

                entity.Property(e => e.Idncc).HasColumnName("IDNCC");

                entity.Property(e => e.DiaChi).HasMaxLength(500);

                entity.Property(e => e.DienThoai).HasMaxLength(50);

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.MaNcc)
                    .HasColumnName("MaNCC")
                    .HasMaxLength(100);

                entity.Property(e => e.Mail).HasMaxLength(100);

                entity.Property(e => e.TenNcc)
                    .HasColumnName("TenNCC")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.Idnv);

                entity.Property(e => e.Idnv).HasColumnName("IDNV");

                entity.Property(e => e.DiaChi).HasMaxLength(500);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Idnnv).HasColumnName("IDNNV");

                entity.Property(e => e.Idtk).HasColumnName("IDTK");

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.MaMv)
                    .HasColumnName("MaMV")
                    .HasMaxLength(50);

                entity.Property(e => e.QueQuan).HasMaxLength(250);

                entity.Property(e => e.Sdt)
                    .HasColumnName("SDT")
                    .HasMaxLength(50);

                entity.Property(e => e.Ten).HasMaxLength(200);

                entity.Property(e => e.Tuoi).HasColumnType("date");

                entity.HasOne(d => d.IdnnvNavigation)
                    .WithMany(p => p.NhanVien)
                    .HasForeignKey(d => d.Idnnv)
                    .HasConstraintName("FK_NhanVien_NhomNhanVien");

                entity.HasOne(d => d.IdtkNavigation)
                    .WithMany(p => p.NhanVien)
                    .HasForeignKey(d => d.Idtk)
                    .HasConstraintName("FK_NhanVien_TaiKhoan");
            });

            modelBuilder.Entity<NhomHangHoa>(entity =>
            {
                entity.HasKey(e => e.Idnhh);

                entity.Property(e => e.Idnhh).HasColumnName("IDNHH");

                entity.Property(e => e.MaNhh)
                    .HasColumnName("MaNHH")
                    .HasMaxLength(100);

                entity.Property(e => e.TenNhh)
                    .HasColumnName("TenNHH")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<NhomNhanVien>(entity =>
            {
                entity.HasKey(e => e.Idnnv);

                entity.Property(e => e.Idnnv).HasColumnName("IDNNV");

                entity.Property(e => e.MaNnv)
                    .HasColumnName("MaNNV")
                    .HasMaxLength(50);

                entity.Property(e => e.TenNnv)
                    .HasColumnName("TenNNV")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<NhomThucAn>(entity =>
            {
                entity.HasKey(e => e.Idnta);

                entity.Property(e => e.Idnta).HasColumnName("IDNTA");

                entity.Property(e => e.MaNta)
                    .HasColumnName("MaNTA")
                    .HasMaxLength(50);

                entity.Property(e => e.TenNta)
                    .HasColumnName("TenNTA")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<PhieuNhap>(entity =>
            {
                entity.HasKey(e => e.Idpn);

                entity.Property(e => e.Idpn).HasColumnName("IDPN");

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.Idncc).HasColumnName("IDNCC");

                entity.Property(e => e.Idnv).HasColumnName("IDNV");

                entity.Property(e => e.NgayHd)
                    .HasColumnName("NgayHD")
                    .HasColumnType("datetime");

                entity.Property(e => e.NgayNhap).HasColumnType("datetime");

                entity.Property(e => e.SoHd)
                    .HasColumnName("SoHD")
                    .HasMaxLength(100);

                entity.Property(e => e.SoPn)
                    .HasColumnName("SoPN")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdnccNavigation)
                    .WithMany(p => p.PhieuNhap)
                    .HasForeignKey(d => d.Idncc)
                    .HasConstraintName("FK_PhieuNhap_NhaCungCap");

                entity.HasOne(d => d.IdnvNavigation)
                    .WithMany(p => p.PhieuNhap)
                    .HasForeignKey(d => d.Idnv)
                    .HasConstraintName("FK_PhieuNhap_NhanVien");
            });

            modelBuilder.Entity<PhieuXuat>(entity =>
            {
                entity.HasKey(e => e.Idpx);

                entity.Property(e => e.Idpx).HasColumnName("IDPX");

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.Idkh).HasColumnName("IDKH");

                entity.Property(e => e.Idnv).HasColumnName("IDNV");

                entity.Property(e => e.NgayHd)
                    .HasColumnName("NgayHD")
                    .HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.SoHd)
                    .HasColumnName("SoHD")
                    .HasMaxLength(100);

                entity.Property(e => e.SoPx)
                    .HasColumnName("SoPX")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdnvNavigation)
                    .WithMany(p => p.PhieuXuat)
                    .HasForeignKey(d => d.Idnv)
                    .HasConstraintName("FK_PhieuXuat_NhanVien");
            });

            modelBuilder.Entity<QlDanhGia>(entity =>
            {
                entity.HasKey(e => e.IddanhGia);

                entity.ToTable("QL_DanhGia");

                entity.Property(e => e.IddanhGia).HasColumnName("IDDanhGia");

                entity.Property(e => e.Idnnv).HasColumnName("IDNNV");

                entity.Property(e => e.Ma).HasMaxLength(50);

                entity.Property(e => e.Ten).HasMaxLength(50);

                entity.HasOne(d => d.IdnnvNavigation)
                    .WithMany(p => p.QlDanhGia)
                    .HasForeignKey(d => d.Idnnv)
                    .HasConstraintName("FK_QL_DanhGia_NhomNhanVien");
            });

            modelBuilder.Entity<Sanh>(entity =>
            {
                entity.HasKey(e => e.Idsanh);

                entity.Property(e => e.Idsanh).HasColumnName("IDSanh");

                entity.Property(e => e.MaSanh).HasMaxLength(50);

                entity.Property(e => e.TenSanh).HasMaxLength(200);
            });

            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasKey(e => e.Idtk);

                entity.Property(e => e.Idtk).HasColumnName("IDTK");

                entity.Property(e => e.Idvt).HasColumnName("IDVT");

                entity.Property(e => e.Pass).HasMaxLength(100);

                entity.Property(e => e.TenTk)
                    .HasColumnName("TenTK")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdvtNavigation)
                    .WithMany(p => p.TaiKhoan)
                    .HasForeignKey(d => d.Idvt)
                    .HasConstraintName("FK_TaiKhoan_VaiTro");
            });

            modelBuilder.Entity<ThucDon>(entity =>
            {
                entity.HasKey(e => e.Idtd);

                entity.Property(e => e.Idtd).HasColumnName("IDTD");

                entity.Property(e => e.Detail).HasMaxLength(800);

                entity.Property(e => e.Idnta).HasColumnName("IDNTA");

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.MaTd)
                    .HasColumnName("MaTD")
                    .HasMaxLength(50);

                entity.Property(e => e.Ten).HasMaxLength(200);

                entity.HasOne(d => d.IdntaNavigation)
                    .WithMany(p => p.ThucDon)
                    .HasForeignKey(d => d.Idnta)
                    .HasConstraintName("FK_ThucDon_NhomThucAn");
            });

            modelBuilder.Entity<TonKho>(entity =>
            {
                entity.HasKey(e => e.Idtk);

                entity.Property(e => e.Idtk).HasColumnName("IDTK");

                entity.Property(e => e.Idctpn).HasColumnName("IDCTPN");

                entity.Property(e => e.NgayNhap).HasColumnType("datetime");

                entity.HasOne(d => d.IdctpnNavigation)
                    .WithMany(p => p.TonKho)
                    .HasForeignKey(d => d.Idctpn)
                    .HasConstraintName("FK_TonKho_ChiTietPhieuNhap");
            });

            modelBuilder.Entity<VaiTro>(entity =>
            {
                entity.HasKey(e => e.Idvt);

                entity.Property(e => e.Idvt).HasColumnName("IDVT");

                entity.Property(e => e.MaVt)
                    .HasColumnName("MaVT")
                    .HasMaxLength(50);

                entity.Property(e => e.TenVt)
                    .HasColumnName("TenVT")
                    .HasMaxLength(200);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
