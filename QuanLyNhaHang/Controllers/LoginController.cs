using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using QuanLyNhaHang.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaHang.Controllers
{
    [AllowAnonymous]

    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("/login")]
        public IActionResult Index(string returnUrl)
        {
            if (HttpContext.User.Identity.Name == null)
                return View("Login");
            else
            {
                if (string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith("/"))
                {
                    returnUrl = "/";
                }
                return Redirect(returnUrl);
            }
        }
        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> LoginUser(TaiKhoan taiKhoan, string returnUrl)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            TaiKhoan a = context.TaiKhoan
                .Include(x => x.IdvtNavigation)
                .FirstOrDefault(x => x.TenTk == taiKhoan.TenTk);
            

            if (a == null)
            {
                return Redirect("/login");
            }
            if (a.Pass.Equals(taiKhoan.Pass))
            {
                await SignInUser(a);
               
                if (string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith("/"))
                {
                    returnUrl = "/";
                }
                return Redirect(returnUrl);
            }
            else
            {
                TempData["LoginFailed"] = true;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
        private async Task SignInUser(TaiKhoan accounts)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            NhanVien user = context.NhanVien.Where(x => x.Idtk == accounts.Idtk).FirstOrDefault();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Idnv.ToString()),
                new Claim(ClaimTypes.Role, accounts.IdvtNavigation.Idvt.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

    }
}
