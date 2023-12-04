using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHang.Controllers
{
    public class KhoHangController : Controller
    {
        public IActionResult KhoHang()
        {
            return View();
        }
    }
}
