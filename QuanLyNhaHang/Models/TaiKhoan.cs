using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class TaiKhoan
    {
        public TaiKhoan()
        {
            NhanVien = new HashSet<NhanVien>();
        }

        public int Idtk { get; set; }
        public string TenTk { get; set; }
        public string Pass { get; set; }
        public bool? Active { get; set; }
        public int? Idvt { get; set; }

        public virtual VaiTro IdvtNavigation { get; set; }
        public virtual ICollection<NhanVien> NhanVien { get; set; }
    }
}
