using System.IO;
using System.IO.Abstractions;
using System.Xml;

namespace PresetParser
{
    public class IfoFileProvider : IIfoFileProvider
    {
        private readonly IFileSystem _fileSystem;
        public IfoFileProvider()
        {
            _fileSystem = new FileSystem();
        }
        public XmlDocument GetIfoFileContent(string basePath, string variationFilename)
        {
            var result = new XmlDocument();

            var pathToFileDirectory = Path.Combine(Path.GetDirectoryName(variationFilename), Path.GetFileNameWithoutExtension(variationFilename));
            var pathToFile = Path.Combine(basePath + "/", string.Format("{0}.ifo", pathToFileDirectory));

            if (_fileSystem.File.Exists(pathToFile))
            {
                result.Load(pathToFile);
                return result;
            }

            return result;
        }
    }
}
