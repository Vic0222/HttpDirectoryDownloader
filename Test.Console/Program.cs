using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloader = new HttpDirectoryDownloader.Downloader();
            downloader.DownloadDirectory(@"http://localhost/TestDirectory/", @"C:\temp").Wait();
        }
    }
}
