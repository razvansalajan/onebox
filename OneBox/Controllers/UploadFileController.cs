using OneBox_BusinessLogic.AzureStorage;
using OneBox_WebServices.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace OneBox_WebServices.Controllers
{
    public class UploadFileController : ApiController
    {
        private IAzureServices azureServices;
        public UploadFileController(IAzureServices serv)
        {
            this.azureServices = serv;
        }
        
        public async Task<List<string>> PostAsync()
        {

            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/uploads");
                //string current_path = Request.RequestUri.ParseQueryString()["currentPath"];
                //MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                var streamProvider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(streamProvider);

                var queryParameters = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

                List<string> messages = new List<string>();
                string currentPath = "";
                foreach (var file in streamProvider.Contents)
                {
                    string name = file.Headers.ContentDisposition.Name.Replace("\"", "");
                    var dataStream = await file.ReadAsStreamAsync();
                    if (name.Equals("currentPath")){
                        byte[] content = new byte[dataStream.Length];
                        dataStream.Read(content, 0, (int)dataStream.Length);
                        currentPath = System.Text.Encoding.UTF8.GetString(content);
                        continue;
                    }

                    //var xx = file.Headers.ContentDisposition.FileName
                   
                   string fileName = file.Headers.ContentDisposition.FileName;
                    fileName = fileName.Replace("\"", "");
                    this.azureServices.AddNewFile(currentPath, fileName, dataStream);
                    messages.Add("File uploaded as " + dataStream.Length + " bytes)");
                }
                
                return messages;
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
            /*
            bool isSavedSuccessfully = false;

            foreach (string fileName in Reques)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                //You can Save the file content here

                isSavedSuccessfully = true;
            }

            return Json(new { Message = string.Empty });
            */
        }
    }
}
