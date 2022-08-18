using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manga.Controlls.views
{
    /// <summary>
    /// Interaction logic for longView.xaml
    /// </summary>
    public partial class longView : UserControl
    {
        int index = 0;
        cButton nextB;
        const int CHANGE = 20;
        int defaultZoom = 200;
        bool initLoad = false;
        bool full = false;
        public longView()
        {
            InitializeComponent();
            nextB = createButton();
        }


        Rect rect = new Rect();
        
        public Dictionary<float, List<Chapter>> ChapterDic
        {
            get { return (Dictionary<float, List<Chapter>>)GetValue(ChapterDicProperty); }
            set { SetValue(ChapterDicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChapterDic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChapterDicProperty =
            DependencyProperty.Register("ChapterDic", typeof(Dictionary<float, List<Chapter>>), typeof(longView), new PropertyMetadata(null));

        public SolidColorBrush Forgr
        {
            get { return (SolidColorBrush)GetValue(ForgrProperty); }
            set { SetValue(ForgrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Forgr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForgrProperty =
            DependencyProperty.Register("Forgr", typeof(SolidColorBrush), typeof(longView), new PropertyMetadata(new SolidColorBrush(Colors.Blue), forgrChanged));


        public Chapter chapter
        {
            get { return (Chapter)GetValue(chapterProperty); }
            set { SetValue(chapterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for chapter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty chapterProperty =
            DependencyProperty.Register("chapter", typeof(Chapter), typeof(longView), new PropertyMetadata(chapterPropertyChanged));

        private static void chapterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            longView? view = sender as longView;
            if(view != null)
            {
                view.chapterChanged();
            }
        }
        private void chapterChanged()
        {
        }

        public static readonly RoutedEvent SaveListEvent = EventManager.RegisterRoutedEvent(
            name: "SaveList",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(longView));

        public event RoutedEventHandler SaveList
        {
            add { AddHandler(SaveListEvent, value); }
            remove { RemoveHandler(SaveListEvent, value); }
        }
        private cButton createButton()
        {
            cButton button = new cButton();
            button.CornerRadus = 10;
            button.BType = 2;
            button.Height = 50;
            button.Background = Forgr;
            return button;
        }
        
        private static void forgrChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            longView? view = sender as longView;
            if (view != null)
            {
                view.forgrChangedCallBack();
            }
        }
        private void forgrChangedCallBack()
        {
            nextB.Background = Forgr;
        }


        public MangaObj Manga
        {
            get { return (MangaObj)GetValue(MangaProperty); }
            set { SetValue(MangaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Manga.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MangaProperty =
            DependencyProperty.Register("Manga", typeof(MangaObj), typeof(longView), new PropertyMetadata());


        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Focus();
        }
        private void button_Tap(object sender, RoutedEventArgs e)
        {
            nextChapter();
        }
        private void nextChapter()
        {
            float[] keys = ChapterDic.Keys.ToArray();
            int indexContains = 0;

            for (int i = 0; i < keys.Length; i++)
            {
                if (ChapterDic[keys[i]].Contains(chapter))
                {
                    indexContains = i;
                }
            }
            if (indexContains >= keys.Length - 1)
            {/*
                RoutedEventArgs routed = new RoutedEventArgs(longView.KillEvent);
                RaiseEvent(routed);*/
            }
            else
            {
                chapter = ChapterDic[keys[indexContains + 1]].First();
                pageContainer.Children.Clear();
                index = 0;
                tmpScroll.ScrollToHome();
                loadStart();
            }
        }
        private void previousChapter()
        {
            float[] keys = ChapterDic.Keys.ToArray();
            int indexContains = 0;

            for (int i = 0; i < keys.Length; i++)
            {
                if (ChapterDic[keys[i]].Contains(chapter))
                {
                    indexContains = i;
                }
            }
            if (indexContains <= 0)
            {
                /*
                RoutedEventArgs routed = new RoutedEventArgs(standardView.KillEvent);
                RaiseEvent(routed);
                */
            }
            else
            {
                chapter = ChapterDic[keys[indexContains - 1]].First();
            }
        }

        public static readonly RoutedEvent FullEvent = EventManager.RegisterRoutedEvent(
            name: "Full",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(longView));

        public event RoutedEventHandler Full
        {
            add { AddHandler(FullEvent, value); }
            remove { RemoveHandler(FullEvent, value); }
        }


        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F)
            {
                if (!full)
                {
                    this.Background = new SolidColorBrush(Colors.Black);
                    full = true;
                    tmpScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
                else
                {
                    this.Background = new SolidColorBrush(Colors.Transparent);
                    full = false;
                    tmpScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
                RoutedEventArgs routed = new RoutedEventArgs(longView.FullEvent);
                RaiseEvent(routed);
            }else if(e.Key == Key.OemPlus)
            {
                double ratio = (pageContainer.ActualWidth + (2 * CHANGE)) / pageContainer.ActualWidth;
                Thickness thickness = pageContainer.Margin;
                thickness.Left -= CHANGE;
                thickness.Right -= CHANGE;
                pageContainer.Margin = thickness;
                tmpScroll.ScrollToVerticalOffset(ratio*tmpScroll.VerticalOffset);
                nextB.Height = nextB.Height * ratio;

            }
            else if (e.Key == Key.OemMinus)
            {
                double ratio = (pageContainer.ActualWidth - (2 * CHANGE)) / pageContainer.ActualWidth;
                Thickness thickness = pageContainer.Margin;
                thickness.Left += CHANGE;
                thickness.Right += CHANGE;
                pageContainer.Margin = thickness;
                tmpScroll.ScrollToVerticalOffset(ratio * tmpScroll.VerticalOffset);
                nextB.Height = nextB.Height * ratio;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                ChapterDic = Manga.ChaptersDic;
                createRect();
                loadStart();
                zoom();
            }
        }
        private void zoom()
        {
            double ratio = (pageContainer.ActualWidth - (2 * defaultZoom)) / pageContainer.ActualWidth;
            Thickness thickness = pageContainer.Margin;
            thickness.Left += defaultZoom;
            thickness.Right += defaultZoom;
            pageContainer.Margin = thickness;
        }
        private void createRect()
        {
            double offset = this.ActualHeight * 0.2;
            rect.Location = new Point(0, -1*offset);
            rect.Height = this.ActualWidth + (2 * offset);
            rect.Width = this.ActualWidth;
        }
        private void loadStart()
        {
            foreach(Chapter chapter in ChapterDic[chapter.chapterN])
            {
                chapter.isRead = true;
            }
            RoutedEventArgs routed = new RoutedEventArgs(longView.SaveListEvent);
            RaiseEvent(routed);

            Rect rec = new Rect();
            double offset = 0;
            index = 0;
            if (chapter.pagesUri.Count == 0){return;}
            do
            {
                BitmapImage img = new BitmapImage(new Uri(chapter.pagesUri[index]));
                Image image = new Image();
                image.Source = img;
                pageContainer.Children.Add(image);
                rec = new Rect(new Point(0, offset), new Size(img.Width, img.Height));
                offset += rec.Height;
                index++;

            }while (rect.IntersectsWith(rec) && (index < chapter.pagesUri.Count));
            //pageContainer.Children.Add(nextB);
            initLoad = true;
           
        }
        
        private void scrollPos()
        {
            double offset = tmpScroll.VerticalOffset;
            double hoff = 0;
            int count = pageContainer.Children.Count;

            if(pageContainer.Children.Count == chapter.pagesUri.Count)
            {
                pageContainer.Children.Add(nextB);
                return;
            }else if(pageContainer.Children.Count > chapter.pagesUri.Count)
            {
                return;
            }
            for(int i = 0; i < count-1; i++)
            {
                FrameworkElement? u = pageContainer.Children[i] as FrameworkElement;
                hoff += u.ActualHeight;
            }
            FrameworkElement? obj = pageContainer.Children[index-1] as FrameworkElement;

            Rect rec = new Rect(new Point(0,hoff-offset), new Size(obj.ActualWidth,obj.ActualHeight));

            if (rect.IntersectsWith(rec))
            {
                BitmapImage img = new BitmapImage(new Uri(chapter.pagesUri[index]));
                Image image = new Image();
                image.Source = img;
                pageContainer.Children.Add(image);
                index++;
            }
        }

        private void scrollNeg()
        {
           //nextB.Text = "";
        }
        private void scroll_Changed(object sender, ScrollChangedEventArgs e)
        {
            if (initLoad)
            {
                if (e.VerticalChange > 0)
                {
                    scrollPos();
                }
                else if (e.VerticalChange < 0)
                {
                    scrollNeg();
                }
            }
        }
    }
}
