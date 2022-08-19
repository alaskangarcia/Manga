using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

namespace Manga.Controlls
{
    /// <summary>
    /// Interaction logic for chapterListSingle.xaml
    /// </summary>
    public partial class chapterListSingle : UserControl
    {
        HttpClient client = new HttpClient();
        mangaController controller;
        public MangaObj manga;
        public chapterListSingle(Chapter chapter, mangaController controller, MangaObj manga)
        {
            InitializeComponent();
            this.chapter = chapter;
            this.controller = controller;
            chapterN.Text = "Chapter. " + chapter.chapterN;
            getScan();
            if (chapter.isDownloaded)
            {
                downloadStatus.Text = "Chapter Downloaded";
            }
            else
            {
                downloadStatus.Text = "Chapter Not Downloaded";
            }
            if (chapter.isRead)
            {
                isReadTxt.Text = "Chapter Read";
            }
            else
            {
                isReadTxt.Text = "Chapter UnRead";
            }
            this.manga = manga;
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

        private async void getScan()
        {
            string name = controller.getGroup(chapter.scanID);
            if(name == "error"|| name == "err")
            {
                if (!CheckConnection())
                {
                    scanG.Text = "No Internet";
                    return;
                }
                controller.setGroup(chapter.scanID, await updateScan());
                chapter.scanNAme = controller.getGroup(chapter.scanID);
                RoutedEventArgs routed = new RoutedEventArgs(chapterListSingle.SaveEvent);
                RaiseEvent(routed);
            }
            else
            {
                chapter.scanNAme = name;
            }
            scanG.Text = chapter.scanNAme;
        }

        private async Task<string> updateScan()
        {
            
            HttpResponseMessage message = await client.GetAsync($@"https://api.mangadex.org/group/{chapter.scanID}");
            try
            {
                JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            
                return json["data"]["attributes"]["name"].ToString();
            }
            catch
            {
                return "No Group";
            }
        }

        public bool longStrip
        {
            get { return (bool)GetValue(longStripProperty); }
            set { SetValue(longStripProperty, value); }
        }

        // Using a DependencyProperty as the backing store for longStrip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty longStripProperty =
            DependencyProperty.Register("longStrip", typeof(bool), typeof(chapterListSingle), new PropertyMetadata(null));

        public Dictionary<float, List<Chapter>> ChapterDic
        {
            get { return (Dictionary<float, List<Chapter>>)GetValue(ChapterDicProperty); }
            set { SetValue(ChapterDicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChapterDic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChapterDicProperty =
            DependencyProperty.Register("ChapterDic", typeof(Dictionary<float, List<Chapter>>), typeof(chapterListSingle), new PropertyMetadata(null));



        public Chapter chapter
        {
            get { return (Chapter)GetValue(chapterProperty); }
            set { SetValue(chapterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for chapter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty chapterProperty =
            DependencyProperty.Register("chapter", typeof(Chapter), typeof(chapterListSingle), new PropertyMetadata(null));


        public static readonly RoutedEvent TapEvent = EventManager.RegisterRoutedEvent(
            name: "Tap",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(chapterListSingle));

        public event RoutedEventHandler Tap
        {
            add { AddHandler(TapEvent, value); }
            remove { RemoveHandler(TapEvent, value); }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (chapter.isDownloaded)
            {
                RoutedEventArgs routed = new RoutedEventArgs(chapterListSingle.TapEvent);
                RaiseEvent(routed);
            }
        }

        public static readonly RoutedEvent SaveEvent = EventManager.RegisterRoutedEvent(
            name: "Save",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(chapterListSingle));

        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }


        public SolidColorBrush ForG
        {
            get { return (SolidColorBrush)GetValue(ForGProperty); }
            set { SetValue(ForGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForGProperty =
            DependencyProperty.Register("ForG", typeof(SolidColorBrush), typeof(chapterListSingle), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


    }
}
