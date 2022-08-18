using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for chapterList.xaml
    /// </summary>
    public partial class chapterList : UserControl
    {
        public bool longs;
        mangaController controller;
        MangaObj manga;
        
        public chapterList(mangaController controller, MangaObj manga)
        {
            InitializeComponent();
            this.controller = controller;
            this.manga = manga;
        }
        public List<Chapter> Chapters
        {
            get { return (List<Chapter>)GetValue(ChaptersProperty); }
            set { SetValue(ChaptersProperty, value); }
        }


        // Using a DependencyProperty as the backing store for Chapters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChaptersProperty =
            DependencyProperty.Register("Chapters", typeof(List<Chapter>), typeof(chapterList), new PropertyMetadata(ChaptersPropertyChanged));

        private static void ChaptersPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            chapterList? list = sender as chapterList;
            if(list != null)
            {
                list.chaptersPropertyChangedCallBack();
            }
        }
        private void chaptersPropertyChangedCallBack()
        {
            if(Chapters.Count > 0)
            {
                foreach(Chapter chapter in Chapters)
                {
                    chapterListSingle single = new chapterListSingle(chapter, controller, manga);
                    single.ForG = ForG;
                    single.longStrip = longs;
                    single.ChapterDic = ChapterDic;
                    single.Name = "ch" + chapter.Id.Split("-")[0];
                    single.Margin = new Thickness(5, 5, 5, 5);
                    chapterInfo.Children.Add(single);
                    chapterslist.Add(single);
                }
            }
        }
        public List<chapterListSingle> chapterslist
        {
            get { return (List<chapterListSingle>)GetValue(chapterslistProperty); }
            set { SetValue(chapterslistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for chapterslist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty chapterslistProperty =
            DependencyProperty.Register("chapterslist", typeof(List<chapterListSingle>), typeof(chapterList), new PropertyMetadata(new List<chapterListSingle>(0)));

        public Dictionary<float, List<Chapter>> ChapterDic
        {
            get { return (Dictionary<float, List<Chapter>>)GetValue(ChapterDicProperty); }
            set { SetValue(ChapterDicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChapterDic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChapterDicProperty =
            DependencyProperty.Register("ChapterDic", typeof(Dictionary<float, List<Chapter>>), typeof(chapterList), new PropertyMetadata(null));



        public SolidColorBrush BackG
        {
            get { return (SolidColorBrush)GetValue(BackGProperty); }
            set { SetValue(BackGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackGProperty =
            DependencyProperty.Register("BackG", typeof(SolidColorBrush), typeof(chapterList), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        public SolidColorBrush ForG
        {
            get { return (SolidColorBrush)GetValue(ForGProperty); }
            set { SetValue(ForGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForGProperty =
            DependencyProperty.Register("ForG", typeof(SolidColorBrush), typeof(chapterList), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


        public static readonly RoutedEvent SaveEvent = EventManager.RegisterRoutedEvent(
            name: "Save",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(chapterList));

        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }

        private void root_Save(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(chapterList.SaveEvent);
            RaiseEvent(routed);
        }
    }
}
