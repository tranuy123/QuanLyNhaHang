using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class LichLamViec
    {
        public int Idllv { get; set; }
        public int? Idkhu { get; set; }
        public int? Idnv { get; set; }
        public int? Idca { get; set; }
        public bool? Active { get; set; }

        public virtual Ca IdcaNavigation { get; set; }
        public virtual Khu IdkhuNavigation { get; set; }
        public virtual NhanVien IdnvNavigation { get; set; }
    }
}
