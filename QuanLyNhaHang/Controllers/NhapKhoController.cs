using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Controllers
{
    public class NhapKhoController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        private readonly IMapper _mapper;
        public NhapKhoController(IMapper mapper)
        {
            _mapper = mapper;
        }
        public IActionResult NhapKho()
        {
            return View();
        }
        [HttpPost("/NhapKho/ThemPhieuNhap")]
        public async Task<dynamic> ThemPhieuNhap([FromBody] TTPhieuNhap data)
        {
            NhanVien nv = context.NhanVien.Find(Int32.Parse(User.Identity.Name));
            PhieuNhapMap phieuNhapMap = data.PhieuNhap;
            List<ChiTietPhieuNhapMap> chiTietPhieuNhapMaps = data.ChiTietPhieuNhap;

           
            using var tran = context.Database.BeginTransaction();
            try
            {
                List<ChiTietPhieuNhap> chiTietPhieuNhaps = _mapper.Map<List<ChiTietPhieuNhap>>(chiTietPhieuNhapMaps);
                PhieuNhap phieuNhap = _mapper.Map<PhieuNhap>(phieuNhapMap);

                phieuNhap.SoPn = taoSoPhieuNhap();
                phieuNhap.Idnv = nv.Idnnv;
                phieuNhap.Active = true;
                await context.PhieuNhap.AddAsync(phieuNhap);
                await context.SaveChangesAsync();
                foreach (ChiTietPhieuNhap chiTiet in chiTietPhieuNhaps)
                {
                    chiTiet.Idpn = phieuNhap.Idpn;
                    chiTiet.Active = true;
                }
                await context.ChiTietPhieuNhap.AddRangeAsync(chiTietPhieuNhaps);
                await context.SaveChangesAsync();
                List<TonKho> tonKhos = new List<TonKho>();  
                foreach(ChiTietPhieuNhap chiTiet1 in chiTietPhieuNhaps)
                {
                    TonKho tonKho = new TonKho();
                    tonKho.Idctpn = chiTiet1.Idctpn;
                    tonKho.SoLuong = chiTiet1.SoLuong;
                    tonKho.NgayNhap = phieuNhap.NgayNhap;
                    tonKhos.Add(tonKho);
                }
                await context.TonKho.AddRangeAsync(tonKhos);
                await context.SaveChangesAsync();
                tran.Commit();
                return new
                {
                    statusCode = 200,
                    message = "Thành công",
                };
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return new
                {
                    statusCode = 500,
                    message = "Thất bại",
                };
            }
        }
        private string taoSoPhieuNhap()
        {
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyyMMdd");
            var phieuNhap = context.PhieuNhap.Where(x => x.SoPn.Contains(date)).ToList();
            return $"{date}-{(phieuNhap.Count() + 1).ToString("D2")}";
        }
        public class TTPhieuNhap
        {
            public PhieuNhapMap PhieuNhap { get; set; }
            public List<ChiTietPhieuNhapMap> ChiTietPhieuNhap { get; set; }
        }
    }
}
