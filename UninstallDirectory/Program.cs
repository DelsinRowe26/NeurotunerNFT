using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UninstallDirectory
{
    internal class Program
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string path2 = Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            if(Directory.Exists("Record"))
            {
                Directory.Delete("Record", true);
            }
            if (Directory.Exists("Image"))
            {
                Directory.Delete("Image", true);
            }
            if (File.Exists("log.tmp"))
            {
                File.Delete("log.tmp");
            }
        }
    }
}
