using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using System;
using System.IO;
using System.Linq;

namespace QuanLyNhaHang.Controllers
{
    public class ThongTinCaNhanController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        public ThongTinCaNhanController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }
        public IActionResult ThongTinCaNhan()
        {
            
            NhanVien nv = context.NhanVien
                .Include(x => x.IdnnvNavigation)
                .Include(x => x.IdtkNavigation)
                .ThenInclude(x => x.IdvtNavigation).FirstOrDefault(x => x.Idnv == Int32.Parse(User.Identity.Name));

            return View("ThongTinCaNhan", nv);
        }
        [HttpPost]
        public IActionResult updateThongTinCaNhan(NhanVien hh, IFormFile Avt)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            TaiKhoan tk = context.TaiKhoan.Find(hh.Idtk);
            tk.Pass = hh.IdtkNavigation.Pass;
            context.TaiKhoan.Update(tk);
            NhanVien nv = context.NhanVien.Find(hh.Idnv);
            nv.Image = UploadedFile(hh, Avt);
            context.NhanVien.Update(nv);
            context.SaveChanges();
            TempData["ThongBao"] = "Cập nhập thành công!";
            return RedirectToAction("ThongTinCaNhan");
        }
        private string UploadedFile(NhanVien model, IFormFile avt)
        {
            string uniqueFileName = null;

            if (avt != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "ImagesTD");
                uniqueFileName = model.MaMv + ".jpg";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    avt.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
