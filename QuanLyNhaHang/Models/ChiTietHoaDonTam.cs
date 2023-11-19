using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class ChiTietHoaDonTam
    {
        public int Idcthdt { get; set; }
        public string Ipmac { get; set; }
        public int? Idtd { get; set; }
        public int? Idhd { get; set; }
        public int? Sl { get; set; }
        public double? DonGia { get; set; }
        public double? ThanhTien { get; set; }
        public bool? HangHoa { get; set; }
    }
}
