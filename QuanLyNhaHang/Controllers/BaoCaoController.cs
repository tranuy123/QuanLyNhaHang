using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using SelectPdf;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHang.Controllers
{
    public class BaoCaoController : Controller
    {
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
