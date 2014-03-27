using System.Net;
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

        //Triggered when server starts (before threads are started, but after some configuration is loaded)
        public static void OnServerStart()
        {
            Log.Debug("Plugin example sees server start!");
        }

        //Triggered when server stops
        public static void OnServerStop()
        {
            Log.Debug("Plugin example sees server stop!");
        }

        //Triggered when server updates (dont use for most things, as this updates every 10ms)
        public static void OnServerUpdate()
        {
        }

        //Commands enter whilst server is running
        public static PluginResult OnServerCommand(string input)
        {
            if (input.ToLower().Trim().Split(' ')[0] == "/test")
            {
                Log.Debug("Server command handled!");
                Log.Debug(input);
                return new PluginResult(true);
            }
            return new PluginResult(false);
        }

        //Commands entered before server is running
        public static PluginResult OnPreServerCommand(string input)
        {
            if (input.ToLower().Trim().Split(' ')[0] == "/test")
            {
                Log.Debug("Preserver command handled!");
                Log.Debug(input);
                return new PluginResult(true);
            }
            return new PluginResult(false);
        }

        //Commands entered by clients and sent to the server
        public static PluginResult OnClientCommand(Client cl, string input)
        {
            if (input.ToLower().Trim().Split(' ')[0] == "!kickme")
            {
                ServerMain.server.markClientForDisconnect(cl,"You got kicked by a plugin!");
                ServerMain.server.sendTextMessageToAdmins(cl.username + " was kicked by a plugin.");
                return new PluginResult(true);//Report to the command API that this command has been handled and no "command not found" message will be sent
            }
            return new PluginResult(false);//report that this plugin didnt handle this command
        }

        //Every non-command chat message sent by a client
        public static PluginResult OnClientChat(Client cl, string input)
        {
            //log to chat
            Log.Chat(cl.username, input);

            //do standard formatting
            string full_message = string.Format("<{0}> {1}", cl.username,input);

            //make everyone LOUD
            full_message = full_message.ToUpper();

            //Send the chat message to all other clients
            ServerMain.server.sendTextMessageToAll(full_message);

            return new PluginResult(true);//Dont continue with standard formatting
        }

        //Every time a http request is made to the servers http server
        public static PluginResult OnHTTPRequest(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;

            string responseText = "TEST!";

            //Send response
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
            response.ContentLength64 = buffer.LongLength;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();


            return new PluginResult(true);//Return to cancel standard http behaviour
        }
    }
}
