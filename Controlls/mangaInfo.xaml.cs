using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;

namespace Manga.Controlls
{
    /// <summary>
    /// Interaction logic for mangaInfo.xaml
    /// </summary>
    public partial class mangaInfo : UserControl
    {
        HttpClient client = new HttpClient();
        bool isAsc = true;
        const string orderDesc = "Order: Desc";
        const string orderAsc = "Order: Asc";
        bool contains = false;
        public Chapter? first { get; set; }
        public Chapter? fromRead { get; set; }
        public bool longstrip = false;
        mangaController controller;
        int del = 3;
        bool ispause = false;
        bool isdownload = false;

        bool ispauseUp = false;
        bool isdownloadUp = false;

        const string CONTENT = "&contentRating%5B%5D=safe&contentRating%5B%5D=suggestive&contentRating%5B%5D=erotica&contentRating%5B%5D=pornographic";
        const string LANG = "&translatedLanguage[]=en";
        const string ORDER = "&order[chapter]=asc";
        const string BLACKLIST = "&excludedGroups[]=4f1de6a2-f0c5-4ac5-bce5-02c7dbb67deb&excludedGroups[]=8d8ecf83-8d42-4f8c-add8-60963f9f28d9&excludedGroups[]=06a9fecb-b608-4f19-b93c-7caab06b7f44";
        const string searchArgs = CONTENT + LANG + ORDER + BLACKLIST;
        public mangaInfo(mangaController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        public mangaDisplay mangaDisplay
        {
            get { return (mangaDisplay)GetValue(mangaDisplayProperty); }
            set { SetValue(mangaDisplayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for mangaDisplay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mangaDisplayProperty =
            DependencyProperty.Register("mangaDisplay", typeof(mangaDisplay), typeof(mangaInfo), new PropertyMetadata(mangaDisplayPropertyChanged));

        private static void mangaDisplayPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) 
        {
            mangaInfo mangaInfo = sender as mangaInfo;
            if(mangaInfo != null)
            {
                mangaInfo.mangaDisplayPropertyChangedCallBack();
            }
        }
        private void mangaDisplayPropertyChangedCallBack()
        {
            mDis.Children.Add(mangaDisplay);
            manga = mangaDisplay.manga;
            longstrip = manga.longstrip;
            if (contains)
            {
                dateTime.Text = "Last Updated At: " + manga.lastUpdate.ToString();
            }
            else
            {
                dateTime.Text = "Not Saved";
                saveBut.Visibility = Visibility.Visible;
            }
            
        }
        private bool CheckConnection()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }
            if (!InternetAvailability.InternetAvailability_IsAvailable())
            {
                return false;
            }
            return true;
        }

        public MangaObj manga
        {
            get { return (MangaObj)GetValue(mangaProperty); }
            set { SetValue(mangaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for manga.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mangaProperty =
            DependencyProperty.Register("manga", typeof(MangaObj), typeof(mangaInfo), new PropertyMetadata(mangaPropertyChanged));
        
        private static void mangaPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            mangaInfo? mangaInfo= sender as mangaInfo;
            if(mangaInfo != null)
            {
                mangaInfo.mangaPropertyChangedCallBack();
            }
        }

        private void mangaPropertyChangedCallBack()
        {
            contains = controller.contains(manga);
            if (manga.chapters.Count == 0)
            {
                return;
            }

            first = manga.chapters[0];
            if (manga.ChaptersDic == null)
            {
                updateDic();
                RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.SaveListEvent);
                RaiseEvent(routed);
            }
            updateList();
        }

        public SolidColorBrush BackG
        {
            get { return (SolidColorBrush)GetValue(BackGProperty); }
            set { SetValue(BackGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackGProperty =
            DependencyProperty.Register("BackG", typeof(SolidColorBrush), typeof(mangaInfo), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        public SolidColorBrush ForG
        {
            get { return (SolidColorBrush)GetValue(ForGProperty); }
            set { SetValue(ForGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForGProperty =
            DependencyProperty.Register("ForG", typeof(SolidColorBrush), typeof(mangaInfo), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        
        private void orderBut_Tap(object sender, RoutedEventArgs e)
        {
            if (isAsc)
            {
                orderBut.Text = orderDesc;
            }
            else
            {
                orderBut.Text = orderAsc;
            }
            reverseChapters();
            isAsc = !isAsc;
        }
        private void reverseChapters()
        {
            int rem = cLists.Children.Count-1;
            for (int i = 0; i < cLists.Children.Count-1; i++)
            {
                UIElement tmp = cLists.Children[rem];
                cLists.Children.RemoveAt(rem);
                cLists.Children.Insert(i, tmp);
            }
        }
        private void readBut_Tap(object sender, RoutedEventArgs e)
        {
            if (first != null)
            {
                RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.ReadFirstEvent);
                RaiseEvent(routed);
            }
        }

        private void continueBut_Tap(object sender, RoutedEventArgs e)
        {
            foreach(Chapter chapter in manga.chapters)
            {
                if (!chapter.isRead)
                {
                    fromRead = chapter;
                    RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.ContinueReadEvent);
                    RaiseEvent(routed);
                    return;
                }
            }
        }
        public static readonly RoutedEvent ContinueReadEvent = EventManager.RegisterRoutedEvent(
            name: "ContinueRead",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(mangaInfo));

        public event RoutedEventHandler ContinueRead
        {
            add { AddHandler(ContinueReadEvent, value); }
            remove { RemoveHandler(ContinueReadEvent, value); }
        }

        public static readonly RoutedEvent ReadFirstEvent = EventManager.RegisterRoutedEvent(
            name: "ReadFirst",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(mangaInfo));

        public event RoutedEventHandler ReadFirst
        {
            add { AddHandler(ReadFirstEvent, value); }
            remove { RemoveHandler(ReadFirstEvent, value); }
        }



        public static readonly RoutedEvent SaveListEvent = EventManager.RegisterRoutedEvent(
            name: "SaveList",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(mangaInfo));

        public event RoutedEventHandler SaveList
        {
            add { AddHandler(SaveListEvent, value); }
            remove { RemoveHandler(SaveListEvent, value); }
        }
        private void saveBut_Tap(object sender, RoutedEventArgs e)
        {
            controller.add(manga);
            contains = true;
            dateTime.Text = "Last Updated At: " + manga.lastUpdate.ToString();
            saveBut.Visibility = Visibility.Collapsed;
            RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.SaveListEvent);
            RaiseEvent(routed);
        }
        private async void updateBut_Tap(object sender, RoutedEventArgs e)
        {

            if (!CheckConnection())
            {
                updateBut.Text = "No Internet";
                return;
            }
           
            if (ispauseUp && isdownloadUp)
            {
                isdownloadUp = false;
                updateBut.Text = "Stopping";
            }
            if (!ispauseUp && !isdownloadUp)
            {
                updateBut.Text = "Pause Update";
                ispauseUp = true;
                isdownloadUp = true;
                await updateChapters();
            }
            
        }

        private async Task updateChapters()
        {
            
            int total;
            int offset = 0;
            int querry = 100;

            JObject json;
            do
            {
                HttpResponseMessage message = await client.GetAsync(@$"https://api.mangadex.org/chapter/?limit={querry}&offset={offset}&manga={manga.ID}{searchArgs}");
                json = JObject.Parse(await message.Content.ReadAsStringAsync());
                foreach (var chapter in json["data"]!)
                {
                    if (!manga.hasChapID(chapter["id"]!.ToString()))
                    {
                        Chapter chap = new Chapter(chapter["id"]!.ToString(), (int)chapter["attributes"]!["pages"]!, chapter["attributes"]!["chapter"]!.ToString(), chapter["relationships"]![0]!["id"]!.ToString());
                        chap.root = manga.folder;
                        chap.chapterPath = manga.folder + "";
                        getScan(chap);
                        manga.addChapter(chap);
                    }
                    if (!isdownloadUp)
                    {
                        updateDic();
                        updateList();
                        ispauseUp = false;
                        updateBut.Text = "Update";
                        RoutedEventArgs rout = new RoutedEventArgs(mangaInfo.SaveListEvent);
                        RaiseEvent(rout);
                        return;
                    }
                }
                offset += querry;
                total = (int)json["total"]!;
                updateDic();
                updateList();
                RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.SaveListEvent);
                RaiseEvent(routed);
            } while (offset < total);
            isdownloadUp = false;
            ispause = false;
            updateBut.Text = "Updated";
            //updateDic();
            //updateList();
        }

        private void updateList()
        {
            cLists.Children.Clear();
            foreach (KeyValuePair<float, List<Chapter>> chapters1 in manga.ChaptersDic)
            {
                chapterList list = new chapterList(controller,manga);
                list.longs = manga.longstrip;
                list.BackG = BackG;
                list.ForG = ForG;
                list.ChapterDic = manga.ChaptersDic;
                list.Margin = new Thickness(10);
                list.Chapters = chapters1.Value;
                cLists.Children.Add(list);
            }
            isAsc = true;
            orderBut.Text = orderAsc;
        }

        private void updateDic()
        {
            Dictionary<float, List<Chapter>> dic = new();
            List<float> cursed = new List<float>(0);
            foreach (Chapter chapter in manga.chapters)
            {
                if (!cursed.Contains(chapter.chapterN))
                {
                    cursed.Add(chapter.chapterN);
                }
            }
            foreach (float ch in cursed)
            {
                dic.Add(ch, new List<Chapter>());
            }
            foreach (Chapter chap in manga.chapters)
            {
                dic[chap.chapterN].Add(chap);
            }
            manga.ChaptersDic = dic;
        }

        
        private async void getScan(Chapter chapter)
        {
            string name = controller.getGroup(chapter.scanID);
            if(name == "error" || name == "err")
            {
                controller.setGroup(chapter.scanID, await updateScan(chapter));
                chapter.scanNAme = controller.getGroup(chapter.scanID);
                RoutedEventArgs routed = new RoutedEventArgs(chapterListSingle.SaveEvent);
                RaiseEvent(routed);
            }
            else
            {
                chapter.scanNAme = name;
            }
        }
        private async Task<string> updateScan(Chapter chapter)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return "err";
            }
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync($@"https://api.mangadex.org/group/{chapter.scanID}");
            try
            {
                JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());

                return json["data"]["attributes"]["name"].ToString();
            }
            catch
            {
                return "err";
            }
        }

        private void delBut_Tap(object sender, RoutedEventArgs e)
        {
            switch (del)
            {
                case 3: delBut.Text = "Are you sure?"; del--; break;
                case 2: delBut.Text = "Really?"; del--; break;
                case 1: delBut.Text = "Ok"; del--; break;
                case 0: delBut.Text = "Done"; delete(); break;
            }
        }
        
        private void delete()
        {
            cLists.Children.Clear();
            mDis.Children.Clear();
            controller.remove(manga);
            RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.SaveListEvent);
            RaiseEvent(routed);
            RoutedEventArgs routed2 = new RoutedEventArgs(mangaInfo.CloseInfoEvent);
            RaiseEvent(routed2);
        }

        public static readonly RoutedEvent CloseInfoEvent = EventManager.RegisterRoutedEvent(
            name: "CloseInfo",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(mangaInfo));

        public event RoutedEventHandler CloseInfo
        {
            add { AddHandler(CloseInfoEvent, value); }
            remove { RemoveHandler(CloseInfoEvent, value); }
        }
        private void root_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                delBut.Visibility = Visibility.Visible;
            }
        }

        private void root_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(this.Visibility == Visibility.Visible)
            {
                Focus();
            }
        }

        private void root_Save(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.SaveListEvent);
            RaiseEvent(routed);
        }

        private async void downloadBut_Tap(object sender, RoutedEventArgs e)
        {
            if (!CheckConnection())
            {
                downloadBut.Text = "No Internet";
                return;
            }
            if (ispause && isdownload)
            {
                isdownload = false;
                downloadBut.Text = "Stoping";
                return;
            }

            if (!ispause && !isdownload)
            {
                isdownload = true;
                ispause = true;
                downloadBut.Text = "Stop Download";
                foreach (Chapter chap in manga.chapters)
                {
                    if (!chap.isDownloaded)
                    {
                        await downloadChapter(chap);
                        updateDic();
                        updateList();
                        RoutedEventArgs routed = new RoutedEventArgs(mangaInfo.SaveListEvent);
                        RaiseEvent(routed);
                    }
                    if (!isdownload)
                    {
                        ispause = false;
                        downloadBut.Text = "Download All";
                        return;
                    }
                }
                isdownload = false;
                ispause=false;
                downloadBut.Text = "Downloaded";
            }


            updateDic();
            updateList();
        }

        private async Task downloadChapter(Chapter chapter)
        {
            HttpResponseMessage message = await client.GetAsync(@$"https://api.mangadex.org/at-home/server/{chapter.Id}");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (json["result"].ToString() == "ok")
            {
                string npath = "";
                await createChapterFolder(chapter);
                string hash = json["chapter"]["hash"].ToString();
                List<Task> tasks = new List<Task>();
                foreach(string page in json["chapter"]["dataSaver"])
                {
                    await Task.Run(() => npath = chapter.chapterPath + @"\" + page);
                    await Task.Run(() => chapter.pagesUri.Add(npath));
                    
                    if (!File.Exists(npath))
                    {
                        try
                        {
                            tasks.Add(downloadPage(hash,page,npath));
                        }
                        catch
                        {
                            File.Delete(npath);
                        }
                        await Task.WhenAll(tasks);
                    }
                    chapter.isDownloaded = true; 
                }
            }
        }

        private async Task downloadPage(string hash, string page, string path)
        {
            HttpResponseMessage message = await client.GetAsync(@$"https://uploads.mangadex.org/data-saver/{hash}/{page}");
            byte[] img = await message.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(path, img);
        }
        private async Task createChapterFolder(Chapter chapter)
        {
            string npath = chapter.root + @"\Chapter " + chapter.chapterN + $" ({controller.getGroup(chapter.scanID)})";
            if (!Directory.Exists(npath)) { Directory.CreateDirectory(npath); }
            chapter.chapterPath = npath;
        }

        private void root_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.Visibility == Visibility.Visible)
            {
                Focus();
                updateDic();
                updateList();
            }
        }

    }
}
