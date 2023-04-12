using System.IO;
using System.Reflection;

namespace ArkEcho.Core.Test
{
    public class FileTestBase
    {
        private const string testFolder = @"\TempFiles\";

        private string getTestFolder()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + testFolder;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public string CreateTestFile(string fileName, int sizeMb)
        {
            DeleteTestFile(fileName);

            string filePath = getTestFolder() + fileName;

            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            fs.Seek(1024 * 1024 * sizeMb, SeekOrigin.Begin);
            fs.WriteByte(0);
            fs.Close();

            return filePath;
        }

        public void DeleteTestFile(string fileName)
        {
            string filePath = getTestFolder() + fileName;

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public string CreateMp3TestFile(string fileName)
        {
            DeleteTestFile(fileName);

            byte[] fileContent = Properties.Resources.documentary_ambient_guitar_william_king_145480;
            string filePath = getTestFolder() + fileName;

            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            fs.Write(fileContent);
            fs.Close();

            return filePath;
        }
    }
}
