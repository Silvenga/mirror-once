using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;
using MirrorOnce.Core.Abstractions;

namespace MirrorOnce.Core.Cli
{
    public interface ISourceEnvironmentOptions
    {
        IEnumerable<string> GetOptions<T>();
    }

    public class SourceEnvironmentOptions : ISourceEnvironmentOptions
    {
        private readonly IEnvironmentWrapper _environmentWrapper;

        public SourceEnvironmentOptions(IEnvironmentWrapper environmentWrapper)
        {
            _environmentWrapper = environmentWrapper;
        }

        public IEnumerable<string> GetOptions<T>()
        {
            foreach (var (environmentName, cliName) in GetCliMapping(typeof(T)))
            {
                if (_environmentWrapper.GetEnvironment(environmentName) is { } environmentValue
                    && !string.IsNullOrWhiteSpace(environmentValue))
                {
                    yield return $"--{cliName}";
                    yield return environmentValue;
                }
            }
        }

        private IEnumerable<EnvironmentToOptionMapping> GetCliMapping(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                 .Where(x => x.CanWrite);

            foreach (var propertyInfo in properties)
            {
                var optionAttribute = propertyInfo.GetCustomAttribute<OptionAttribute>(true);
                if (optionAttribute?.LongName is { } cliName)
                {
                    var environmentAttributes = propertyInfo.GetCustomAttributes<SourceFromEnvironmentAttribute>(true);
                    foreach (var environmentAttribute in environmentAttributes)
                    {
                        var environmentName = environmentAttribute.Name;

                        yield return new EnvironmentToOptionMapping(environmentName, cliName);
                    }
                }
            }
        }

        private record EnvironmentToOptionMapping(string EnvironmentName, string CliName);
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SourceFromEnvironmentAttribute : Attribute
    {
        public string Name { get; }

        public SourceFromEnvironmentAttribute(string name)
        {
            Name = name;
        }
    }
}
