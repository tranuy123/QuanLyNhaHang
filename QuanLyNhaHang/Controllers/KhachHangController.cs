using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Controllers
{
    public class KhachHangController : Controller
    {

        public IActionResult Index()
        {
            return View("Menu");
        }

        public IActionResult menu1()
        {

            return View("menu1");
        }
        //[HttpGet("/KhachHang/{mac}")]
        //public async Task<IActionResult> menu1(string mac)
        //{
        //    ViewBag.MAC = mac;
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Dns, mac),
        //    };

        //    var claimsIdentity = new ClaimsIdentity(
        //        claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    await HttpContext.SignInAsync(
        //        CookieAuthenticationDefaults.AuthenticationScheme,
        //        new ClaimsPrincipal(claimsIdentity));
        //    return View("menu1");
        //}
        [Route("/KhachHang/GoiMon")]
        public IActionResult GoiMon()
        {
            return View("GoiMon");
        }
        [HttpGet("/KhachHang/HoaDon")]
        public IActionResult HoaDon()
        {
            return View("HoaDonKhachHang");
        }
        [HttpPost("/loadNhomThucAn")]

        public IActionResult loadNhomThucAn(int idnta)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            string macAddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            var IDkhu = context.Ban.FirstOrDefault(x => x.Ipmac == macAddress);
            var IDSanh = context.Khu.FirstOrDefault(x => x.Idkhu == IDkhu.Idkhu);
            if(idnta == 0)
            {
                ViewBag.NTA = context.Gia.Include(x => x.IdtdNavigation).Where(x => x.Idsanh == IDSanh.Idsanh).ToList();

            }
            else
            {
                ViewBag.NTA = context.Gia.Include(x =>x.IdtdNavigation).Where(x => x.Idsanh == IDSanh.Idsanh && x.IdtdNavigation.Idnta == idnta).ToList();

            }

       
            return PartialView();
        }
        [HttpPost("/addHoaDon")]
        public dynamic addHoaDon(string IPMAC,int IDTD, float DonGia, int HangHoa)
        {
            try
            {

                bool hh = false;
                if(HangHoa == 1)
                {
                    hh = true;
                }
                QuanLyNhaHangContext context = new QuanLyNhaHangContext();
                ChiTietHoaDonTam ct = new ChiTietHoaDonTam();

                ct.Ipmac = IPMAC;
                ct.Idtd = IDTD;
                ct.DonGia = DonGia;
                ct.Sl = 1;
                ct.ThanhTien = DonGia;
                ct.HangHoa = hh;
                context.ChiTietHoaDonTam.Add(ct);
                context.SaveChanges();
                return new
                {
                    StatusCode = 200,
                    message = "Hủy món thành công",
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    StatusCode = 500,
                    message = "Hủy món thất bại",
                };
            }

        }
        [HttpPost("/HuyHoaDonTam")]
        public dynamic HuyHoaDonTam(string IPMAC, int IDTD)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDonTam ct = context.ChiTietHoaDonTam.FirstOrDefault(x =>x.Ipmac == IPMAC && x.Idtd==IDTD);
            try
            {
                context.ChiTietHoaDonTam.Remove(ct);
                context.SaveChanges();
                return new
                {
                    StatusCode = 200,
                    message = "Hủy món thành công",
                };
            }
            catch(Exception ex)
            {
                return new
                {
                    StatusCode = 500,
                    message = "Hủy món thất bại",
                };
            }
        }
        [HttpPost("/UpdateSL")]
        public string UpdateSL(int IDCTHDT, int SL, string DonGia, string ThanhTien)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDonTam ct = context.ChiTietHoaDonTam.Find(IDCTHDT);

            ct.Sl = SL;
            ct.DonGia = float.Parse(DonGia);
            ct.ThanhTien = float.Parse(ThanhTien);
            context.ChiTietHoaDonTam.Update(ct);
            context.SaveChanges();


            return "Thay doi so luong thanh cong";
        }
        [HttpPost("/addChiTietHoaDon")]

        public IActionResult addChiTietHoaDon( string ipmac, float tongtien, int idban)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            List<ChiTietHoaDonTam> cthdt = context.ChiTietHoaDonTam.Where(c => c.Ipmac == ipmac).ToList();
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyyMMdd");


            string orderCode = $"{now:yyyyMMdd}-{ipmac}";
            int count = context.HoaDon.Count(x => x.MaHd == orderCode);

            string mahd = $"{now:yyyyMMdd}-{ipmac}-{count + 1}";
            int count1 = context.HoaDon.Count(x => x.MaHd == mahd);
            string mahd1 = $"{now:yyyyMMdd}-{ipmac}-{count1 + 1}";
            try
            {
                HoaDon hd = new HoaDon();
                hd.MaHd = mahd1;
                hd.Idban = idban;
                hd.TongTien = tongtien;
                hd.TinhTrang = false;
                context.HoaDon.Add(hd);
                context.SaveChanges();
                // Lưu lại Idhd mới được tạo ra
                int hdId = hd.Idhd;
                foreach (ChiTietHoaDonTam ctt in cthdt)
                {
                    ChiTietHoaDon ct = new ChiTietHoaDon();
                    ct.Idhd = hdId;
                    ct.Tgorder = DateTime.Now;
                    ct.Idtd = ctt.Idtd;
                    ct.DonGia = ctt.DonGia;
                    ct.Sl = ctt.Sl;
                    context.ChiTietHoaDon.Add(ct);
                    context.SaveChanges();
                    context.ChiTietHoaDonTam.Remove(ctt);
                    context.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            return RedirectToAction("Index");
        }
        [Route("/download/hoadon/{id:int}")]
        public IActionResult downloadHoaDon(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var tinhtranghoadon = context.ChiTietHoaDon.FirstOrDefault(x => x.Idhd == id && x.TghoanThanh == null);
            //if (tinhtranghoadon != null)
            //{
            //    TempData["ThongBao"] = "Có món ăn trong danh sách vẫn chưa hoàn thành!, chưa thể thanh toán";

            //    return RedirectToAction("HoaDon");
            //}
            //else
            //{
                int idnv = int.Parse(User.FindFirstValue(ClaimTypes.Name));

                HoaDon hd = context.HoaDon.Find(id);
                hd.Idnv = idnv;
                hd.Tgxuat = DateTime.Now;
                context.HoaDon.Update(hd);
                context.SaveChanges();


                var fullView = new HtmlToPdf();
                fullView.Options.WebPageWidth = 700;
                fullView.Options.PdfPageSize = PdfPageSize.A7;
                fullView.Options.MarginTop = 20;
                fullView.Options.MarginBottom = 20;
                fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

                var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                var pdf = fullView.ConvertUrl(currentUrl + "/HoaDonPDF/" + id);

                var pdfBytes = pdf.Save();
                var fileResult = File(pdfBytes, "application/pdf", "HoaDon.pdf");

                return fileResult;



            //}
        }
        [Route("/HoaDonPDF/{id:int}")] 
        public IActionResult viewPDF(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
           
                var hoadon = context.HoaDon
                    .Include(x => x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation)
                    .Include(x => x.ChiTietHoaDon)
                    .Where(x => x.Idhd == id).FirstOrDefault();
                return View("HoaDonPDF", hoadon);
            
        }

    }
}
