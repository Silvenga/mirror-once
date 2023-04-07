using CommandLine;

namespace MirrorOnce.Core.Cli
{
    public class CliOptions
    {
        // Standard options.

        [Option('s', "source",
            HelpText = "The source directory to mirror from. "
                       + "But be readable. "
                       + "May also be specified using MIRROR_ONCE_SOURCE.",
            Required = true)]
        [SourceFromEnvironment("MIRROR_ONCE_SOURCE")]
        public string SourcePath { get; set; } = null!;

        [Option('t', "target",
            HelpText = "The target directory to mirror to. "
                       + "Must be readable and writable. "
                       + "May also be specified using MIRROR_ONCE_TARGET.",
            Required = true)]
        [SourceFromEnvironment("MIRROR_ONCE_TARGET")]
        public string TargetPath { get; set; } = null!;

        [Option('i', "interval",
            HelpText = "The number of seconds to wait between mirroring attempts. "
                       + "Must be greater than 0. "
                       + "May also be specified using MIRROR_ONCE_INTERVAL.",
            Required = true)]
        [SourceFromEnvironment("MIRROR_ONCE_INTERVAL")]
        public int InternalSeconds { get; set; }

        // Advanced options.

        [Option("cache-file",
            HelpText = "The file that cache data will be stored in. "
                       + "May also be specified using MIRROR_ONCE_CACHE_FILE.",
            Required = true)]
        [SourceFromEnvironment("MIRROR_ONCE_CACHE_FILE")]
        public string CachePath { get; set; } = null!;

        [Option("one-shot", HelpText = "Exit after the first mirror.")]
        public bool OneShot { get; set; }
    }
}
