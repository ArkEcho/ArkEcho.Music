using System.Collections.Generic;

namespace ArkEcho.Core
{
    public static class PathHandling
    {
        public static string ReplaceBackForwardSlashPath(string uri)
        {
            return uri.Replace(@"\", "/");
        }

        public static List<string> ReplaceBackForwardSlashPathList(List<string> urlList)
        {
            List<string> replaced = new List<string>();
            urlList.ForEach(x => replaced.Add(x.Replace(@"\", @"/")));
            return replaced;
        }
    }
}
