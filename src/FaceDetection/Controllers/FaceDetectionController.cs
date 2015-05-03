using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FaceDetection.Models;
using FaceDetection.Services;
using Newtonsoft.Json;

namespace FaceDetection.Controllers
{
    public class FaceDetectionController : ApiController
    {
        /// <summary>
        /// Posts to the Alchemy API and looks for faces.
        /// </summary>
        /// <returns>FaceDetectionResult</returns>
        public async Task<FaceDetectionResult> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

            var files = provider.Files;
            var file1 = files[0];
            var fileStream = await file1.ReadAsStreamAsync();

            var alchemyApi = new AlchemyApi();
            return await alchemyApi.FaceDetection(fileStream);
        }

        /// <summary>
        /// Useful for a simple demonstration of the results w/o having to submit a post.
        /// </summary>
        /// <returns>FaceDetectionResult</returns>
        public async Task<FaceDetectionResult> Get()
        {
            var httpClient = new HttpClient();
            using (var content = new MultipartFormDataContent("BOUNDARY"))
            {
                var root = HttpContext.Current.Server.MapPath("~/bin");
                string path = root + "\\wade.jpg";
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                content.Add(new StreamContent(fileStream), "test", "wade.jpg");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost:21346/api/facedetection"),
                    Content = content
                };

                var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var envelope = JsonConvert.DeserializeObject<FaceDetectionResult>(response);
                    return envelope;
                }
            }

            return null;
        }
    }
}
