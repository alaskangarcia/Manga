using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Manga.Controlls.Search;

namespace Manga.Controlls
{
    /// <summary>
    /// Interaction logic for CStack.xaml
    /// </summary>
    public partial class CStack : UserControl
    {
        public CStack()
        {
            InitializeComponent();
        }

        public cButton ActiveButton
        {
            get { return (cButton)GetValue(ActiveButtonProperty); }
            set { SetValue(ActiveButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveButtonProperty =
            DependencyProperty.Register("ActiveButton", typeof(cButton), typeof(CStack), new PropertyMetadata(null,ActiveButtonC));


        private static void ActiveButtonC(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CStack? cStack = sender as CStack;
            if (cStack != null)
            {
                cStack.ActiveButtonChanged();
            }
        }

        private void ActiveButtonChanged()
        {
            updateBackground();
        }

        public List<cButton> cButtons
        {
            get { return (List<cButton>)GetValue(cButtonsProperty); }
            set { SetValue(cButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for cButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty cButtonsProperty =
            DependencyProperty.Register("cButtons", typeof(List<cButton>), typeof(cButton), new PropertyMetadata(new List<cButton>(0), buttonsChanged));

        private static void buttonsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CStack? cStack = sender as CStack;
            if(cStack != null)
            {
                cStack.buttonsChangedCallback();
            }
        }
        private void buttonsChangedCallback()
        {
            updateBackground();
        }

        public SolidColorBrush ActiveBackground
        {
            get { return (SolidColorBrush)GetValue(ActiveBackgroundProperty); }
            set { SetValue(ActiveBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveBackgroundProperty =
            DependencyProperty.Register("ActiveBackground", typeof(SolidColorBrush), typeof(CStack), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));



        public int ButtonWidth
        {
            get { return (int)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register("ButtonWidth", typeof(int), typeof(CStack), new PropertyMetadata(100,widthChanged));

        private static void widthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CStack? cStack = sender as CStack;
            if(cStack != null)
            {
                cStack.widthChangedCallBack();
            }
        }
       
        private void widthChangedCallBack()
        {
            updateButtons();
        }

        public SolidColorBrush InactiveBackground
        {
            get { return (SolidColorBrush)GetValue(InactiveBackgroundProperty); }
            set { SetValue(InactiveBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InactiveBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InactiveBackgroundProperty =
            DependencyProperty.Register("InactiveBackground", typeof(SolidColorBrush), typeof(CStack), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public void addCButton(cButton button)
        {
            cButtons.Add(button);
            ButtonContainer.Children.Add(button);
            updateButtons();
            setActive(button);
        }
        private void updateButtons()
        {
            updateBackground();
            updateWidth();
        }
        private void updateWidth()
        {
            if(cButtons != null)
            {
                if(cButtons.Count > 0)
                {
                    foreach (cButton button in cButtons)
                    {
                        button.Width = ButtonWidth;
                    }
                }
            }
        }
        private void updateBackground()
        {
            if (cButtons != null)
            {
                foreach(cButton button in cButtons)
                {
                    if(button != ActiveButton)
                    {
                        button.Background = InactiveBackground;
                    }else if(button == ActiveButton)
                    {
                        button.Background = ActiveBackground;
                    }
                }
            }
        }
        public void setActive(cButton cButton)
        {
            foreach (cButton button in cButtons)
            {
                if (button != cButton)
                {
                    setInActive(button);
                }
                else
                {
                    button.showHeld();
                    ActiveButton = button;
                }
            }
        }
        public void setInActive(cButton button)
        {
            foreach (cButton button1 in cButtons)
            {
                if(button1 == button)
                {
                    button.collapseHeld();
                }
            }
        }

        public bool removeButton(cButton button)
        {
            if(cButtons.Count <= 1 || cButtons.IndexOf(button) == 0)
            {
                return false;
            }
            int index = cButtons.IndexOf(button);
            cButton butt = cButtons[index];
            if(butt.heldObject.GetType() == typeof(searchName))
            {
                searchName bh = (searchName)butt.heldObject;
                bh.dispose();
            }else if (butt.heldObject.GetType() == typeof(searchID))
            {
                searchID bh = (searchID)butt.heldObject;
                bh.dispose();
            }
            cButtons.Remove(button);
            ButtonContainer.Children.Remove(button);
            if (index >= cButtons.Count)
            {
                setActive(cButtons[cButtons.Count-1]);
            }
            else if (index < cButtons.Count)
            {
                setActive(cButtons[index]);
            }

            updateButtons();
            return true;
        }

        public static readonly RoutedEvent RemEvent = EventManager.RegisterRoutedEvent(
            name: "Rem",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(CStack));

        public event RoutedEventHandler Rem
        {
            add { AddHandler(RemEvent, value); }
            remove { RemoveHandler(RemEvent, value); }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            //this.Visibility = Visibility.Collapsed;
            if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                
                RoutedEventArgs routed = new RoutedEventArgs(CStack.RemEvent);
                RaiseEvent(routed);
                
            }
        }
        
    }
}