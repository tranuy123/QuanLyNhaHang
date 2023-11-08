using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class ThucDon
    {
        public ThucDon()
        {
            ChiTietHoaDon = new HashSet<ChiTietHoaDon>();
            Gia = new HashSet<Gia>();
        }

        public int Idtd { get; set; }
        public string MaTd { get; set; }
        public string Ten { get; set; }
        public string Image { get; set; }
        public string Detail { get; set; }
        public bool? TinhTrang { get; set; }
        public bool? Active { get; set; }
        public int? Idnta { get; set; }
        public int? Idhh { get; set; }

        public virtual HangHoa IdhhNavigation { get; set; }
        public virtual NhomThucAn IdntaNavigation { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDon { get; set; }
        public virtual ICollection<Gia> Gia { get; set; }
    }
}
