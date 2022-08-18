using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Manga.Controlls.Search
{
    /// <summary>
    /// Interaction logic for searchName.xaml
    /// </summary>
    public partial class searchName : UserControl
    {
        HttpClient client;
        const string baseurlManga = @"https://api.mangadex.org/manga/";
        const string CONTENT = "&contentRating%5B%5D=safe&contentRating%5B%5D=suggestive&contentRating%5B%5D=erotica&contentRating%5B%5D=pornographic";
        const string LANG = "&availableTranslatedLanguage[]=en";
        const string RELEVANCE = "&order[relevance]=desc";
        const string HASAVAILABLECHAPTERS = "&hasAvailableChapters=true";
        const string basesearchArgs = CONTENT+LANG+HASAVAILABLECHAPTERS;
        const string baseurlCover = @"https://api.mangadex.org/cover/";
        List<MemoryStream> ms = new List<MemoryStream>(0);
        List<BitmapImage> images = new List<BitmapImage>(0);
        mangaController controller;
        public MangaObj clickedObj { get; set; }

        public searchName()
        {
            InitializeComponent();
        }
        public searchName(mangaController controller, HttpClient client)
        {
            InitializeComponent();
            this.controller = controller;
            this.client = client;
        }
        private async void searchBar_Tap(object sender, RoutedEventArgs e)
        {
            //this.Visibility = System.Windows.Visibility.Collapsed;
            Focus();
            await search();
        }
        private bool CheckConnection()
        {
            byte[] googledns = { 0x08, 0x08, 0x08, 0x08 };
            Ping ping = new Ping();
            PingReply reply = ping.Send(new IPAddress(googledns));
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            return false;
        }

        private async Task search()
        {
            
            string search = search_Bar.SearchText;
            HttpResponseMessage request;
            request = await client.GetAsync(baseurlManga +$"?title={search}"+basesearchArgs);
            JObject json = JObject.Parse(await request.Content.ReadAsStringAsync());
            List<Task> tasks = new List<Task>(0);
            foreach (var tok in json["data"]!)
            {
                //await processManga(tok);
                tasks.Add(processManga(tok));
            }
            //await Task.WhenAll(tasks);
        }

        private async Task processManga(JToken json)
        {
            MangaObj manga;
            foreach(MangaObj obj in controller.Mangas)
            {
                if(obj.ID == json["id"].ToString())
                {
                    manga = (MangaObj)obj;
                    displayManga(manga);
                    return;
                }
            }
            if (!json["attributes"]["description"].HasValues)
            {
                tmpT.Text += "returned";
                return;
            }
            string coverart = "";
            foreach (var a in json["relationships"]!)
            {
                if (a["type"].ToString() == "cover_art")
                {
                    coverart = a["id"].ToString();
                }
            }
            tmpT.Text += " "+coverart+" ";

            manga = new MangaObj(filterChar(json["attributes"]["title"]["en"].ToString()), json["id"].ToString(), json["attributes"]["description"]["en"].ToString(), coverart);
            foreach (var b in json["attributes"]["tags"])
            {
                if (b["id"].ToString() == "3e2b8dae-350e-4ab8-a8ce-016e844b9f0d")
                {
                    manga.longstrip = true;
                }
            }
            if (json["attributes"]!["contentRating"]!.ToString() == "pornographic")
            {
                manga.content = true;
            }
            manga.folder = controller.path + @"\" + manga.Name;
            await downloadtmpCover(manga);
        }

        private async Task downloadtmpCover(MangaObj obj)
        {
            HttpResponseMessage message = await client.GetAsync(baseurlCover + obj.Cover[0]);
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            string cover = json["data"]["attributes"]["fileName"].ToString();

            string cpath = obj.folder + @"\" + cover;
            obj.CoverPath = cpath;
            byte[] img = await client.GetByteArrayAsync($@"https://uploads.mangadex.org/covers/{obj.ID}/{cover}");
            tmpT.Text += " " + img.Length;
            BitmapImage image = new BitmapImage();
            MemoryStream mss = new MemoryStream(img);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.BeginInit();
            image.StreamSource = mss;
            image.EndInit();
            image.Freeze();
            displayManga(obj, image, img);
        }
        private void displayManga(MangaObj obj, BitmapImage image, byte[] cover)
        {
            mangaDisplay display = new mangaDisplay(obj, image, cover);
            display.ForG = ForG;
            display.BackG = BackG;
            display.Margin = new Thickness(10);
            display.imageWidth = 100;
            mangaHolder.Children.Add(display);
        }
        private void displayManga(MangaObj obj)
        {
            mangaDisplay display = new mangaDisplay(obj);
            display.ForG = ForG;
            display.BackG = BackG;
            display.Margin = new Thickness(10);
            display.imageWidth = 100;
            mangaHolder.Children.Add(display);
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
        public void dispose()
        {
            client.Dispose();
        }
        private void mangaDis_Tap(object sender, RoutedEventArgs e)
        {
            mangaDisplay display = e.OriginalSource as mangaDisplay;
            if (display.type == 2)
            {
                createFolder(display.manga, display.type2Cover);
            }
            clickedObj = display.manga;
            //controller.add(clickedObj);
            RoutedEventArgs routed = new RoutedEventArgs(MangaDisRaiseEvent);
            RaiseEvent(routed);
        }

        private void createFolder(MangaObj obj, byte[] cover)
        {
            if (!Directory.Exists(obj.folder))
            {
                Directory.CreateDirectory(obj.folder);
            }
            saveCover(obj, cover);
        }

        private void saveCover(MangaObj obj, byte[] cover)
        {
            if (!File.Exists(obj.CoverPath))
            {
                File.WriteAllBytes(obj.CoverPath, cover);
            }
        }

        public static readonly RoutedEvent MangaDisRaiseEvent = EventManager.RegisterRoutedEvent(
            name: "MangaDisRaise",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(searchName));

        public event RoutedEventHandler MangaDisRaise
        {
            add { AddHandler(MangaDisRaiseEvent, value); }
            remove { RemoveHandler(MangaDisRaiseEvent, value); }
        }



        public SolidColorBrush ForG
        {
            get { return (SolidColorBrush)GetValue(ForGProperty); }
            set { SetValue(ForGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForGProperty =
            DependencyProperty.Register("ForG", typeof(SolidColorBrush), typeof(searchName), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public SolidColorBrush BackG
        {
            get { return (SolidColorBrush)GetValue(BackGProperty); }
            set { SetValue(BackGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackGProperty =
            DependencyProperty.Register("BackG", typeof(SolidColorBrush), typeof(searchName), new PropertyMetadata(new SolidColorBrush(Colors.White)));


    }
}
