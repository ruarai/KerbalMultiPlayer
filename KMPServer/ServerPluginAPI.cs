using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace KMPServer
{
    internal static class ServerPluginAPI
    {
        const string pluginFolder = "plugins";

        public static List<Plugin> plugins = new List<Plugin>();

        public static void Load()
        {
            if (!Directory.Exists(pluginFolder))
                Directory.CreateDirectory(pluginFolder);

            var folder = new DirectoryInfo(pluginFolder);

            foreach (var file in folder.GetFiles("*.dll"))
            {
                var assembly = Assembly.LoadFile(file.FullName);

                var plugin = new Plugin(assembly,file.Name);
                plugin.Initialize();
                plugins.Add(plugin);

            }
        }


        public static void InvokeAll(string methodName)
        {
            foreach (Plugin plugin in plugins)
            {
                plugin.Invoke("methodName");
            }
        }

        public static void InvokeAll(string methodName,params object[] parameters)
        {
            foreach (Plugin plugin in plugins)
            {
                plugin.Invoke("methodName",parameters);
            }
        }

        public static object ReturnFirst(string methodName,object defaultReturn)
        {
            foreach (Plugin plugin in plugins)
            {
                return plugin.Invoke(methodName);
            }
            return defaultReturn; //return whatever they specified if nothing happens
            //This allows for much cleaner code when calling this method
            //For example, 
            //if(ServerPluginAPI.ReturnFirst("onServerCommand",false))
            //would mean that even if no onServerCommand method existed in any plugins
            //no extra handling of any possible null types would be neccessary
        }

        public static object ReturnFirst(string methodName,object defaultReturn,params object[] parameters)
        {
            foreach (Plugin plugin in plugins)
            {
                return plugin.Invoke(methodName,parameters);
            }
            return defaultReturn;
        }
    }

    public class Plugin
    {
        public Plugin(Assembly assembly, string fileName)
        {
            Assembly = assembly;
            FileName = fileName;
        }
        public Assembly Assembly;
        public string FileName;

        public void Initialize()
        {
            Log.Debug("Initializing {0}",FileName);

            Invoke("PluginEntry");
        }

        public object Invoke(string methodName)
        {
            try
            {
                return FindMethod(methodName).Invoke(null, null);
            }
            catch (Exception e)
            {
                Log.Debug("Plugin API exception in Invoke(methodName) from {0}",FileName);
                Log.Debug(e.ToString());
                return null;
            }
        }

        public object Invoke(string methodName,params object[] parameters)
        {
            try
            {
                return FindMethod(methodName).Invoke(null, parameters);
            }
            catch (Exception e)
            {
                Log.Debug("Plugin API exception in Invoke(methodName,parameters) from {0}",FileName);
                Log.Debug(e.ToString());
                return null;
            }
        }
        

        private MethodInfo FindMethod(string methodName)
        {
            foreach (var type in Assembly.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic && method.Name == methodName)
                    {
                        return method;
                    }
                }
            }
            return null;
        }
    }
}
