using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Controllers
{
    public class DinhMucController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();  
        public IActionResult DinhMuc()
        {
            ViewBag.ThucDon = context.ThucDon
                .Include(x => x.DinhMuc).Where(x => x.Active == true).ToList();
            ViewBag.HangHoa = context.HangHoa
                .Include(x => x.IdnhhNavigation)
                .Where(x => x.Active == true && x.IdnhhNavigation.NguyenLieu == true).ToList();    
            return View();
        }
        [HttpPost("DinhMuc/showDinhMuc")]

        public async Task<List<DinhMuc>> showDinhMuc(long idPrf)
        {
            List<DinhMuc> proFileCt = await context.DinhMuc
                .Where(x => x.Idtd == idPrf)
                .Select(x => new DinhMuc()
                {
                    Iddm = x.Iddm,
                    IdtdNavigation = new ThucDon()
                    {
                        Idtd = x.IdtdNavigation.Idtd,
                        Ten = x.IdtdNavigation.Ten
                    },
                    IdhhNavigation = new HangHoa()
                    {
                        Idhh = x.IdhhNavigation.Idhh,
                        TenHh = x.IdhhNavigation.TenHh
                    },
                    SoLuong = x.SoLuong,
                }).ToListAsync();
            return proFileCt;
        }
        public class DinhMucData
        {
            public int IdPRF { get; set; }
            public List<DinhMuc> IdDVKT { get; set; }
        }
        public class DinhMucTam
        {
            int idhh { get; set; }
            string soLuong { get; set; }
        }
        [HttpPost("DinhMuc/LuuDinhMuc")]

        public async Task<dynamic> luuProfileCt([FromBody] DinhMucData data)
        {
            int idPRF = data.IdPRF;
            List<DinhMuc> idDVKT = data.IdDVKT;
            try
            {
                var proFileCts = await context.DinhMuc.Where(x => x.Idtd == idPRF).ToListAsync();
                if (proFileCts != null)
                {
                    context.DinhMuc.RemoveRange(proFileCts);
                }
                var DinhMuc = new List<DinhMuc>();
                foreach (DinhMuc iddvkt in idDVKT)
                {
                    
                    DinhMuc.Add(new DinhMuc
                    {
                        Idtd = idPRF,
                        Idhh = iddvkt.Idhh,
                        SoLuong =iddvkt.SoLuong,
                    });
                }
                await context.DinhMuc.AddRangeAsync(DinhMuc);
                await context.SaveChangesAsync();
                return new
                {
                    statusCode = 200,
                    message = "Thành công!",
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    statusCode = 500,
                    message = "Thất bại!",
                };
            }
        }

    }


}
