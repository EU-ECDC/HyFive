namespace HyFive.Services.Authentication.Configuration
{
    
    /// <summary>
    /// Class for strongly typed configuration of HelseId.
    /// </summary>
    public class HandhygieneConfiguration : SecuritySettings
    {
        public bool ExcludeStaticAssetsFromProtection { get; set; }
        public bool CacheStaticAssets { get; set; }
    }
}
