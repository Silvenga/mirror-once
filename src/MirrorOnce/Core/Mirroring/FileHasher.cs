using System;
using System.IO;
using System.Threading.Tasks;

namespace MirrorOnce.Core.Mirroring
{
    public interface IFileHasher
    {
    }

    public class FileHasher : IFileHasher
    {
        public async ValueTask<FileContentIdentity> Calculate(string path)
        {
            if (File.Exists(path))
            {
                await using var fileStream = File.OpenRead(path);
            }

            throw new InvalidOperationException();
        }
    }

    public record FileContentIdentity(byte[] Hash, long LengthBytes, long LastModifiedEpoch);
}
