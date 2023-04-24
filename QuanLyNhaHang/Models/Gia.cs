using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class Gia
    {
        public int Idgia { get; set; }
        public int? Idsanh { get; set; }
        public int? Idtd { get; set; }
        public double? Gia1 { get; set; }

        public virtual Sanh IdsanhNavigation { get; set; }
        public virtual ThucDon IdtdNavigation { get; set; }
    }
}
