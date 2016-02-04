using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WGX.Site.Common
{
    public static class ConfigHelper
    {
        public static string UploadFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadFilePath"];
            }
        }

        public static string ConvertSwfPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ConvertSwfPath"];
            }
        }

        public static string P2FServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["P2FServerPath"];
            }
        }

        public static string ListCommonComment
        {
            get
            {
                return ConfigurationManager.AppSettings["ListCommonComment"];
            }
        }
    }
}