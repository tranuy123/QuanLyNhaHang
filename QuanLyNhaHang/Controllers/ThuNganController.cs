using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyNhaHang.Controllers
{
    public class ThuNganController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("/ThuNgan/UpdateHH")]
        public void UpdateHH([FromBody] UpdateHoaDon data)
        {
            double tongTien = 0;
            using var tran = context.Database.BeginTransaction();
            try
            {
                List<ChiTietHoaDon> chiTietHoaDons = new List<ChiTietHoaDon>(); 
                foreach (ChiTietHoaDon ct in data.ChiTietHoaDon)
                {
                    ChiTietHoaDon c = context.ChiTietHoaDon.Find(ct.Idcthd);
                    c.TyLeGiam = ct.TyLeGiam;
                    c.ThanhTien = ct.ThanhTien;
                    tongTien += (double)ct.ThanhTien;
                    chiTietHoaDons.Add(c);  
                }
                context.ChiTietHoaDon.UpdateRange(chiTietHoaDons);
                context.SaveChanges();
                HoaDon hoa = context.HoaDon.Find(int.Parse(data.IdHD));
                hoa.TongTien = data.ChiTietHoaDon.Sum(x => x.ThanhTien);
                context.HoaDon.Update(hoa);
                context.SaveChanges();
                tran.Commit();  
            }
            catch (Exception ex)
            {
                tran.Rollback();
            }
        }
        [HttpPost("/ThuNgan/HuyXNTT")]
        public dynamic HuyXNTT(int idHD)
        {
            try
            {
                HoaDon hoaDon = context.HoaDon.Find(idHD);
                hoaDon.Tgxuat = null;
                hoaDon.TinhTrang = false;
                hoaDon.TinhTrangTt = null;
                context.HoaDon.Update(hoaDon);
                context.SaveChanges();
                return new
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    statusCode = 500,
                    message = "Thất bại"
                };
}

        }
        public class UpdateHoaDon {
            public List<ChiTietHoaDon> ChiTietHoaDon { get; set; }
            public string IdHD { get; set; }   
        }
    }
}
