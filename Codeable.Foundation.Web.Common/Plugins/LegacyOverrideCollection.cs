using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;

namespace Codeable.Foundation.UI.Web.Common.Plugins
{
    // prevents reference duplicates
    public class LegacyOverrideCollection : Collection<LegacyOverride>
    {
        public LegacyOverrideCollection()
        {
            _nameLookup = new Dictionary<string, LegacyOverride>(StringComparer.OrdinalIgnoreCase);
            _readerWriterLock = new ReaderWriterLockSlim();
        }

        private ReaderWriterLockSlim _readerWriterLock;
        private Dictionary<string, LegacyOverride> _nameLookup;

        public void Add(string name, LegacyOverride legacyOverride)
        {
            if (legacyOverride == null) { throw new ArgumentNullException("legacyOverride"); }
            if (!string.IsNullOrEmpty(name) && _nameLookup.ContainsKey(name))
            {
                throw new ArgumentException(string.Format("An item with the name {0} already exists in the collection. You must remove or replace it.", name));
            }
            base.Add(legacyOverride);
            if (!string.IsNullOrEmpty(name))
            {
                _nameLookup[name] = legacyOverride;
            }
        }

        public LegacyOverride this[string name]
        {
            get
            {
                LegacyOverride result = null;
                if (!string.IsNullOrEmpty(name) && _nameLookup.TryGetValue(name, out result))
                {
                    return result;
                }
                return null;
            }
        }

        protected override void InsertItem(int index, LegacyOverride item)
        {
            if (item == null) { throw new ArgumentNullException("item"); }
            if (base.Contains(item))
            {
                throw new ArgumentException("The item already exists in the collection.");
            }
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            this.RemoveNamedOfIndex(index);
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            _nameLookup.Clear();
            base.ClearItems();
        }
        protected override void SetItem(int index, LegacyOverride item)
        {
            if (item == null) { throw new ArgumentNullException("item"); }
            if (base.Contains(item))
            {
                throw new ArgumentException("The item already exists in the collection.");
            }
            this.RemoveNamedOfIndex(index);
            base.SetItem(index, item);
        }

        private void RemoveNamedOfIndex(int index)
        {
            LegacyOverride found = base[index];
            foreach (KeyValuePair<string, LegacyOverride> pair in _nameLookup)
            {
                if (pair.Value == found)
                {
                    this._nameLookup.Remove(pair.Key);
                    break;
                }
            }
        }

    }
}
