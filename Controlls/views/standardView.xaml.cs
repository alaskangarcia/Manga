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

namespace Manga.Controlls.views
{

    public partial class standardView : UserControl
    {
        //MangaObj Manga;
        int page = 0;
        List<BitmapImage> images = new List<BitmapImage>();
        bool full = false;
        bool showCount = false;
        bool previous = false;
        public standardView()
        {
            InitializeComponent();
        }

        public MangaObj Manga
        {
            get { return (MangaObj)GetValue(MangaProperty); }
            set { SetValue(MangaProperty, value); }
        }

        public Dictionary<float, List<Chapter>> ChapterDic
        {
            get { return (Dictionary<float, List<Chapter>>)GetValue(ChapterDicProperty); }
            set { SetValue(ChapterDicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChapterDic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChapterDicProperty =
            DependencyProperty.Register("ChapterDic", typeof(Dictionary<float, List<Chapter>>), typeof(standardView), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Manga.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MangaProperty =
            DependencyProperty.Register("Manga", typeof(MangaObj), typeof(standardView), new PropertyMetadata(mangaChanged));

        private static void mangaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            standardView? c = sender as standardView;
            if (c != null)
            {
                c.mangaChangedCallBack();
            }
        }

        private void mangaChangedCallBack()
        {
            ChapterDic = Manga.ChaptersDic;
        }

        public Chapter Chapter
        {
            get { return (Chapter)GetValue(ChapterProperty); }
            set { SetValue(ChapterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Chapter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChapterProperty =
            DependencyProperty.Register("Chapter", typeof(Chapter), typeof(standardView), new PropertyMetadata(chapterChanged));

        private static void chapterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            standardView? c = sender as standardView;
            if (c != null)
            {
                c.chapterChangedCallBack();
            }
        }

        private void chapterChangedCallBack()
        {
            RoutedEventArgs routed = new RoutedEventArgs(standardView.SaveListEvent);
            RaiseEvent(routed);
            loadPages();
        }

        public static readonly RoutedEvent SaveListEvent = EventManager.RegisterRoutedEvent(
            name: "SaveList",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(standardView));

        public event RoutedEventHandler SaveList
        {
            add { AddHandler(SaveListEvent, value); }
            remove { RemoveHandler(SaveListEvent, value); }
        }

        public int Page
        {
            get { return (int)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Page.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register("Page", typeof(int), typeof(standardView), new PropertyMetadata(pageChanged));

        private static void pageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            standardView? c = sender as standardView;
            if (c != null)
            {
                c.pageChangedCallBack();
            }

        }

        private void pageChangedCallBack()
        {
            
        }

        private void loadPages()
        {
            markRead();
            images.Clear();
            if (previous)
            {
                previous = false;
                foreach (string pageUri in Chapter.pagesUri)
                {
                    images.Add(new BitmapImage(new Uri(pageUri)));
                }
                setPage(images.Count-1);
                chapterCountText.Text = (page + 1) + @"/" + Chapter.pagesUri.Count;
            }
            else
            {
                foreach (string pageUri in Chapter.pagesUri)
                {
                    images.Add(new BitmapImage(new Uri(pageUri)));
                }
                setPage(0);
                chapterCountText.Text = (page + 1) + @"/" + Chapter.pagesUri.Count;

            }
            
        }
        private void setPage(int page)
        {
            pageDisplay.Source = images[page];
            this.page = page;
        }

        private void leftSubGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            previousPage();
        }

        private void rightSubGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            nextPage();
        }

        public void previousPage()
        {
            if (page <= 0)
            {
                previousChapter();
                return;
            }
            page--;
            pageDisplay.Source = images[page];
            chapterCountText.Text = (page + 1) + @"/" + Chapter.pagesUri.Count;
        }

        public void nextPage()
        {
            if(page >= images.Count-1)
            {
                nextChapter();
                return;
            }
            page++;
            pageDisplay.Source = images[page];
            chapterCountText.Text = (page+1) + @"/" + Chapter.pagesUri.Count;
        }
        private void nextChapter()
        {
            float[] keys = ChapterDic.Keys.ToArray();

            for (int i = 0; i < keys.Length - 1; i++)
            {
                foreach (var chapter in ChapterDic[keys[i]])
                {
                    if (Chapter.Id == chapter.Id)
                    {
                        Chapter = ChapterDic[keys[i + 1]].First();

                        RoutedEventArgs routed = new RoutedEventArgs(standardView.SaveListEvent);
                        RaiseEvent(routed);
                        return;
                    }
                }
            }/*
            RoutedEventArgs routed = new RoutedEventArgs(standardView.KillEvent);
            RaiseEvent(routed);*/
        }
        private void previousChapter()
        {
            float[] keys = ChapterDic.Keys.ToArray();
            keys = keys.Reverse().ToArray();
            
            for (int i = 0; i < keys.Length-1; i++)
            {
                foreach(var chapter in ChapterDic[keys[i]])
                {
                    if(Chapter.Id == chapter.Id)
                    {
                        previous = true;
                        Chapter = ChapterDic[keys[i + 1]].First();
                        RoutedEventArgs routed = new RoutedEventArgs(standardView.SaveListEvent);
                        RaiseEvent(routed);
                        return;
                    }
                }
            }
            /*
            RoutedEventArgs routed = new RoutedEventArgs(standardView.KillEvent);
            RaiseEvent(routed);*/
        }

        public static readonly RoutedEvent FullEvent = EventManager.RegisterRoutedEvent(
            name: "Full",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(standardView));

        public event RoutedEventHandler Full
        {
            add { AddHandler(FullEvent, value); }
            remove { RemoveHandler(FullEvent, value); }
        }
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //this.Visibility = Visibility.Collapsed;
            if (e.Key == Key.Right)
            {
                nextPage();
            }
            else if (e.Key == Key.Left)
            {
                previousPage();
            }
            else if (e.Key == Key.F)
            {
                if (!full)
                {
                    this.Background = new SolidColorBrush(Colors.Black);
                    full = true;
                }
                else
                {
                    this.Background = new SolidColorBrush(Colors.Transparent);
                    full = false;
                }
                RoutedEventArgs routed = new RoutedEventArgs(standardView.FullEvent);
                RaiseEvent(routed);
            }else if(e.Key == Key.Q)
            {
                if (showCount)
                {
                    showCount = !showCount;
                    chapterCountText.Visibility = Visibility.Hidden;
                }
                else
                {
                    showCount = !showCount;
                    chapterCountText.Visibility = Visibility.Visible;
                }
            }
        }
        public static readonly RoutedEvent KillEvent = EventManager.RegisterRoutedEvent(
            name: "Kill",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(standardView));

        public event RoutedEventHandler Kill
        {
            add { AddHandler(FullEvent, value); }
            remove { RemoveHandler(FullEvent, value); }
        }
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Focus();
            if (leftSubGrid.IsMouseOver)
            {
                previousPage();
            }else if (rightSubGrid.IsMouseOver)
            {
                nextPage();
            }
        }

        private void markRead()
        {
            foreach(Chapter chapter in Manga.chapters)
            {
                if(Chapter.chapterN == chapter.chapterN)
                {
                    chapter.isRead = true;
                }
            }
        }
    }
}
