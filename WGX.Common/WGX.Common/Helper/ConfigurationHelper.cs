using System.Configuration;

namespace WGX.Common.Helper
{
    public static class ConfigurationHelper
    {
        public static T GetSection<T>() where T : ConfigurationSection, new()
        {
            var section = (T)ConfigurationManager.GetSection(typeof(T).Name) ?? new T();

            return section;
        }
    }
}
