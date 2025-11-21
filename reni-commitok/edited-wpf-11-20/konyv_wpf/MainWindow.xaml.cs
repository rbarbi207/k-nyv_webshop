using Newtonsoft.Json;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace konyv_wpf
{
    /// error to-do: már létező könyv (minden adat egyezik, kivéve a feltöltés dátuma, és a példányszám)
    /// - Kitöltetlen mező
    /// - nincs találat
    /// - Nem lehet speciális karakter -> név, szerző (kivéve ','; '-')
    /// 


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool sorted = false; 
        /// false -> alap: legkorábbi
        public MainWindow()
        {
            InitializeComponent();
            //loadBooks(/*tc_fájlneve*/);
         
        }   

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            lbl_lang.Opacity = 0.4;
            
        }

        private void lbl_lang_MouseLeave(object sender, MouseEventArgs e)
        {
            lbl_lang.Opacity = 0.6;
        }

        private void lbl_lang_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(lbl_lang.Content.ToString().Trim() == "magyar") {
            lbl_lang.Content = "English";
            }
            else
            {
                lbl_lang.Content = "magyar";
            }
            /////// Angolra fordítás
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            img_sort.Opacity = 0.6;
        }

        private void img_sort_MouseLeave(object sender, MouseEventArgs e)
        {
            img_sort.Opacity = 1;
        }

        private void img_sort_MouseDown(object sender, MouseButtonEventArgs e)
        { 
            ///// itemek rendezése dátum szerint
        }

        //private void sort(List<Book> books)
        //{

        //}






        //private void loadBooks(/*string tc_variableNev*/)
        //{
        //    //Manage nuget packages -> newtonsoft.json letöltése

        //    string json = File.ReadAllText("filmek.json");

        //    // JSON → List<Item>
        //    List<Film>? items = JsonConvert.DeserializeObject<List<Film>>(json);

        //    if (items != null)
        //    {
        //        foreach (var item in items)
        //        {
        //            //lblid.Content = item.Title;
        //        }
        //    }

        //}

    }
}

