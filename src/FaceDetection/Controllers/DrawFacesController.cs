using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using FaceDetection.Services;

namespace FaceDetection.Controllers
{
    public class DrawFacesController : ApiController
    {
        /// <summary>
        /// Posts to the Alchemy API and return an image with faces drawn.
        /// </summary>
        /// <returns>FaceDetectionResult</returns>
        public async Task<HttpResponseMessage> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

            var files = provider.Files;
            var file1 = files[0];
            var fileStream = await file1.ReadAsStreamAsync();

            var imageFileStream = new MemoryStream();
            fileStream.CopyTo(imageFileStream);
            fileStream.Seek(0, SeekOrigin.Begin);

            var alchemyApi = new AlchemyApi();
            var faceDetectionResult = await alchemyApi.FaceDetection(fileStream);

            var image = Image.FromStream(imageFileStream);

            var httpResponseMessage = new HttpResponseMessage();
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Jpeg);

                using (Graphics g = Graphics.FromImage(image))
                {
                    foreach (var imageFace in faceDetectionResult.imageFaces)
                    {
                        var x = Convert.ToInt32(imageFace.positionX);
                        var y = Convert.ToInt32(imageFace.positionY);
                        var width = Convert.ToInt32(imageFace.width);
                        var height = Convert.ToInt32(imageFace.height);

                        var destRect = new Rectangle(x, y, width, height);
                        var customColor = Color.FromArgb(50, Color.Green);
                        var shadowBrush = new SolidBrush(customColor);

                        g.FillRectangles(shadowBrush, new RectangleF[] {destRect});
                    }
                }

                using (var memory = new MemoryStream())
                {
                    image.Save(memory, ImageFormat.Jpeg);
                    memory.Position = 0;

                    httpResponseMessage.Content = new ByteArrayContent(memory.ToArray());

                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;

                    return httpResponseMessage;
                }

               
            }
        }
    }
}
