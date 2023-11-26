using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System;
using System.Threading.Tasks;
using QuanLyNhaHang.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Internal;
using AutoMapper.Configuration.Conventions;
using System.Collections.Generic;

namespace QuanLyNhaHang.Controllers
{
    public class TonKhoController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();  
        public IActionResult TonKho()
        {
            return View();
        }
        [HttpPost("/TonKho/BaoCaoTongHop")]
        public async Task<dynamic> getBaoCaoTongHop(int idNhomHang, int idHangHoa)
        {
            try
            {

                var tonKho = await context.TonKho
                    .Include(x => x.IdctpnNavigation)
                    .ThenInclude(x => x.IdhhNavigation)
                    .Where(x => (idNhomHang == 0 || x.IdctpnNavigation.IdhhNavigation.Idnhh == idNhomHang)
                                && (idHangHoa == 0 || x.IdctpnNavigation.Idhh == idHangHoa))
                    .ToListAsync();
                var tonkho1 = tonKho.GroupBy(x => x.IdctpnNavigation.Idhh)
                    .Select(x => new
                    {
                        Id = x.Key,
                        MaHang = getMaHang((int)x.Key),
                        TenHang = getTenHang((int)x.Key),
                        TongSL = Math.Round((float)x.Sum(x => x.SoLuong),3),
                        TongTien = Math.Round((float)x.Sum(x => x.IdctpnNavigation.Gia * x.SoLuong),3)

                    })
                    .ToList();
                return Ok(tonkho1);
            }
            catch (Exception ex)
            {
                return ex.Message; 
            }
        }
        public string getMaHang(int idhh)
        {
            HangHoa hangHoa = context.HangHoa.Find(idhh);
            return hangHoa.MaHh;
        }
        public string getTenHang(int idhh)
        {
            HangHoa hangHoa = context.HangHoa.Find(idhh);
            return hangHoa.TenHh;
        }
        [HttpPost("/TonKho/BaoCaoChiTiet")]
        public async Task<dynamic> getBaoCaoChiTiet(int idNCC, int idNhomHang, int idHangHoa, string tuNgay, string denNgay)
        {
            DateTime FromDay = DateTime.ParseExact(tuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(denNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var tonKho = await context.TonKho
                .Include(x => x.IdctpnNavigation.IdpnNavigation.IdnccNavigation)
                .Include(x => x.IdctpnNavigation)
                .ThenInclude(x => x.IdhhNavigation.IddvtNavigation)
                .Where(x => (x.NgayNhap.Value.Date >= FromDay.Date && x.NgayNhap.Value.Date <= ToDay)
                            && (idNhomHang == 0 || x.IdctpnNavigation.IdhhNavigation.Idnhh == idNhomHang)
                            && (idHangHoa == 0 || x.IdctpnNavigation.Idhh == idHangHoa)
                            && (idNCC == 0 || x.IdctpnNavigation.IdpnNavigation.Idncc == idNCC))
                .ToListAsync();
            var tonkho1 = tonKho;
            return tonkho1.Select(x => new
            {
                Id = x.Idctpn,
                NgayNhap = x.NgayNhap.Value.ToString("dd-MM-yyyy"),
                NhaCungCap = x.IdctpnNavigation.IdpnNavigation.IdnccNavigation.TenNcc,
                MaHang = x.IdctpnNavigation.IdhhNavigation.MaHh,
                TenHang = x.IdctpnNavigation.IdhhNavigation.TenHh,
                NgaySX = x.IdctpnNavigation.Nsx.Value.ToString("dd-MM-yyyy"),
                HanSD = x.IdctpnNavigation.Hsd.Value.ToString("dd-MM-yyyy"),
                SoLuongNhap = Math.Round((float)x.IdctpnNavigation.SoLuong,3),
                SoLuongXuat = Math.Round(getSoLuongXuat((int)x.Idctpn),3),
                SoLuongTon = Math.Round((float)x.SoLuong,3),
                DonViTinh = x.IdctpnNavigation.IdhhNavigation.IddvtNavigation.TenDvt,
                GiaNhap = x.IdctpnNavigation.Gia,
                ThanhTien = Math.Round((float)(x.IdctpnNavigation.Gia * x.SoLuong), 3),
            }) ;
        }
        public double getSoLuongXuat(int idCTPN)
        {
            List<ChiTietPhieuXuat> chiTietPhieuXuats = context.ChiTietPhieuXuat.Where(x => x.Idctpn == idCTPN)
                .ToList();
            double soLuong = 0;
            if (chiTietPhieuXuats.Count()> 0)
            {
                soLuong = (double)chiTietPhieuXuats.Sum(x => x.SoLuong);
            }
            return soLuong;
        }
    }
}
