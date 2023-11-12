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
using QuanLyNhaHang.Services;
using System.Threading.Tasks;

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
        public IActionResult Error()
        {
            return View("Shared/Error");
        }
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
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
        [HttpPost("DanhMuc/Sanh/api/getModelsCB")]
        public async Task<dynamic> getModelsCB(string key)
        {

            var models = await context.Sanh.Where(x => (key == "" ? true : ((x.MaSanh != null && x.MaSanh.ToLower().Contains(key.ToLower())) ||
                                               (x.TenSanh != null && x.TenSanh.ToLower().Contains(key.ToLower())))) &&
                                                x.Active == true).Select(x => new
                                                {
                                                    Id = x.Idsanh,
                                                    Ma = x.MaSanh,
                                                    Ten = x.TenSanh,
                                                }).ToListAsync();
            return models;

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
            var listKhu = context.Khu.Where(k => k.Idsanh == idSanh && k.Active == true).ToList();

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
                ViewBag.NSX = context.Ban.Include(x => x.IdkhuNavigation.IdsanhNavigation).Where(x => x.Active == true).ToList();
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
            Gia a = context.Gia.FirstOrDefault(x => x.Idsanh == ids && x.Idtd == idtd && x.Active == true);
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
            Gia nsx = context.Gia.FirstOrDefault(x => x.Idsanh == ids && x.Idtd == idtd && x.Active == true);

            context.Gia.Remove(nsx);
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
        public IActionResult loadTableLichSuHD(string fromDay, string toDay, string MaHD, int IdSanh, int IdTieuKhu, int IdBan, int IdTD)
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
            if (IdSanh == 0 && IdTieuKhu == 0 && IdBan == 0 && IdTD == 0)
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
            ViewBag.TuNgay = fromDay;
            ViewBag.DenNgay = toDay;
            return PartialView("TableLichSuHD");
        }
        [HttpPost("/ViewThongTinHD")]
        public IActionResult ViewThongTinHD(int idPN)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var hd = context.HoaDon.Include(x => x.ChiTietHoaDon).Include(x => x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation).Include(x => x.IdnvNavigation).FirstOrDefault(x => x.Idhd == idPN);
            return PartialView(hd);
        }
        //-------------------------------------------------------------------------
        public IActionResult ViewNNV()
        {
            return View("ViewNNV");
        }
        public IActionResult ViewInsertNNV()
        {
            return View("ViewInsertNNV");
        }
        [HttpPost]
        public IActionResult InsertNNV(NhomNhanVien nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.NhomNhanVien.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewNNV");

        }
        [Route("/QuanLy/ViewUpdateNNV/{id}")]
        public IActionResult ViewUpdateNNV(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin nhóm nhân viên";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomNhanVien nsx = context.NhomNhanVien.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateNNV(NhomNhanVien nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomNhanVien n = context.NhomNhanVien.Find(nsx.Idnnv);
            n.MaNnv = nsx.MaNnv;
            n.TenNnv = nsx.TenNnv;



            context.NhomNhanVien.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewNNV");
        }
        [Route("/QuanLy/deleteNNV/{id}")]
        public IActionResult DeleteNNV(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomNhanVien nsx = context.NhomNhanVien.Find(id);
            nsx.Active = false;

            context.NhomNhanVien.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewNNV");
        }
        [HttpPost("/loadViewNNV")]
        public IActionResult loadViewNNV(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.NhomNhanVien.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.NhomNhanVien.ToList();
            }
            return PartialView("ViewDivNNV");
        }
        public IActionResult khoiphucNNV(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomNhanVien dvt = context.NhomNhanVien.Find(id);

            dvt.Active = true;

            context.NhomNhanVien.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewNNV");
        }
        //------------------------------------------------
        public IActionResult ViewLLV()
        {
            return View("ViewLLV");
        }
        public IActionResult ViewInsertLLV()
        {
            return View("ViewInsertLLV");
        }
        [HttpPost]
        public IActionResult InsertLLV(LichLamViec nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.LichLamViec.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewLLV");

        }
        [Route("/QuanLy/ViewUpdateLLV/{id}")]
        public IActionResult ViewUpdateLLV(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin lịch làm việc";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            LichLamViec n = context.LichLamViec.Include(x => x.IdcaNavigation).Include(x => x.IdkhuNavigation).Include(x => x.IdnvNavigation).FirstOrDefault(x => x.Idllv == id);

            return View(n);
        }
        [Route("/QuanLy/deleteLLV/{id}")]
        public IActionResult deleteLLV(int id)

        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            LichLamViec n = context.LichLamViec.Include(x => x.IdcaNavigation).Include(x => x.IdkhuNavigation).Include(x => x.IdnvNavigation).FirstOrDefault(x => x.Idllv == id);
            n.Active = false;
            context.LichLamViec.Update(n);
            context.SaveChanges();
            return RedirectToAction("ViewLLV");
        }
        public IActionResult UpdateLLV(LichLamViec nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            LichLamViec n = context.LichLamViec.Find(nsx.Idllv);

            n.Idkhu = nsx.Idkhu;
            n.Idnv = nsx.Idnv;
            n.Idca = nsx.Idca;



            context.LichLamViec.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewLLV");
        }
        //--------------------------------------
        public IActionResult ViewQLDanhGia()
        {
            return View("ViewQLDanhGia");
        }
        public IActionResult ViewInsertQLDanhGia()
        {
            return View("ViewInsertQLDanhGia");
        }
        [HttpPost]
        public IActionResult InsertQLDanhGia(QlDanhGia nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.QlDanhGia.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewQLDanhGia");

        }
        [Route("/QuanLy/ViewUpdateQLDanhGia/{id}")]
        public IActionResult ViewUpdateQLDanhGia(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin lịch làm việc";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            QlDanhGia n = context.QlDanhGia.Include(x => x.IdnnvNavigation).FirstOrDefault(x => x.IddanhGia == id);

            return View(n);
        }
        public IActionResult UpdateQLDanhGia(QlDanhGia nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            QlDanhGia n = context.QlDanhGia.Find(nsx.IddanhGia);

            n.Ma = nsx.Ma;
            n.Ten = nsx.Ten;
            n.Idnnv = nsx.Idnnv;
            n.ThoiGianTu = nsx.ThoiGianTu;
            n.ThoiGianDen = nsx.ThoiGianDen;
            n.Diem = nsx.Diem;
            n.Active = nsx.Active;
            context.QlDanhGia.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewQLDanhGia");
        }
        /////////////////////////////////////////////////////
        public IActionResult ViewNhomHangHoa()
        {
            return View("ViewNhomHangHoa");
        }
        public IActionResult ViewInsertNhomHangHoa()
        {
            return View("ViewInsertNhomHangHoa");
        }
        [HttpPost]
        public IActionResult InsertNhomHangHoa(NhomHangHoa nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.NhomHangHoa.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewNhomHangHoa");

        }
        [Route("/QuanLy/ViewUpdateNhomHangHoa/{id}")]
        public IActionResult ViewUpdateNhomHangHoa(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin NhomHangHoa";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomHangHoa nsx = context.NhomHangHoa.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateNhomHangHoa(NhomHangHoa nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomHangHoa n = context.NhomHangHoa.Find(nsx.Idnhh);
            n.MaNhh = nsx.MaNhh;
            n.TenNhh = nsx.TenNhh;


            context.NhomHangHoa.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewNhomHangHoa");
        }
        [Route("/QuanLy/deleteNhomHangHoa/{id}")]
        public IActionResult DeleteNhomHangHoa(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomHangHoa nsx = context.NhomHangHoa.Find(id);
            nsx.Active = false;

            context.NhomHangHoa.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewNhomHangHoa");
        }
        [HttpPost("/loadViewNhomHangHoa")]
        public IActionResult loadViewNhomHangHoa(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.NhomHangHoa.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.NhomHangHoa.ToList();
            }
            return PartialView("ViewDivNhomHangHoa");
        }
        [Route("/QuanLy/khoiphucNhomHangHoa/{id}")]
        public IActionResult khoiphucNhomHangHoa(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhomHangHoa dvt = context.NhomHangHoa.Find(id);

            dvt.Active = true;

            context.NhomHangHoa.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewNhomHangHoa");
        }
        ///////////////////////////////////////////////////////////////////
        public IActionResult ViewNhaCungCap()
        {
            return View("ViewNhaCungCap");
        }
        public IActionResult ViewInsertNhaCungCap()
        {
            return View("ViewInsertNhaCungCap");
        }
        [HttpPost]
        public IActionResult InsertNhaCungCap(NhaCungCap nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.NhaCungCap.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewNhaCungCap");

        }
        [Route("/QuanLy/ViewUpdateNhaCungCap/{id}")]
        public IActionResult ViewUpdateNhaCungCap(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin NhaCungCap";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhaCungCap nsx = context.NhaCungCap.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateNhaCungCap(NhaCungCap nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhaCungCap n = context.NhaCungCap.Find(nsx.Idncc);
            n.MaNcc = nsx.MaNcc;
            n.TenNcc = nsx.TenNcc;
            n.DiaChi = nsx.DiaChi;
            n.DienThoai = nsx.DienThoai;
            n.Mail = nsx.Mail;
            n.GhiChu = nsx.GhiChu;


            context.NhaCungCap.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewNhaCungCap");
        }
        [Route("/QuanLy/deleteNhaCungCap/{id}")]
        public IActionResult DeleteNhaCungCap(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhaCungCap nsx = context.NhaCungCap.Find(id);
            nsx.Active = false;

            context.NhaCungCap.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewNhaCungCap");
        }
        [HttpPost("/loadViewNhaCungCap")]
        public IActionResult loadViewNhaCungCap(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.NhaCungCap.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.NhaCungCap.ToList();
            }
            return PartialView("ViewDivNhaCungCap");
        }
        [Route("/QuanLy/khoiphucNhaCungCap/{id}")]
        public IActionResult khoiphucNhaCungCap(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhaCungCap dvt = context.NhaCungCap.Find(id);

            dvt.Active = true;

            context.NhaCungCap.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewNhaCungCap");
        }
        /////////////////////////////////////////
        public IActionResult ViewHangHoa()
        {
            return View("ViewHangHoa");
        }
        public IActionResult ViewInsertHangHoa()
        {
            return View("ViewInsertHangHoa");
        }
        [HttpPost]
        public IActionResult InsertHangHoa(HangHoa nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.HangHoa.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewHangHoa");

        }
        [Route("/QuanLy/ViewUpdateHangHoa/{id}")]
        public IActionResult ViewUpdateHangHoa(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin HangHoa";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            HangHoa nsx = context.HangHoa.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateHangHoa(HangHoa nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            HangHoa n = context.HangHoa.Find(nsx.Idhh);
            n.MaHh = nsx.MaHh;
            n.TenHh = nsx.TenHh;
            n.Idnhh = nsx.Idnhh;
            n.Iddvt = nsx.Iddvt;


            context.HangHoa.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewHangHoa");
        }
        [Route("/QuanLy/deleteHangHoa/{id}")]
        public IActionResult DeleteHangHoa(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            HangHoa nsx = context.HangHoa.Find(id);
            nsx.Active = false;

            context.HangHoa.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewHangHoa");
        }
        [HttpPost("/loadViewHangHoa")]
        public IActionResult loadViewHangHoa(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.HangHoa.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.HangHoa.ToList();
            }
            return PartialView("ViewDivHangHoa");
        }
        [Route("/QuanLy/khoiphucHangHoa/{id}")]
        public IActionResult khoiphucHangHoa(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            HangHoa dvt = context.HangHoa.Find(id);

            dvt.Active = true;

            context.HangHoa.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewHangHoa");
        }
        /////////////////////////////////// đơn vị tính
        public IActionResult ViewDonViTinh()
        {
            return View("ViewDonViTinh");
        }
        public IActionResult ViewInsertDonViTinh()
        {
            return View("ViewInsertDonViTinh");
        }
        [HttpPost]
        public IActionResult InsertDonViTinh(DonViTinh nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

            nsx.Active = true;
            context.DonViTinh.Add(nsx);
            context.SaveChanges();
            TempData["ThongBao"] = "Thêm thành công!";
            return RedirectToAction("ViewDonViTinh");

        }
        [Route("/QuanLy/ViewUpdateDonViTinh/{id}")]
        public IActionResult ViewUpdateDonViTinh(int id)

        {
            ViewData["Title"] = "Cập nhập thông tin DonViTinh";
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            DonViTinh nsx = context.DonViTinh.Find(id);
            return View(nsx);
        }
        public IActionResult UpdateDonViTinh(DonViTinh nsx)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            DonViTinh n = context.DonViTinh.Find(nsx.Iddvt);
            n.MaDvt = nsx.MaDvt;
            n.TenDvt = nsx.TenDvt;

            context.DonViTinh.Update(n);
            context.SaveChanges();
            TempData["ThongBao"] = "Sửa thành công!";
            return RedirectToAction("ViewDonViTinh");
        }
        [Route("/QuanLy/deleteDonViTinh/{id}")]
        public IActionResult DeleteDonViTinh(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            DonViTinh nsx = context.DonViTinh.Find(id);
            nsx.Active = false;

            context.DonViTinh.Update(nsx);
            context.SaveChanges();
            return RedirectToAction("ViewDonViTinh");
        }
        [HttpPost("/loadViewDonViTinh")]
        public IActionResult loadViewDonViTinh(bool active)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            if (active)
            {
                ViewBag.NSX = context.DonViTinh.Where(x => x.Active == true).ToList();
            }
            else
            {
                ViewBag.NSX = context.DonViTinh.ToList();
            }
            return PartialView("ViewDivDonViTinh");
        }
        [Route("/QuanLy/khoiphucDonViTinh/{id}")]
        public IActionResult khoiphucDonViTinh(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            DonViTinh dvt = context.DonViTinh.Find(id);

            dvt.Active = true;

            context.DonViTinh.Update(dvt);
            context.SaveChanges();
            TempData["ThongBao"] = "Khôi phục thành công!";
            return RedirectToAction("ViewDonViTinh");
        }
    }
}
