using System.Web.Http;
using Sitecore.Pipelines;

namespace PackageCreator.Processors
{
    public class RegisterRoutesProcessor
    {
        public virtual void Process(PipelineArgs args)
        {
            GlobalConfiguration.Configure(config =>
            {
                config.Routes.MapHttpRoute("WebApiRoute", "api/packages/Generate", new
                {
                    controller = "PackageCreator"
                });
            });
        }
    }
}