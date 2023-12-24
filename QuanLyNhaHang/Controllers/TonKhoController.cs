using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System;
using System.Threading.Tasks;
using QuanLyNhaHang.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Internal;
using AutoMapper.Configuration.Conventions;
using System.Collections.Generic;
using SelectPdf;

namespace QuanLyNhaHang.Controllers
{
    public class TonKhoController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();  
        public IActionResult TonKho()
        {
            return View();
        }
        [HttpPost("/TonKho/BaoCaoTongHop")]
        public async Task<dynamic> getBaoCaoTongHop(int idNhomHang, int idHangHoa)
        {
            try
            {

                var tonKho = await context.TonKho
                    .Include(x => x.IdctpnNavigation)
                    .ThenInclude(x => x.IdhhNavigation)
                    .Where(x => (idNhomHang == 0 || x.IdctpnNavigation.IdhhNavigation.Idnhh == idNhomHang)
                                && (idHangHoa == 0 || x.IdctpnNavigation.Idhh == idHangHoa))
                    .ToListAsync();
                var tonkho1 = tonKho.GroupBy(x => x.IdctpnNavigation.Idhh)
                    .Select(x => new
                    {
                        Id = x.Key,
                        MaHang = getMaHang((int)x.Key),
                        TenHang = getTenHang((int)x.Key),
                        TongSL = Math.Round((float)x.Sum(x => x.SoLuong),3),
                        TongTien = Math.Round((float)x.Sum(x => x.IdctpnNavigation.Gia * x.SoLuong),3)

                    })
                    .ToList();
                return Ok(tonkho1);
            }
            catch (Exception ex)
            {
                return ex.Message; 
            }
        }
        public class BaoCaoTonKho
        {
            public int Id { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public double TongSL { get; set; }
            public double TongTien { get; set; }

        }
        //
        [HttpPost("/download/BaoCaoTongHop")]
        public IActionResult downloadBaoCaoTongHop(int nhomHang, int hangHoa)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var url = Url.Action("viewBaoCaoTongHop", "TonKho", new { idNhomHang = nhomHang, idHangHoa = hangHoa });

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + url;

            var pdf = fullView.ConvertUrl(currentUrl);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "BaoCaoTonKho.pdf");
        }
        public IActionResult viewBaoCaoTongHop(int idNhomHang, int idHangHoa)
        {
            var tonKho = context.TonKho
                .Include(x => x.IdctpnNavigation)
                .ThenInclude(x => x.IdhhNavigation)
                .Where(x => (idNhomHang == 0 || x.IdctpnNavigation.IdhhNavigation.Idnhh == idNhomHang)
                            && (idHangHoa == 0 || x.IdctpnNavigation.Idhh == idHangHoa))
                .ToList();
            List<BaoCaoTonKho> tonkho1 = tonKho.GroupBy(x => x.IdctpnNavigation.Idhh)
                .Select(x => new BaoCaoTonKho
                {
                    Id = (int)x.Key,
                    MaHang = getMaHang((int)x.Key),
                    TenHang = getTenHang((int)x.Key),
                    TongSL = Math.Round((float)x.Sum(x => x.SoLuong), 3),
                    TongTien = Math.Round((float)x.Sum(x => x.IdctpnNavigation.Gia * x.SoLuong), 3)

                })
                .ToList();
            ViewBag.TonKho = tonkho1;
            return View("BaoCaoTonKhoPDF");

        }
        public string getMaHang(int idhh)
        {
            HangHoa hangHoa = context.HangHoa.Find(idhh);
            return hangHoa.MaHh;
        }
        public string getTenHang(int idhh)
        {
            HangHoa hangHoa = context.HangHoa.Find(idhh);
            return hangHoa.TenHh;
        }
        [HttpPost("/TonKho/BaoCaoChiTiet")]
        public async Task<dynamic> getBaoCaoChiTiet(int idNCC, int idNhomHang, int idHangHoa, string tuNgay, string denNgay)
        {
            DateTime FromDay = DateTime.ParseExact(tuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(denNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var tonKho = await context.TonKho
                .Include(x => x.IdctpnNavigation.IdpnNavigation.IdnccNavigation)
                .Include(x => x.IdctpnNavigation)
                .ThenInclude(x => x.IdhhNavigation.IddvtNavigation)
                .Where(x => (x.NgayNhap.Value.Date >= FromDay.Date && x.NgayNhap.Value.Date <= ToDay)
                            && (idNhomHang == 0 || x.IdctpnNavigation.IdhhNavigation.Idnhh == idNhomHang)
                            && (idHangHoa == 0 || x.IdctpnNavigation.Idhh == idHangHoa)
                            && (idNCC == 0 || x.IdctpnNavigation.IdpnNavigation.Idncc == idNCC))
                .ToListAsync();
            var tonkho1 = tonKho;
            return tonkho1.Select(x => new
            {
                Id = x.Idctpn,
                NgayNhap = x.NgayNhap.Value.ToString("dd-MM-yyyy"),
                NhaCungCap = x.IdctpnNavigation.IdpnNavigation.IdnccNavigation.TenNcc,
                MaHang = x.IdctpnNavigation.IdhhNavigation.MaHh,
                TenHang = x.IdctpnNavigation.IdhhNavigation.TenHh,
                NgaySX = x.IdctpnNavigation.Nsx.Value.ToString("dd-MM-yyyy"),
                HanSD = x.IdctpnNavigation.Hsd.Value.ToString("dd-MM-yyyy"),
                SoLuongNhap = Math.Round((float)x.IdctpnNavigation.SoLuong, 3),
                SoLuongXuat = Math.Round(getSoLuongXuat((int)x.Idctpn), 3),
                SoLuongTon = Math.Round((float)x.SoLuong, 3),
                DonViTinh = x.IdctpnNavigation.IdhhNavigation.IddvtNavigation.TenDvt,
                GiaNhap = x.IdctpnNavigation.Gia,
                ThanhTien = Math.Round((float)(x.IdctpnNavigation.Gia * x.SoLuong), 3),
                CanhBao = hanSuDung((int)x.IdctpnNavigation.Idhh,x)
            }); ;
        }
        public double getSoLuongXuat(int idCTPN)
        {
            List<ChiTietPhieuXuat> chiTietPhieuXuats = context.ChiTietPhieuXuat.Where(x => x.Idctpn == idCTPN)
                .ToList();
            double soLuong = 0;
            if (chiTietPhieuXuats.Count()> 0)
            {
                soLuong = (double)chiTietPhieuXuats.Sum(x => x.SoLuong);
            }
            return soLuong;
        }
        public bool hanSuDung(int idHH, TonKho tonKho)
        {
            HangHoa hh = context.HangHoa
                .Include(x => x.IdnhhNavigation)
                .FirstOrDefault(x => x.Idhh == idHH);
            int hsd = TinhHieuNgay((DateTime)tonKho.IdctpnNavigation.Hsd);
            int soNgay = (int)hh.IdnhhNavigation.SoNgayCanhBao;
            if (hsd <= soNgay)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public int TinhHieuNgay(DateTime ngaySau)
        {
            // Tính hiệu giữa hai ngày và trả về số ngày dưới dạng int
            int hieuNgay = (int)(ngaySau - DateTime.Now).TotalDays;

            // Trả về kết quả
            return hieuNgay + 1;
        }
    }
}
