using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.UI.Web.Core.MVC.Embedded
{
    [Serializable]
    public class WebPluginEmbeddedCache
    {
        public WebPluginEmbeddedCache()
        {
            _embeddedPermittedCache = new Dictionary<string, List<WebPluginEmbeddedInfo>>(StringComparer.OrdinalIgnoreCase);
            _embeddedCompleteCache = new Dictionary<string, List<WebPluginEmbeddedInfo>>(StringComparer.OrdinalIgnoreCase);
            _embeddedResultCache = new Dictionary<string, WebPluginEmbeddedInfo>(StringComparer.OrdinalIgnoreCase);
        }

        private static readonly object _syncLockRoot = new object();
        private readonly Dictionary<string, List<WebPluginEmbeddedInfo>> _embeddedCompleteCache;
        private readonly Dictionary<string, List<WebPluginEmbeddedInfo>> _embeddedPermittedCache;
        private readonly Dictionary<string, WebPluginEmbeddedInfo> _embeddedResultCache;

        public virtual void AddEmbeddedItem(WebPluginEmbeddedInfo item)
        {
            lock (_syncLockRoot)
            {
                if (!_embeddedCompleteCache.ContainsKey(item.VirtualItemName))
                {
                    _embeddedCompleteCache[item.VirtualItemName] = new List<WebPluginEmbeddedInfo>();
                }
                if (!_embeddedPermittedCache.ContainsKey(item.VirtualItemName))
                {
                    _embeddedPermittedCache[item.VirtualItemName] = new List<WebPluginEmbeddedInfo>();
                }
                _embeddedPermittedCache[item.VirtualItemName].Add(item);
                _embeddedCompleteCache[item.VirtualItemName].Add(item);
                _embeddedResultCache[item.VirtualItemName] = item;
            }
        }
        public virtual void ClearAllItems()
        {
            lock (_syncLockRoot)
            {
                _embeddedCompleteCache.Clear();
                _embeddedResultCache.Clear();
                _embeddedPermittedCache.Clear();
            }
        }
        public virtual WebPluginEmbeddedInfo[] GetAllEmbeddedItems()
        {
            lock (_syncLockRoot)
            {
                return _embeddedResultCache.Values.ToArray();
            }
        }

        public virtual void TrimAndArrange(Func<WebPluginEmbeddedInfo, bool> permitted, Comparison<WebPluginEmbeddedInfo> comparison)
        {
            lock (_syncLockRoot)
            {
                _embeddedPermittedCache.Clear();
                foreach (var item in _embeddedCompleteCache)
                {
                    _embeddedResultCache.Remove(item.Key);
                    if (item.Value.Count > 1)
                    {
                        item.Value.Sort(comparison);
                    }
                    WebPluginEmbeddedInfo highestPermitted = null;
                    foreach (WebPluginEmbeddedInfo info in item.Value)
                    {
                        if (permitted(info))
                        {
                            highestPermitted = info;
                            if (!_embeddedPermittedCache.ContainsKey(item.Key))
                            {
                                _embeddedPermittedCache[item.Key] = new List<WebPluginEmbeddedInfo>();
                            }
                            _embeddedPermittedCache[item.Key].Add(info);
                        }
                    }
                    if (highestPermitted != null)
                    {
                        _embeddedResultCache[item.Key] = highestPermitted;
                    }
                }
            }
        }
        
        public virtual bool ContainsEmbeddedItem(string virtualItemName)
        {
            virtualItemName = TranslateVirtualPath(virtualItemName);
            return (!string.IsNullOrEmpty(virtualItemName) && _embeddedCompleteCache.ContainsKey(virtualItemName) && _embeddedCompleteCache[virtualItemName].Count > 0);
        }
        public virtual bool HasPermittedEmbeddedItem(string virtualItemName)
        {
            virtualItemName = TranslateVirtualPath(virtualItemName);
            return (!string.IsNullOrEmpty(virtualItemName) && _embeddedResultCache.ContainsKey(virtualItemName));
        }
        public virtual WebPluginEmbeddedInfo GetEmbeddedItem(string virtualItemName)
        {
            virtualItemName = TranslateVirtualPath(virtualItemName);
            if (!string.IsNullOrEmpty(virtualItemName) && _embeddedResultCache.ContainsKey(virtualItemName))
            {
                return _embeddedResultCache[virtualItemName];
            }
            return null;
        }
        public virtual IEnumerable<WebPluginEmbeddedInfo> GetChainedEmbeddedItems(string virtualItemName)
        {
            virtualItemName = TranslateVirtualPath(virtualItemName);
            if (!string.IsNullOrEmpty(virtualItemName) && _embeddedPermittedCache.ContainsKey(virtualItemName))
            {
                return _embeddedPermittedCache[virtualItemName];
            }
            return null;
        }

        protected virtual string TranslateVirtualPath(string virtualItemName)
        {
            return virtualItemName.Replace(@"~/", string.Empty).Replace(@"/",".");
        }
    }
}
