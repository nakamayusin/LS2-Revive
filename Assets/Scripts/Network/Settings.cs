public class Settings : Photon.MmoDemo.Client.Settings
{
    public static Settings GetDefaultSettings()
    {
        Settings result = new Settings
        {
            ServerAddress = "127.0.0.1:5061"
        };
        return result;
    }
}