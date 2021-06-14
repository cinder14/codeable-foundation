using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Web.Compilation;
using Codeable.Foundation.UI.Web.Common.Plugins;
using Codeable.Foundation.Common.Plugins;
using Codeable.Foundation.UI.Web.Core;

// Thanks Umbraco [even though "shadow" isn't real]
[assembly: PreApplicationStartMethod(typeof(WebPreApplicationStartMethod), "Initialize")]
namespace Codeable.Foundation.UI.Web.Core
{
    public static class WebPreApplicationStartMethod
    {
        private static readonly object InitializeSyncRoot = new object();
        private static DirectoryInfo _shadowCopyFolder = null;

        public static IEnumerable<PluginConfig> LoadedPlugins { get; set; }

        /// <summary>
        /// Called via PreApplicationStartMethod, never use directly by code.
        /// </summary>
        public static void Initialize()
        {
            lock (InitializeSyncRoot)
            {
                DirectoryInfo pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(WebAssumptions.WEB_PLUGIN_PATH));
                _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(WebAssumptions.WEB_PLUGIN_SHADOWCOPY_PATH));

                List<PluginConfig> installedPlugins = new List<PluginConfig>();

                try
                {
                    Debug.WriteLine("Creating shadow copy folder and querying for dlls");
                    Directory.CreateDirectory(pluginFolder.FullName);
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

                    //clear out shadow copied plugins
                    FileInfo[] oldFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                    foreach (FileInfo file in oldFiles)
                    {
                        Debug.WriteLine("Deleting " + file.Name);
                        try
                        {
                            File.Delete(file.FullName);
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine("Error pre-deleting file " + file.Name + ". Exception: " + exc);
                        }
                    }

                    foreach (FileInfo pluginConfigFile in pluginFolder.GetFiles(WebAssumptions.WEB_PLUGIN_CONFIG_NAME, SearchOption.AllDirectories))
                    {
                        try
                        {
                            //parse file
                            PluginConfig config = ParsePluginConfigFile(pluginConfigFile.FullName);

                            //some validation
                            if (String.IsNullOrWhiteSpace(config.SystemName))
                            {
                                throw new Exception(string.Format("A plugin has no system name. Try assigning the plugin a unique name and recompiling.", config.SystemName));
                            }
                            if (LoadedPlugins != null && LoadedPlugins.Contains(config))
                            {
                                throw new Exception(string.Format("A plugin with '{0}' system name is already defined", config.SystemName));
                            }

                            //get list of all DLLs in plugins (not in bin!)
                            List<FileInfo> pluginDependencyFiles = pluginConfigFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                //just make sure we're not registering shadow copied plugins
                                .Where(x => !oldFiles.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => IsPackagePluginFolder(x.Directory))
                                .ToList();

                            //other plugin description info
                            FileInfo mainPluginFile = pluginDependencyFiles.Where(x => x.Name.Equals(config.AssemblyFileName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (mainPluginFile == null)
                            {
                                throw new Exception(string.Format("Unable to find plugin assembly: '{0}'", config.AssemblyFileName));
                            }
                            config.SourceAssemblyFile = mainPluginFile;

                            //shadow copy files
                            config.ReferencedAssembly = PerformFileDeploy(mainPluginFile);

                            //load all other assemblies now
                            foreach (FileInfo plugin in pluginDependencyFiles.Where(x => !x.Name.Equals(mainPluginFile.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                PerformFileDeploy(plugin);
                            }

                            //init plugin type 
                            Type[] referencedTypes = null;
                            try
                            {
                                referencedTypes = config.ReferencedAssembly.GetTypes();
                            }
                            catch (System.Reflection.ReflectionTypeLoadException rex)
                            {
                                Trace.Write("WebPreAppStartMethod:Error:: " + rex.Message, "Error");
                                foreach (var item in rex.LoaderExceptions)
	                            {
                                    Trace.Write("WebPreAppStartMethod:Error:: " + item.Message, "Error");
	                            }
                            }
                            catch (Exception ex)
                            {
                                Trace.Write("WebPreAppStartMethod:Error:: " + ex.Message, "Error");
                            }
                            if(referencedTypes != null)
                            {
                                foreach (Type t in referencedTypes)
                                {
                                    if (config.PluginType == null)
                                    {
                                        if (typeof(IWebPlugin).IsAssignableFrom(t))
                                        {
                                            if (!t.IsInterface && t.IsClass && !t.IsAbstract)
                                            {
                                                config.PluginType = t;
                                            }
                                        }
                                    }
                                    if (config.ConfigClassType == null)
                                    {
                                        if (typeof(IPluginConfigClass).IsAssignableFrom(t))
                                        {
                                            if (!t.IsInterface && t.IsClass && !t.IsAbstract)
                                            {
                                                config.ConfigClassType = t;
                                            }
                                        }
                                    }
                                    if ((config.PluginType != null) && (config.ConfigClassType != null))
                                    {
                                        break;
                                    }
                                }
                            }

                            installedPlugins.Add(config);
                        }
                        catch (Exception ex)
                        {
                            Exception fail = new Exception("Could not initialize plugin folder: " + ex.Message, ex);
                            Debug.WriteLine(fail.Message);
                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exception fail = new Exception("Could not initialize plugin folder", ex);
                    Debug.WriteLine(fail.Message, fail);
                    throw fail;
                }

                LoadedPlugins = installedPlugins;
            }
        }

        private static Assembly PerformFileDeploy(FileInfo plug)
        {
            if (plug.Directory.Parent == null)
            {
                throw new InvalidOperationException("The plugin directory for the " + plug.Name + " file exists in a folder outside of the allowed folder heirarchy");
            }

            FileInfo shadowCopiedPlugin = null;

            if (WebCoreUtility.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            {
                //all plugins will need to be copied to ~/Plugins/bin/
                //this is aboslutely required because all of this relies on probingPaths being set statically in the web.config
                //were running in med trust, so copy to custom bin folder
                DirectoryInfo shadowCopyPluginFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
                shadowCopiedPlugin = InitializeMediumTrust(plug, shadowCopyPluginFolder);
            }
            else
            {
                string directory = AppDomain.CurrentDomain.DynamicDirectory;
                Debug.WriteLine(plug.FullName + " to " + directory);
                //were running in full trust so copy to standard dynamic folder
                shadowCopiedPlugin = InitializeFullTrust(plug, new DirectoryInfo(directory));
            }

            try
            {
                //we can now register the plugin definition
                Assembly shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlugin.FullName));

                //add the reference to the build manager
                Debug.WriteLine("Adding to BuildManager: '{0}'", shadowCopiedAssembly.FullName);
                BuildManager.AddReferencedAssembly(shadowCopiedAssembly);
                return shadowCopiedAssembly;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to load assembly: " + ex.Message);
                return null;
            }
        }

        private static FileInfo InitializeFullTrust(FileInfo pluginFileInfo, DirectoryInfo shadowCopyPluginFolder)
        {
            FileInfo shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPluginFolder.FullName, pluginFileInfo.Name));
            try
            {
                File.Copy(pluginFileInfo.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " has denied access, attempting to rename");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    string oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin");
                    throw;
                }
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    string oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin");
                    throw;
                }
                //ok, we've made it this far, now retry the shadow copy
                File.Copy(pluginFileInfo.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }
        private static FileInfo InitializeMediumTrust(FileInfo pluginFileInfo, DirectoryInfo shadowCopyPluginFolder)
        {
            bool shouldCopy = true;
            FileInfo shadowCopiedPlugin = new FileInfo(Path.Combine(shadowCopyPluginFolder.FullName, pluginFileInfo.Name));

            //check if a shadow copied file already exists and if it does, check if its updated, if not don't copy
            if (shadowCopiedPlugin.Exists)
            {
                if (shadowCopiedPlugin.CreationTimeUtc.Ticks == pluginFileInfo.CreationTimeUtc.Ticks)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlugin.Name);
                    shouldCopy = false;
                }
            }

            if (shouldCopy)
            {
                try
                {
                    File.Copy(pluginFileInfo.FullName, shadowCopiedPlugin.FullName, true);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine(shadowCopiedPlugin.FullName + " has denied access, attempting to rename");
                    //this occurs when the files are locked,
                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                    try
                    {
                        string oldFile = shadowCopiedPlugin.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlugin.FullName, oldFile);
                    }
                    catch (IOException)
                    {
                        Debug.WriteLine(shadowCopiedPlugin.FullName + " rename failed, cannot initialize plugin");
                        throw;
                    }
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlugin.FullName + " is locked, attempting to rename");
                    //this occurs when the files are locked,
                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                    try
                    {
                        string oldFile = shadowCopiedPlugin.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlugin.FullName, oldFile);
                    }
                    catch (IOException)
                    {
                        Debug.WriteLine(shadowCopiedPlugin.FullName + " rename failed, cannot initialize plugin");
                        throw;
                    }
                    //ok, we've made it this far, now retry the shadow copy
                    File.Copy(pluginFileInfo.FullName, shadowCopiedPlugin.FullName, true);
                }
            }

            return shadowCopiedPlugin;
        }

        private static bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder == null)
            {
                return false;
            }
            if (folder.Parent == null)
            {
                return false;
            }
            if (!folder.Parent.Name.Equals(WebAssumptions.WEB_PLUGIN_FOLDER_NAME, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
        private static PluginConfig ParsePluginConfigFile(string filePath)
        {
            string fileContents = File.ReadAllText(filePath);

            PluginConfig config = new PluginConfig();
            if (!string.IsNullOrEmpty(fileContents))
            {
                string[] settings = fileContents.Replace("\r","").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string setting in settings)
                {
                    int separatorIndex = setting.IndexOf(':');
                    if (separatorIndex == -1)
                    {
                        continue;
                    }
                    string key = setting.Substring(0, separatorIndex).Trim();
                    string value = setting.Substring(separatorIndex + 1).Trim();

                    switch (key)
                    {
                        case "Feature":
                            config.Feature = value;
                            break;
                        case "FriendlyName":
                            config.FriendlyName = value;
                            break;
                        case "SystemName":
                            config.SystemName = value;
                            break;
                        case "Version":
                            config.Version = value;
                            break;
                        case "Author":
                            config.Author = value;
                            break;
                        case "Description":
                            config.Description = value;
                            break;
                        case "AssemblyFileName":
                            config.AssemblyFileName = value;
                            break;
                        default:
                            break;
                    }
                }
            }
            return config;
        }

    }
}
