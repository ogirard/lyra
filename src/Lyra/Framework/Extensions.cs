using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Lyra.Framework
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
            {
                return;
            }

            foreach (var item in enumerable)
            {
                action?.Invoke(item);
            }
        }

        public static IConfigurationBuilder AddAdditionalJsonFiles(this IConfigurationBuilder configurationBuilder, IReadOnlyCollection<string> additionalJsonFiles)
        {
            additionalJsonFiles.ForEach(f => configurationBuilder.AddJsonFile(f, optional: true, reloadOnChange: true));
            return configurationBuilder;
        }
    }
}