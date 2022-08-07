using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TEST_API
{
    internal class mac
    {
        public static string GetMacAddressHash(bool SHOWhash = true)//Возвращает список MAC-адресов, подключенных к интерфейсу PCI
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
            ("Select MACAddress,PNPDeviceID FROM Win32_NetworkAdapter WHERE MACAddress IS NOT NULL AND" +
             " PNPDeviceID IS NOT NULL AND" +
             " PhysicalAdapter = true");
            ManagementObjectCollection mObject = searcher.Get();

            string macs = (from ManagementObject obj in mObject
                           let pnp = obj["PNPDeviceID"].ToString()
                           where !(pnp.Contains("ROOT\\"))
                           select obj).Select(obj => obj["MACAddress"].ToString())
                .Aggregate<string, string>(null, (mac, currentMac) => mac + currentMac + ", ");
            macs = macs.Remove(macs.Length - 1, 1);
            string STRmac = !string.IsNullOrEmpty(macs) ? macs.Substring(0, macs.Length - 1) : macs;
            return (SHOWhash ? SHA256hash(STRmac).Replace("-", "") : STRmac);
        }

        public static string SHA256hash(string infa)
        {
            var bytes = Encoding.ASCII.GetBytes(infa);
            SHA256Managed sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash);
        }
    }
}
