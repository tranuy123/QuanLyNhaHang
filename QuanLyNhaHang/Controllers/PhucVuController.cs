using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace QuanLyNhaHang.Controllers
{
    [Authorize(Roles = "1")]
    

    public class PhucVuController : Controller
    {
        QuanLyNhaHangContext _context = new QuanLyNhaHangContext();
        public IActionResult Index()
        {
            DateTime now = DateTime.Now;
            DayOfWeek dayOfWeek = now.DayOfWeek;
            string dayOfWeekString = dayOfWeek.ToString();
            var ca = _context.Ca.FirstOrDefault(x => x.Thu == dayOfWeekString && now.TimeOfDay >= x.TgbatDau && now.TimeOfDay <= x.TgketThuc && x.Active == true).Idca;
            int idnv = int.Parse(User.FindFirstValue(ClaimTypes.Name));
            var idkhu = _context.LichLamViec.FirstOrDefault(x => x.Idca == ca && x.Idnv == idnv && x.Active == true)?.Idkhu;
            if (idkhu == null)
            {
                return View("Error");
            }
            return View("ViewPhucVu");
        }
        public IActionResult Order()
        {
            DateTime now = DateTime.Now;
            DayOfWeek dayOfWeek = now.DayOfWeek;
            string dayOfWeekString = dayOfWeek.ToString();
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var ca = context.Ca.FirstOrDefault(x => x.Thu == dayOfWeekString && now.TimeOfDay >= x.TgbatDau && now.TimeOfDay <= x.TgketThuc && x.Active == true).Idca;
            int idnv = int.Parse(User.FindFirstValue(ClaimTypes.Name));
            var idkhu = context.LichLamViec.FirstOrDefault(x => x.Idca == ca && x.Idnv == idnv && x.Active == true)?.Idkhu;
            if (idkhu == null)
            {
                return View("Error");
            }
            return View("ViewOrderPhucVu");

        }
        [Route("/PhucVu/MenuPV/{idb}")]

        public IActionResult MenuPV(int idb)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ViewBag.idb = idb;
            var IDkhu = context.Ban.FirstOrDefault(x => x.Idban == idb);
            var IDSanh = context.Khu.FirstOrDefault(x => x.Idkhu == IDkhu.Idkhu);
            ViewBag.menuPV = context.Gia.Include(x => x.IdtdNavigation).Where(x => x.Idsanh == IDSanh.Idsanh && x.Active==true).ToList(); ;
            return View("MenuPV");
        }
        [Route("/PhucVu/HoaDonPV/{idb}")]

        public IActionResult HoaDonPV(int idb)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ViewBag.idb = idb;
            ViewBag.IPMAC = context.Ban.FirstOrDefault(x => x.Idban == idb).Ipmac;
            ViewBag.HoaDonPV = context.ChiTietHoaDon.Include(x => x.IdhdNavigation.IdbanNavigation).Where(x => x.HangHoa != true && x.IdhdNavigation.Idban == idb && (x.IdhdNavigation.TinhTrangTt == false || x.IdhdNavigation.TinhTrangTt == null)).ToList();
            return View("HoaDonPV");
        }
        [HttpPost("/loadNhomThucAnPV")]

        public IActionResult loadNhomThucAnPV(int idnta, int idb)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
          
            var IDkhu = context.Ban.FirstOrDefault(x => x.Idban == idb);
            var IDSanh = context.Khu.FirstOrDefault(x => x.Idkhu == IDkhu.Idkhu);
            if (idnta == 0)
            {
                ViewBag.NTA = context.Gia.Include(x => x.IdtdNavigation).Where(x => x.Idsanh == IDSanh.Idsanh && x.Active == true).ToList();

            }
            else
            {
                ViewBag.NTA = context.Gia.Include(x => x.IdtdNavigation).Where(x => x.Idsanh == IDSanh.Idsanh && x.IdtdNavigation.Idnta == idnta && x.Active == true).ToList();

            }

            ViewBag.IPMAC = context.Ban.FirstOrDefault(x => x.Idban == idb).Ipmac;
            return PartialView();
        }
        [HttpPost("/addHoaDonPV")]
        public string addHoaDonPV(string IPMAC, int IDTD, float DonGia, int HangHoa)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDonTam ct = new ChiTietHoaDonTam();

            ct.Ipmac = IPMAC;
            ct.Idtd = IDTD;
            ct.DonGia = DonGia;
            ct.Sl = 1;
            ct.HangHoa = HangHoa == 1 ? true : false;
            context.ChiTietHoaDonTam.Add(ct);
            context.SaveChanges();


            return "Thêm món thành công";
        }
        [HttpPost("/HuyHoaDonTamPV")]
        public string HuyHoaDonTamPV(string IPMAC, int IDTD)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDonTam ct = context.ChiTietHoaDonTam.FirstOrDefault(x => x.Ipmac == IPMAC && x.Idtd == IDTD);


            context.ChiTietHoaDonTam.Remove(ct);
            context.SaveChanges();


            return "Hủy món thành công";
        }
        [Route("/PhucVu/GoiMonPV/{idb}")]
        public IActionResult GoiMonPV(int idb)
        {
            ViewBag.idb = idb;
            return View("GoiMonPV");
        }
        [HttpPost("/UpdateSLPV")]
        public string UpdateSLPV(int IDCTHDT, int SL, string DonGia, string ThanhTien)
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
        [HttpPost("/UpdateTGPV")]
        public string UpdateTGPV(int IDCTHDT)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDon ct = context.ChiTietHoaDon.Find(IDCTHDT);

            ct.TgphucVu = DateTime.Now;
            context.ChiTietHoaDon.Update(ct);
            context.SaveChanges();


            return "Cap nhap thanh cong";
        }
        
    }
}
