using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Manga.Controlls 
{
    public partial class cButton : UserControl
    {
        public cButton()
        {
            InitializeComponent();
            if(BType == 0)
            {
                baseBorder.CornerRadius = new CornerRadius((int)GetValue(CornerRadusProperty), (int)GetValue(CornerRadusProperty), 0, 0);

            }
            
            //baseBorder.CornerRadius = new CornerRadius(CornerRadus,CornerRadus, 0, 0);
        }

        public UserControl heldObject
        {
            get { return (UserControl)GetValue(heldObjectProperty); }
            set { SetValue(heldObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for heldObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty heldObjectProperty =
            DependencyProperty.Register("heldObject", typeof(UserControl), typeof(CStack), new PropertyMetadata(null));



        public static readonly RoutedEvent TapEvent = EventManager.RegisterRoutedEvent(
            name: "Tap",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(cButton));

        public event RoutedEventHandler Tap
        {
            add { AddHandler(TapEvent, value); }
            remove { RemoveHandler(TapEvent, value); }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                RoutedEventArgs routed = new RoutedEventArgs(cButton.TapEvent);
                RaiseEvent(routed);
            }else if(e.ChangedButton == MouseButton.Middle)
            {
                RoutedEventArgs routed = new RoutedEventArgs(cButton.KillEvent);
                RaiseEvent(routed);
            }
        }

        public static readonly RoutedEvent KillEvent = EventManager.RegisterRoutedEvent(
            name:"Kill",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(cButton));

        public event RoutedEventHandler Kill
        {
            add { AddHandler(KillEvent, value); }
            remove { RemoveHandler(KillEvent, value); }
        }
        public int CornerRadus
        {
            get { return (int)GetValue(CornerRadusProperty); }
            set { SetValue(CornerRadusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadusProperty =
            DependencyProperty.Register("CornerRadus", typeof(int), typeof(cButton), new PropertyMetadata(CornerChanged));  

        
        public static void CornerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            cButton? c = sender as cButton;
            if(c!= null)
            {
                c.changed();
            }
        }

        void changed()
        {
            updateBorder();
            //baseBorder.CornerRadius = new CornerRadius((int)GetValue(CornerRadusProperty), (int)GetValue(CornerRadusProperty), 0, 0);
        }
        public new SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(cButton), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        public new SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(cButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(cButton), new PropertyMetadata(""));



        public string TextAlignment
        {
            get { return (string)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(string), typeof(cButton), new PropertyMetadata("center"));



        public int BType
        {
            get { return (int)GetValue(BTypeProperty); }
            set { SetValue(BTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BTypeProperty =
            DependencyProperty.Register("BType", typeof(int), typeof(cButton), new PropertyMetadata(0,BTypeChanged));

        private static void BTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            cButton? button = sender as cButton;
            if(button!= null)
            {
                button.updateBorder();
            }
        }


        private void updateBorder()
        {
            switch (BType)
            {
                case 0: baseBorder.CornerRadius = new CornerRadius((int)GetValue(CornerRadusProperty), (int)GetValue(CornerRadusProperty), 0, 0); break;
                case 1: baseBorder.CornerRadius = new CornerRadius((int)GetValue(CornerRadusProperty)); break;
                case 2: baseBorder.CornerRadius = new CornerRadius(0, 0, (int)GetValue(CornerRadusProperty), (int)GetValue(CornerRadusProperty)); break;
            }
        }

        public void close()
        {
            
        }
        
        public void showHeld()
        {
            heldObject.Visibility = Visibility.Visible;
            Keyboard.Focus(heldObject);
            /*
            if(heldObject.GetType() == typeof(listManga))
            {
                UserControl tmp = (UserControl)heldObject;
                tmp.Visibility = Visibility.Visible;
            }else
            {

            }
            */
        }
        public void collapseHeld()
        {
            heldObject.Visibility = Visibility.Collapsed;
            Keyboard.ClearFocus();
        }


    }
}
