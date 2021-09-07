using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Newtonsoft.Json;

using PackageCreator.Models;

using Sitecore.Services.Infrastructure.Web.Http;

namespace PackageCreator.Controllers
{
    using Sitecore.Services.Infrastructure.Security;

    [RequiredApiKey]
    public class PackageCreatorController : ServicesApiController
    {
        [HttpPost]
        public HttpResponseMessage Generate(PackageManifest manifest)
        {
            try
            {
                var filePath = Services.PackageBuilder.GeneratePackage(manifest);


                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(filePath))
                };
                result.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"{manifest.PackageName}.zip"
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                return result;


            }
            catch (Exception exception)
            {
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                httpResponseMessage.Content = new StringContent(
                    JsonConvert.SerializeObject(exception.Message),
                    System.Text.Encoding.UTF8, "application/json");
                return httpResponseMessage;
            }
        }
    }
}