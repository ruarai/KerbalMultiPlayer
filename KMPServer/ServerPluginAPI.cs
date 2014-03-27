using System;
using System.Collections.Generic;
using System.Linq;
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

                var plugin = new Plugin(assembly, file.Name);
                plugin.Initialize();
                plugins.Add(plugin);

            }
        }


        public static void InvokeAll(string methodName)
        {
            InvokeAll(methodName, null);
        }

        public static void InvokeAll(string methodName, params object[] parameters)
        {
            foreach (Plugin plugin in plugins)
            {
                plugin.Invoke(methodName, parameters);
            }
        }

        public static PluginResult InvokeReturnResult(string methodName)
        {
            return InvokeReturnResult(methodName, null);
        }

        public static PluginResult InvokeReturnResult(string methodName, params object[] parameters)
        {
            foreach (Plugin plugin in plugins)
            {
                var returned = plugin.Invoke(methodName, parameters);

                if (returned != null)
                {
                    var pluginResult = (PluginResult) returned;

                    if (pluginResult.Success)
                    {
                        //Log.Debug("Plugin {0} handled {1}", plugin.FileName, methodName);
                        return pluginResult;
                    }
                }
            }
            return new PluginResult(false);
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

        private static List<MethodInfo> methods = new List<MethodInfo>();

        public void Initialize()
        {
            Log.Debug("Initializing {0}", FileName);

            foreach (var type in Assembly.GetTypes())
            {
                foreach (var method in type.GetMethods().Where(method => method.IsStatic))
                {
                    methods.Add(method);
                }
            }

            Invoke("PluginEntry");//invoke the entry method to allow for loading 
        }

        public object Invoke(string methodName)
        {
            return Invoke(methodName, null);
        }

        public object Invoke(string methodName, params object[] parameters)
        {
            try
            {
                var method = FindMethod(methodName);
                return (method != null) ? method.Invoke(null, parameters) : null;
            }
            catch (Exception e)
            {
                Log.Debug("Plugin API exception in Invoke(methodName,parameters) from {0}", FileName);
                Log.Debug(e.ToString());
                return null;
            }
        }


        private static MethodInfo FindMethod(string methodName)
        {
            return methods.FirstOrDefault(m => m.Name == methodName);
        }
    }

    public class PluginResult
    {
        public PluginResult(bool success)
        {
            Success = success;
        }
        public object Data;
        public bool Success;
    }
}
