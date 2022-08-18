using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Manga.Controlls.Search
{
    /// <summary>
    /// Interaction logic for searchID.xaml
    /// </summary>
    public partial class searchID : UserControl
    {
        HttpClient client;
        const string baseurlManga = @"https://api.mangadex.org/manga/";
        const string baseurlCover = @"https://api.mangadex.org/cover/";
        mangaController controller;

        public searchID()
        {
            InitializeComponent();
        }

        public searchID(mangaController controller, HttpClient client)
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



        private async Task search()
        {

            /*
            if (!CheckConnection())
            {
                return;
            }*/
            string search = search_Bar.SearchText;

            foreach (MangaObj  obj in controller.Mangas)
            {
                if(obj.ID == search)
                {
                    displayManga(obj);
                    controller.setLatest(search);
                    return;
                }
            }
            HttpResponseMessage request;
            request = await client.GetAsync(baseurlManga + search);
            JObject json = JObject.Parse(await request.Content.ReadAsStringAsync());
            
            string coverart = "";
            foreach (var a in json["data"]!["relationships"]!)
            {
                if (a["type"].ToString() == "cover_art")
                {
                    coverart = a["id"].ToString();
                }
            }
            controller.newManga(
                json["data"]!["id"]!.ToString(),
                filterChar(json["data"]!["attributes"]!["title"]!["en"]!.ToString()),
                json["data"]!["attributes"]!["description"]!["en"]!.ToString(),
                coverart);

            MangaObj manga = controller.getLatest();
            foreach (var b in json["data"]["attributes"]["tags"])
            {
                if (b["id"].ToString() == "3e2b8dae-350e-4ab8-a8ce-016e844b9f0d")
                {
                    manga.longstrip = true ;
                }
            }
            if (json["data"]!["attributes"]!["contentRating"]!.ToString() == "pornographic")
            {
                manga.content = true;
            }
            manga.folder = controller.path + @"\" + manga.Name;
            createFolder(manga);
            await downloadCover(manga);

            //displayManga(manga);
        }
        private void createFolder(MangaObj obj)
        {
            if (!Directory.Exists(obj.folder))
            {
                Directory.CreateDirectory(obj.folder);
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
        private async Task downloadCover(MangaObj obj)
        {
            HttpResponseMessage message = await client.GetAsync(baseurlCover + obj.Cover[0]);
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            string cover = json["data"]["attributes"]["fileName"].ToString();
            string cpath = obj.folder + @"\" + cover;
            if (!File.Exists(cpath))
            {
                byte[] img = await client.GetByteArrayAsync($@"https://uploads.mangadex.org/covers/{obj.ID}/{cover}");
                File.WriteAllBytes(cpath, img);
            }
            obj.CoverPath = cpath;
            obj.lastUpdate = DateTime.Now;
            displayManga(obj);
        }
        private void displayManga(MangaObj obj) 
        {
            displayGrid.Children.Clear();
            mangaDisplay display = new mangaDisplay(obj);
            display.ForG = ForG;
            display.BackG = BackG;
            display.VerticalAlignment = VerticalAlignment.Center;
            displayGrid.Children.Add(display);
            RoutedEventArgs routed = new RoutedEventArgs(SaveListEvent);
            RaiseEvent(routed);
        }

        public static readonly RoutedEvent SaveListEvent = EventManager.RegisterRoutedEvent(
            name: "SaveList",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(searchID));

        public event RoutedEventHandler SaveList
        {
            add { AddHandler(SaveListEvent, value); }
            remove { RemoveHandler(SaveListEvent, value); }
        }
        private void mangaDis_Tap(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(MangaDisRaiseEvent);
            RaiseEvent(routed);
        }

        public static readonly RoutedEvent MangaDisRaiseEvent = EventManager.RegisterRoutedEvent(
            name: "MangaDisRaise",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(searchID));

        public event RoutedEventHandler MangaDisRaise
        {
            add { AddHandler(MangaDisRaiseEvent, value); }
            remove { RemoveHandler(MangaDisRaiseEvent, value); }
        }
        public void dispose()
        {
            //client.Dispose();
        }




        public SolidColorBrush ForG
        {
            get { return (SolidColorBrush)GetValue(ForGProperty); }
            set { SetValue(ForGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForGProperty =
            DependencyProperty.Register("ForG", typeof(SolidColorBrush), typeof(searchID), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public SolidColorBrush BackG
        {
            get { return (SolidColorBrush)GetValue(BackGProperty); }
            set { SetValue(BackGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackGProperty =
            DependencyProperty.Register("BackG", typeof(SolidColorBrush), typeof(searchID), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        private bool CheckConnection()
        {
            byte[] googledns = { 0x08, 0x08, 0x08, 0x08};
            Ping ping = new Ping();
            PingReply reply = ping.Send(new IPAddress(googledns));
            if(reply.Status == IPStatus.Success)
            {
                return true;
            }
            return false;
        }



    }
}
