using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ForestChurches.Components.ImageHandler
{
    public class ImageController : Controller, ImageInterface
    {
        private IConfiguration _configuration;

        public ImageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<byte[]> ConvertToByteArray(IFormFile file)
        {
            byte[] imageBytes;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                imageBytes = stream.ToArray();
            }

            return imageBytes;
        }

        public async Task<byte[]> ConvertToByteArrayFromUrl(string url)
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                byte[] imagebytes = client.DownloadData(url);

                return imagebytes;
            }
        }
    }
}
