using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FaceDetection.Models;
using Newtonsoft.Json;

namespace FaceDetection.Services
{
    public class AlchemyApi
    {
        private readonly string _alchemyApiKey = "YourAPIKey";
        private const string _url = "http://access.alchemyapi.com/calls/image/ImageGetRankedImageFaceTags";

        public async Task<FaceDetectionResult> FaceDetection(Stream fileStream)
        {
            var alchemyUrl =
                string.Format(
                    "{0}?apikey={1}&outputMode=json&knowledgeGraph=1&imagePostMode=raw", _url, _alchemyApiKey);

            using (var client = new HttpClient())
            {
                var responseMessage =
                    await client.PostAsync(alchemyUrl, new StreamContent(fileStream)).ConfigureAwait(false);

                if (!responseMessage.IsSuccessStatusCode) return null;

                var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                var r = JsonConvert.DeserializeObject<FaceDetectionResult>(response);
                return r;
            }
        }
    }
}