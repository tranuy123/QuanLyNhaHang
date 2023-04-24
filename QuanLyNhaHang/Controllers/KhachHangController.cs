﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace QuanLyNhaHang.Controllers
{
    public class KhachHangController : Controller
    {

        public IActionResult Index()
        {
            return View("Menu");
        }
        [Route("/KhachHang/GoiMon")]
        public IActionResult GoiMon()
        {
            return View("GoiMon");
        }
        [Route("/KhachHang/HoaDon")]
        public IActionResult HoaDon()
        {
            return View("HoaDonKhachHang");
        }
        [HttpPost("/loadNhomThucAn")]

        public IActionResult loadNhomThucAn(int idnta)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            string macAddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            var IDkhu = context.Ban.FirstOrDefault(x => x.Ipmac == macAddress);
            var IDSanh = context.Khu.FirstOrDefault(x => x.Idkhu == IDkhu.Idkhu);
            if(idnta == 0)
            {
                ViewBag.NTA = context.Gia.Where(x => x.Idsanh == IDSanh.Idsanh).ToList();

            }
            else
            {
                ViewBag.NTA = context.Gia.Where(x => x.Idsanh == IDSanh.Idsanh && x.IdtdNavigation.Idnta == idnta).ToList();

            }

       
            return PartialView();
        }
        [HttpPost("/addHoaDon")]
        public string addHoaDon(string IPMAC,int IDTD, float DonGia)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDonTam ct = new ChiTietHoaDonTam();
            
            ct.Ipmac = IPMAC;
            ct.Idtd = IDTD;
            ct.DonGia = DonGia;
            ct.Sl = 1;
            context.ChiTietHoaDonTam.Add(ct);
            context.SaveChanges();


            return "Thêm món thành công";
        }
        [HttpPost("/UpdateSL")]
        public string UpdateSL(int IDCTHDT, int SL, string DonGia, string ThanhTien)
        {
            QuanLyNhaHangContext context = new QuanLyNhaHangContext();
            ChiTietHoaDonTam ct = context.ChiTietHoaDonTam.Find(IDCTHDT);

            ct.Sl = SL;
            ct.DonGia = float.Parse(DonGia);
            ct.ThanhTien = float.Parse(ThanhTien);
            context.ChiTietHoaDonTam.Update(ct);
            context.SaveChanges();


            return "Thay doi so luong thanh cong";
        }
        [HttpPost("/addChiTietHoaDon")]

        public IActionResult addChiTietHoaDon( string ipmac, float tongtien, int idban)
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
                hd.Idban = idban;
                hd.TongTien = tongtien;
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
            return RedirectToAction("Index");
        }

    }
}
