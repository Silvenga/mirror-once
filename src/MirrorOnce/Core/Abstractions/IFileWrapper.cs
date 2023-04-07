using System.IO;

namespace MirrorOnce.Core.Abstractions
{
    public interface IFileWrapper
    {
        bool Exists(string filePath);
    }

    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
