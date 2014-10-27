using System;

namespace EnterprisePics.Web.MappedDrive.Tools
{
    public class FilesMappedDriveException : Exception
    {
        public uint ErrorCode
        {
            get;
            set;
        }

        public FilesMappedDriveException(string message, uint errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}