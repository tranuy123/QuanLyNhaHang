using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class Khu
    {
        public Khu()
        {
            Ban = new HashSet<Ban>();
            LichLamViec = new HashSet<LichLamViec>();
        }

        public int Idkhu { get; set; }
        public string MaKhu { get; set; }
        public string TenKhu { get; set; }
        public bool? Active { get; set; }
        public int? Idsanh { get; set; }

        public virtual Sanh IdsanhNavigation { get; set; }
        public virtual ICollection<Ban> Ban { get; set; }
        public virtual ICollection<LichLamViec> LichLamViec { get; set; }
    }
}
