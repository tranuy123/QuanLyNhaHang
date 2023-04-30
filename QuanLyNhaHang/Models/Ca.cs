using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class Ca
    {
        public Ca()
        {
            ChiTietHoaDon = new HashSet<ChiTietHoaDon>();
            LichLamViec = new HashSet<LichLamViec>();
        }

        public int Idca { get; set; }
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public TimeSpan? TgbatDau { get; set; }
        public TimeSpan? TgketThuc { get; set; }
        public string Thu { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDon { get; set; }
        public virtual ICollection<LichLamViec> LichLamViec { get; set; }
    }
}
