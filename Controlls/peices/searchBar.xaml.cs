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

namespace Manga.Controlls.peices
{
    /// <summary>
    /// Interaction logic for searchBar.xaml
    /// </summary>
    public partial class searchBar : UserControl
    {
        public searchBar()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent TapEvent = EventManager.RegisterRoutedEvent(
            name: "Tap",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(searchBar));

        public event RoutedEventHandler Tap
        {
            add { AddHandler(TapEvent, value); }
            remove { RemoveHandler(TapEvent, value); }
        }

        private void border_MU(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs routed = new RoutedEventArgs(searchBar.TapEvent);
            RaiseEvent(routed);
        }

        private void key_Down(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                RoutedEventArgs routed = new RoutedEventArgs(searchBar.TapEvent);
                RaiseEvent(routed);
            }
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(searchBar), new PropertyMetadata(string.Empty));

    }
}
