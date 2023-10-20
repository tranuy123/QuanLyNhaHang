using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class NhomNhanVien
    {
        public NhomNhanVien()
        {
            NhanVien = new HashSet<NhanVien>();
            QlDanhGia = new HashSet<QlDanhGia>();
        }

        public int Idnnv { get; set; }
        public string MaNnv { get; set; }
        public string TenNnv { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<NhanVien> NhanVien { get; set; }
        public virtual ICollection<QlDanhGia> QlDanhGia { get; set; }
    }
}
