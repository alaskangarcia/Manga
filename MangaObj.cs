using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga
{
    public class MangaObj
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ID { get; set; }
        public string[] Cover { get; set; }
        public string CoverPath { get; set; }
        public List<Chapter> chapters { get; set; }
        public string folder { get; set; }
        public bool longstrip { get; set; }
        public bool content { get; set; }
        public Dictionary<float, List<Chapter>> ChaptersDic { get; set; }

        private Chapter currentChapter;
        public DateTime lastUpdate { get; set; }
        public MangaObj()
        {
            longstrip = false;
            content = false;
            lastUpdate = DateTime.Now;
        }

        public MangaObj(string nAME) : this()
        {

            Name = nAME;
            chapters = new List<Chapter>(0);
        }
        public MangaObj(string name, string iD) : this(name)
        {
            ID = iD;
        }

        public MangaObj(string nAME, string iD, string description) : this(nAME, iD)
        {
            Description = description;
        }

        public MangaObj(string name, string iD, string description, string cOVER) : this(name, iD, description)
        {
            Cover = new string[1];
            Cover[0] = cOVER;
        }
        public MangaObj(string name, string iD, string description, string cover, string coverPath) : this(name, iD, description, cover)
        {
            CoverPath = coverPath;
        }

        public string toString()
        {
            return $"{Name}\nChapters {chapters.Count()}\n{Description}";
        }

        public void addChapter(Chapter chapter)
        {
            lastUpdate = DateTime.Now;
            chapters.Add(chapter);
            chapters.Sort(delegate (Chapter o1, Chapter o2) { return o1.chapterN.CompareTo(o2.chapterN); });
        }

        public Chapter nextChapter()
        {
            foreach (Chapter chapter in chapters)
            {
                if (chapter.chapterN > currentChapter.chapterN) { return chapter; }
            }
            return new Chapter().Empty();
        }
        public Chapter previousChapter()
        {
            List<Chapter> rChapters = getReverse(chapters);
            foreach (Chapter chapter in rChapters)
            {
                if (currentChapter.chapterN < chapter.chapterN)
                {
                    currentChapter = chapter;
                    return chapter;
                }
            }
            return new Chapter().Empty();
        }
        public void setChapter(int i)
        {
            if (i < chapters.Count)
            {
                currentChapter = chapters[i];
            }
            else
            {
                currentChapter = new Chapter().Empty();
            }
        }
        public Chapter getChapter(int i)
        {
            currentChapter = chapters[i];
            return chapters[i];
        }
        public Chapter getFirstChapter()
        {
            currentChapter = chapters[0];
            return chapters[0];
        }
        private List<Chapter> getReverse(List<Chapter> rChapter)
        {
            List<Chapter> rChapters = new List<Chapter>(rChapter.Count);
            int i = rChapter.Count - 1;
            foreach (Chapter c in rChapter)
            {
                rChapters[i] = c;
            }
            return rChapters;
        }
        public bool hasChapID(string id)
        {
            foreach (Chapter chap in chapters)
            {
                if (chap.Id == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
