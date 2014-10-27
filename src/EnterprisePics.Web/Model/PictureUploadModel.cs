using System.Web;

namespace EnterprisePics.Web.Model
{
    public class PictureUploadModel : BaseModel
    {
        public string DirectoryName
        {
            get;
            set;
        }

        public HttpPostedFileBase File
        {
            get;
            set;
        }
    }
}