using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class DinhMuc
    {
        public int Iddm { get; set; }
        public int? Idtd { get; set; }
        public int? Idhh { get; set; }
        public double? SoLuong { get; set; }

        public virtual HangHoa IdhhNavigation { get; set; }
        public virtual ThucDon IdtdNavigation { get; set; }
    }
}
