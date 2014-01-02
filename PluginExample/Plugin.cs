using KMPServer;

namespace PluginExample
{
    public class Plugin
    {
        public static void PluginEntry()
        {
            Log.Debug("Hello PluginEntry!");
        }

        public static void OnServerStart()
        {
            Log.Debug("Hello OnServerStart!");
        }

    }
}
