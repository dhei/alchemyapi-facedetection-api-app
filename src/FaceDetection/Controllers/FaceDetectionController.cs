using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FaceDetection.Controllers
{
    public class FaceDetectionController : ApiController
    {
        private const string _alchemyApiKey = "";
        private const string _url = "http://access.alchemyapi.com/calls/image/ImageGetRankedImageFaceTags";

        public async Task<JToken> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

            IList<HttpContent> files = provider.Files;
            var file1 = files[0];
            var fileStream = await file1.ReadAsStreamAsync();

            var alchemyUrl =
                string.Format(
                    "{0}?apikey={1}&outputMode=json&knowledgeGraph=1&imagePostMode=raw", _url, _alchemyApiKey);

            using (var client = new HttpClient())
            {
                var responseMessage =
                    await client.PostAsync(alchemyUrl, new StreamContent(fileStream)).ConfigureAwait(false);

                if (!responseMessage.IsSuccessStatusCode) return "error";

                var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                var r = JsonConvert.DeserializeObject<dynamic>(response);
                return JObject.Parse(r.ToString());
            }
        }
    }
}
