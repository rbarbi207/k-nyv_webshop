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
    /// - max karakterszám (szerző, cím)
    /// - Kitöltetlen mező
    /// - nincs találat
    /// - Nem lehet speciális karakter -> név, szerző (kivéve ','; '-')
    /// - törlés és módosítás esetén hoverkor jelenjen meg szövegben az hogy nincs kiválasztva elem dőlt betűkkel (nem súlyos), ilyenkor NE változzon a kinézet hover esetén (plussz a kurzor se)
    /// új ötlet: módosításnál felülíródik a mentett dátum <summary>
    /// error to-do: már létező könyv (minden adat egyezik, kivéve a feltöltés dátuma, és a példányszám)
    /// 
    /// 
    /// todo:  az ikonok működése reszponzívan
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool sorted = true; 
        public List<Book> books = new List<Book>();
        /// false -> alap: legkorábbi <summary>
        /// kiválasztott elem eltárolása ? 
        public MainWindow()
        {
            InitializeComponent();
            loadBooks("books.json");
            rct_error.Visibility = Visibility.Hidden;
            border_error.Visibility = Visibility.Hidden;

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
            if (tbx_searchbar.Text != "")
            {
                List<Book> filteredBooks = filterBooks();
                if (filteredBooks.Count > 0) PrintSortedBooks(filteredBooks, true);
            }
            else
            {
                PrintSortedBooks(books,true);
            }
        }

        private void PrintSortedBooks(List<Book> books, bool clicked)
        {
            books = makeSortList(books, clicked);
            foreach (Book book in books)
            {
                ListBoxItem item = new ListBoxItem();
                string displayTitle = "";
                if (book.Title.Length > 40)
                {
                    displayTitle = book.Title.Substring(0, 37) + "...";
                }
                else
                {
                    displayTitle = book.Title;
                }

                // stackpanel -> függőlegesen rendezi a title-t és a dátumot
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Vertical;

                // cím
                Label lbl_title = new Label();
                lbl_title.Name = "lbl_title";
                lbl_title.Content = displayTitle;
                lbl_title.FontSize = 16;
                lbl_title.FontWeight = FontWeights.Bold;

                // dátum
                Label lbl_date = new Label();
                lbl_date.Name = "lbl_date";
                lbl_date.Content = "módosítva: " + book.DateEdited;
                lbl_date.FontSize = 12;
                lbl_date.HorizontalAlignment = HorizontalAlignment.Right;
                lbl_date.Margin = new Thickness(0, -5, 0, 0);
                lbl_date.FontStyle = FontStyles.Italic;

                panel.Children.Add(lbl_title);
                panel.Children.Add(lbl_date);

                item.Content = panel;

                item.HorizontalAlignment = HorizontalAlignment.Stretch;
                item.Width = rct_list.Width - 110;
                item.Cursor = Cursors.Hand;
                item.Padding = new Thickness(5, 0, 5, 0);
                lbx_books.Items.Add(item);
            }


        }

        private List<Book> makeSortList(List<Book> books, bool clicked)
        {
            List<Book> sortedBooks = new List<Book>();
            lbx_books.Items.Clear();
            sortedBooks = sort(books);
            if (clicked)
            {
                if (!sorted)
                {
                    sorted = true;
                }
                else
                {
                    sortedBooks.Reverse();
                    sorted = false;
                }

            }
            else
            {
                if (!sorted)
                {
                    sortedBooks.Reverse();
                }
            }
            return sortedBooks;
        }

        private List<Book> sort(List<Book> books)
        {
            List<Book> sortedBooks = new List<Book>();
            List <DateTime> dates = new List<DateTime>();
            foreach (Book book in books)
            {
                dates.Add(book.DateEdited);
            }
            dates.Sort();

            foreach (DateTime date in dates)
            {
                List<int> indexes = searchIndexbyDate(date, books);
                foreach(int index in indexes)
                {
                    sortedBooks.Add(books[index]);
                }
            }
            return sortedBooks;
        }

        private List<int> searchIndexbyDate(DateTime date, List<Book> books)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < books.Count; i++)
            {
         
                if (DateTime.Equals(books[i].DateEdited, date))
                {
                    indexes.Add(i);
                }
            }
            return indexes;
        }

        private void loadBooks(string filename)
        {
            string json = File.ReadAllText("books.json");
            books = JsonConvert.DeserializeObject<List<Book>>(json);
            foreach (Book book in books)
            {
                book.DateEdited = DateTime.Now;
            }
            books[3].DateEdited = DateTime.Now.AddDays(4);
            PrintSortedBooks(books, false);

        }

        private void tbx_searchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Book> filteredBooks = filterBooks();
            if (filteredBooks.Count > 0 && tbx_searchbar.Text != "")
            {
                PrintSortedBooks(filteredBooks, false);
            }
            else if (tbx_searchbar.Text == "")
            {
                PrintSortedBooks(books, false);
            }
            else
            {
                lbx_books.Items.Clear();
                ListBoxItem item = new ListBoxItem();
                item.Content = "Nincs találat ezen a néven!";
                item.HorizontalAlignment = HorizontalAlignment.Center;
                item.IsHitTestVisible = false;  
                item.Focusable = false;
                lbx_books.Items.Add(item);
            }
        }

        private void tbx_searchbar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Keyboard.ClearFocus();
        }
        private List<Book> filterBooks()
        {
            List<Book> filteredBooks = new List<Book>();
            string text = tbx_searchbar.Text;
            foreach (Book book in books)
            {
                if (book.Title.ToLower().StartsWith(text.Trim().ToLower()))
                {
                    filteredBooks.Add(book);
                }
            }

            return filteredBooks;
        } 

        //megcsinálni v
        private void tbx_searchbar_GotFocus(object sender, RoutedEventArgs e)
        {
            tbx_searchbar.Text = "";

            PrintSortedBooks(books,false);
        }

        private void tbx_searchbar_MouseDown(object sender, MouseButtonEventArgs e)
        {

            tbx_searchbar.Text = " ";

            PrintSortedBooks(books,false);
        }


    }
}

