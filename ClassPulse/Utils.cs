using System;
using System.IO;

namespace At.luki0606.ClassPulse
{
    /// <summary>
    /// General helper methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Returns the folder that stores the database and settings file, creating it if necessary.
        /// </summary>
        /// <returns>The path of <c>%LOCALAPPDATA%\ClassPulse</c> on Windows.</returns>
        public static string GetAppdataFolderPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(appDataPath, "ClassPulse");
            Directory.CreateDirectory(folderPath);

            return folderPath;
        }
    }
}
