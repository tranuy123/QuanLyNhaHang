using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System;
using QuanLyNhaHang.Models;
using System.Collections.Generic;

namespace QuanLyNhaHang.Controllers
{
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

    }

}
