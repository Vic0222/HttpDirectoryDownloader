using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpDirectoryDownloader
{
    public class Downloader: IDisposable
    {
        
        public Downloader()
        {
        }

        public async Task DownloadDirectory(string httpDirectory, string downloadPath)
        {
            using (var httpClient = new HttpClient())
            {

                var response = await httpClient.GetAsync(httpDirectory);
                string body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    
                    throw new Exception(string.Format("Http Error : {0} \n Message Body: {1}",response.StatusCode.ToString(), body));
                }
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(body);
                var anchors = htmlDoc.DocumentNode.SelectNodes("//a");
                ICollection<FileInfo> fileInfoes = anchors.Select(a => new FileInfo() { FileName = a.InnerText, Link = a.GetAttributeValue("href", null) }).ToList();
                foreach (var fileInfo in fileInfoes)
                {
                    var uri = new Uri(new Uri(httpDirectory), fileInfo.Link);
                    var getResponse = await httpClient.GetAsync(uri);
                    if (!getResponse.IsSuccessStatusCode && getResponse.StatusCode != HttpStatusCode.NotFound)
                    {
                        string message = await getResponse.Content.ReadAsStringAsync();
                        throw new Exception(string.Format("Http Error : {0} \n Message Body: {1}", getResponse.StatusCode.ToString(), body));
                    }

                    Stream file = await getResponse.Content.ReadAsStreamAsync();
                    using (file)
                    {
                        var path = Path.Combine(downloadPath, fileInfo.FileName);
                        using (var streamWriter = new FileStream(path, FileMode.Create))
                        {
                            file.Seek(0, SeekOrigin.Begin);
                            file.CopyTo(streamWriter);
                        }
                    }
                }
               
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
