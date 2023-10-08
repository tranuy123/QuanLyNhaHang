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
    }
}
