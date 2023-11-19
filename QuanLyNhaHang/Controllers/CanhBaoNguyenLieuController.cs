using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Models.Mapping;
using QuanLyNhaHang.Models.ModelsTam;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Controllers
{
    public class CanhBaoNguyenLieuController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        public IActionResult CanhBaoNguyenLieu()
        {
            ViewBag.ThucDon = context.ThucDon
                 .Include(x => x.DinhMuc).Where(x => x.Active == true).ToList();
            ViewBag.HangHoa = context.HangHoa
                .Include(x => x.IdnhhNavigation)
                .Where(x => x.Active == true && x.IdnhhNavigation.NguyenLieu == true).ToList();
            List<ThucDon> thucDons = context.ThucDon.Where(x => x.Active == true).ToList();
            List<ChiTietHoaDon> chiTietHoaDons = new List<ChiTietHoaDon>();
            foreach (ThucDon thucDon in thucDons)
            {
                ChiTietHoaDon chiTietHoaDon = new ChiTietHoaDon();
                chiTietHoaDon.Idtd = thucDon.Idtd;
                chiTietHoaDon.Sl = 1;
                chiTietHoaDons.Add(chiTietHoaDon);
            }
            string TuNgay = DateTime.Now.ToString("dd-MM-yyyy");
            string DenNgay = DateTime.Now.ToString("dd-MM-yyyy");
            CBNL cBNL = new CBNL();
            cBNL.TuNgay = TuNgay;
            cBNL.DenNgay = DenNgay;
            cBNL.ChiTietHoaDons = chiTietHoaDons;
            ViewBag.DSNL = getDSCanhBaoNguyenLieu(cBNL);
            return View();
        }
        public class CBNL
        {
            public List<ChiTietHoaDon> ChiTietHoaDons { get; set; }
            public string TuNgay { get; set; }
            public string DenNgay { get; set; }
        }
        [HttpPost("/CanhBaoNguyenLieu/getDSCanhBaoNguyenLieu")]
        public dynamic getDSCanhBaoNguyenLieu([FromBody] CBNL data)
        {
            List<ChiTietHoaDon> chiTietHoaDons = data.ChiTietHoaDons;
            DateTime tuNgay = DateTime.ParseExact(data.TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(data.DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            int soNgay = TinhSoNgayGiuaHaiNgay(tuNgay, denNgay) + 1;
            var listHH = new List<dynamic>();
            foreach (ChiTietHoaDon ct in chiTietHoaDons)
            {
                ct.Sl = soNgay * ct.Sl;
            }
            foreach (ChiTietHoaDon cthd in chiTietHoaDons)
            {
                List<DinhMuc> dinhMucs = context.DinhMuc
                    .Include(x => x.IdhhNavigation)
                    .ThenInclude(x => x.ChiTietPhieuNhap)
                    .ThenInclude(x => x.TonKho).Where(x => x.Idtd == cthd.Idtd).ToList();
                foreach (DinhMuc dm in dinhMucs)
                {
                    var hanghoa = new
                    {
                        idhh = dm.Idhh,
                        soLuong = (float)(dm.SoLuong * cthd.Sl),
                        donGia = dm.IdhhNavigation.ChiTietPhieuNhap.Where(x => x.TonKho.Any()).Max(x => x.Gia),
                    };
                    listHH.Add(hanghoa);
                }
            }
            var hangHoas = listHH.GroupBy(x => x.idhh)
                .Select(x => new CanhBaoNguyenLieu                {
                    Idhh = (int)x.Key,
                    TenHangHoa = getTenHH((int)x.Key),
                    TonKho = getTonKho((int)x.Key),
                    DonViTinh = getDonViTinh((int)x.Key),
                    SoLuong = (float)x.Sum(x => (float)x.soLuong),
                    DonGia = x.First().donGia,
                }).ToList();

            return hangHoas;
        }
        public double getTonKho(int idhh)
        {
            List<TonKho> tonKhos = context.TonKho
                .Include(x => x.IdctpnNavigation)
                .Where(x => x.IdctpnNavigation.Idhh == idhh).ToList();
            if (tonKhos.Count() == 0)
            {
                return 0;
            }
            return (double)tonKhos.Sum(x => x.SoLuong);
        }
        public string getDonViTinh(int idHH)
        {
            HangHoa dvt = context.HangHoa
                .Include(x => x.IddvtNavigation)
                .FirstOrDefault(x => x.Idhh == idHH);
            return dvt.IddvtNavigation.TenDvt;
        }
        public string getTenHH(int idHH)
        {
            HangHoa tenhh = context.HangHoa
                .Include(x => x.IddvtNavigation)
                .FirstOrDefault(x => x.Idhh == idHH);
            return tenhh.TenHh;
        }
        public int TinhSoNgayGiuaHaiNgay(DateTime ngay1, DateTime ngay2)
        {
            // Sử dụng phương thức Subtract để lấy TimeSpan giữa hai ngày
            TimeSpan khoangCach = ngay2.Subtract(ngay1);

            // Lấy số ngày từ TimeSpan
            int soNgay = (int)khoangCach.TotalDays;

            return soNgay;
        }
    }
}
