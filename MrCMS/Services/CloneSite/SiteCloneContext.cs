using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;

namespace MrCMS.Services.CloneSite
{
    public class SiteCloneContext
    {
        public SiteCloneContext()
        {
            Entries = new HashSet<SiteCloneContextEntry>();
        }

        public HashSet<SiteCloneContextEntry> Entries { get; private set; }

        public void AddEntry<T>(T original, T newEntity) where T : SystemEntity
        {
            Entries.Add(new SiteCloneContextEntry
            {
                Id = original.Id,
                Type = original.GetType(),
                Original = original,
                NewEntity = newEntity
            });
        }

        public T FindNew<T>(int id) where T : SystemEntity
        {
            SiteCloneContextEntry entry = Entries.FirstOrDefault(x => typeof (T).IsAssignableFrom(x.Type) && x.Id == id);

            if (entry != null) return entry.NewEntity as T;
            return null;
        }

        public T GetOriginal<T>(T newEntity) where T : SystemEntity
        {
            SiteCloneContextEntry entry = Entries.FirstOrDefault(x => x.NewEntity == newEntity);
            if (entry != null) return entry.Original as T;
            return null;
        }
    }
}