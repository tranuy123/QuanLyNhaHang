using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using SelectPdf;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;

namespace QuanLyNhaHang.Controllers
{
    public class BaoCaoController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        [HttpGet]
        public IActionResult BaoCao() 
        {
            //return View("BaoCaoLoiNhuan");
            return View();
        }
        [HttpPost("/doanhThuTheoNgay")]
        public async Task<dynamic> doanhThuTheoNgay(string fromDay, string toDay)
        {
            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var data = await context.HoaDon
              .Where(x => x.Tgxuat != null && (x.Tgxuat.Value.Date >= FromDay && x.Tgxuat.Value.Date <= ToDay))
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToListAsync();
            return data;
        }
        [HttpPost("/doanhThuTheoThang")]
        public async Task<dynamic> doanhThuTheoThang()
        {
            var data = await context.HoaDon
              .Where(x => x.Tgxuat != null)
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Month)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToListAsync();
            return data;
        }
        [HttpGet("/QuanLy/ViewBaoCaoLoiNhuan")]
        public IActionResult ViewBaoCaoLoiNhuan()
        {
            return View("BaoCaoLoiNhuan");
        }
        [HttpPost("/baoCaoLoiNhuan")]
        public async Task<dynamic> baoCaoLoiNhuan(string TuNgay, string DenNgay)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var doanhThu = await context.HoaDon
              .Where(x => x.Tgxuat.Value.Date >= tuNgay.Date && x.Tgxuat.Value.Date <= denNgay.Date)
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToListAsync();
            var giaVon = context.PhieuXuat
              .Include(x => x.ChiTietPhieuXuat)
              .Where(x => x.NgayTao.Value.Date >= tuNgay.Date && x.NgayTao.Value.Date <= denNgay.Date)
              .ToList()
              .OrderBy(x => x.NgayTao)
              .GroupBy(x => x.NgayTao.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = tongPhieuNhap(x.ToList()),
              })
              .ToList();
            return new
            {
                doanhThu = doanhThu,
                giaVon = giaVon,
            };
        }
        public double tongPhieuNhap(List<PhieuXuat> phieuNhaps)
        {
            double tong = (double)phieuNhaps.Sum(x => x.ChiTietPhieuXuat.Sum(ct => ct.Gia));
            return tong;
        }
        public double tongChiTietPhieuNhap(List<ChiTietPhieuNhap> chiTietPhieuNhaps)
        {
            double tong = (double)chiTietPhieuNhaps.Sum(ct => ct.Gia);
            return tong;
        }
        // -------------------------------------------------- đánh giá hiệu xuất nhân viên
        [HttpGet("/BaoCao/HieuSuatNhanVien")]
        public  IActionResult ViewHieuSuatNhanVien()
        {
            return View("HieuSuatNhanVien");
        }
        [HttpPost("/BaoCao/getDuLieuHieuSuatNhanVien")]
        public async Task<dynamic> getDuLieuHieuSuatNhanVien(string fromDay, string toDay)
        {
            fromDay = "01-01-2023";
            toDay = "13-11-2023";

            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            //var data = await context.NhanVien
            //    .Include(x => x.HoaDon)
            //    .ThenInclude(x => x.ChiTietHoaDon)
            //    .Include(x => x.IdnnvNavigation.QlDanhGia)
            //    .Where(x => x.HoaDon.Any(hd => hd.Tgxuat != null && (hd.Tgxuat.Value.Date >= FromDay && hd.Tgxuat.Value.Date <= ToDay)
            //                            ))
            //    .ToListAsync(); 




            //var data = await context.ChiTietHoaDon
            //    .AsQueryable()
            //    .Include(x => x.IdhdNavigation.IdbanNavigation)
            //    .Include(x => x.IdcaNavigation.LichLamViec)
            //    .ThenInclude(x => x.IdnvNavigation)
            //    .Where(x => x.IdcaNavigation.LichLamViec.Any())
            //    .ToListAsync();
            //data = data.GroupBy(x => new { x.Idca, x.IdhdNavigation.IdbanNavigation.Idkhu })
            //    .Select(group => group.First())
            //    .ToList();
            //data = data.GroupBy(x => x.IdcaNavigation.LichLamViec.FirstOrDefault().Idnv)
            //    .Select(group => group.First())
            //    .ToList();
            //var data1 = data.Select(x => new
            //{
            //    TenNhanVien = x.IdcaNavigation.LichLamViec.FirstOrDefault().IdnvNavigation.Ten,
            //    Diem = (x.TgphucVu - x.TghoanThanh)
            //})
            //.ToList();


            var data = await context.NhanVien
                .Include(x => x.LichLamViec)
                .ThenInclude(x => x.IdcaNavigation)
                .ThenInclude(x => x.ChiTietHoaDon)
                .ThenInclude(x => x.IdhdNavigation)
                .Where(x => x.LichLamViec.Any(llv => llv.IdcaNavigation.ChiTietHoaDon.Any(ct => ct.IdhdNavigation.Tgxuat.Value.Date >= FromDay && ct.IdhdNavigation.Tgxuat.Value.Date <= ToDay)))
                .Select(x => new NhanVien()
                {
                    Ten = x.Ten,
                    LichLamViec = x.LichLamViec
                    .Where(llv => llv.IdcaNavigation.ChiTietHoaDon.Count() >0)
                    .Select(llv => new LichLamViec()
                    {
                        IdcaNavigation = new Ca()
                        {
                            ChiTietHoaDon = llv.IdcaNavigation.ChiTietHoaDon.Select(ct => new ChiTietHoaDon()
                            {
                                TgphucVu = ct.TgphucVu,
                                TghoanThanh = ct.TghoanThanh,   
                                IdhdNavigation = new HoaDon()
                                {
                                    Tgxuat = ct.IdhdNavigation.Tgxuat,
                                }
                            })
                            .Where(ct => ct.IdhdNavigation.Tgxuat.Value.Date >= FromDay && ct.IdhdNavigation.Tgxuat.Value.Date <= ToDay)
                            .ToList(),
                        }
                    })
                    .ToList(),
                })
                .ToListAsync();
            var data1 = data.Select(x => new
            {
                Ten = x.Ten,
                Diem = tinhDiemHieuSuatTheoLichLamViecs(x.LichLamViec.ToList()),
            })
            .ToList();
            return data1;
        }
        public float tinhDiemHieuSuatTheoLichLamViecs(List<LichLamViec> lichLamViecs)
        {
            float diem = 0;
            int i = 0;
            foreach (LichLamViec lich in lichLamViecs)
            {
                diem += tinhDiemHieuSuatTheoChiTietHoaDon(lich.IdcaNavigation.ChiTietHoaDon.Where(x => x.TgphucVu!=null).ToList());
                i++;
            }
            diem /=i;
            return diem;
        }
        public float tinhDiemHieuSuatTheoChiTietHoaDon(List<ChiTietHoaDon> chiTietHoaDons)
        {
            float diem= 0;
            int i = 0;
            foreach (ChiTietHoaDon chiTietHoaDon in chiTietHoaDons)
            {
                diem += tinhDiemHieuSuatChiTietHoaDon(chiTietHoaDon);
                i++;
            }
            diem /= i;
            return diem;
        }
        public int tinhDiemHieuSuatChiTietHoaDon(ChiTietHoaDon chiTietHoaDon)
        {
            DateTime? tgPhucVu = chiTietHoaDon?.TgphucVu;
            DateTime? tgHoanThanh = chiTietHoaDon?.TghoanThanh;
            int thoiGianHoanThanh = 0;

            int diem = 0;
            TimeSpan timeSpan = tgPhucVu.Value - tgHoanThanh.Value;
            thoiGianHoanThanh = (int)timeSpan.TotalSeconds;
            QlDanhGia qlDanhGia = context.QlDanhGia.FirstOrDefault(x => x.ThoiGianTu <= thoiGianHoanThanh && x.ThoiGianDen >= thoiGianHoanThanh);
            if (qlDanhGia == null)
            {
                diem = 1;
            }
            else
            {
                diem = (int)qlDanhGia.Diem;
            }
            return diem;
        }
        [Route("/download/ChiTietHoaDon/{id:int}")]
        public IActionResult downloadPChiTietHoaDon(int id)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var pdf = fullView.ConvertUrl(currentUrl + "/ChiTietHoaDonPDF/" + id);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "ChiTietHoaDon.pdf");
        }

        [Route("/ChiTietHoaDonPDF/{id:int}")]
        public IActionResult viewPDF(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var phieu = context.HoaDon
                .Include(x => x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation)
                .Include(x => x.ChiTietHoaDon)
                .Where(x => x.Idhd == id).FirstOrDefault();
            return View("ChiTietHoaDonPDF", phieu);
        }
        [HttpPost("/download/BaoCaoHoaDon")]
        public IActionResult downloadBaoCaoHoaDon(string fromDay, string toDay, string MaHD, int IdSanh, int IdTieuKhu, int IdBan, int IdTD)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var url = Url.Action("viewBaoCaoHoaDonPDF", "BaoCao", new { fromDay = fromDay, toDay = toDay, MaHD = MaHD, IdSanh = IdSanh, IdTieuKhu = IdTieuKhu, IdBan = IdBan, IdTD = IdTD });

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + url;

            var pdf = fullView.ConvertUrl(currentUrl);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "BaoCaoHoaDon.pdf");
        }
        public IActionResult viewBaoCaoHoaDonPDF(string fromDay, string toDay, string MaHD, int IdSanh, int IdTieuKhu, int IdBan, int IdTD)
        {
            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            List<HoaDon> listPhieu = context.HoaDon
            .Where(x => x.Tgxuat.Value.Date >= FromDay
            && x.Tgxuat.Value.Date <= ToDay
            && x.Active == true)
            .Include(x => x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation)
            .Include(x => x.IdnvNavigation)
            .Include(x => x.ChiTietHoaDon)
            .OrderByDescending(x => x.Idhd)
            .ToList();
            ViewBag.tungay = fromDay;
            ViewBag.denngay = toDay;
            if (IdSanh == 0 && IdTieuKhu == 0 && IdBan == 0 && IdTD == 0)
            {

                return View("BaoCaoHoaDonPDF", listPhieu.Where(x => (MaHD == null ? true : x.MaHd.Contains(MaHD.ToUpper()))).ToList());
            }
            else
            {
                return View("BaoCaoHoaDonPDF", listPhieu.Where(x => (IdTD == 0 ? true : (x.ChiTietHoaDon.Where(y => y.Idtd == IdTD).Count() > 0 ? true : false))
                && (IdSanh == 0 ? true : x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation.Idsanh == IdSanh)
                && (IdTieuKhu == 0 ? true : x.IdbanNavigation.IdkhuNavigation.Idkhu == IdTieuKhu)
                && (IdBan == 0 ? true : x.IdbanNavigation.Idban == IdBan)
                && (MaHD == null ? true : x.MaHd.Contains(MaHD.ToUpper()))).ToList());
            }

        }
    }
}
