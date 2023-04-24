using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class VaiTro
    {
        public VaiTro()
        {
            TaiKhoan = new HashSet<TaiKhoan>();
        }

        public int Idvt { get; set; }
        public string MaVt { get; set; }
        public string TenVt { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<TaiKhoan> TaiKhoan { get; set; }
    }
}
