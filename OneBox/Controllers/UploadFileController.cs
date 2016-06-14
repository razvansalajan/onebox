
using OneBox_DataAccess.DataServices;
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
        private IDataServices dataServices;
        public UploadFileController(IDataServices serv)
        {
            this.dataServices = serv;
        }

        public async Task<List<string>> PostAsync()
        {
            Dictionary<string, string> queryParam = new Dictionary<string, string>();
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = HttpContext.Current.Server.MapPath("~/uploads");
                //string current_path = Request.RequestUri.ParseQueryString()["currentPath"];
                //MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                var streamProvider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(streamProvider);

                //var queryParameters = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

                List<string> messages = new List<string>();
                string currentPath = "";
                foreach (var file in streamProvider.Contents)
                {
                    string name = file.Headers.ContentDisposition.Name.Replace("\"", "");
                    var dataStream = await file.ReadAsStreamAsync();
                    if (!name.Equals("file"))
                    {
                        byte[] content = new byte[dataStream.Length];
                        dataStream.Read(content, 0, (int)dataStream.Length);
                        string currentParam = System.Text.Encoding.UTF8.GetString(content);
                        queryParam.Add(name, currentParam);
                        continue;
                    }
                    else
                    {
                        long chunkIndex = Int64.Parse(queryParam["resumableChunkNumber"]);
                        long totalFileSize = Int64.Parse(queryParam["resumableTotalSize"]);
                        long totalNumberOfChunks = Int64.Parse(queryParam["resumableTotalChunks"]);
                        long chunkSize = Int64.Parse(queryParam["resumableChunkSize"]);
                        string blobPath = queryParam["currentPath"] + "/" + queryParam["resumableFilename"];
                        dataServices.AddNewFileChunk(dataStream, chunkIndex, blobPath, totalFileSize);
                        if (chunkIndex == totalNumberOfChunks)
                        {
                            dataServices.CommitFileChunks(blobPath, (int)totalNumberOfChunks);
                        }
                    }
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
