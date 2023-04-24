using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHang.Controllers
{
    public class TrangChuController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View("TrangChu");
        }
    }
}
