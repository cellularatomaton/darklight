using System.Configuration;

namespace DarkLight.Common.Config
{
    public static class SectionHandler<T> where T : ConfigurationSection
    {
        public static T GetSection(string section)
        {
            T config = (T)ConfigurationManager.GetSection(section);
            return config;
        }

        public static T GetSection(string section, Configuration config)
        {
            T configSection = (T)config.GetSection(section);
            return configSection;
        }
    }
}