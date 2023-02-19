using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;

namespace ArkEcho.Server
{
    public static class ShellFileAccess
    {
        public static int GetRating(string path)
        {
            using (ShellFile file = ShellFile.FromFilePath(path))
            {
                ShellProperty<uint?> prop = getRatingProperty(file);

                if (prop.Value == null)
                    return 0;
                else if (prop.Value == 99)
                    return 5;
                else if (prop.Value == 75 || prop.Value == 50 || prop.Value == 25)
                    return Convert.ToInt32(prop.Value / 25) + 1;
                else if (prop.Value == 1)
                    return 1;

                throw new Exception($"Unkown Rating Value {prop.Value} on file " + path);
            }
        }

        public static void SetRating(string path, int value)
        {
            Dictionary<int, uint?> starRatingMap = new Dictionary<int, uint?>()
            {
                { 0, 0 }, { 1, 1 }, { 2, 25 }, { 3, 50 }, { 4, 75 }, { 5, 99 },
            };

            using (ShellFile file = ShellFile.FromFilePath(path))
            {
                ShellProperty<uint?> prop = getRatingProperty(file);

                using (ShellPropertyWriter writer = file.Properties.GetPropertyWriter())
                {
                    writer.WriteProperty("System.Rating", starRatingMap[value]);
                }
            }
        }

        private static ShellProperty<uint?> getRatingProperty(ShellFile file)
        {
            return file.Properties.GetProperty<uint?>("System.Rating");
        }
    }
}
