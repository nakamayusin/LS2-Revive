public class Settings : Photon.MmoDemo.Client.Settings
{
    public static Settings GetDefaultSettings()
    {
        Settings result = new Settings
        {
            ServerAddress = "35.236.31.96:5061"
        };
        return result;
    }
}