using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MirrorOnce.Core.Cli;
using NLog;

namespace MirrorOnce
{
    public class Executor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICliParser _cliParser;

        public Executor(ICliParser cliParser)
        {
            _cliParser = cliParser;
        }

        public async Task<int> Run(IEnumerable<string> args, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_cliParser.TryParse(args, out var options, out var errors))
            {
                while (!options.OneShot
                       && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {

                    }
                    catch (TaskCanceledException)
                    {
                        // Ignored.
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, "Handled exception, will retry (--one-shot is not specified).");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(9), cancellationToken);
                }

                return 0;
            }

            Logger.Error("Failed to validate options.");
            foreach (var error in errors)
            {
                Logger.Error(error);
            }

            return 1;
        }
    }
}
