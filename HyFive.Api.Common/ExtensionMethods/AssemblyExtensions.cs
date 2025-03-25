using System;
using System.Globalization;
using System.Reflection;

namespace HyFive.Api.Common.ExtensionMethods
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Hent byggetid.
        /// 
        /// Avhengig av at følgende entry eksisterer i csproj-fila:
        ///     <PropertyGroup>
        ///         <SourceRevisionId>build$([System.DateTime]::UtcNow.ToString("yyyyMMddHHmmss"))</SourceRevisionId>
        ///     </PropertyGroup>
        ///
        /// Kilde: SO (https://stackoverflow.com/a/69721691)
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static DateTime GetBuildTime(this Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                    if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
            }

            return default;
        }
    }
}