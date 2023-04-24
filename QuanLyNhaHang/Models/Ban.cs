using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class Ban
    {
        public Ban()
        {
            HoaDon = new HashSet<HoaDon>();
        }

        public int Idban { get; set; }
        public string Ipmac { get; set; }
        public string MaBan { get; set; }
        public string TenBan { get; set; }
        public bool? Active { get; set; }
        public int? Idkhu { get; set; }

        public virtual Khu IdkhuNavigation { get; set; }
        public virtual ICollection<HoaDon> HoaDon { get; set; }
    }
}
