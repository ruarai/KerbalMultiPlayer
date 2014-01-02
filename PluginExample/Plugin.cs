using KMPServer;

namespace PluginExample
{
    public class Plugin
    {
        //Triggered when plugin is first loaded
        public static void PluginEntry()
        {
            Log.Debug("Plugin example loaded.");
        }

        //Triggered when server starts
        public static void OnServerStart()
        {
            Log.Debug("Plugin example sees server start!");
        }

        //Commands enter whilst server is running
        public static bool OnServerCommand(string input)
        {
            if (input.ToLower().Trim().Split(' ')[0] == "/test")
            {
                Log.Debug("Server command handled!");
                return true;
            }
            return false;
        }

        //Commands entered before server is running
        public static bool OnPreServerCommand(string input)
        {
            if (input.ToLower().Trim().Split(' ')[0] == "/test")
            {
                Log.Debug("Preserver command handled!");
                return true;
            }
            return false;
        }

        //Commands entered by clients and sent to the server
        public static bool OnClientCommand(Client cl, string input)
        {
            if (input.ToLower().Trim().Split(' ')[0] == "!kickme")
            {
                ServerMain.server.markClientForDisconnect(cl,"You got kicked by a plugin!");
                ServerMain.server.sendTextMessageToAdmins(cl.username + " was kicked by a plugin.");
                return true;
            }
            return false;
        }
    }
}
