using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Models.Mapping;
using QuanLyNhaHang.Services;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static QuanLyNhaHang.Controllers.XuatKhoController;

namespace QuanLyNhaHang.Controllers
{
    public class NhapKhoController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        private readonly IMapper _mapper;
        public NhapKhoController(IMapper mapper)
        {
            _mapper = mapper;
        }
        public IActionResult NhapKho()
        {
            return View();
        }
        [HttpPost("/NhapKho/getDonViTinh")]
        public async Task<dynamic> getDonViTinh(int idHH)
        {
            HangHoa dvt = await context.HangHoa
                .Include(x => x.IddvtNavigation)
                .FirstOrDefaultAsync(x => x.Idhh == idHH);
            return dvt.IddvtNavigation.TenDvt;
        }
        [HttpPost("/NhapKho/ThemPhieuNhap")]
        public async Task<dynamic> ThemPhieuNhap([FromBody] TTPhieuNhap data)
        {
            NhanVien nv = context.NhanVien.Find(Int32.Parse(User.Identity.Name));
            PhieuNhapMap phieuNhapMap = data.PhieuNhap;
            List<ChiTietPhieuNhapMap> chiTietPhieuNhapMaps = data.ChiTietPhieuNhap;

           
            using var tran = context.Database.BeginTransaction();
            try
            {
                List<ChiTietPhieuNhap> chiTietPhieuNhaps = _mapper.Map<List<ChiTietPhieuNhap>>(chiTietPhieuNhapMaps);
                PhieuNhap phieuNhap = _mapper.Map<PhieuNhap>(phieuNhapMap);

                phieuNhap.SoPn = CommonServices.taoSoPhieuNhap(context);
                phieuNhap.Idnv = nv.Idnnv;
                phieuNhap.Active = true;
                await context.PhieuNhap.AddAsync(phieuNhap);
                await context.SaveChangesAsync();
                foreach (ChiTietPhieuNhap chiTiet in chiTietPhieuNhaps)
                {
                    chiTiet.Idpn = phieuNhap.Idpn;
                    chiTiet.Active = true;
                }
                await context.ChiTietPhieuNhap.AddRangeAsync(chiTietPhieuNhaps);
                await context.SaveChangesAsync();
                List<TonKho> tonKhos = new List<TonKho>();  
                foreach(ChiTietPhieuNhap chiTiet1 in chiTietPhieuNhaps)
                {
                    TonKho tonKho = new TonKho();
                    tonKho.Idctpn = chiTiet1.Idctpn;
                    tonKho.SoLuong = chiTiet1.SoLuong;
                    tonKho.NgayNhap = phieuNhap.NgayNhap;
                    tonKhos.Add(tonKho);
                }
                await context.TonKho.AddRangeAsync(tonKhos);
                await context.SaveChangesAsync();
                tran.Commit();
                return new
                {
                    statusCode = 200,
                    message = "Thành công",
                };
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return new
                {
                    statusCode = 500,
                    message = "Thất bại",
                };
            }
        }
        [HttpPost("/NhapKho/LichSuNhap")]
        public async Task<dynamic> LichSuNhap(string TuNgay, string DenNgay, string maPhieu)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var PhieuNhaps = await context.PhieuNhap
                .Where(x => (x.NgayNhap.Value.Date >= tuNgay.Date && x.NgayNhap.Value.Date <= denNgay.Date)
                && (maPhieu == "" || maPhieu == null || x.SoPn == maPhieu))
             .Select(x => new
             {
                 SoPx = x.SoPn,
                 NgayTao = x.NgayNhap,
                 IdnvNavigation = x.IdnvNavigation,
                 GhiChu = x.GhiChu,
                 TongTien = x.ChiTietPhieuNhap.Sum(x => x.SoLuong * x.Gia),
                 NhaCungCap = x.IdnccNavigation.TenNcc,
                 SoLuongHH = x.ChiTietPhieuNhap.Count(),
                 ChiTietPhieuNhap = x.ChiTietPhieuNhap.Select(x => new
                 {
                     IdhhNavigation = x.IdhhNavigation,
                     SoLuong = x.SoLuong,
                     Gia = x.Gia,
                     DVT = x.IdhhNavigation.IddvtNavigation.TenDvt,
                 }).ToList()
             })
            .ToListAsync();
            //var ketqua = PhieuNhaps.Select(x => new
            //{
            //    PhieuNhap = x,
            //    ChiTietPhieuNhap = GetChiTietPhieuNhaps(x.Idpx),
            //}).ToList();
            return Ok(PhieuNhaps);
        }
        public List<ChiTietPhieuNhap> GetChiTietPhieuNhaps(int id)
        {
            List<ChiTietPhieuNhap> chiTietPhieuNhaps = context.ChiTietPhieuNhap.Where(x => x.Idpn == id).ToList();
            return chiTietPhieuNhaps;
        }
        public class TTPhieuNhap
        {
            public PhieuNhapMap PhieuNhap { get; set; }
            public List<ChiTietPhieuNhapMap> ChiTietPhieuNhap { get; set; }
        }
        [HttpPost("/download/BaoCaoLSNhapKho")]
        public IActionResult downloadBaoCaoLSNhapKho(string fromDay, string toDay, string soPhieuLS)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var url = Url.Action("viewBaoCaoLSNhapKhoPDF", "NhapKho", new { TuNgay = fromDay, DenNgay = toDay, maPhieu = soPhieuLS});

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + url;

            var pdf = fullView.ConvertUrl(currentUrl);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "BaoCaoLSNhapKho.pdf");
        }
        public IActionResult viewBaoCaoLSNhapKhoPDF(string TuNgay, string DenNgay, string maPhieu)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            List<LSNhapKho> phieuXuats = context.PhieuNhap
                .Where(x => (x.NgayNhap.Value.Date >= tuNgay.Date && x.NgayNhap.Value.Date <= denNgay.Date)
                && (maPhieu == "" || maPhieu == null || x.SoPn == maPhieu))
             .Select(x => new LSNhapKho
             {
                 SoPx = x.SoPn,
                 NgayTao = x.NgayNhap,
                 GhiChu = x.GhiChu,
                 TongTien = NhapKhoController.getTongTien(x.ChiTietPhieuNhap.ToList()),
                 SoLuongHH = x.ChiTietPhieuNhap.Count(),
             })
            .ToList();
            ViewBag.tuNgay = tuNgay.Date;
            ViewBag.denNgay = denNgay.Date;
            ViewBag.PhieuNhap = phieuXuats;
            return View("BaoCaoLSNhapKhoPDF");

        }
        public static double getTongTien(List<ChiTietPhieuNhap> chiTietPhieuXuats)
        {
            double tongTien = 0;
            foreach (ChiTietPhieuNhap ct in chiTietPhieuXuats)
            {
                tongTien += (float)(Math.Round((float)ct.SoLuong, 3) * Math.Round((float)ct.Gia, 3));
            }
            return (double)Math.Round(tongTien, 3);
        }
        public class LSNhapKho
        {
            public string SoPx { get; set; }
            public DateTime? NgayTao { get; set; }
            public string GhiChu { get; set; }
            public double TongTien { get; set; }
            public int SoLuongHH { get; set; }

        }
    }

}
