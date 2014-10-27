using System.Web;

using EnterprisePics.Web.MappedDrive.Tools;

namespace EnterprisePics.Web.MappedDrive
{
    public class MapDriveHttpModule : IHttpModule
    {
        /// <summary>
        /// Use FileMappedDrive (from the RedDog.Storage project) to mount a drive.
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            FilesMappedDrive.Mount("Z:", @"\\azurefileservicedemo.file.core.windows.net\pictures", 
                "azurefileservicedemo", "7DR/wKFA80CiRbiO5EvDZgVkdf+cCRnJqSW8md/gDy2aZXm1JqwcLy76riMJHIprsPgOshj8f7KekSxWhXiItg==");
        }

        public void Dispose()
        {
            
        }
    }
}
