using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using System;

namespace QuanLyNhaHang.Controllers
{

    [Authorize(Roles = "2")]

    public class NhaBepController : Controller
    {
        public IActionResult Index()
        {

            return View("ViewNhaBep");
        }

        [HttpPost("/XacNhanMon")]

        public string XacNhanMon(string id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDon ct = context.ChiTietHoaDon.Find(int.Parse(id));
            ct.Tgbep = DateTime.Now;
            context.ChiTietHoaDon.Update(ct);
            context.SaveChanges();
            return "Xác nhận thành công";
        }
    }
}
