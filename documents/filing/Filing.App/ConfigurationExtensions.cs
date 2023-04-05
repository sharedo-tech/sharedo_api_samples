namespace Filing.App
{
    public static class ConfigurationExtensions
    {
        public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        {
            return configuration.GetValue<T>(key) ??
                throw new InvalidOperationException($"Missing configuration for key {key}");
        }
    }
}
