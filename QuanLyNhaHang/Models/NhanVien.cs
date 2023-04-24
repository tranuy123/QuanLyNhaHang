using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            HoaDon = new HashSet<HoaDon>();
            LichLamViec = new HashSet<LichLamViec>();
        }

        public int Idnv { get; set; }
        public string MaMv { get; set; }
        public string Ten { get; set; }
        public DateTime? Tuoi { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public int? Idtk { get; set; }
        public int? Idnnv { get; set; }

        public virtual NhomNhanVien IdnnvNavigation { get; set; }
        public virtual TaiKhoan IdtkNavigation { get; set; }
        public virtual ICollection<HoaDon> HoaDon { get; set; }
        public virtual ICollection<LichLamViec> LichLamViec { get; set; }
    }
}
