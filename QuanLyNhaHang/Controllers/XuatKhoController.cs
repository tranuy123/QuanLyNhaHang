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
                .Where(x => x.IdctpnNavigation.Idhh == idHH)
                .ToList();
            var ton = new
            {
                DonViTinh = getDonViTinh((int)tonKho.First().IdctpnNavigation.Idhh),
                SoLuong = tonKho.Sum(x => x.SoLuong),
                DonGia = tonKho.Max(x => x.IdctpnNavigation.Gia),
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
                context.PhieuXuat.Add(phieuXuat);
                context.SaveChanges();

                foreach (ChiTietPhieuXuatMap t in chiTietPhieuXuatMaps.ToList())
                {
                    double slq = double.Parse(t.SoLuong);
                    foreach (TonKho slhhc in soLuongHhcon.Where(x => x.IdctpnNavigation.Idhh == int.Parse(t.Idhh)))
                    {
                        ChiTietPhieuXuat ct = new ChiTietPhieuXuat();
                        ct.Idhh = int.Parse(t.Idhh);
                        ct.Idpx = phieuXuat.Idpx;
                        ct.Gia = double.Parse(t.Gia);
                        ct.Idctpn = slhhc.Idctpn;
                        ct.Active = true;
                        //nếu mà trong kho còn nhiều hơn số xuất
                        if (slhhc.SoLuong > slq)
                        {
                            ct.SoLuong = double.Parse(t.SoLuong);
                            slhhc.SoLuong -= slq;
                            context.TonKho.Update(slhhc);
                            context.ChiTietPhieuXuat.Add(ct);
                            context.SaveChanges();
                            break;
                        }
                        //nếu mà trong kho ngang với số cần xuất
                        if (slhhc.SoLuong == slq)
                        {
                            ct.SoLuong = double.Parse(t.SoLuong);
                            context.TonKho.Remove(slhhc);
                            context.ChiTietPhieuXuat.Add(ct);
                            context.SaveChanges();
                            break;
                        }
                        //nếu trong kho còn ít hơn số cần xuất
                        if (slhhc.SoLuong < slq)
                        {
                            ct.SoLuong = (double)slhhc.SoLuong;
                            slq = (double)(slq - slhhc.SoLuong);

                            t.SoLuong = slq.ToString();
                            context.TonKho.Remove(slhhc);
                            context.ChiTietPhieuXuat.Add(ct);
                            context.SaveChanges();
                        }
                    }
                    chiTietPhieuXuatMaps.Remove(t);
                    context.SaveChanges();
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
    }

    
    public class TTPhieuXuat
    {
        public PhieuXuatMap PhieuXuat { get; set; }
        public List<ChiTietPhieuXuatMap> ChiTietPhieuXuat { get; set; }
    }
}
