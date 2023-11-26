using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Models.Mapping;
using QuanLyNhaHang.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            try
            {
                ChiTietHoaDon ct = context.ChiTietHoaDon.FirstOrDefault(x =>x.Idcthd == int.Parse(id));

                ct.TrangThaiOrder = true;
                context.ChiTietHoaDon.Update(ct);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }


            await Clients.All.SendAsync("ReceiveMessage", id);
        }

        public async Task SendMessageNB(string id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            //ChiTietHoaDon ct = context.ChiTietHoaDon.FirstOrDefault(x => x.Idcthd == int.Parse(id));
            ChiTietHoaDon ct = context.ChiTietHoaDon.Find(int.Parse(id));
            ct.Tgbep = DateTime.Now;
            context.ChiTietHoaDon.Update(ct);
            context.SaveChanges();
            await Clients.All.SendAsync("ReceiveMessageNB", id);
        }
        public async Task SaveCTHD(string ipmac, string tongtien, string idban)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            List<ChiTietHoaDonTam> cthdt = context.ChiTietHoaDonTam.Where(c => c.Ipmac == ipmac).ToList();
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyyMMdd");

            string orderCode = $"{now:yyyyMMdd}-{ipmac}";
            int count = context.HoaDon.Count(x => x.MaHd == orderCode);

            string mahd = $"{now:yyyyMMdd}-{ipmac}-{count + 1}";
            int count1 = context.HoaDon.Count(x => x.MaHd == mahd);
            string mahd1 = $"{now:yyyyMMdd}-{ipmac}-{count1 + 1}";
            var idb = context.Ban.FirstOrDefault(x => x.Ipmac == ipmac && x.Active == true).Idban;
            var TinhTrangHD = context.HoaDon.Where(x => x.TinhTrang == null && x.Idban == idb);
            var TinhTrangXNHD = context.HoaDon.Where(x=> x.TinhTrang == false && x.Idban == idb && x.TinhTrangTt == null);
            var TinhTrangXNHD1 = context.HoaDon.Where(x => x.TinhTrang == true && x.Idban == idb && x.TinhTrangTt == false);


            DayOfWeek dayOfWeek = now.DayOfWeek;
            string dayOfWeekString = dayOfWeek.ToString();
            var ca = context.Ca.FirstOrDefault(x => x.Thu == dayOfWeekString && now.TimeOfDay >= x.TgbatDau && now.TimeOfDay <= x.TgketThuc).Idca;
            if (TinhTrangXNHD.Count() > 0 || TinhTrangXNHD1.Count()>0)
            {
                var chitiethoadon = context.ChiTietHoaDon
                .Include(x => x.IdhdNavigation.IdbanNavigation)
                .Where(x => x.Tgbep == null)
                .Select(x => new
                {
                    x.Idcthd,
                    x.Idtd,
                    x.IdtdNavigation.Ten,
                    x.IdhdNavigation.IdbanNavigation.TenBan,
                    x.TrangThaiOrder,
                    x.Sl
                })
                .ToList();
                int tb = 0;
                await Clients.All.SendAsync("HienThiCTHD", chitiethoadon, ipmac, tongtien, idban,tb);
            }
            else
            {
                try
                {

                    if (TinhTrangHD.Count() > 0)
                    {
                        HoaDon hoaDon = context.HoaDon.FirstOrDefault(x => x.TinhTrang == null && x.Idban == idb);
                        hoaDon.TongTien = hoaDon.TongTien + float.Parse(tongtien);
                        context.HoaDon.Update(hoaDon);
                        context.SaveChanges();
                        int hdID = hoaDon.Idhd;
                        foreach (ChiTietHoaDonTam ctt in cthdt)
                        {
                            ChiTietHoaDon ct = new ChiTietHoaDon();
                            ct.Idhd = hdID;
                            ct.Tgorder = DateTime.Now;
                            ct.Idtd = ctt.Idtd;
                            ct.DonGia = ctt.DonGia;
                            ct.Sl = ctt.Sl;
                            ct.DonGia = ctt.DonGia;
                            ct.TyLeGiam = 0;
                            ct.HangHoa = ctt.HangHoa;
                            ct.Active = true;
                            ct.Idca = ca;
                            if (ctt.Sl == 1)
                            {
                                ct.ThanhTien = ctt.DonGia;
                            }
                            else
                            {
                                ct.ThanhTien = ctt.ThanhTien;
                            }
                            context.ChiTietHoaDon.Add(ct);
                            context.SaveChanges();
                            context.ChiTietHoaDonTam.Remove(ctt);
                            context.SaveChanges();


                        }

                    }
                    else
                    {
                        HoaDon hd = new HoaDon();
                        hd.MaHd = mahd1;
                        hd.Idban = int.Parse(idban);

                        hd.TongTien = float.Parse(tongtien);
                        hd.Active = true;
                        context.HoaDon.Add(hd);
                        context.SaveChanges();
                        // Lưu lại Idhd mới được tạo ra
                        int hdId = hd.Idhd;
                        foreach (ChiTietHoaDonTam ctt in cthdt)
                        {
                            ChiTietHoaDon ct = new ChiTietHoaDon();
                            ct.Idhd = hdId;
                            ct.Tgorder = DateTime.Now;
                            ct.Idtd = ctt.Idtd;
                            ct.DonGia = ctt.DonGia;
                            ct.Sl = ctt.Sl;
                            ct.TyLeGiam = 0;
                            ct.HangHoa = ctt.HangHoa;
                            ct.DonGia = ctt.DonGia;
                            ct.Active = true;
                            ct.Idca = ca;

                            if (ctt.Sl == 1)
                            {
                                ct.ThanhTien = ctt.DonGia;
                            }
                            else
                            {
                                ct.ThanhTien = ctt.ThanhTien;
                            }
                            context.ChiTietHoaDon.Add(ct);
                            context.SaveChanges();
                            context.ChiTietHoaDonTam.Remove(ctt);
                            context.SaveChanges();


                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }

                var chitiethoadon = context.ChiTietHoaDon
                .Include(x => x.IdhdNavigation.IdbanNavigation)
                .Where(x => x.Tgbep == null)
                .Select(x => new
                {
                    x.Idcthd,
                    x.Idtd,
                    x.IdtdNavigation.Ten,
                    x.IdhdNavigation.IdbanNavigation.TenBan,
                    x.TrangThaiOrder,
                    x.Sl
                })
                .ToList();
                int tb = 1;
                await Clients.All.SendAsync("HienThiCTHD", chitiethoadon, ipmac, tongtien, idban,tb);
            }  
        }

        public async Task SendPhucVu(string id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            try
            {
                ChiTietHoaDon ct = context.ChiTietHoaDon.FirstOrDefault(x => x.Idcthd == int.Parse(id));
                ct.TghoanThanh = DateTime.Now;
                context.ChiTietHoaDon.Update(ct);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            var listCTHDPV = context.ChiTietHoaDon.Include(x => x.IdhdNavigation.IdbanNavigation)
                    .Where(x => x.TgphucVu == null && x.TghoanThanh != null)
                    .Select(x => new {
                        x.Idcthd,
                        x.Idtd,
                        x.IdtdNavigation.Ten,
                        x.IdhdNavigation.IdbanNavigation.TenBan,
                        x.TrangThaiOrder,
                        x.IdhdNavigation.IdbanNavigation.Idkhu,
                        x.Sl
                    }).ToList();

            await Clients.All.SendAsync("ReceviPhucVu", listCTHDPV);
        }

        public async Task DeleteCTHD(string idcthd)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            


            try
            {
                ChiTietHoaDon cthd = context.ChiTietHoaDon.FirstOrDefault(x => x.Idcthd == int.Parse(idcthd));
                HoaDon hd = context.HoaDon.FirstOrDefault(x => x.Idhd == cthd.Idhd);
                hd.TongTien = hd.TongTien - cthd.ThanhTien;
                context.HoaDon.Update(hd);
                context.SaveChanges();
                context.ChiTietHoaDon.Remove(cthd);
                context.SaveChanges();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            var chitiethoadon = context.ChiTietHoaDon
            .Include(x => x.IdhdNavigation.IdbanNavigation)
            .Where(x => x.Tgbep == null)
            .Select(x => new {
                x.Idcthd,
                x.Idtd,
                x.IdtdNavigation.Ten,
                x.IdhdNavigation.IdbanNavigation.TenBan,
                x.TrangThaiOrder
            })
            .ToList();

            await Clients.All.SendAsync("HienThiCTHD1", chitiethoadon);
        }
        public async Task SendHD(string IDHD)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var tinhtranghoadon = context.ChiTietHoaDon.FirstOrDefault(x => x.Idhd == int.Parse(IDHD) && x.TghoanThanh == null);
            using var tran = context.Database.BeginTransaction();
            try
            {
                    HoaDon hd = context.HoaDon.FirstOrDefault(x => x.Idhd == int.Parse(IDHD));
                    List<HangHoa> hangHoas = context.HangHoa.Where(x => x.Active == true).ToList();
                    List<ThucDon> thucDons = context.ThucDon.Where(x => x.Active == true).ToList();
                    hd.TinhTrang = false;
                    context.HoaDon.Update(hd);
                    context.SaveChanges();
                    var HD = context.HoaDon
                    .Where(x => x.TinhTrang == false && x.TinhTrangTt == null && x.Tgxuat == null && x.Idhd == int.Parse(IDHD))
                    .Select(x => new
                    {
                        x.Idhd,
                        x.IdbanNavigation.TenBan,
                        x.IdbanNavigation.IdkhuNavigation.TenKhu,
                        x.IdbanNavigation.IdkhuNavigation.IdsanhNavigation.TenSanh,
                        x.TongTien,
                        ChiTietHoaDon = x.ChiTietHoaDon.Select(ct => new
                        {
                            IdCTHD = ct.Idcthd,
                            Ten = ct.HangHoa == true ? CommonServices.GetTenHH((int)ct.Idtd, hangHoas) : CommonServices.GetTenTD((int)ct.Idtd, thucDons),
                            SoLuong = ct.Sl,
                            DonGia = ct.DonGia,
                            ThanhTien = ct.Sl * ct.DonGia,
                        }).ToList()
                        //ChiTietHoaDon = x.ChiTietHoaDon.Select(y => new
                        //{
                        //    TenHH = GetTenHH((int)y.Idtd),
                        //    TenTD = GetTenTD((int)y.Idtd),
                        //    SoLuong = y.Sl,
                        //    DonGia = y.DonGia,
                        //}).ToList(),
                    })
                    .ToList();
                
                //int tt = tinhtranghoadon != null ? 0 : 1;
                int tt = 1;

                var ipmac = context.HoaDon.Include(x => x.IdbanNavigation).FirstOrDefault(x => x.Idhd == int.Parse(IDHD)).IdbanNavigation.Ipmac;
                var item = HD.First();
                tran.Commit();
                await Clients.All.SendAsync("GiveHD", item, tt, ipmac);

            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task SendHHD(string IDHD)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var tinhtranghoadon = context.ChiTietHoaDon.FirstOrDefault(x => x.Idhd == int.Parse(IDHD) && x.TghoanThanh == null);
            var ipmac = context.HoaDon.Include(x => x.IdbanNavigation).FirstOrDefault(x => x.Idhd == int.Parse(IDHD)).IdbanNavigation.Ipmac;
            using var tran = context.Database.BeginTransaction();
            try
            {
                HoaDon hd = context.HoaDon.FirstOrDefault(x => x.Idhd == int.Parse(IDHD));
                hd.TinhTrang = null;
                hd.Tgxuat = null;
                hd.TinhTrangTt = null;
                context.HoaDon.Update(hd);
                context.SaveChanges();
                tran.Commit();
                var data = new
                {
                    ipmac = ipmac,
                    idHD = int.Parse(IDHD),
                };
                await Clients.All.SendAsync("GiveHHD", data);

            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task SendXNTT(string id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            //ChiTietHoaDon ct = context.ChiTietHoaDon.FirstOrDefault(x => x.Idcthd == int.Parse(id));
            HoaDon ct = context.HoaDon.Find(int.Parse(id));
            ct.TinhTrang = true;
            ct.TinhTrangTt = false;
            context.HoaDon.Update(ct);
            context.SaveChanges();
            var ipmac = context.HoaDon.Include(x => x.IdbanNavigation).FirstOrDefault(x => x.Idhd == int.Parse(id)).IdbanNavigation.Ipmac;
            await Clients.All.SendAsync("NhanXNTT", ipmac);
        }
        public async Task SendHuyHDXNNT(string id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            //ChiTietHoaDon ct = context.ChiTietHoaDon.FirstOrDefault(x => x.Idcthd == int.Parse(id));
            var ipmac = context.HoaDon.Include(x => x.IdbanNavigation).FirstOrDefault(x => x.Idhd == int.Parse(id)).IdbanNavigation.Ipmac;
            await Clients.All.SendAsync("HuyHDXNNT", ipmac);
        }
        public async Task SendXNNT(string id)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            //ChiTietHoaDon ct = context.ChiTietHoaDon.FirstOrDefault(x => x.Idcthd == int.Parse(id));
            HoaDon ct = context.HoaDon.Find(int.Parse(id));
            ct.TinhTrangTt = true;  
            context.HoaDon.Update(ct);
            context.SaveChanges();
            List<ChiTietHoaDon> chiTietHoaDons = context.ChiTietHoaDon
                .Include(x => x.IdtdNavigation)
                .ThenInclude(x => x.DinhMuc)
                .ThenInclude(x => x.IdhhNavigation)
                .ThenInclude(x => x.IdnhhNavigation)
                .Include(x => x.IdhdNavigation)
                .ThenInclude(x => x.IdbanNavigation)
                .ThenInclude(x => x.IdkhuNavigation)
                .ThenInclude(x => x.IdsanhNavigation)
                .Where(x => x.Idhd == int.Parse(id)).ToList();
            List<ChiTietPhieuXuatMap> chiTietPhieuXuats = new List<ChiTietPhieuXuatMap>();
            foreach (ChiTietHoaDon chiTiet in chiTietHoaDons)
            {
                
                if (chiTiet.HangHoa == true)
                {
                        ChiTietPhieuXuatMap px = new ChiTietPhieuXuatMap();
                        px.Idhh = chiTiet.Idtd.ToString();
                        px.SoLuong = chiTiet.Sl.ToString();
                        px.ThucXuat = chiTiet.Sl.ToString();
                        px.ChenhLech = "0";

                    px.Gia = chiTiet.DonGia.ToString();
                        chiTietPhieuXuats.Add(px);
                }
            }
            if (chiTietPhieuXuats.Count() > 0)
            {
                await themPhieuNhap(chiTietPhieuXuats, ct.Idhd);
            }
            var ipmac = context.HoaDon.Include(x => x.IdbanNavigation).FirstOrDefault(x => x.Idhd == int.Parse(id)).IdbanNavigation.Ipmac;
            await Clients.All.SendAsync("NhanXNNT", ipmac);
        }
        public double getGiaHangHoa(int idHH)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            var tonKho = context.TonKho
                .Include(x => x.IdctpnNavigation)
                .Where(x => x.IdctpnNavigation.Idhh == idHH)
                .ToList();
            return (double)tonKho.Max(x => x.IdctpnNavigation.Gia);

        }
        public async Task themPhieuNhap(List<ChiTietPhieuXuatMap> chiTietPhieuXuatMaps, int idhd)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            List<TonKho> soLuongHhcon = await context.TonKho
                .Include(x => x.IdctpnNavigation)
                .OrderBy(x => x.NgayNhap).ToListAsync();
            var tran = context.Database.BeginTransaction();
            try
            {
                PhieuXuat phieuXuat = new PhieuXuat();
                phieuXuat.Active = true;
                phieuXuat.Idnv = 1;
                phieuXuat.Idkh = idhd;
                phieuXuat.NgayTao = DateTime.Now;
                phieuXuat.SoPx = CommonServices.taoSoPhieuXuat(context);
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
                tran.Commit();

            }
            catch (Exception ex)
            {
                tran.Rollback();
            }
        }
        public async Task YeuCauHoTro(string mac)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();

                Ban ban = context.Ban.FirstOrDefault(x => x.Ipmac == mac);


            await Clients.All.SendAsync("NhanYeuCauHoTro", ban);
        }
    }
}