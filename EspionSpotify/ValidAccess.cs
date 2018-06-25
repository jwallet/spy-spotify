using System.IO;

namespace EspionSpotify
{
    internal class ValidAccess
    {
        public static bool IsReadOnly(string basePath, string file) => (File.GetAttributes($"{basePath}{file}") & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;

        public static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove) => attributes | attributesToRemove;

        public static void AddReadOnly(string basePath, string file) => File.SetAttributes($"{basePath}{file}", File.GetAttributes($"{basePath}{file}") | FileAttributes.ReadOnly);

        public static void RemoveReadOnly(string basePath, string file)
        {
            var attributes = File.GetAttributes($"{basePath}{file}");
            if ((attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly) return;

            attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
            File.SetAttributes($"{basePath}{file}", attributes);
        }

        public static bool ToFile(string basePath, string file)
        {
            try
            {
                var filePath = $"{basePath}{file}";

                using (File.Open($"{filePath}.bak", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) { }
                using (File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) { }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
