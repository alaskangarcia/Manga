using System.Collections.Generic;
using System.Linq;

namespace Manga
{
    public class mangaController
    {
        public List<MangaObj> Mangas { get; set; }
        private MangaObj lastAdded { get; set; }
        public string path { get; set; }
        public Dictionary<string, string> scanGroups { get; set; }
        public mangaController()
        {
            Mangas = new List<MangaObj>(0);
        }
        public void newManga(string id, string name, string description, string cover)
        {
            foreach(var manga in Mangas)
            {
                if(manga.ID == id)
                {
                    lastAdded = manga;
                    return;
                }
            }
            MangaObj obj = new MangaObj(name, id, description, cover);
            add(obj);
        }

        public void add(MangaObj manga)
        {
            lastAdded = manga;
            Mangas.Add(manga);
            Mangas.Sort(delegate (MangaObj o1, MangaObj o2) { return o1.Name.CompareTo(o2.Name); });
        }
        public MangaObj getLatest()
        {
            return lastAdded;
        }
        public void setLatest(string id)
        {
            foreach (MangaObj obj in Mangas)
            {
                if (obj.ID == id)
                {
                    lastAdded = obj;
                    return;
                }
            }
        }

        public bool contains(MangaObj obj)
        {
            return Mangas.Contains(obj);
        }

        public bool remove(MangaObj obj)
        {
            if (Mangas.Contains(obj))
            {
                return Mangas.Remove(obj);
            }
            return false;
        }

        public string getGroup(string id)
        {
            if(scanGroups == null)
            {
                scanGroups = new Dictionary<string, string>();
            }
            if (scanGroups.ContainsKey(id))
            {
                scanGroups.TryGetValue(id, out var group);
                return filterChar(group);
            }
            return "error";
        }
        public void setGroup(string id, string value)
        {
            if (scanGroups.ContainsKey(id))
            {
                scanGroups[id] = filterChar(value);
            }
            else
            {
                scanGroups.Add(id, filterChar(value));
            }
        }
        private string filterChar(string fl)
        {
            char[] badchars = { '/', (char)92, ':', '*', '"', '?', '|', '<', '>' };
            foreach (char ch in badchars)
            {
                fl = fl.Replace(ch, (char)32);
            }
            return cleanChars(fl);
        }
        private string cleanChars(string cC)
        {
            cC = cC.Trim();
            cC = cC.Replace("  ", " ");
            return cC;
        }
    }
}
