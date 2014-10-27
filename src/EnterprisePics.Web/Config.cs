using System.Configuration;

namespace EnterprisePics.Web
{
    public static class Config
    {
        public static string GetPicturesPath()
        {
            return ConfigurationManager.AppSettings["PicturesPath"];
        }
    }
}