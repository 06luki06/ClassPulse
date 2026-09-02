using System;
using System.IO;

namespace At.luki0606.ClassPulse
{
    public static class Utils
    {
        public static string GetAppdataFolderPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(appDataPath, "ClassPulse");
            Directory.CreateDirectory(folderPath);

            return folderPath;
        }
    }
}