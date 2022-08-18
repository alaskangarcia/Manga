using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Manga.Controlls
{
    /// <summary>
    /// Interaction logic for mangaDisplay.xaml
    /// </summary>
    public partial class mangaDisplay : UserControl
    {
        public int type;
        MemoryStream stream;
        public bool hasContent = false;
        public byte[] type2Cover { get; set; }
        public mangaDisplay()
        {
            InitializeComponent();
        }
        public mangaDisplay(MangaObj manga)
        {
            InitializeComponent();
            type = 1;
            this.manga = manga;
            this.hasContent = manga.content;
            if (manga.CoverPath != String.Empty && manga.CoverPath != null)
            {
                try
                {
                    SearchImageSrc = new BitmapImage(new Uri(manga.CoverPath));
                }
                catch
                {
                    mTitle += "IMAGE FAILED";
                }
            }
            iconView.Width = imageWidth;
            //SearchImageSrc = new BitmapImage(new Uri(manga.CoverPath));
            mTitle += manga.Name;
            mDet = manga.Description;
        }
        public mangaDisplay(MangaObj manga, BitmapImage image, byte[] type2Cover)
        {
            InitializeComponent();
            type = 2;
            this.manga = manga;
            SearchImageSrc = image;
            iconView.Width = imageWidth;
            mTitle = manga.Name;
            mDet = manga.Description;
            this.type2Cover = type2Cover;
        }

        public static readonly RoutedEvent TapEvent = EventManager.RegisterRoutedEvent(
            name: "Tap",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(mangaDisplay));

        public event RoutedEventHandler Tap
        {
            add { AddHandler(TapEvent, value); }
            remove { RemoveHandler(TapEvent, value); }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(mangaDisplay.TapEvent);
            RaiseEvent(routed);
        }

        public MangaObj manga
        {
            get { return (MangaObj)GetValue(mangaProperty); }
            set { SetValue(mangaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for manga.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mangaProperty =
            DependencyProperty.Register("manga", typeof(MangaObj), typeof(mangaDisplay), new PropertyMetadata(null));



        public double imageWidth
        {
            get { return (double)GetValue(imageWidthProperty); }
            set { SetValue(imageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for imageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty imageWidthProperty =
            DependencyProperty.Register("imageWidth", typeof(double), typeof(mangaDisplay), new PropertyMetadata(100.0,imageWidthPropertyChanged));
        
        private static void imageWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            mangaDisplay? display = d as mangaDisplay;
            if (display != null)
            {
                display.imageWidthPropertyChangedCallBack();
            }
        }
        private void imageWidthPropertyChangedCallBack()
        {
            iconView.Width = imageWidth;
        }

        public BitmapImage SearchImageSrc
        {
            get { return (BitmapImage)GetValue(SearchImageSrcProperty); }
            set { SetValue(SearchImageSrcProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchImageSrc.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchImageSrcProperty =
            DependencyProperty.Register("SearchImageSrc", typeof(BitmapImage), typeof(mangaDisplay), new PropertyMetadata(SearchImageSrcPropertyChanged));

        private static void SearchImageSrcPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            mangaDisplay? display = d as mangaDisplay;
            if (display != null)
            {
                display.SearchImageSrcPropertyChangedCallBack();
            }
        }

        private void SearchImageSrcPropertyChangedCallBack()
        {
            iconView.Source = SearchImageSrc;
        }



        public string mTitle
        {
            get { return (string)GetValue(mTitleProperty); }
            set { SetValue(mTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for mTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mTitleProperty =
            DependencyProperty.Register("mTitle", typeof(string), typeof(mangaDisplay), new PropertyMetadata(mangaTitlePropertyChanged));

        private static void mangaTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            mangaDisplay? display = d as mangaDisplay;
            if(display != null)
            {
                display.mangaTitlePropertyChangedCallBack();
            }
        }

        private void mangaTitlePropertyChangedCallBack()
        {
            titleText.Text = mTitle;
        }

        public string mDet
        {
            get { return (string)GetValue(mDetProperty); }
            set { SetValue(mDetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for mDet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mDetProperty =
            DependencyProperty.Register("mDet", typeof(string), typeof(mangaDisplay), new PropertyMetadata(mangaDesPropertyChanged));

        private static void mangaDesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            mangaDisplay? display = d as mangaDisplay;
            if(display != null)
            {
                display.mangaDesPropertyChangedCallBack();
            }
        }
        private void mangaDesPropertyChangedCallBack()
        {
            desText.Text = mDet;
        }



        public SolidColorBrush BackG
        {
            get { return (SolidColorBrush)GetValue(BackGProperty); }
            set { SetValue(BackGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackGProperty =
            DependencyProperty.Register("BackG", typeof(SolidColorBrush), typeof(mangaDisplay), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public SolidColorBrush ForG
        {
            get { return (SolidColorBrush)GetValue(ForGProperty); }
            set { SetValue(ForGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForGProperty =
            DependencyProperty.Register("ForG", typeof(SolidColorBrush), typeof(mangaDisplay), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


    }
}
