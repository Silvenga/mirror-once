using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using MirrorOnce.Core.Abstractions;
using NLog;

namespace MirrorOnce.Core.Cli
{
    public interface ICliParser
    {
        bool TryParse(IEnumerable<string> args,
                      [NotNullWhen(true)] out CliOptions? options,
                      [NotNullWhen(false)] out IReadOnlyCollection<string>? errors);
    }

    public class CliParser : ICliParser
    {
        private readonly IFileWrapper _fileWrapper;
        private readonly ISourceEnvironmentOptions _environmentOptions;

        public CliParser(IFileWrapper fileWrapper, ISourceEnvironmentOptions environmentOptions)
        {
            _fileWrapper = fileWrapper;
            _environmentOptions = environmentOptions;
        }

        public bool TryParse(IEnumerable<string> args,
                             [NotNullWhen(true)] out CliOptions? options,
                             [NotNullWhen(false)] out IReadOnlyCollection<string>? errors)
        {
            var argsFromEnv = _environmentOptions.GetOptions<CliOptions>();
            var mergedArgs = OrderedDistinct(args.Concat(argsFromEnv));

            var parser = new Parser(ConfigureParser);
            var result = parser.ParseArguments<CliOptions>(mergedArgs);
            if (result is Parsed<CliOptions>)
            {
                if (!IsValid(result.Value, out errors))
                {
                    options = default;
                    return false;
                }

                options = result.Value;
                errors = default;
                return true;
            }

            options = default;
            errors = Array.Empty<string>();
            return false;
        }

        private static void ConfigureParser(ParserSettings settings)
        {
            settings.HelpWriter = LoggingTextWriter.Default;
        }

        private bool IsValid(CliOptions options, out IReadOnlyCollection<string> errors)
        {
            var errorSet = new HashSet<string>();

            if (!_fileWrapper.Exists(options.SourcePath))
            {
                errorSet.Add($"The source path '{options.SourcePath}' must exist.");
            }

            if (!_fileWrapper.Exists(options.TargetPath))
            {
                errorSet.Add($"The source path '{options.TargetPath}' must exist.");
            }

            if (options.InternalSeconds <= 0)
            {
                errorSet.Add($"The interval {options.InternalSeconds} must be greater than 0.");
            }

            errors = errorSet;
            return errors.Count == 0;
        }

        private IEnumerable<string> OrderedDistinct(IEnumerable<string> enumerable)
        {
            var memory = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var v in enumerable)
            {
                if (memory.Add(v))
                {
                    // Not seen before.
                    yield return v;
                }
            }
        }

        private class LoggingTextWriter : TextWriter
        {
            public static LoggingTextWriter Default { get; } = new();

            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            public override Encoding Encoding { get; } = Encoding.UTF8;

            public override void Write(string? value)
            {
                if (value != null)
                {
                    Logger.Error(value.TrimEnd());
                }
            }
        }
    }
}
