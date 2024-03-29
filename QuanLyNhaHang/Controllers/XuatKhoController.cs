﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Models.Mapping;
using QuanLyNhaHang.Services;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static QuanLyNhaHang.Controllers.TonKhoController;

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
        public double getSLTon(int idHH)
        {
            List<TonKho> tonKhos = context.TonKho
                .Include(x => x.IdctpnNavigation)
                .Where(x => x.IdctpnNavigation.Idhh == idHH)
                .ToList();
            double result = (double)tonKhos.Sum(x => x.SoLuong);
            return result;
        }
        public string taoSoPhieuXuat(List<PhieuXuat>  phieuNhaps)
        {
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyyMMdd");
            var phieuNhap = phieuNhaps.Where(x => x.SoPx.Contains(date)).ToList();
            return $"PX-{date}-{(phieuNhap.Count() + 1).ToString("D2")}";
        }
        [HttpPost("/XuatKho/ThemPhieuXuat")]
        public async Task<dynamic> luuPhieuXuat([FromBody] TTPhieuXuat data)
        {
            NhanVien nv = context.NhanVien.Find(Int32.Parse(User.Identity.Name));
            PhieuXuatMap ph = data.PhieuXuat;
            PhieuXuat phieuXuat = _mapper.Map<PhieuXuat>(ph);
            List<ChiTietPhieuXuatMap> chiTietPhieuXuatMaps = data.ChiTietPhieuXuat;
            List<PhieuXuat> phieuNhaps = context.PhieuXuat.ToList();

            List<TonKho> soLuongHhcon = await context.TonKho
                .Include(x => x.IdctpnNavigation)
                .OrderBy(x => x.IdctpnNavigation.Hsd).ToListAsync();
            var tran = context.Database.BeginTransaction();
            try
            {
                phieuXuat.TieuHuy = data.TieuHuy;
                phieuXuat.Active = true;
                phieuXuat.Idnv = nv.Idnnv;
                phieuXuat.SoPx = taoSoPhieuXuat(phieuNhaps);
                phieuXuat.NgayTao = DateTime.Now;
                context.PhieuXuat.Add(phieuXuat);
                context.SaveChanges();

                foreach (ChiTietPhieuXuatMap t in chiTietPhieuXuatMaps.ToList())
                {
                    double slq = double.Parse(t.ThucXuat);
                    var soLuongCon = soLuongHhcon.Where(x => x.IdctpnNavigation.Idhh == int.Parse(t.Idhh)).ToList();
                    double a = (double)soLuongHhcon.Sum(x => x.SoLuong);
                    if (soLuongCon.Sum(x => x.SoLuong) < slq || soLuongCon.Count() == 0)
                    {
                            if (data.TieuHuy == false)
                            {

                                return new
                                {
                                    statusCode = 500,
                                    message = "Không đủ nguyên liệu để xuất",
                                };
                            }

                    }
                    else
                    {
                        foreach (TonKho slhhc in soLuongCon)
                        {

                            ChiTietPhieuXuat ct = new ChiTietPhieuXuat();
                            ct.Idhh = int.Parse(t.Idhh);
                            ct.Idpx = phieuXuat.Idpx;
                            ct.Gia = double.Parse(t.Gia);
                            ct.Idctpn = slhhc.Idctpn;
                            ct.SoLuong = t.SoLuong == null ? double.Parse(t.ThucXuat) : double.Parse(t.SoLuong);
                            ct.ChenhLech = t.ChenhLech == "" ? 0 : double.Parse(t.ChenhLech);
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
                        if (data.TieuHuy == false) {
                            if (data.TuNgay != null && data.DenNgay != null)
                            {
                                await updateDSChiTietHoaDon(data.TuNgay, data.DenNgay);
                            } 
                        }
                    }

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
        public string taoMaHD()
        {
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyyMMdd");
            var phieuNhap = context.HoaDon.Where(x => x.MaHd.Contains(date)).ToList();
            return $"BL-{date}-{(phieuNhap.Count() + 1).ToString("D2")}";
        }
        [HttpPost("/XuatKho/ThemPhieuXuatHangHoa")]
        public async Task<dynamic> luuPhieuXuatHangHoa([FromBody] TTPhieuXuat data)
        {
            NhanVien nv = context.NhanVien.Find(Int32.Parse(User.Identity.Name));
            PhieuXuatMap ph = data.PhieuXuat;
            PhieuXuat phieuXuat = _mapper.Map<PhieuXuat>(ph);
            List<ChiTietPhieuXuatMap> chiTietPhieuXuatMaps = data.ChiTietPhieuXuat;
            List<PhieuXuat> phieuNhaps = context.PhieuXuat.ToList();

            List<TonKho> soLuongHhcon = await context.TonKho
                .Include(x => x.IdctpnNavigation)
                .OrderBy(x => x.IdctpnNavigation.Hsd).ToListAsync();
            var tran = context.Database.BeginTransaction();
            try
            {
                HoaDon hoaDon = new HoaDon();
                hoaDon.MaHd = taoMaHD();
                hoaDon.Tgxuat = DateTime.Now;
                hoaDon.TongTien = chiTietPhieuXuatMaps.Sum(x => double.Parse(x.Gia) * double.Parse(x.ThucXuat));
                hoaDon.TinhTrang = true;
                hoaDon.TinhTrangTt = true;
                hoaDon.Active = true;
                hoaDon.Idnv = nv.Idnnv;
                hoaDon.Idban = 14;
                context.HoaDon.Add(hoaDon);
                context.SaveChanges();

                phieuXuat.TieuHuy = false;
                phieuXuat.Idkh = hoaDon.Idhd;
                phieuXuat.Active = true;
                phieuXuat.Idnv = nv.Idnnv;
                phieuXuat.SoPx = taoSoPhieuXuat(phieuNhaps);
                phieuXuat.NgayTao = DateTime.Now;
                context.PhieuXuat.Add(phieuXuat);
                context.SaveChanges();

                foreach (ChiTietPhieuXuatMap t in chiTietPhieuXuatMaps.ToList())
                {
                    double slq = double.Parse(t.ThucXuat);
                    var soLuongCon = soLuongHhcon.Where(x => x.IdctpnNavigation.Idhh == int.Parse(t.Idhh)).ToList();
                    double a = (double)soLuongHhcon.Sum(x => x.SoLuong);
                    if (soLuongCon.Sum(x => x.SoLuong) < slq || soLuongCon.Count() == 0)
                    {
                        return new
                        {
                            statusCode = 500,
                            message = "Không đủ hàng hóa để xuất",
                        };
                    }
                    else
                    {
                        foreach (TonKho slhhc in soLuongCon)
                        {

                            ChiTietPhieuXuat ct = new ChiTietPhieuXuat();
                            ct.Idhh = int.Parse(t.Idhh);
                            ct.Idpx = phieuXuat.Idpx;
                            ct.Gia = soLuongCon.Max(x => x.IdctpnNavigation.Gia);
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

                }
                TaoChiTietHoaDon(phieuXuat.Idpx, hoaDon.Idhd);
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
        public double getGiaBan(int idHH)
        {
            HangHoa hangHoa = context.HangHoa.Find(idHH);
            return (double)hangHoa.GiaBan;
        }
        public void TaoChiTietHoaDon(int idPX, int idHD)
        {
            List<ChiTietPhieuXuat> chiTietPhieuXuats = context.ChiTietPhieuXuat
                .Include(x => x.IdhhNavigation)
                .Where(x => x.Idpx == idPX)
                .ToList();
            List<ChiTietHoaDon> chiTietHoaDons = new List<ChiTietHoaDon>();
            foreach (ChiTietPhieuXuat ct in chiTietPhieuXuats)
            {
                ChiTietHoaDon cthd = new ChiTietHoaDon();
                cthd.Idtd = ct.Idhh;
                cthd.Idhd = idHD;
                cthd.Sl = Convert.ToInt32(ct.SoLuong);
                cthd.DonGia = getGiaBan((int)ct.Idhh);
                cthd.ThanhTien = ct.IdhhNavigation.GiaBan * ct.SoLuong;
                cthd.DaXuat = true;
                cthd.HangHoa = true;
                cthd.Active = true;
                cthd.Tgorder = DateTime.Now;    
                cthd.Tgbep = DateTime.Now;
                cthd.TghoanThanh = DateTime.Now;    
                cthd.TgphucVu = DateTime.Now;

                chiTietHoaDons.Add(cthd);
            }
            context.ChiTietHoaDon.AddRange(chiTietHoaDons);
            context.SaveChanges();
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
        public async Task<dynamic> getDSXuatKho(string TuNgay, string DenNgay, string TieuHuy)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var listHH = new List<dynamic>();
            List<ChiTietHoaDon> chiTietHoaDons = await context.ChiTietHoaDon
                .Include(x => x.IdhdNavigation)
                .Where(x => x.IdhdNavigation.Tgxuat.Value.Date >= tuNgay.Date && x.IdhdNavigation.Tgxuat.Value.Date <= denNgay.Date && x.DaXuat != true && x.IdtdNavigation.Idnta != 3 && x.HangHoa != true).ToListAsync();
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
                        donGia = getGiaTrungBinh((int)dm.Idhh),
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
                    soLuong = Math.Round((float)x.Sum(x => (float)x.soLuong), 3),
                    tonKho = getSLTon((int)x.Key),
                    donGia = Math.Round(x.First().donGia, 3),
                });
            // nếu là tiêu hủy hàng hoá
            if (TieuHuy == "true") {
              var hangHoasHuy = context.TonKho
                    .Include(x => x.IdctpnNavigation)
                    .ThenInclude(x => x.IdhhNavigation)
                    .ThenInclude(x => x.IddvtNavigation)
                    .Where(x => x.IdctpnNavigation.Hsd.Value.Date <= DateTime.Now.Date)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        idhh = x.IdctpnNavigation.Idhh,
                        tenHangHoa = x.IdctpnNavigation.IdhhNavigation.TenHh,
                        donViTinh = x.IdctpnNavigation.IdhhNavigation.IddvtNavigation.TenDvt,
                        soLuong = Math.Round((float)x.SoLuong, 3),
                        tonKho = getSLTon((int)x.IdctpnNavigation.Idhh),
                        donGia = Math.Round((double)x.IdctpnNavigation.Gia, 3),
                    });
                return hangHoasHuy;
            }
            return hangHoas;
        }
        public double getGiaTrungBinh(int idHH)
        {
            var tonKhos = context.TonKho
                .Include(x => x.IdctpnNavigation)
                .ThenInclude(x => x.IdpnNavigation)
                .Where(x => x.IdctpnNavigation.Idhh == idHH)
                .ToList();
            double tongSL = 0;
            double tongThanhTien = 0;
            foreach (TonKho tk in tonKhos)
            {
                tongSL += (double)tk.SoLuong;
                tongThanhTien += (double)(tk.SoLuong * tk.IdctpnNavigation.Gia);
            }
            return Math.Round((Math.Round(tongThanhTien)/Math.Round(tongSL,3)));
        }
        [HttpGet("/XuatKho/XuatKhoNguyenLieu")]
        public IActionResult ViewXuatNguyenLieu()
        {
            return View("PhieuXuatNguyenLieu");
        }
        [HttpPost("/XuatKho/LichSuXuat")]
        public async Task<dynamic> LichSuXuat(string TuNgay, string DenNgay, string maPhieu, bool TieuHuy, int idHH)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var phieuXuats = await context.PhieuXuat
                .Include(x => x.ChiTietPhieuXuat)
                .Where(x => (x.NgayTao.Value.Date >= tuNgay.Date && x.NgayTao.Value.Date <= denNgay.Date)
                && (maPhieu == "" || maPhieu == null || x.SoPx == maPhieu)
                && (TieuHuy == false || x.TieuHuy == TieuHuy)
                && (idHH == 0 || x.ChiTietPhieuXuat.Any(ct => ct.IdctpnNavigation.Idhh == idHH)))
             .Select(x => new
             {
                 SoPx = x.SoPx,
                 NgayTao = x.NgayTao,
                 IdnvNavigation = x.IdnvNavigation,
                 GhiChu = x.GhiChu,
                 TongTien = XuatKhoController.getTongTien(x.ChiTietPhieuXuat.ToList()),
                 TongChenhLech = x.ChiTietPhieuXuat
                    .Sum(x => x.ChenhLech),

                 SoLuongHH = x.ChiTietPhieuXuat.Count(),
                 ChiTietPhieuXuat = x.ChiTietPhieuXuat.Select(x => new
                 {
                     IdhhNavigation = new HangHoa()
                     {
                         MaHh = x.IdhhNavigation.MaHh,
                         TenHh = x.IdhhNavigation.TenHh,
                     },
                     SoLuong = Math.Round((float) x.SoLuong,3),
                     ChenhLech = Math.Round((float)x.ChenhLech,3),
                     ThucXuat = Math.Round((float)x.ThucXuat, 3),
                     Gia = Math.Round((float)x.Gia,3),
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
        public class LSXuatKho
        {
            public string SoPx { get; set; }
            public DateTime? NgayTao { get; set; }
            public string GhiChu { get; set; }
            public double TongTien { get; set; }
            public int SoLuongHH { get; set; }

        }
        [HttpPost("/download/BaoCaoLSXuatKho")]
        public IActionResult downloadBaoCaoLSXuatKho(string fromDay, string toDay, string soPhieuLS, bool TieuHuy)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var url = Url.Action("viewBaoCaoLSXuatKhoPDF", "XuatKho", new { TuNgay = fromDay, DenNgay = toDay, maPhieu = soPhieuLS, TieuHuy = TieuHuy });

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + url;

            var pdf = fullView.ConvertUrl(currentUrl);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "BaoCaoLSXuatKho.pdf");
        }
        public IActionResult viewBaoCaoLSXuatKhoPDF(string TuNgay, string DenNgay, string maPhieu, bool TieuHuy)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            List<LSXuatKho> phieuXuats = context.PhieuXuat
                .Where(x => (x.NgayTao.Value.Date >= tuNgay.Date && x.NgayTao.Value.Date <= denNgay.Date)
                && (maPhieu == "" || maPhieu == null || x.SoPx == maPhieu)
                && (TieuHuy == false || x.TieuHuy == TieuHuy))
             .Select(x => new LSXuatKho
             {
                 SoPx = x.SoPx,
                 NgayTao = x.NgayTao,
                 GhiChu = x.GhiChu,
                 TongTien = XuatKhoController.getTongTien(x.ChiTietPhieuXuat.ToList()),
                 SoLuongHH = x.ChiTietPhieuXuat.Count(),
             })
            .ToList();
            ViewBag.tuNgay = tuNgay.Date;
            ViewBag.denNgay = denNgay.Date;  
            ViewBag.PhieuXuat = phieuXuats;
            return View("BaoCaoLSXuatKhoPDF");

        }
        public static double getTongTien(List<ChiTietPhieuXuat> chiTietPhieuXuats)
        {
            double tongTien = 0;
            foreach (ChiTietPhieuXuat ct in chiTietPhieuXuats)
            {
                tongTien += (float)(Math.Round((float)ct.ThucXuat,3) * Math.Round((float)ct.Gia,3));
            }
            return (double)Math.Round(tongTien,3);
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
        public bool TieuHuy { get; set; }
    }
}
