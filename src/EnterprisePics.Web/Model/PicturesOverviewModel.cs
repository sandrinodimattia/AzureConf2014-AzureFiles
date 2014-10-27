using System.Collections.Generic;
using System.IO;

namespace EnterprisePics.Web.Model
{
    public class PicturesOverviewModel : BaseModel
    {
        public IEnumerable<FileInfo> Items
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }
    }
}