using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class ChiTietHoaDon
    {
        public int Idcthd { get; set; }
        public int? Idtd { get; set; }
        public int? Idhd { get; set; }
        public int? Sl { get; set; }
        public double? DonGia { get; set; }
        public int? TyLeGiam { get; set; }
        public double? ThanhTien { get; set; }
        public DateTime? Tgorder { get; set; }
        public DateTime? Tgbep { get; set; }
        public DateTime? TghoanThanh { get; set; }
        public DateTime? TgphucVu { get; set; }
        public bool? TrangThaiOrder { get; set; }
        public bool? DaXuat { get; set; }
        public bool? HangHoa { get; set; }
        public bool? Active { get; set; }
        public int? Idca { get; set; }

        public virtual Ca IdcaNavigation { get; set; }
        public virtual HoaDon IdhdNavigation { get; set; }
        public virtual ThucDon IdtdNavigation { get; set; }
    }
}
