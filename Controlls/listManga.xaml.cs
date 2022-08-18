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
    /// Interaction logic for listManga.xaml
    /// </summary>
    public partial class listManga : UserControl
    {
        public MangaObj ClickedmangaObj { get; set; }
        public listManga()
        {
            InitializeComponent();
            
        }

        public mangaController controller
        {
            get { return (mangaController)GetValue(controllerProperty); }
            set { SetValue(controllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty controllerProperty =
            DependencyProperty.Register("controller", typeof(mangaController), typeof(listManga), new PropertyMetadata(controllerPropertyChanged));

        private static void controllerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            listManga? list = d as listManga;
            if(list != null)
            {
                list.controllerPropertyChangedCallBack();
            }
        }

        public bool hideContent
        {
            get { return (bool)GetValue(hideContentProperty); }
            set { SetValue(hideContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for hideContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty hideContentProperty =
            DependencyProperty.Register("hideContent", typeof(bool), typeof(listManga), new PropertyMetadata(false));


        private void controllerPropertyChangedCallBack()
        {
            if (controller != null)
            {
                mangaHolder.Children.Clear();
                mangaDisplays.Clear();
                controller.Mangas.Sort(delegate (MangaObj o1, MangaObj o2) { return o1.Name.CompareTo(o2.Name); });
                foreach(MangaObj manga in controller.Mangas)
                {
                    mangaDisplay display = new mangaDisplay(manga);
                    display.Margin = new Thickness(10);
                    display.BackG = BackG;
                    display.ForG = ForG;
                    display.imageWidth = imageWidth;
                    if (manga.content && hideContent)
                    {
                        display.Visibility = Visibility.Collapsed;
                    }
                    mangaHolder.Children.Add(display);
                    mangaDisplays.Add(display);
                }
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(listManga), new PropertyMetadata(string.Empty));



        public List<mangaDisplay> mangaDisplays
        {
            get { return (List<mangaDisplay>)GetValue(mangaDisplaysProperty); }
            set { SetValue(mangaDisplaysProperty, value); }
        }

        // Using a DependencyProperty as the backing store for mangaDisplays.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mangaDisplaysProperty =
            DependencyProperty.Register("mangaDisplays", typeof(List<mangaDisplay>), typeof(listManga), new PropertyMetadata(new List<mangaDisplay>(0)));



        public double imageWidth
        {
            get { return (double)GetValue(imageWidthProperty); }
            set { SetValue(imageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for imageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty imageWidthProperty =
            DependencyProperty.Register("imageWidth", typeof(double), typeof(listManga), new PropertyMetadata(1.0));
        
        private void mangaDis_Tap(object sender, RoutedEventArgs e)
        {
            mangaDisplay display = e.OriginalSource as mangaDisplay;
            ClickedmangaObj = display.manga;
            RoutedEventArgs routed = new RoutedEventArgs(MangaDisRaiseEvent);
            RaiseEvent(routed);
        }

        public static readonly RoutedEvent MangaDisRaiseEvent = EventManager.RegisterRoutedEvent(
            name: "MangaDisRaise",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(listManga));

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
            DependencyProperty.Register("ForG", typeof(SolidColorBrush), typeof(listManga), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public SolidColorBrush BackG
        {
            get { return (SolidColorBrush)GetValue(BackGProperty); }
            set { SetValue(BackGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackGProperty =
            DependencyProperty.Register("BackG", typeof(SolidColorBrush), typeof(listManga), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.Visibility == Visibility.Visible)
            {
                controllerPropertyChangedCallBack();
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(this.Visibility != Visibility.Visible)
            {
                Focus();
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.H)
            {
                if (hideContent)
                {
                    foreach(mangaDisplay child in mangaDisplays)
                    {
                        if (child.hasContent)
                        {
                            child.Visibility = Visibility.Visible;
                        }
                    }
                    hideContent = !hideContent;
                }else
                {
                    foreach (mangaDisplay child in mangaDisplays)
                    {
                        if (child.hasContent)
                        {
                            child.Visibility = Visibility.Collapsed;
                        }
                    }
                    hideContent = !hideContent;
                }
            }
        }
    }
}
