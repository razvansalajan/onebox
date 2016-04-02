using OneBox_WebServices.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OneBox_WebServices.Controllers
{
    public class UploadFileController : ApiController
    {
        public async Task<List<string>> PostAsync()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/uploads");

                //MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                var streamProvider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                
                List<string> messages = new List<string>();
                
                foreach (var file in streamProvider.Contents)
                {
                    var dataStream = await file.ReadAsStreamAsync();

                    messages.Add("File uploaded as " + dataStream.Length+ " bytes)");
                }
                 
                return messages;
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }
    }
}
