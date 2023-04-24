using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class Sanh
    {
        public Sanh()
        {
            Gia = new HashSet<Gia>();
            Khu = new HashSet<Khu>();
        }

        public int Idsanh { get; set; }
        public string MaSanh { get; set; }
        public string TenSanh { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<Gia> Gia { get; set; }
        public virtual ICollection<Khu> Khu { get; set; }
    }
}
