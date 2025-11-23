using konyv_webshop;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Xml.Schema;

namespace konyv_webshop
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
        private List<konyv_webshop.Book> books = new();
        public List<string> Genres { get; set; } = new();

        /// false -> alap: legkorábbi <summary>
        /// kiválasztott elem eltárolása ? 
        public MainWindow()
        {
            InitializeComponent();
            loadBooks("books.json");
            HideError();
            ShowPlus();
            HideSave();
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
            if(lbl_lang.Content.ToString()!.Trim() == "Magyar") 
            {
            lbl_lang.Content = "English";
            }
            else
            {
                lbl_lang.Content = "Magyar";
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
            books = JsonConvert.DeserializeObject<List<Book>>(json)!;
            foreach (Book book in books)
            {
                book.DateEdited = DateTime.Now;
                Genres.Add(book.Genre);
            }
            // Mindig a program elindítása az editedidő???
            books[3].DateEdited = DateTime.Now.AddDays(4);
            PrintSortedBooks(books, false);
            Genres = Genres.Distinct().ToList();
            cmbGenre.ItemsSource = Genres;
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


        private void tbx_searchbar_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            tbx_searchbar.Text = " ";

            PrintSortedBooks(books, false);
        }

        private void txtAuthor_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtAuthor.BorderBrush = Brushes.Green;
        }

        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtTitle.BorderBrush = Brushes.Green;
        }
        private void cmbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbGenre.BorderBrush = Brushes.Green;
        }
        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            HideError();
            bool valid = true;
            if (txtTitle.Text.Trim() == "") 
            {
                valid = false;
                txtTitle.BorderBrush = Brushes.Red;
            }
            if (txtAuthor.Text.Trim() == "")
            {
                valid = false;
                txtAuthor.BorderBrush = Brushes.Red;
            }
            if ((cmbGenre.SelectedItem == null && txtGenre.Text.Trim() == ""))
            {
                valid = false;
                txtGenre.BorderBrush = Brushes.Red;
                cmbGenre.BorderBrush = Brushes.Red;
            }
            if (txtPublisher.Text.Trim() == "")
            {
                valid = false;
                txtPublisher.BorderBrush = Brushes.Red;
            }
            foreach (var book in books)
            {
                if (txtTitle.Text == book.Title && txtAuthor.Text == book.Author && (cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre))
                {
                    ShowError("Már létezik ilyen könyv a listában!");
                    ShowError("Hozzáad egy példányt?");
                }
            }
            if (valid == true)
            {
                HidePlus();
                ShowSave();
            }
            else
            {
                errorMsg.Foreground = Brushes.Red;
                ShowError("Kérem adja meg a hiányzó adatokat! ");
            }
        }

        private void txtGenre_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtGenre.BorderBrush = Brushes.Green;
        }

        private void ShowError(string msg)
        {
            errorMsg.Text = msg;
            border_error.Visibility = Visibility.Visible;
            rct_error.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            rct_error.Visibility= Visibility.Collapsed;
            border_error.Visibility = Visibility.Collapsed;
        }


        // Új könyv hozzáadás biztos mentés
        private void ShowPlus()
        {
            rct_newBook.Visibility = Visibility.Visible;
            PlusButton.Visibility = Visibility.Visible;
        }
        private void HidePlus()
        {
            rct_newBook.Visibility = Visibility.Collapsed;
            PlusButton.Visibility = Visibility.Collapsed;
        }


        // Mentés mégse
        private void ShowSave()
        {
            rct_Save.Visibility = Visibility.Visible;
            rct_Cancel.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Visible;
        }
        private void HideSave()
        {
            rct_Save.Visibility = Visibility.Collapsed;
            rct_Cancel.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;
        }


        // Könyv Módosítás biztos mentés
        private void ShowChange()
        {
            rct_newBook.Visibility = Visibility.Visible;
            PlusButton.Visibility = Visibility.Visible;
        }
        private void HideChange()
        {
            rct_newBook.Visibility = Visibility.Collapsed;
            PlusButton.Visibility = Visibility.Collapsed;
        }

        private void txtPublisher_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPublisher.BorderBrush = Brushes.Green;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGenre.SelectedItem == null)
            {
                Genres.Add(txtGenre.Text);
                books.Add(new Book()
                {
                    Title = txtTitle.Text,
                    Author = txtAuthor.Text,
                    Genre = txtGenre.Text,
                    Publisher = txtPublisher.Text,
                    DateEdited = DateTime.Now
                });
            }
            else
            {
                books.Add(new Book()
                {
                    Title = txtTitle.Text,
                    Author = txtAuthor.Text,
                    Genre = cmbGenre.SelectedItem.ToString()!,
                    Publisher = txtPublisher.Text,
                    DateEdited = DateTime.Now
                });
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("awdasdaw");
            HideSave();
            ShowPlus();
            txtTitle.BorderBrush = Brushes.Transparent;
            txtAuthor.BorderBrush = Brushes.Transparent;
            cmbGenre.BorderBrush = Brushes.Transparent;
            txtGenre.BorderBrush = Brushes.Transparent;
            txtPublisher.BorderBrush = Brushes.Transparent;
        }
    }
}

