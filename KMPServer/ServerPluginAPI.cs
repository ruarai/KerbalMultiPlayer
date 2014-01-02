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
                plugin.Invoke(methodName);
            }
        }

        public static void InvokeAll(string methodName,params object[] parameters)
        {
            foreach (Plugin plugin in plugins)
            {
                plugin.Invoke(methodName,parameters);
            }
        }


        public static object ReturnIfEqual(string methodName,object equalTo,object defaultReturn)
        {
            foreach (Plugin plugin in plugins)
            {
                var returned = plugin.Invoke(methodName, null);
                if (returned == equalTo) return equalTo;
            }
            return defaultReturn;
        }

        public static object ReturnIfEqual(string methodName, object equalTo, object defaultReturn,params object[] parameters)
        {
            foreach (Plugin plugin in plugins)
            {
                var returned = plugin.Invoke(methodName, parameters);
                if (returned == equalTo) return equalTo;
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
                var method = FindMethod(methodName);
                return method != null ? method.Invoke(null, null) : null;
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
                var method = FindMethod(methodName);
                return method != null ? method.Invoke(null, parameters) : null;
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
                    Log.Info(method.Name);
                }
            }
            return null;
        }
    }
}
