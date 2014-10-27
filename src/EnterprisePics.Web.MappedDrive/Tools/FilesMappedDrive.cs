
namespace EnterprisePics.Web.MappedDrive.Tools
{
    public partial class FilesMappedDrive
    {
        public string DriveLetter
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        internal FilesMappedDrive(string driveLetter, string path)
        {
            DriveLetter = driveLetter;
            Path = path;
        }
    }
}
