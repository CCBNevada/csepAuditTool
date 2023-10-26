using SimpleLogger;
using System.Configuration;

namespace csepAuditTool.Model
{
    internal class ProtectConfigurationSectionModel
    {
        static public bool ProtectConfigurationSection()
        {
            // Get the current configuration file.
            var configured = false;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var connectionStrings = config.GetSection("connectionStrings");
            if (connectionStrings != null)
            {
                if (!connectionStrings.SectionInformation.IsProtected)
                {

                    connectionStrings.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    config.ConnectionStrings.SectionInformation.ForceSave = true;
                    config.Save();
                    connectionStrings = config.GetSection("connectionStrings");
                    if (connectionStrings == null || !connectionStrings.SectionInformation.IsProtected)
                    {
                        SimpleLog.Error("App.config Connection String Failed to Encrypt. (ProtectConfigurationSection())");
                        return false;
                    }
                    configured = true;
                    SimpleLog.Info("App.config Connection Strings Successfully Encrypted. (ProtectConfigurationSection())");
                }
            }

            var appSettings = config.GetSection("appSettings");
            if (appSettings != null)
            {
                if (!appSettings.SectionInformation.IsProtected)
                {
                    appSettings.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    config.AppSettings.SectionInformation.ForceSave = true;
                    config.Save();
                    appSettings = config.GetSection("appSettings");
                    if (appSettings == null || !appSettings.SectionInformation.IsProtected)
                    {
                        SimpleLog.Error("App.config App Settings Failed to Encrypt. (ProtectConfigurationSection())");
                        return false;
                    }
                    configured = true;
                    SimpleLog.Info("App.config App Settings Successfully Encrypted. (ProtectConfigurationSection())");
                }
            }
            if (configured) SimpleLog.Info("App.config Sections Successfully Encrypted. (ProtectConfigurationSection())");
            return true;
        }
    }
}
