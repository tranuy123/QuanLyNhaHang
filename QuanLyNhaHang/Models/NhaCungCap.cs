using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class NhaCungCap
    {
        public NhaCungCap()
        {
            PhieuNhap = new HashSet<PhieuNhap>();
        }

        public int Idncc { get; set; }
        public string MaNcc { get; set; }
        public string TenNcc { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Mail { get; set; }
        public string GhiChu { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<PhieuNhap> PhieuNhap { get; set; }
    }
}
