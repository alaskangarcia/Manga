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
    /// Interaction logic for defaultPage.xaml
    /// </summary>
    public partial class defaultPage : UserControl
    {
        public defaultPage()
        {
            InitializeComponent();
        }



        public SolidColorBrush BackgroundS
        {
            get { return ( SolidColorBrush)GetValue(BackgroundSProperty); }
            set { SetValue(BackgroundSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundS.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundSProperty =
            DependencyProperty.Register("BackgroundS", typeof( SolidColorBrush), typeof(defaultPage), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));



        private void close_Tap(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        public static readonly RoutedEvent OpenListEvent = EventManager.RegisterRoutedEvent(
            name: "OpenList",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(defaultPage));

        public event RoutedEventHandler OpenList
        {
            add { AddHandler(OpenListEvent, value); }
            remove { RemoveHandler(OpenListEvent, value); }
        }

        public static readonly RoutedEvent OpenSearchNameEvent = EventManager.RegisterRoutedEvent(
            name: "OpenSearchName",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(defaultPage));

        public event RoutedEventHandler OpenSearchName
        {
            add { AddHandler(OpenSearchNameEvent, value); }
            remove { RemoveHandler(OpenSearchNameEvent, value); }
        }

        public static readonly RoutedEvent OpenSearchIDEvent = EventManager.RegisterRoutedEvent(
            name: "OpenSearchID",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(defaultPage));

        public event RoutedEventHandler OpenSearchID
        {
            add { AddHandler(OpenSearchIDEvent, value); }
            remove { RemoveHandler(OpenSearchIDEvent, value); }
        }

        public static readonly RoutedEvent OpenRandomEvent = EventManager.RegisterRoutedEvent(
            name: "OpenRandom",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(defaultPage));

        public event RoutedEventHandler OpenRandom
        {
            add { AddHandler(OpenRandomEvent, value); }
            remove { RemoveHandler(OpenRandomEvent, value); }
        }

        private void List_Tap(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(defaultPage.OpenListEvent);
            RaiseEvent(routed);
        }

        private void SearchN_Tap(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(defaultPage.OpenSearchNameEvent);
            RaiseEvent(routed);
        }

        private void SearchID_Tap(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(defaultPage.OpenSearchIDEvent);
            RaiseEvent(routed);
        }

        private void Rand_Tap(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(defaultPage.OpenRandomEvent);
            RaiseEvent(routed);
        }
    }
}