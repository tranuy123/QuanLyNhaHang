using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System;
using QuanLyNhaHang.Models;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using SelectPdf;

namespace QuanLyNhaHang.Controllers
{
    [Authorize(Roles = "3")]

    public class QuanLyController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        public QuanLyController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View("ThucDon");
        }
        public IActionResult ViewInsertThucDon()
        {
            return View("ViewInsertTD");
        }
        [HttpPost]
        public IActionResult insertThucDon(ThucDon hh, IFormFile Avt)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            hh.Image = UploadedFile(hh, Avt);
            hh.Active = true;
            hh.TinhTrang = true;
            context.ThucDon.Add(hh);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("Index");
        }
        private string UploadedFile(ThucDon model, IFormFile avt)
        {
            string uniqueFileName = null;

            if (avt != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "ImagesTD");
                uniqueFileName = model.MaTd + ".jpg";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    avt.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [Route("/QuanLy/ViewUpdateTD/{id}")]
        public IActionResult ViewUpdateTD(int id)
        {
            ViewData["Title"] = "Cập nhập thực đơn";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ThucDon td = context.ThucDon.Find(id);
            return View(td);
        }
        [HttpPost]
        public IActionResult updateTD(ThucDon dvt, IFormFile avt)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ThucDon dv = context.ThucDon.Find(dvt.Idtd);          
            
            dv.Ten = dvt.Ten;
            dv.MaTd = dvt.MaTd;
            dv.Idnta = dvt.Idnta;
            dv.Detail = dvt.Detail;
            dv.Image = UploadedFile(dvt, avt);

            context.ThucDon.Update(dv);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("Index");
        }
        [Route("/QuanLy/XoaTD/{id}")]
        public IActionResult deleteTD(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ThucDon dvt = context.ThucDon.Find(id);
          
            dvt.Active = false;

            context.ThucDon.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Xoá thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost("/loadDivTD")]
        public IActionResult loadDivTD(bool active, int idnta)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                if (idnta != 0)
                {
                    ViewBag.HangHoas = context.ThucDon.Where(x => x.Active == active && x.Idnta == idnta).ToList();
                }
                else
                {
                    ViewBag.HangHoas = context.ThucDon.Where(x => x.Active == active).ToList();
                }
            }
            else
            {
                if (idnta != 0)
                {
                    ViewBag.HangHoas = context.ThucDon.Where(x => x.Idnta == idnta).ToList();
                }
                else
                {
                    ViewBag.HangHoas = context.ThucDon.ToList();
                }
            }
            return PartialView("viewDivTD");
        }
        [Route("/QuanLy/khoiphucTD/{id}")]
        public IActionResult khoiphucTD(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ThucDon dvt = context.ThucDon.Find(id);
           
            dvt.Active = true;

            context.ThucDon.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost("/searchTD")]
        public IActionResult searchTD(string key)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ViewBag.HangHoas = context.ThucDon.FromSqlRaw("select*from ThucDon where concat_ws(' ',MaTD,Ten) LIKE N'%" + key + "%';").ToList();
            return PartialView("viewDivTD");
        }
        ////////////////////////////////////////////////////////////////////////
        public IActionResult ViewSanh()
        {
            return View("ViewSanh");
        }
        public IActionResult ViewInsertSanh()
        {
            return View("ViewInsertSanh");  
        }
        [HttpPost]
        public IActionResult InsertSanh(Sanh nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            
            nsx.Active = true;
            context.Sanh.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewSanh");

        }
        [Route("/QuanLy/deleteSanh/{id}")]
        public IActionResult DeleteSanh(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Sanh nsx = context.Sanh.Find(id);         
            nsx.Active = false;

            context.Sanh.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewSanh");
        }
        [Route("/QuanLy/ViewUpdateSanh/{id}")]
        public IActionResult ViewUpdateSanh(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin sảnh";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Sanh nsx = context.Sanh.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateSanh(Sanh nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Sanh n = context.Sanh.Find(nsx.Idsanh);
            n.MaSanh = nsx.MaSanh;
            n.TenSanh = nsx.TenSanh;
           

            context.Sanh.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewSanh");
        }
        [HttpPost("/loadViewSanh")]
        public IActionResult loadViewSanh(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.Sanh.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.Sanh.ToList();
            }
            return PartialView("ViewDivSanh");
        }
        [Route("/QuanLy/khoiphucSanh/{id}")]
        public IActionResult khoiphucSanh(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Sanh dvt = context.Sanh.Find(id);

            dvt.Active = true;

            context.Sanh.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewSanh");
        }
        //////////////////////////////////////////////////////////////////////
        public IActionResult ViewKhu()
        {
            return View("ViewKhu");
        }
        public IActionResult ViewInsertKhu()
        {
            return View("ViewInsertKhu");
        }
        [HttpPost]
        public IActionResult InsertKhu(Khu nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.Khu.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewKhu");

        }
        [Route("/QuanLy/ViewUpdateKhu/{id}")]
        public IActionResult ViewUpdateKhu(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin khu";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Khu nsx = context.Khu.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateKhu(Khu nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Khu n = context.Khu.Find(nsx.Idkhu);
            n.MaKhu = nsx.MaKhu;
            n.TenKhu = nsx.TenKhu;
            n.Idsanh = nsx.Idsanh;


            context.Khu.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewKhu");
        }
        [Route("/QuanLy/deleteKhu/{id}")]
        public IActionResult DeleteKhu(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Khu nsx = context.Khu.Find(id);
            nsx.Active = false;

            context.Khu.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewKhu");
        }
        [HttpPost("/loadViewKhu")]
        public IActionResult loadViewKhu(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.Khu.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.Khu.ToList();
            }
            return PartialView("ViewDivKhu");
        }
        [Route("/QuanLy/khoiphucKhu/{id}")]
        public IActionResult khoiphucKhu(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Khu dvt = context.Khu.Find(id);

            dvt.Active = true;

            context.Khu.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewKhu");
        }
        /////////////////////////
        public IActionResult ViewBan()
        {
            return View("ViewBan");
        }
        public IActionResult ViewInsertBan()
        {
            return View("ViewInsertBan");
        }
        [HttpPost("/get-list-khu")]

        public List<Khu> getListKhu(int idSanh)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            // Lấy danh sách khu theo id sảnh
            var listKhu = context.Khu.Where(k => k.Idsanh == idSanh && k.Active==true).ToList();

            return listKhu;
        }
        [HttpPost]
        public IActionResult InsertBan(Ban nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.Ban.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewBan");

        }
        [Route("/QuanLy/ViewUpdateBan/{id}")]
        public IActionResult ViewUpdateBan(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin Bàn";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ban nsx = context.Ban.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateBan(Ban nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ban n = context.Ban.Find(nsx.Idban);
            n.MaBan = nsx.MaBan;
            n.Ipmac = nsx.Ipmac;
            n.TenBan = nsx.TenBan;
            n.Idkhu = nsx.Idkhu;


            context.Ban.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewBan");
        }
        [Route("/QuanLy/deleteBan/{id}")]
        public IActionResult DeleteBan(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ban nsx = context.Ban.Find(id);
            nsx.Active = false;

            context.Ban.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewBan");
        }
        [HttpPost("/loadViewBan")]
        public IActionResult loadViewBan(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.Ban.Include(x =>x.IdkhuNavigation.IdsanhNavigation).Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.Ban.Include(x => x.IdkhuNavigation.IdsanhNavigation).ToList();
            }
            return PartialView("ViewDivBan");
        }
        [Route("/QuanLy/khoiphucBan/{id}")]
        public IActionResult khoiphucBan(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ban dvt = context.Ban.Find(id);

            dvt.Active = true;

            context.Ban.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewBan");
        }
        /////////////////////////////////
        public IActionResult ViewGia()
        {
            return View("ViewGia");
        }
        [Route("/QuanLy/ViewGiaSanh/{id}")]
        public IActionResult ViewGiaSanh(int id)

        {
            ViewData["Title"] = "Giá theo sảnh";
            ViewBag.Idsanh = id;
            return View();
        }
        [HttpPost("/UpdateGia")]
        public string UpdateGia(int idtd, int ids, float gia)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Gia a = context.Gia.FirstOrDefault(x =>x.Idsanh==ids && x.Idtd==idtd);
            if (a != null)
            {
                a.Gia1 = gia;
                context.Gia.Update(a);
                context.SaveChanges();
                return "Lưu thành công";
            }
            else
            {
                Gia giamoi = new Gia();
                giamoi.Idsanh = ids;
                giamoi.Idtd = idtd;
                giamoi.Gia1 = gia;
                giamoi.Active = true;
                context.Gia.Add(giamoi);
                context.SaveChanges();
                return "Thêm thành công";
            }
        }
        [HttpPost("/DeleteGia")]
        public IActionResult DeleteGia(int idtd, int ids)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Gia nsx = context.Gia.FirstOrDefault(x =>x.Idsanh==ids && x.Idtd==idtd);
            nsx.Active = false;

            context.Gia.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewGia");
        }
        [HttpPost("/loadViewGia")]
        public IActionResult loadViewGia(bool active, int ids)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.ThucDon.Where(x => x.Active == true).OrderByDescending(x => x.Idnta).ToList();
            }
            else
            {
                ViewBag.NSX = context.ThucDon.Where(x => x.Active == true).OrderByDescending(x => x.Idnta).ToList();
            }
            ViewBag.Idsanh = ids;
            return PartialView("ViewDivGia");
        }
        ////////////////////////////////////////
        public IActionResult ViewCa()
        {
            return View("ViewCa");
        }
        public IActionResult ViewInsertCa()
        {
            return View("ViewInsertCa");
        }
        [HttpPost]
        public IActionResult InsertCa(Ca nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.Ca.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewCa");

        }
        [Route("/QuanLy/ViewUpdateCa/{id}")]
        public IActionResult ViewUpdateCa(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin ca";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ca nsx = context.Ca.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateCa(Ca nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ca n = context.Ca.Find(nsx.Idca);
            n.MaCa = nsx.MaCa;
            n.TenCa = nsx.TenCa;
            n.Thu = nsx.Thu;
            n.TgbatDau = nsx.TgbatDau;
            n.TgketThuc = nsx.TgketThuc;


            context.Ca.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewCa");
        }
        [Route("/QuanLy/deleteCa/{id}")]
        public IActionResult DeleteCa(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ca nsx = context.Ca.Find(id);
            nsx.Active = false;

            context.Ca.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewCa");
        }
        [HttpPost("/loadViewCa")]
        public IActionResult loadViewCa(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.Ca.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.Ca.ToList();
            }
            return PartialView("ViewDivCa");
        }
        public IActionResult khoiphucCa(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            Ca dvt = context.Ca.Find(id);

            dvt.Active = true;

            context.Ca.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewCa");
        }
        ///////////////////////////////
        public IActionResult ViewNTA()
        {
            return View("ViewNTA");
        }
        public IActionResult ViewInsertNTA()
        {
            return View("ViewInsertNTA");
        }
        [HttpPost]
        public IActionResult InsertNTA(NhomThucAn nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.NhomThucAn.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewNTA");

        }
        [Route("/QuanLy/ViewUpdateNTA/{id}")]
        public IActionResult ViewUpdateNTA(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin nhóm thức ăn";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomThucAn nsx = context.NhomThucAn.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateNTA(NhomThucAn nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomThucAn n = context.NhomThucAn.Find(nsx.Idnta);
            n.MaNta = nsx.MaNta;
            n.TenNta = nsx.TenNta;
            


            context.NhomThucAn.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewNTA");
        }
        [Route("/QuanLy/deleteNTA/{id}")]
        public IActionResult DeleteNTA(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomThucAn nsx = context.NhomThucAn.Find(id);
            nsx.Active = false;

            context.NhomThucAn.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewNTA");
        }
        [HttpPost("/loadViewNTA")]
        public IActionResult loadViewNTA(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.NhomThucAn.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.NhomThucAn.ToList();
            }
            return PartialView("ViewDivNTA");
        }
        public IActionResult khoiphucNTA(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomThucAn dvt = context.NhomThucAn.Find(id);

            dvt.Active = true;

            context.NhomThucAn.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewNTA");
        }
        //------------------------------------------------------------
        public IActionResult ViewBaoCao()
        {
            return View("ViewBaoCao");
        }
        [HttpPost("/loadTableLichSuHD")]
        public IActionResult loadTableLichSuHD(string fromDay, string toDay,string MaHD,int IdSanh,int IdTieuKhu, int IdBan, int IdTD)
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
            if (IdSanh==0 && IdTieuKhu==0 && IdBan==0 && IdTD==0)
            {
                ViewBag.ListPhieuNhap = listPhieu.Where(x => (MaHD == null ? true : x.MaHd.Contains(MaHD.ToUpper())));
            }
            else
            {
                ViewBag.ListPhieuNhap = listPhieu.Where(x => (IdTD == 0 ? true : (x.ChiTietHoaDon.Where(y => y.Idtd == IdTD).Count() > 0 ? true : false))
                && (IdSanh == 0 ? true : x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation.Idsanh == IdSanh)
                && (IdTieuKhu == 0 ? true : x.IdbanNavigation.IdkhuNavigation.Idkhu == IdTieuKhu)
                && (IdBan == 0 ? true : x.IdbanNavigation.Idban == IdBan)
                && (MaHD == null ? true : x.MaHd.Contains(MaHD.ToUpper())));
            }

            return PartialView("TableLichSuHD");
        }
        [HttpPost("/ViewThongTinHD")]
        public IActionResult ViewThongTinHD(int idPN)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var hd = context.HoaDon.Include(x => x.ChiTietHoaDon).Include(x => x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation).Include(x => x.IdnvNavigation).FirstOrDefault(x => x.Idhd == idPN);
            return PartialView(hd);
        }
       
    }

}
