using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace QuanLyNhaHang.Services
{
    public static class CommonServices
    {
        public static string getDiaChiMac()
        {
            string macAddress = "";
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // Lọc ra các card mạng không phải là loopback và đã kích hoạt
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    // Lấy địa chỉ MAC
                    macAddress = networkInterface.GetPhysicalAddress().ToString();

                }
            }
            return macAddress;
        }
        public static string taoSoPhieuNhap(QuanLyNhaHangContext context)
        {
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyyMMdd");
            var phieuNhap = context.PhieuNhap.Where(x => x.SoPn.Contains(date)).ToList();
            return $"PN-{date}-{(phieuNhap.Count() + 1).ToString("D2")}";
        }
        public static string taoSoPhieuXuat(QuanLyNhaHangContext context)
        {
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyyMMdd");
            var phieuNhap = context.PhieuXuat.Where(x => x.SoPx.Contains(date)).ToList();
            return $"PX-{date}-{(phieuNhap.Count() + 1).ToString("D2")}";
        }
        public static string GetTenHH(int idHH, List<HangHoa> hangHoas)
        {
            return hangHoas.FirstOrDefault(x => x.Idhh == idHH).TenHh;
        }
        public static string GetTenTD(int idTD, List<ThucDon> thucDons)
        {
            return thucDons.FirstOrDefault(x => x.Idtd == idTD).Ten;
        }
    }
}
