using Fhi.HelseId.Web;

namespace HyFive.Services.Authentication.Configuration
{
    public interface IHelseIdConfiguration : IHelseIdWebKonfigurasjon
    {

    }

    /// <summary>
    /// Class for strongly typed configuration of HelseId.
    /// </summary>
    public class HandhygieneHelseIdKonfigurasjon : HelseIdWebKonfigurasjon, IHelseIdConfiguration
    {
        public bool ExcludeStaticAssetsFromProtection { get; set; }
        public bool CacheStaticAssets { get; set; }
    }
}
