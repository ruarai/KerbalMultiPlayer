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

        public static bool OnServerCommand(string input)
        {
            if (input.ToLower().Trim().Split(' ')[0] == "/test")
            {
                Log.Debug("Test completed!");
                return true;
            }
            return false;
        }
    }
}
