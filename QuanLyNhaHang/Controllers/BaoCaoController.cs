using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using SelectPdf;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using QuanLyNhaHang.Services;
using static QuanLyNhaHang.Controllers.XuatKhoController;

namespace QuanLyNhaHang.Controllers
{
    public class BaoCaoController : Controller
    {
        QuanLyNhaHangContext context = new QuanLyNhaHangContext();
        [HttpGet]
        public IActionResult BaoCao() 
        {
            //return View("BaoCaoLoiNhuan");
            return View();
        }
        [HttpPost("/doanhThuTheoNgay")]
        public async Task<dynamic> doanhThuTheoNgay(string fromDay, string toDay)
        {
            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var data = await context.HoaDon
              .Where(x => x.Tgxuat != null && (x.Tgxuat.Value.Date >= FromDay && x.Tgxuat.Value.Date <= ToDay))
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToListAsync();
            return data;
        }
        [HttpPost("/doanhThuTheoThang")]
        public async Task<dynamic> doanhThuTheoThang(string fromDay, string toDay)
        {
                        DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var data = await context.HoaDon
              .Where(x => x.Tgxuat != null && (x.Tgxuat.Value.Date >= FromDay.Date && x.Tgxuat.Value.Date <= ToDay.Date))
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Month)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToListAsync();
            return data;
        }
        [HttpGet("/QuanLy/ViewBaoCaoLoiNhuan")]
        public IActionResult ViewBaoCaoLoiNhuan()
        {
            return View("BaoCaoLoiNhuan");
        }
        [HttpPost("/baoCaoLoiNhuan")]
        public async Task<dynamic> baoCaoLoiNhuan(string TuNgay, string DenNgay)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var doanhThu = await context.HoaDon
              .Where(x => x.Tgxuat.Value.Date >= tuNgay.Date && x.Tgxuat.Value.Date <= denNgay.Date)
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToListAsync();
            var giaVon = context.PhieuXuat
              .Include(x => x.ChiTietPhieuXuat)
              .Where(x => x.NgayTao.Value.Date >= tuNgay.Date && x.NgayTao.Value.Date <= denNgay.Date)
              .ToList()
              .OrderBy(x => x.NgayTao)
              .GroupBy(x => x.NgayTao.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = tongPhieuNhap(x.ToList()),
              })
              .ToList();
            var listLoiNhuan = new List<dynamic>();
            foreach (var d in doanhThu)
            {
                var data = new
                {
                    Ngay = d.label,
                    DoanhThu = (double)d.doanhthu,
                    GiaVon = (double)0,
                };
                listLoiNhuan.Add (data);
            }
            foreach (var d in giaVon)
            {
                var data = new
                {
                    Ngay = d.label,
                    DoanhThu = (double)0,
                    GiaVon = (double)d.doanhthu,
                };
                listLoiNhuan.Add(data);
            }
            List<LoiNhuan> newlist = listLoiNhuan.GroupBy(x => x.Ngay)
            .Select(x => new LoiNhuan()
            {
                Ngay = x.Key,
                DoanhThu = (double)x.Sum(item => (double)item.DoanhThu),
                GiaVon = (double)x.Sum(item => (double)item.GiaVon)
            }).ToList();
            return new
            {
                doanhThu = doanhThu,
                giaVon = giaVon,
                listLoiNhuan = newlist,
            };
        }
        [HttpPost("/download/BaoCaoLoiNhuan")]
        public IActionResult downloadBaoCaoLoiNhuan(string tuNgay, string denNgay)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var url = Url.Action("viewBaoCaoLoiNhuanPDF", "BaoCao", new { TuNgay = tuNgay, DenNgay = denNgay });

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + url;

            var pdf = fullView.ConvertUrl(currentUrl);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "BaoCaoLoiNhuan.pdf");
        }
        public IActionResult viewBaoCaoLoiNhuanPDF(string TuNgay, string DenNgay)
        {
            DateTime tuNgay = DateTime.ParseExact(TuNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime denNgay = DateTime.ParseExact(DenNgay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var doanhThu = context.HoaDon
              .Where(x => x.Tgxuat.Value.Date >= tuNgay.Date && x.Tgxuat.Value.Date <= denNgay.Date)
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToList();
            var giaVon = context.PhieuXuat
              .Include(x => x.ChiTietPhieuXuat)
              .Where(x => x.NgayTao.Value.Date >= tuNgay.Date && x.NgayTao.Value.Date <= denNgay.Date)
              .ToList()
              .OrderBy(x => x.NgayTao)
              .GroupBy(x => x.NgayTao.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = tongPhieuNhap(x.ToList()),
              })
              .ToList();
            var listLoiNhuan = new List<LoiNhuan>();
            foreach (var d in doanhThu)
            {
                var data = new LoiNhuan
                {
                    Ngay = d.label,
                    DoanhThu = (double)d.doanhthu,
                    GiaVon = (double)0,
                };
                listLoiNhuan.Add(data);
            }
            foreach (var d in giaVon)
            {
                var data = new LoiNhuan
                {
                    Ngay = d.label,
                    DoanhThu = (double)0,
                    GiaVon = (double)d.doanhthu,
                };
                listLoiNhuan.Add(data);
            }
            List<LoiNhuan> newlist = listLoiNhuan.GroupBy(x => x.Ngay)
            .Select(x => new LoiNhuan()
            {
                Ngay = x.Key,
                DoanhThu = (double)x.Sum(item => (double)item.DoanhThu),
                GiaVon = (double)x.Sum(item => (double)item.GiaVon)
            }).ToList();
            ViewBag.tuNgay = tuNgay.Date;
            ViewBag.denNgay = denNgay.Date;
            //ViewBag.LoiNhuan = listLoiNhuan.GroupBy(x => x.Ngay).Select(x => new LoiNhuan()
            //{
            //    Ngay = x.Key,
            //    DoanhThu = x.First().DoanhThu,
            //    GiaVon = x.Where(x => x.GiaVon != 0).First().GiaVon,
            //});
            ViewBag.LoiNhuan = newlist;
            return View("BaoCaoLoiNhuanPDF");

        }
        public class LoiNhuan
        {
            public DateTime? Ngay { get; set; }
            public double? DoanhThu { get; set; }
            public double? GiaVon { get; set; }
        }
        public double tongPhieuNhap(List<PhieuXuat> phieuNhaps)
        {
            double tong = (double)phieuNhaps.Sum(x => x.ChiTietPhieuXuat.Sum(ct => ct.Gia * ct.ThucXuat));
            return tong;
        }
        public double tongChiTietPhieuNhap(List<ChiTietPhieuNhap> chiTietPhieuNhaps)
        {
            double tong = (double)chiTietPhieuNhaps.Sum(ct => ct.Gia);
            return tong;
        }
        //--------------------------------------------------- báo cáo chênh lệch
        [HttpGet("/BaoCao/ViewBaoCaoChenhLech")]
        public IActionResult BaoCaoChenhLech()
        {
            return View();
        }
        [HttpPost("/BaoCao/BaoCaoChenhLech")]
        public async Task<dynamic> dataBaoCaoChenhLech(string fromDay, string toDay)
        {
            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            List<ChiTietPhieuXuat> chiTietPhieuXuats = context.ChiTietPhieuXuat
                .Include(x => x.IdpxNavigation)
                .Where(x => x.IdpxNavigation.NgayTao.Value.Date >= FromDay && x.IdpxNavigation.NgayTao.Value.Date <= ToDay && x.ChenhLech != 0)
                .ToList();
            List<ChenhLech> chenhLech = chiTietPhieuXuats.GroupBy(x => x.Idhh)
                .Select(x => new ChenhLech()
                {
                    Idhh = x.Key,
                    label = CommonServices.GetTenHH((int) x.Key, context.HangHoa.Where(x => x.Active == true).ToList()),
                    soLuong = getChenhLech(x.ToList()),
                    donViTinh = getDonViTinh((int)x.Key),
                }).ToList();
            var hangHoas = context.ThucDon
                .Include(x => x.DinhMuc)
                .ThenInclude(x => x.IdhhNavigation)
                .ThenInclude(x => x.IddvtNavigation)
                .Where(x => x.Active == true)
                .Select(x => new
                {
                    DinhMuc = x.DinhMuc.Select(dm => new 
                    {
                        Idhh = dm.Idhh,
                        MaHh = dm.IdhhNavigation.MaHh,
                        TenHh = dm.IdhhNavigation.TenHh,
                        dvt = dm.IdhhNavigation.IddvtNavigation.TenDvt,
                        SoLuong = dm.SoLuong,
                        ChenhLech = BaoCaoController.getSLChenhLech(chenhLech, (int)dm.Idhh),
                    }).ToList(),
                    Ten = x.Ten,
                    MaTd = x.MaTd
                }).ToList();
            var hangs = new List<dynamic>();
            foreach (var h in hangHoas)
            {
                if (h.DinhMuc.Any(x => chenhLech.Any(cl => cl.Idhh == x.Idhh)))
                {
                    hangs.Add(h);
                }
            }
            return new {
                dataDoThi = chenhLech,
                dataTable = hangs
            };

        }
        public class ChenhLech
        {
            public int? Idhh { get; set; }   
            public string label { get; set; }
            public double soLuong { get; set; }
            public string donViTinh { get; set; }
        }
        public static double getSLChenhLech(List<ChenhLech> chenhLech, int idHH)
        {
            double sl = 0;
            foreach (ChenhLech cl in chenhLech)
            {
                if (cl.Idhh == idHH)
                {
                    sl = cl.soLuong;
                }
            }
            return sl;
        }
        public double getChenhLech(List<ChiTietPhieuXuat> chiTietPhieuXuats)
        {
            List<ChiTietPhieuXuat> chiTiets = chiTietPhieuXuats
                .GroupBy(chiTiet => chiTiet.Idpx) // Nhóm theo Idpx
                .Select(group => group.First())
                .ToList();

            return Math.Round((double)chiTiets.Sum(x => x.ChenhLech), 3);
        }
        public string getDonViTinh(int idHH)
        {
            HangHoa dvt = context.HangHoa
                .Include(x => x.IddvtNavigation)
                .FirstOrDefault(x => x.Idhh == idHH);
            return dvt.IddvtNavigation.TenDvt;
        }
        //--------------------------------------------------- báo cáo chỉ tiêu
        [HttpPost("/BaoCao/BaoCaoChiTieu")]
        public async Task<dynamic> BaoCaoChiTieu(string fromDay, string toDay)
        {
            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var data = await context.HoaDon
              .Where(x => x.Tgxuat != null && (x.Tgxuat.Value.Date >= FromDay && x.Tgxuat.Value.Date <= ToDay))
              .OrderBy(x => x.Tgxuat)
              .GroupBy(x => x.Tgxuat.Value.Date)
              .Select(x => new
              {
                  label = x.Key,
                  doanhthu = x.Sum(x => x.TongTien)
              })
              .ToListAsync();
            var chiTietHoaDons = await context.ChiTietHoaDon
                .Include(x => x.IdhdNavigation)
                .ThenInclude(x => x.IdbanNavigation)
                .Where(x => x.IdhdNavigation.Tgxuat.Value.Date >= FromDay.Date && x.IdhdNavigation.Tgxuat.Value.Date <= ToDay.Date)
                .ToListAsync();
            var thucDons = await context.ThucDon
                .Include(x => x.IdntaNavigation)
                .Where(x => x.Active == true)
                .Select(x => new
                {
                    label = x.Ten,
                    soLuong = BaoCaoController.tinhSLThucDon(x.Idtd, chiTietHoaDons),
                })
                .ToListAsync();
            var hangHoas = await context.HangHoa
                .Include(x => x.IdnhhNavigation)
                .Where(x => x.Active == true && x.IdnhhNavigation.HangHoa == true)
                .Select(x => new
                {
                    label = x.TenHh,
                    soLuong = BaoCaoController.tinhSLHangHoa(x.Idhh, chiTietHoaDons),
                }).ToListAsync();
            thucDons = thucDons.Concat(hangHoas).ToList();
            var khus = await context.Khu
                .Where(x => x.Active == true)
                .Select(x => new
                {
                    label = x.TenKhu,
                    soLuong =BaoCaoController.tinhDoanhThuKhu(x.Idkhu, chiTietHoaDons),
                })
                .ToListAsync();
            return new
            {
                doThiThucDon = thucDons.Where(x => x.soLuong != 0).OrderByDescending(x => x.soLuong),
                doThiKhu = khus.Where(x => x.soLuong != 0).OrderByDescending(x => x.soLuong)
,
            };
        }
        public static int tinhSLThucDon(int idtd, List<ChiTietHoaDon> chiTietHoaDons)
        {
            int soluong = 0;
            foreach (ChiTietHoaDon c in chiTietHoaDons)
            {
                if (c.Idtd == idtd && c.HangHoa != true)
                {
                    soluong += (int)c.Sl;
                }
            }
            return soluong;
        }
        public static int tinhSLHangHoa(int idtd, List<ChiTietHoaDon> chiTietHoaDons)
        {
            int soluong = 0;
            foreach (ChiTietHoaDon c in chiTietHoaDons)
            {
                if (c.Idtd == idtd && c.HangHoa == true)
                {
                    soluong += (int)c.Sl;
                }
            }
            return soluong;
        }
        public static double tinhDoanhThuKhu(int idKhu, List<ChiTietHoaDon> chiTietHoaDons)
        {
            double doanhThu = 0;
            foreach (ChiTietHoaDon c in chiTietHoaDons)
            {
                if(c.IdhdNavigation.IdbanNavigation.Idkhu == idKhu)
                {
                    doanhThu += (double)c.ThanhTien;
                }
            }
            return doanhThu;
        }
        // -------------------------------------------------- đánh giá hiệu xuất nhân viên
        [HttpGet("/BaoCao/HieuSuatNhanVien")]
        public  IActionResult ViewHieuSuatNhanVien()
        {
            return View("HieuSuatNhanVien");
        }
        [HttpPost("/BaoCao/getDuLieuHieuSuatNhanVien")]
        public async Task<dynamic> getDuLieuHieuSuatNhanVien(string fromDay, string toDay)
        {
            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            //var data = await context.NhanVien
            //    .Include(x => x.HoaDon)
            //    .ThenInclude(x => x.ChiTietHoaDon)
            //    .Include(x => x.IdnnvNavigation.QlDanhGia)
            //    .Where(x => x.HoaDon.Any(hd => hd.Tgxuat != null && (hd.Tgxuat.Value.Date >= FromDay && hd.Tgxuat.Value.Date <= ToDay)
            //                            ))
            //    .ToListAsync(); 




            //var data = await context.ChiTietHoaDon
            //    .AsQueryable()
            //    .Include(x => x.IdhdNavigation.IdbanNavigation)
            //    .Include(x => x.IdcaNavigation.LichLamViec)
            //    .ThenInclude(x => x.IdnvNavigation)
            //    .Where(x => x.IdcaNavigation.LichLamViec.Any())
            //    .ToListAsync();
            //data = data.GroupBy(x => new { x.Idca, x.IdhdNavigation.IdbanNavigation.Idkhu })
            //    .Select(group => group.First())
            //    .ToList();
            //data = data.GroupBy(x => x.IdcaNavigation.LichLamViec.FirstOrDefault().Idnv)
            //    .Select(group => group.First())
            //    .ToList();
            //var data1 = data.Select(x => new
            //{
            //    TenNhanVien = x.IdcaNavigation.LichLamViec.FirstOrDefault().IdnvNavigation.Ten,
            //    Diem = (x.TgphucVu - x.TghoanThanh)
            //})
            //.ToList();


            var data = await context.NhanVien
                .Include(x => x.LichLamViec)
                .ThenInclude(x => x.IdcaNavigation)
                .ThenInclude(x => x.ChiTietHoaDon)
                .ThenInclude(x => x.IdhdNavigation)
                .ThenInclude(x => x.IdbanNavigation)
                .Include(x => x.IdtkNavigation)
                .Where(x => x.IdtkNavigation.Idvt == 1 && x.LichLamViec.Any(llv => llv.IdcaNavigation.ChiTietHoaDon.Any(ct => ct.IdhdNavigation.Tgxuat.Value.Date >= FromDay && ct.IdhdNavigation.Tgxuat.Value.Date <= ToDay)))
                .Select(x => new NhanVien()
                {
                    Ten = x.Ten,
                    Idnv = x.Idnv,
                    LichLamViec = x.LichLamViec
                    .Where(llv => llv.IdcaNavigation.ChiTietHoaDon.Count() >0)
                    .Select(llv => new LichLamViec()
                    {
                        Idkhu = llv.Idkhu,
                        Idnv = llv.Idnv,    
                        IdcaNavigation = new Ca()
                        {
                            Idca = llv.IdcaNavigation.Idca,
                            ChiTietHoaDon = llv.IdcaNavigation.ChiTietHoaDon.Select(ct => new ChiTietHoaDon()
                            {
                                TgphucVu = ct.TgphucVu,
                                TghoanThanh = ct.TghoanThanh,   
                                IdhdNavigation = new HoaDon()
                                {
                                    Tgxuat = ct.IdhdNavigation.Tgxuat,
                                    IdbanNavigation = ct.IdhdNavigation.IdbanNavigation,
                                },
                                
                            })
                            .Where(ct => ct.IdhdNavigation.Tgxuat.Value.Date >= FromDay && ct.IdhdNavigation.Tgxuat.Value.Date <= ToDay && ct.IdhdNavigation.IdbanNavigation.Idkhu == llv.Idkhu)
                            .ToList(),
                        }
                    })
                    .ToList(),
                })
                .ToListAsync();

            var data1 = data.Select(x => new
            {
                Ten = x.Ten,
                Diem = tinhDiemLamViecTheoNhanVien(x),
            })
            .ToList();
            return data1.Where(x => !double.IsNaN(x.Diem)).ToList();


        }
        public float tinhDiemLamViecTheoNhanVien(NhanVien nhanViens)
        {
            return tinhDiemHieuSuatTheoLichLamViecs(nhanViens.LichLamViec.Where(llv => llv.IdcaNavigation.ChiTietHoaDon.Count() > 0).ToList());
        }
        public float tinhHieuSuatNhanVien(DateTime FromDay, DateTime ToDay, int idnv, List<LichLamViec> lichLamViecs)
        {
            var khus = lichLamViecs.GroupBy(x => x.Idkhu)
                .Select(x => x.Key);

            var chiTietHoaDon = context.ChiTietHoaDon
                .Include(x => x.IdcaNavigation)
                .ThenInclude(x => x.LichLamViec)
                .Where(ct => ct.IdhdNavigation.Tgxuat.Value.Date >= FromDay && ct.IdhdNavigation.Tgxuat.Value.Date <= ToDay
                 && ct.IdcaNavigation.LichLamViec.Any(x => x.Idnv == idnv && khus.Contains(x.Idkhu))).ToList();
            return tinhDiemHieuSuatTheoChiTietHoaDon(chiTietHoaDon);
        }
        public float tinhDiemHieuSuatTheoLichLamViecs(List<LichLamViec> lichLamViecs)
        {
            float diem = 0;
            int i = 0;
            foreach (LichLamViec lich in lichLamViecs)
            {
                diem += tinhDiemHieuSuatTheoChiTietHoaDon(lich.IdcaNavigation.ChiTietHoaDon.Where(x => x.TgphucVu!=null).ToList());
                i++;
            }
            diem /=i;
            return diem;
        }
        public float tinhDiemHieuSuatTheoChiTietHoaDon(List<ChiTietHoaDon> chiTietHoaDons)
        {
            float diem= 0;
            int i = 0;
            foreach (ChiTietHoaDon chiTietHoaDon in chiTietHoaDons)
            {
                diem += tinhDiemHieuSuatChiTietHoaDon(chiTietHoaDon);
                i++;
            }
            if (diem != 0)
            {
                diem /= i;

            }
            return diem;
        }
        public int tinhDiemHieuSuatChiTietHoaDon(ChiTietHoaDon chiTietHoaDon)
        {
            DateTime? tgPhucVu = chiTietHoaDon?.TgphucVu;
            DateTime? tgHoanThanh = chiTietHoaDon?.TghoanThanh;
            int thoiGianHoanThanh = 0;

            int diem = 0;
            TimeSpan timeSpan = tgPhucVu.Value - tgHoanThanh.Value;

            thoiGianHoanThanh = (int)timeSpan.TotalSeconds;
            if (thoiGianHoanThanh == null)
            {
                thoiGianHoanThanh = 1;
            }
            QlDanhGia qlDanhGia = context.QlDanhGia.FirstOrDefault(x => x.ThoiGianTu <= thoiGianHoanThanh && x.ThoiGianDen >= thoiGianHoanThanh);
            if (qlDanhGia == null)
            {
                diem = 1;
            }
            else
            {
                diem = (int)qlDanhGia.Diem;
            }
            return diem;
        }
        [HttpGet("/BaoCao/BaoCaoChiTieu")]
        public IActionResult BaoCaoChiTieu()
        {
            return View();
        }
        [Route("/download/ChiTietHoaDon/{id:int}")]
        public IActionResult downloadPChiTietHoaDon(int id)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var pdf = fullView.ConvertUrl(currentUrl + "/ChiTietHoaDonPDF/" + id);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "ChiTietHoaDon.pdf");
        }

        [Route("/ChiTietHoaDonPDF/{id:int}")]
        public IActionResult viewPDF(int id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var phieu = context.HoaDon
                .Include(x => x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation)
                .Include(x => x.ChiTietHoaDon)
                .Where(x => x.Idhd == id).FirstOrDefault();
            return View("ChiTietHoaDonPDF", phieu);
        }
        [HttpPost("/download/BaoCaoHoaDon")]
        public IActionResult downloadBaoCaoHoaDon(string fromDay, string toDay, string MaHD, int IdSanh, int IdTieuKhu, int IdBan, int IdTD)
        {
            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1280;
            fullView.Options.PdfPageSize = PdfPageSize.A4;
            fullView.Options.MarginTop = 20;
            fullView.Options.MarginBottom = 20;
            fullView.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            var url = Url.Action("viewBaoCaoHoaDonPDF", "BaoCao", new { fromDay = fromDay, toDay = toDay, MaHD = MaHD, IdSanh = IdSanh, IdTieuKhu = IdTieuKhu, IdBan = IdBan, IdTD = IdTD });

            var currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}" + url;

            var pdf = fullView.ConvertUrl(currentUrl);

            var pdfBytes = pdf.Save();
            return File(pdfBytes, "application/pdf", "BaoCaoHoaDon.pdf");
        }
        public IActionResult viewBaoCaoHoaDonPDF(string fromDay, string toDay, string MaHD, int IdSanh, int IdTieuKhu, int IdBan, int IdTD)
        {
            DateTime FromDay = DateTime.ParseExact(fromDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDay = DateTime.ParseExact(toDay, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            List<HoaDon> listPhieu = context.HoaDon
            .Where(x => x.Tgxuat.Value.Date >= FromDay
            && x.Tgxuat.Value.Date <= ToDay
            && x.Active == true)
            .Include(x => x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation)
            .Include(x => x.IdnvNavigation)
            .Include(x => x.ChiTietHoaDon)
            .OrderByDescending(x => x.Idhd)
            .ToList();
            ViewBag.tungay = fromDay;
            ViewBag.denngay = toDay;
            if (IdSanh == 0 && IdTieuKhu == 0 && IdBan == 0 && IdTD == 0)
            {

                return View("BaoCaoHoaDonPDF", listPhieu.Where(x => (MaHD == null ? true : x.MaHd.Contains(MaHD.ToUpper()))).ToList());
            }
            else
            {
                return View("BaoCaoHoaDonPDF", listPhieu.Where(x => (IdTD == 0 ? true : (x.ChiTietHoaDon.Where(y => y.Idtd == IdTD).Count() > 0 ? true : false))
                && (IdSanh == 0 ? true : x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation.Idsanh == IdSanh)
                && (IdTieuKhu == 0 ? true : x.IdbanNavigation.IdkhuNavigation.Idkhu == IdTieuKhu)
                && (IdBan == 0 ? true : x.IdbanNavigation.Idban == IdBan)
                && (MaHD == null ? true : x.MaHd.Contains(MaHD.ToUpper()))).ToList());
            }

        }
    }
}
