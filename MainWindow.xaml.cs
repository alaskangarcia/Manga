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
using Manga.Controlls;
using Manga;
using Manga.Controlls.Search;
using Manga.Controlls.views;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace Manga
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        string mangaUri = "";
        GridLength rowHeight;
        string DOCPATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        const string PATH = @"C:\Users\kyleg\programTMP\Manga\";
        const string PATHLIST = PATH + "list.json";
        string errorLog = "";
        mangaController controller;
        
        public MainWindow()
        {
            InitializeComponent();
            controller = new mangaController();
            loadList();
            controller.path = PATH;
            buttonStack.Focus();
            defaultPage defaultPage = new defaultPage();
            defaultPage.Style = this.FindResource("defaultP") as Style;
            lowerGrid.Children.Add(defaultPage);
            buttonStack.addCButton(newButton(defaultPage));
            rowHeight = parrentGrid.RowDefinitions[0].Height;
        }
        private void loadList()
        {
            string json = File.ReadAllText(PATHLIST);
            if(json != String.Empty && json != null)
            {
                JsonConvert.PopulateObject(json, controller);
            }
        }
        private void saveList()
        {
            string json = JsonConvert.SerializeObject(controller);
            File.WriteAllText(PATHLIST, json);
        }
        private cButton newButton(UserControl held)
        {
            cButton newButton = new cButton();
            newButton.heldObject = held;
            newButton.CornerRadus = 20;
            //newButton.AddHandler(mangaDisplay.TapEvent,mangaDisplay_Tap);
            return newButton;
        }
        
        private listManga newList(Grid grid, mangaController controller)
        {
            listManga listManga = new listManga();
            listManga.Style = this.FindResource("mangaListStyle") as Style;
            listManga.hideContent = true;
            listManga.imageWidth = 100;
            listManga.VerticalAlignment = VerticalAlignment.Top;
            listManga.Visibility = Visibility.Collapsed;
            listManga.controller = controller;
            listManga.Width = lowerGrid.Width;
            grid.Children.Add(listManga);
            return listManga;
        }
        private mangaInfo newInfo(Grid grid, mangaDisplay display)
        {
            mangaInfo info = new mangaInfo(controller);
            display.Margin = new Thickness(10, 10, 10, 0);
            display.Style = this.FindResource("mangaDPrimary") as Style;
            info.Style = this.FindResource("mangaInfoStyle") as Style;
            info.mangaDisplay = display;
            grid.Children.Add(info);
            info.Visibility = Visibility.Collapsed;
            return info;
        }
        private standardView newStandard(Grid grid, Chapter chapter , MangaObj manga)
        {
            standardView chapterStandard = new standardView();
            chapterStandard.Manga = manga;
            chapterStandard.Height = grid.Height;
            chapterStandard.Width = grid.Width;
            chapterStandard.Chapter = chapter;
            //chapterStandard.Margin = new Thickness(10, 10, 10, 10);
            chapterStandard.Visibility = Visibility.Collapsed;
            grid.Children.Add(chapterStandard);
            return chapterStandard;
        }

        private longView newLong(Grid grid, Chapter chapter, MangaObj manga)
        {
            longView longView = new longView();
            //longView.Forgr = new SolidColorBrush(Colors.White);
            longView.Manga = manga;
            longView.Style = this.FindResource("longVStyle") as Style;
            longView.Height = grid.Height;
            longView.Width = grid.Width;
            longView.chapter = chapter;
            //longView.Margin = new Thickness(10, 10, 10, 10);
            longView.Visibility = Visibility.Collapsed;

            grid.Children.Add(longView);
            return longView;
        }

        private searchID newSearchID(Grid grid)
        {
            searchID searchID = new searchID(controller,client);
            searchID.Style = this.FindResource("searchIDStyle") as Style;
            grid.Children.Add(searchID);
            return searchID ;
        }
        private searchName newSearchName(Grid grid)
        {
            searchName searchName = new searchName(controller, client);
            searchName.Style = this.FindResource("searchNameStyle") as Style;
            grid.Children.Add(searchName);
            return searchName;
        }
        public void endManga()
        {

        }

        private void buttonStack_Tap(object sender, RoutedEventArgs e)
        {
            if(sender is CStack)
            {
                try
                {
                    CStack stack = (CStack)sender;
                    
                    stack.setActive((cButton)e.OriginalSource);
                    stack.Focus();
                }
                catch (Exception ex)
                {
                    tmpTxt.Text += ex.ToString();
                }
            }
        }
        private void mangaDisplay_Tap(object sender, RoutedEventArgs e)
        {
            tmpTxt.Visibility = Visibility.Visible;
            tmpTxt.Text += "Clicked";
            try
            {

                mangaDisplay? display = e.OriginalSource as mangaDisplay;

                if (display?.Parent != null)
                {
                    if (typeof(Grid) == display.Parent.GetType())
                    {
                        return;
                    }
                }
                buttonStack.addCButton(newButton(newInfo(lowerGrid, new mangaDisplay(display.manga))));
            }
            catch(Exception ex)
            {
                tmpTxt.Text += ex.ToString();
            }
        }

        private void chapter_Full(object sender, RoutedEventArgs e)
        {
            
            if(upperGrid.Visibility == Visibility.Visible)
            {
                upperGrid.Visibility = Visibility.Collapsed;

                parrentGrid.RowDefinitions[0].Height = GridLength.Auto;
            }
            else if(upperGrid.Visibility == Visibility.Collapsed)
            {
                upperGrid.Visibility = Visibility.Visible;

                parrentGrid.RowDefinitions[0].Height = rowHeight;
            }
            
        }

        private void single_Tap(object sender, RoutedEventArgs e)
        {
            
            chapterListSingle? single = e.OriginalSource as chapterListSingle;
            //buttonStack.addCButton(newButton("cSing" + single.chapter.Id.Split("-")[0], newStandard("sing" + single.chapter.Id.Split("-")[0], lowerGrid, single.chapter)));
            tmpTxt.Text += single?.longStrip;
            if (single.longStrip)
            {
                try
                {
                    buttonStack.addCButton(newButton(newLong(lowerGrid, single.chapter, single.manga)));

                }
                catch (Exception ex)
                {
                    tmpTxt.Text = ex.ToString();
                }
                //buttonStack.addCButton(newButton("cLong" + single.chapter.Id.Split("-")[0], newLong("sing" + single.chapter.Id.Split("-")[0], lowerGrid, single.chapter)));
            }
            else
            {
                
                buttonStack.addCButton(newButton(newStandard(lowerGrid, single.chapter, single.manga)));

            }
        }

        private void Window_Rem(object sender, RoutedEventArgs e)
        {
            
            cButton button = buttonStack.ActiveButton;
            UserControl held = button.heldObject;
            if (buttonStack.removeButton(button))
            {
                lowerGrid.Children.Remove(held);
                buttonStack.Focus();
            }
            tmpTxt.Text += button.Name;
            //lowerGrid.Children.Remove(stack.ActiveButton);

            tmpTxt.Text += "kekw\n";
        }

        private void view_Kill(object sender, RoutedEventArgs e)
        {
            cButton button = buttonStack.ActiveButton;
            UserControl held = buttonStack.ActiveButton.heldObject;
            if (buttonStack.removeButton(button))
            {
                lowerGrid.Children.Remove(held);
                buttonStack.Focus();
            }
        }

        private void buttonStack_Kill(object sender, RoutedEventArgs e)
        {
            cButton button = e.OriginalSource as cButton;
            UserControl held = button.heldObject;
            if (buttonStack.removeButton(button))
            {
                lowerGrid.Children.Remove(held);
                buttonStack.Focus();
            }
        }

        private void default_NameSearchOpen(object sender, RoutedEventArgs e)
        {
            buttonStack.addCButton(newButton(newSearchName(lowerGrid)));
        }
        private void default_IDSearchOpen(object sender, RoutedEventArgs e)
        {
            buttonStack.addCButton(newButton(newSearchID(lowerGrid)));
        }
        private void default_RandomOpen(object sender, RoutedEventArgs e)
        {

        }
        private void default_ListOpen(object sender, RoutedEventArgs e)
        {
            buttonStack.addCButton(newButton(newList(lowerGrid, controller)));
        }

        private void searchRaise_Tap(object sender, RoutedEventArgs e)
        {
            buttonStack.addCButton(newButton(newInfo(lowerGrid, new mangaDisplay(controller.getLatest()))));
        }

        private void mangaListRaise_Tap(object sender, RoutedEventArgs e)
        {
            listManga info = e.OriginalSource as listManga;
            if(info.ClickedmangaObj != null)
            {
                buttonStack.addCButton(newButton(newInfo(lowerGrid, new mangaDisplay(info.ClickedmangaObj))));
            }
            
        }

        private void searchNameRise_Tap(object sender, RoutedEventArgs e)
        {
            searchName naem = e.OriginalSource as searchName;
            if(naem.clickedObj != null)
            {
                buttonStack.addCButton(newButton(newInfo(lowerGrid, new mangaDisplay(naem.clickedObj))));
            }
        }

        private void SaveList(object sender, RoutedEventArgs e)
        {
            saveList();
        }

        private void readFirst_Open(object sender, RoutedEventArgs e)
        {
            mangaInfo info = e.OriginalSource as mangaInfo;
            if (info.longstrip)
            {
                buttonStack.addCButton(newButton(newLong(lowerGrid, info.first, info.manga)));
            }
            else
            {
                buttonStack.addCButton(newButton(newStandard(lowerGrid, info.first, info.manga)));
            }
        }

        private void readContinue_Open(object sender, RoutedEventArgs e)
        {
            mangaInfo info = e.OriginalSource as mangaInfo;
            if (info.longstrip)
            {
                buttonStack.addCButton(newButton(newLong(lowerGrid, info.fromRead, info.manga)));
            }
            else
            {
                buttonStack.addCButton(newButton(newStandard(lowerGrid, info.fromRead, info.manga)));
            }
        }
        private void Window_CloseInfo(object sender, RoutedEventArgs e)
        {
            cButton button = buttonStack.ActiveButton;
            UserControl held = buttonStack.ActiveButton.heldObject;
            if (buttonStack.removeButton(button))
            {
                lowerGrid.Children.Remove(held);
                buttonStack.Focus();
            }
        }

        private void close_Tap(object sender,RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
