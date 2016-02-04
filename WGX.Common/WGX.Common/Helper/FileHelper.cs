using System.IO;

namespace WGX.Common.Helper
{
    public static class FileHelper
    {
        public static string GetMIMEType(this string file)
        {
            var mimeType = "application/unknown";
            var extension = Path.GetExtension(file);
            if (extension != null)
            {
                var ext = extension.ToLower();
                var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
    }
}
