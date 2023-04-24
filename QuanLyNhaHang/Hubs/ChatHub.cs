using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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
            ChiTietHoaDon ct = context.ChiTietHoaDon.FirstOrDefault(x => x.Idcthd == int.Parse(id));
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
            try
            {
                HoaDon hd = new HoaDon();
                hd.MaHd = mahd1;
                hd.Idban = int.Parse(idban);

                hd.TongTien = float.Parse(tongtien);
                hd.TinhTrang = false;
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
                    ct.DonGia = ctt.DonGia;
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

            await Clients.All.SendAsync("HienThiCTHD", chitiethoadon,ipmac, tongtien, idban);
        }


    }
}