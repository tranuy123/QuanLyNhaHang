using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Models.Mapping;
using QuanLyNhaHang.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Controllers
{
    public class XuatKhoController : Controller
    {
        private readonly IMapper _mapper;
        public XuatKhoController(IMapper mapper)
        {
            _mapper = mapper;
        }
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        public IActionResult XuatKho()
        {
            return View("PhieuXuatKho");
        }
        [HttpPost("/XuatKho/getDonViTinhVaSL")]
        public async Task<dynamic> getDonViTinhVaSL(int idHH)
        {
            var tonKho = context.TonKho
                .Include(x => x.IdctpnNavigation)
                .ThenInclude(x => x.IdhhNavigation)
                .Where(x => x.IdctpnNavigation.Idhh == idHH)
                .ToList();
            var ton = new
            {
                DonViTinh = getDonViTinh((int)tonKho.First().IdctpnNavigation.Idhh),
                SoLuong = tonKho.Sum(x => x.SoLuong),
                DonGia = tonKho.Max(x => x.IdctpnNavigation.IdhhNavigation.GiaBan),
            };

            return ton;
        }
        public string getDonViTinh(int idHH)
        {
            HangHoa dvt = context.HangHoa
                .Include(x => x.IddvtNavigation)
                .FirstOrDefault(x => x.Idhh == idHH);
            return dvt.IddvtNavigation.TenDvt;
        }
        public string getTenHH(int idHH)
        {
            HangHoa tenhh = context.HangHoa
                .Include(x => x.IddvtNavigation)
                .FirstOrDefault(x => x.Idhh == idHH);
            return tenhh.TenHh;
        }
        [HttpPost("/XuatKho/ThemPhieuXuat")]
        public async Task<dynamic> luuPhieuXuat([FromBody] TTPhieuXuat data)
        {
            NhanVien nv = context.NhanVien.Find(Int32.Parse(User.Identity.Name));
            PhieuXuatMap ph = data.PhieuXuat;
            PhieuXuat phieuXuat = _mapper.Map<PhieuXuat>(ph);
            List<ChiTietPhieuXuatMap> chiTietPhieuXuatMaps = data.ChiTietPhieuXuat;
            List<TonKho> soLuongHhcon = await context.TonKho
                .Include(x => x.IdctpnNavigation)
                .OrderBy(x => x.NgayNhap).ToListAsync();
            var tran = context.Database.BeginTransaction();
            try
            {
                phieuXuat.Active = true;
                phieuXuat.Idnv = nv.Idnnv;
                phieuXuat.SoPx = CommonServices.taoSoPhieuXuat(context);
                phieuXuat.NgayTao = DateTime.Now;
                context.PhieuXuat.Add(phieuXuat);
                context.SaveChanges();

                foreach (ChiTietPhieuXuatMap t in chiTietPhieuXuatMaps.ToList())
                {
                    double slq = double.Parse(t.ThucXuat);
                    foreach (TonKho slhhc in soLuongHhcon.Where(x => x.IdctpnNavigation.Idhh == int.Parse(t.Idhh)))
                    {
                        ChiTietPhieuXuat ct = new ChiTietPhieuXuat();
                        ct.Idhh = int.Parse(t.Idhh);
                        ct.Idpx = phieuXuat.Idpx;
                        ct.Gia = double.Parse(t.Gia);
                        ct.Idctpn = slhhc.Idctpn;
                        ct.SoLuong = t.SoLuong == null ? double.Parse(t.ThucXuat) : double.Parse(t.SoLuong);
                        ct.ChenhLech = t.ChenhLech == null ? 0 : double.Parse(t.ChenhLech);
                        ct.Active = true;
                        //nếu mà trong kho còn nhiều hơn số xuất
                        if (slhhc.SoLuong > slq)
                        {
                            ct.ThucXuat = double.Parse(t.ThucXuat);
                            slhhc.SoLuong -= slq;
                            context.TonKho.Update(slhhc);
                            context.ChiTietPhieuXuat.Add(ct);
                            context.SaveChanges();
                            break;
                        }
                        //nếu mà trong kho ngang với số cần xuất
                        if (slhhc.SoLuong == slq)
                        {
                            ct.ThucXuat = double.Parse(t.ThucXuat);
                            context.TonKho.Remove(slhhc);
                            context.ChiTietPhieuXuat.Add(ct);
                            context.SaveChanges();
                            break;
                        }
                        //nếu trong kho còn ít hơn số cần xuất
                        if (slhhc.SoLuong < slq)
                        {
                            ct.ThucXuat = (double)slhhc.SoLuong;
                            slq = (double)(slq - slhhc.SoLuong);

                            t.ThucXuat = slq.ToString();
                            context.TonKho.Remove(slhhc);
                            context.ChiTietPhieuXuat.Add(ct);
                            context.SaveChanges();
                        }
                    }
                    chiTietPhieuXuatMaps.Remove(t);
                    context.SaveChanges();
                }
                if (data.TuNgay != null && data.DenNgay != null) {
                    await updateDSChiTietHoaDon(data.TuNgay, data.DenNgay);
                }
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
        public async Task updateDSChiTietHoaDon(string TuNgay, string DenNgay)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var listHH = new List<dynamic>();
            List<ChiTietHoaDon> chiTietHoaDons = await context.ChiTietHoaDon
                .Include(x => x.IdhdNavigation)
                .Where(x => x.IdhdNavigation.Tgxuat.Value.Date >= tuNgay.Date && x.IdhdNavigation.Tgxuat.Value.Date <= denNgay.Date && x.DaXuat != true && x.IdtdNavigation.Idnta != 3).ToListAsync();
            foreach (ChiTietHoaDon chiTietHoaDon in chiTietHoaDons)
            {
                chiTietHoaDon.DaXuat = true;
            }
            context.ChiTietHoaDon.UpdateRange(chiTietHoaDons);
            await context.SaveChangesAsync();

        }
        [HttpPost("/XuatKho/getDSXuatKho")]
        public async Task<dynamic> getDSXuatKho(string TuNgay, string DenNgay)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var listHH = new List<dynamic>();
            List<ChiTietHoaDon> chiTietHoaDons = await context.ChiTietHoaDon
                .Include(x => x.IdhdNavigation)
                .Where(x => x.IdhdNavigation.Tgxuat.Value.Date >= tuNgay.Date && x.IdhdNavigation.Tgxuat.Value.Date <= denNgay.Date && x.DaXuat != true && x.IdtdNavigation.Idnta != 3).ToListAsync();
            foreach (ChiTietHoaDon cthd in chiTietHoaDons)
            {
                List<DinhMuc> dinhMucs = context.DinhMuc
                    .Include(x => x.IdhhNavigation)
                    .ThenInclude(x => x.ChiTietPhieuNhap)
                    .ThenInclude(x => x.TonKho).Where(x => x.Idtd == cthd.Idtd).ToList();
                foreach (DinhMuc dm in dinhMucs)
                {
                    var hanghoa = new
                    {
                        idhh = dm.Idhh,
                        soLuong = (float)(dm.SoLuong * cthd.Sl),
                        donGia = dm.IdhhNavigation.ChiTietPhieuNhap.Where(x => x.TonKho.Any()).Max(x => x.Gia),
                    };
                    listHH.Add(hanghoa);    
                }
            }
            var hangHoas = listHH.GroupBy(x => x.idhh)
                .Select(x => new
                {
                    idhh = x.Key,
                    tenHangHoa = getTenHH((int)x.Key),
                    donViTinh = getDonViTinh((int)x.Key),
                    soLuong = (float)x.Sum(x => (float)x.soLuong),
                    donGia = x.First().donGia,
                });
            return hangHoas;
        }
        [HttpGet("/XuatKho/XuatKhoNguyenLieu")]
        public IActionResult ViewXuatNguyenLieu()
        {
            return View("PhieuXuatNguyenLieu");
        }
        [HttpPost("/XuatKho/LichSuXuat")]
        public async Task<dynamic> LichSuXuat(string TuNgay, string DenNgay, string maPhieu)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var phieuXuats = await context.PhieuXuat
                .Where(x => (x.NgayTao.Value.Date >= tuNgay.Date && x.NgayTao.Value.Date <= denNgay.Date)
                && (maPhieu == "" || maPhieu == null || x.SoPx == maPhieu))
             .Select(x => new
             {
                 SoPx = x.SoPx,
                 NgayTao = x.NgayTao,
                 IdnvNavigation = x.IdnvNavigation,
                 GhiChu = x.GhiChu,
                 TongTien = x.ChiTietPhieuXuat.Sum(x => x.SoLuong * x.Gia),
                 TongChenhLech = x.ChiTietPhieuXuat.Sum(x => x.ChenhLech),
                 SoLuongHH = x.ChiTietPhieuXuat.Count(),
                 ChiTietPhieuXuat = x.ChiTietPhieuXuat.Select(x => new
                 {
                     IdhhNavigation = x.IdhhNavigation,
                     SoLuong = x.SoLuong,
                     ChenhLech = x.ChenhLech,
                     ThucXuat = x.ThucXuat,
                     Gia = x.Gia,
                     DVT = x.IdhhNavigation.IddvtNavigation.TenDvt,
                 }).ToList()
             })
            .ToListAsync();
            //var ketqua = phieuXuats.Select(x => new
            //{
            //    PhieuXuat = x,
            //    ChiTietPhieuXuat = GetChiTietPhieuXuats(x.Idpx),
            //}).ToList();
            return Ok(phieuXuats);
        }
        public List<ChiTietPhieuXuat> GetChiTietPhieuXuats(int id)
        {
            List<ChiTietPhieuXuat> chiTietPhieuXuats = context.ChiTietPhieuXuat.Where(x => x.Idpx == id).ToList();
            return chiTietPhieuXuats;
        }
    }    
    public class TTPhieuXuat
    {
        public PhieuXuatMap PhieuXuat { get; set; }
        public List<ChiTietPhieuXuatMap> ChiTietPhieuXuat { get; set; }
        public string TuNgay { get;set; }
        public string DenNgay { get; set; }
    }
}
