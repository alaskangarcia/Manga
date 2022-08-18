using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga
{
    public class Chapter
    {
        public string root { get; set; }
        public string Id { get; set; }
        public float chapterN { get; set; }
        public int pages { get; set; }
        public List<string> pagesUri { get; set; }
        public string chapterPath { get; set; }
        public string scanID { get; set; }
        public string scanNAme { get; set; }
        public bool isDownloaded { get; set; }
        public int volume { get; set; }
        public bool isRead { get; set; }
        public Chapter()
        {

        }
        public Chapter(string id, int pages, string chapterN, string scanId)
        {
            this.pages = pages;
            Id = id;
            pagesUri = new List<string>(0);
            this.scanID = scanId;
            if (chapterN != null)
            {
                if (float.TryParse(chapterN, out float pars))
                {
                    this.chapterN = pars;
                }
                else { this.chapterN = -1f; }
            }
            else { this.chapterN = -1f; }
        }

        public string getPageUriString(int index)
        {
            return pagesUri![index];
        }
        public string getPageUriLastString()
        {
            return pagesUri!.Last();
        }
        public Chapter Empty()
        {
            return new Chapter("null", 0, "null", "null");
        }
    }
}
