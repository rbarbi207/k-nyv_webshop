using konyv_webshop;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
   
    
    public partial class MainWindow : Window
    {
        public bool sorted = false;
        private List<konyv_webshop.Book> books = new();
        public List<string> Genres { get; set; } = new();
        public bool exists = false;
        public Book? matchingbook = null;
        public string errorMessage = "";
        private List<string> errors = new List<string>();


        private bool hasBeenDone = false;
        public MainWindow()
        {
            InitializeComponent();
            loadBooks("books.json");
            HideError();
            ShowPlus();
            HideSave();
            HideForm();
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
            if (lbl_lang.Content.ToString()!.Trim() == "Magyar")
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
                PrintSortedBooks(books, true);
            }
        }

        private void PrintSortedBooks(List<Book> books, bool clicked)
        {
            lbx_books.Items.Clear();
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
                item.Tag = book;
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
            List<DateTime> dates = new List<DateTime>();
            foreach (Book book in books)
            {
                dates.Add(book.DateEdited);
            }
            dates.Sort();

            foreach (DateTime date in dates)
            {
                List<int> indexes = searchIndexbyDate(date, books);
                foreach (int index in indexes)
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
            // Mindig a program elindítása az editedidő
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
            txtGenre.Text = "";
            cmbGenre.BorderBrush = Brushes.Green;
        }


        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (lbx_books.SelectedItem != null)
            {

                errors.Add("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?");
                HideForm();
                ShowError();
                ShowSave();
                HidePlus();
                ShowCancel();
                /// hide edit
                return;
            }
            else
            {
                ShowForm();
                ShowSave();
                HideError();
                HidePlus();
                ShowCancel();
            }
            return;
        }

        //private void PlusButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (lbx_books.SelectedItem != null)
        //    {
        //        lbx_books.SelectedItem = null;
        //    }
        //    Delete();
        //    ShowForm();
        //    ShowSave();
        //    ShowCancel();
        //    HideError();
        //    HidePlus();
        //    return;


        //}



        private void txtGenre_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtGenre.BorderBrush = Brushes.Green;
        }

        private void ShowError()
        {
            errorMsg.Text = "";
            errorMsg.Text = string.Join("\n", errors);
            border_error.Visibility = Visibility.Visible;
            rct_error.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            errors.Clear();
            errorMsg.Text = "";
            rct_error.Visibility = Visibility.Collapsed;
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

        private void dpDate_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            dpDate.BorderBrush = Brushes.Green;
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
        private Book? searchformatchingbook()
        {
            foreach (var book in books)
            {
                if (txtTitle.Text.ToLower() == book.Title.ToLower() && txtAuthor.Text.ToLower() == book.Author.ToLower() &&
                    (cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre) &&
                    (rad_ebook.IsChecked != book.Paper || rad_paper.IsChecked == book.Paper) &&
                    DateOnly.FromDateTime(dpDate.SelectedDate!.Value) == book.Year && txtnational.Text == book.Nationality && txtcopy.Text != "" )
                {
                    matchingbook = book;
                    exists = true;
                    errors.Add("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?");
                    hasBeenDone = true;
                    ShowSave();
                    return matchingbook;
                }
            }
            return null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            HideSave();
            HidePlus();
            ShowError();
            matchingbook = searchformatchingbook();
            hasBeenDone = true;
            if (hasBeenDone && matchingbook != null)
            {
                int i = books.IndexOf(matchingbook);
                books[i].Copies++;
                books[i].DateEdited = DateTime.Now;
                /////			
                HideError();
                PrintSortedBooks(books, false);
                ////
                ShowPlus();
                Delete();
                exists = false;
                hasBeenDone = false;
                matchingbook = null;        /// első mentés után még nem ír ki de utána igen

                
            }

            else
            {
                bool valid = true;
                int copy = 0;
                errors.Clear();




                if (txtTitle.Text.Trim() == "")
                {
                    valid = false;
                    txtTitle.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv címét! ");

                }
                if (txtAuthor.Text.Trim() == "")
                {
                    valid = false;
                    txtAuthor.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv szerzőjét! ");

                }
                if ((cmbGenre.SelectedItem == null && txtGenre.Text.Trim() == ""))
                {
                    valid = false;
                    txtGenre.BorderBrush = Brushes.Red;
                    cmbGenre.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv műfaját! ");

                }
                if (txtPublisher.Text.Trim() == "")
                {
                    valid = false;
                    txtPublisher.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv kiadóját! ");

                }
                if (rad_ebook.IsChecked == false && rad_paper.IsChecked == false)
                {
                    valid = false;
                    rad_ebook.BorderBrush = Brushes.Red;
                    rad_paper.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg hogy a könyv ebbok vagy papír! ");

                }
                if (dpDate.SelectedDate == null)
                {
                    valid = false;
                    dpDate.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv kiadási dátumát! ");

                }
                if (txtnational.Text.Trim() == "")
                {
                    valid = false;
                    txtnational.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg hogy milyen nyelven íródott a könyv! ");

                }
                if (txtcopy.Text.Trim() == "")
                {
                    valid = false;
                    txtcopy.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a példányszámot! ");

                }
                if (!int.TryParse(txtcopy.Text, out copy) && txtcopy.Text.Trim() != "")
                {
                    txtcopy.BorderBrush = Brushes.Red;
                    errors.Add("A példányszámnak számnak kell lennie!");

                }
                if (copy <= 0 && txtcopy.Text.Trim() != "" && int.TryParse(txtcopy.Text, out copy))
                {
                    txtcopy.BorderBrush = Brushes.Red;
                    errors.Add("A példányszámnak minimum 1-nek kell lennie!");
                }
                foreach (var book in books)
                {
                    if (txtTitle.Text.ToLower() == book.Title.ToLower() && txtAuthor.Text.ToLower() == book.Author.ToLower() &&
                        (cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre) &&
                        (rad_ebook.IsChecked != book.Paper || rad_paper.IsChecked == book.Paper) &&
                        DateOnly.FromDateTime(dpDate.SelectedDate!.Value) == book.Year && txtnational.Text == book.Nationality && (txtcopy.Text != "" && copy > 0))
                    {
                        matchingbook = book;
                        exists = true;
                        errors.Add("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?");
                        hasBeenDone = true;
                        ShowSave();
                        break;
                    }
                    else if (valid == true && (txtTitle.Text.ToLower() != book.Title.ToLower() &&
                        txtAuthor.Text.ToLower() != book.Author.ToLower() &&
                        (cmbGenre.SelectedItem?.ToString() != book.Genre || txtGenre.Text != book.Genre) &&
                        (rad_ebook.IsChecked == book.Paper || rad_paper.IsChecked != book.Paper)) &&
                        DateOnly.FromDateTime(dpDate.SelectedDate!.Value) != book.Year && txtnational.Text != book.Nationality && (txtcopy.Text != "" && copy > 0))
                    {
                        exists = false;
                        HidePlus();
                        ShowSave();
                    }
                    else if (valid == false && !hasBeenDone)
                    {
                        exists = false;

                    }
                    if (errors.Count > 0)
                    {

                        ShowError();
                        return;
                    }

                }

                //// új könyv létrehozása
                int j = books.Count;
                if (exists == false && valid == true)
                {
                    if (cmbGenre.SelectedItem == null)
                    {
                        if (rad_paper.IsChecked == true)
                        {
                            Genres.Add(txtGenre.Text);
                            books.Add(new Book()
                            {
                                Id = j + 1,
                                Title = txtTitle.Text,
                                Author = txtAuthor.Text,
                                Genre = txtGenre.Text,
                                Publisher = txtPublisher.Text,
                                Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                                Copies = 1,
                                Paper = true,
                                Nationality = txtnational.Text,
                                DateEdited = DateTime.Now
                            });
                        }
                        else
                        {
                            Genres.Add(txtGenre.Text);
                            books.Add(new Book()
                            {
                                Id = j + 1,
                                Title = txtTitle.Text,
                                Author = txtAuthor.Text,
                                Genre = txtGenre.Text,
                                Publisher = txtPublisher.Text,
                                Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                                Copies = 1,
                                Paper = false,
                                Nationality = txtnational.Text,
                                DateEdited = DateTime.Now
                            });
                        }
                    }
                    else
                    {
                        if (rad_paper.IsChecked == true)
                        {
                            Genres.Add(txtGenre.Text);
                            books.Add(new Book()
                            {
                                Id = j + 1,
                                Title = txtTitle.Text,
                                Author = txtAuthor.Text,
                                Genre = cmbGenre.SelectedItem.ToString()!,
                                Publisher = txtPublisher.Text,
                                Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                                Copies = 1,
                                Paper = true,
                                Nationality = txtnational.Text,
                                DateEdited = DateTime.Now
                            });
                        }
                        else
                        {
                            Genres.Add(txtGenre.Text);
                            books.Add(new Book()
                            {
                                Id = j + 1,
                                Title = txtTitle.Text,
                                Author = txtAuthor.Text,
                                Genre = cmbGenre.SelectedItem.ToString()!,
                                Publisher = txtPublisher.Text,
                                Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                                Copies = 1,
                                Paper = false,
                                Nationality = txtnational.Text,
                                DateEdited = DateTime.Now
                            });
                        }
                    }
                }

                File.WriteAllText("books.json",
                JsonConvert.SerializeObject(books, Formatting.Indented));

                HideError();
                HideForm();
                PrintSortedBooks(books, false);
                Delete();
            }
        }
        private void SaveButton_Clicka(object sender, RoutedEventArgs e)
        {
            //ShowError();

            // hasbeendone ->már létezik ÉS talált rá megegyező könyvet
            if (hasBeenDone && matchingbook != null)
            {
                int i = books.IndexOf(matchingbook);
                books[i].Copies++ ;
                books[i].DateEdited = DateTime.Now;

                File.WriteAllText("books.json",
            JsonConvert.SerializeObject(books, Formatting.Indented));
                HideSave(); 
                ShowPlus();
                Delete();
            }

            else
            {
            HidePlus();
            bool valid = true;
            int copy = 0;

            errors.Clear();



            foreach (var book in books)
            {
                if (txtTitle.Text.Trim() == "")
                {
                    valid = false;
                    txtTitle.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv címét! ");
                }
                if (txtAuthor.Text.Trim() == "")
                {
                    valid = false;
                    txtAuthor.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv szerzőjét! ");
                }
                if ((cmbGenre.SelectedItem == null && txtGenre.Text.Trim() == ""))
                {
                    valid = false;
                    txtGenre.BorderBrush = Brushes.Red;
                    cmbGenre.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv műfaját! ");
                }
                if (txtPublisher.Text.Trim() == "")
                {
                    valid = false;
                    txtPublisher.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv kiadóját! ");
                }
                if (rad_ebook.IsChecked == false && rad_paper.IsChecked == false)
                {
                    valid = false;
                    rad_ebook.BorderBrush = Brushes.Red;
                    rad_paper.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg hogy a könyv ebbok vagy papír! ");
                }
                if (dpDate.SelectedDate == null)
                {
                    valid = false;
                    dpDate.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a könyv kiadási dátumát! ");
                }
                if (txtnational.Text.Trim() == "")
                {
                    valid = false;
                    txtnational.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg hogy milyen nyelven íródott a könyv! ");
                }
                if (txtcopy.Text.Trim() == "")
                {
                    valid = false;
                    txtcopy.BorderBrush = Brushes.Red;
                    errors.Add("Kérem adja meg a példányszámot! ");
                }
                if (!int.TryParse(txtcopy.Text, out copy) && txtcopy.Text.Trim() != "")
                {
                    txtcopy.BorderBrush = Brushes.Red;
                    errors.Add("A példányszámnak számnak kell lennie!");
                }
                if (copy <= 0 && txtcopy.Text.Trim() != "" && int.TryParse(txtcopy.Text, out copy))
                {
                    txtcopy.BorderBrush = Brushes.Red;
                    errors.Add("A példányszámnak minimum 1-nek kell lennie!");
                }

                if (txtTitle.Text.ToLower() == book.Title.ToLower() && txtAuthor.Text.ToLower() == book.Author.ToLower() &&
                    (cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre) &&
                    (rad_ebook.IsChecked != book.Paper || rad_paper.IsChecked == book.Paper) &&
                    DateOnly.FromDateTime(dpDate.SelectedDate!.Value) == book.Year && txtnational.Text == book.Nationality && (txtcopy.Text != "" && copy > 0))
                {
                    matchingbook = book;
                    exists = true;
                    errors.Add("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?");
                    hasBeenDone = true;
                    ShowSave();
                    break;
                }
                else if (valid == true && (txtTitle.Text.ToLower() != book.Title.ToLower() &&
                    txtAuthor.Text.ToLower() != book.Author.ToLower() &&
                    (cmbGenre.SelectedItem?.ToString() != book.Genre || txtGenre.Text != book.Genre) &&
                    (rad_ebook.IsChecked == book.Paper || rad_paper.IsChecked != book.Paper)) &&
                    DateOnly.FromDateTime(dpDate.SelectedDate!.Value) != book.Year && txtnational.Text != book.Nationality && (txtcopy.Text != "" && copy > 0))
                {
                    exists = false;
                    HidePlus();
                    ShowSave();
                }
                else if (valid == false && !hasBeenDone)
                {
                    exists = false;

                }
                    if (errors.Count > 0)
                    {

                        ShowError();
                    }

                }
          
            //// új könyv létrehozása
            int j = books.Count;
            if (exists == false && valid == true)
            {
                if (cmbGenre.SelectedItem == null)
                {
                    if (rad_paper.IsChecked == true)
                    {
                        Genres.Add(txtGenre.Text);
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            Genre = txtGenre.Text,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = true,
                            Nationality = txtnational.Text,
                            DateEdited = DateTime.Now
                        });
                    }
                    else
                    {
                        Genres.Add(txtGenre.Text);
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            Genre = txtGenre.Text,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = false,
                            Nationality = txtnational.Text,
                            DateEdited = DateTime.Now
                        });
                    }
                }
                else
                {
                    if (rad_paper.IsChecked == true)
                    {
                        Genres.Add(txtGenre.Text);
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            Genre = cmbGenre.SelectedItem.ToString()!,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = true,
                            Nationality = txtnational.Text,
                            DateEdited = DateTime.Now
                        });
                    }
                    else
                    {
                        Genres.Add(txtGenre.Text);
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            Genre = cmbGenre.SelectedItem.ToString()!,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = false,
                            Nationality = txtnational.Text,
                            DateEdited = DateTime.Now
                        });
                    }
                }
            }
            }
 
            File.WriteAllText("books.json",
            JsonConvert.SerializeObject(books, Formatting.Indented));

            HideForm();
            PrintSortedBooks(books, false);
            Delete();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideForm();
            ShowPlus();
            HideSave();
            HideError();
            // kell if hogyha hozzáadásnál van 
            txtTitle.BorderBrush = Brushes.Transparent;
            txtAuthor.BorderBrush = Brushes.Transparent;
            cmbGenre.BorderBrush = Brushes.Transparent;
            txtGenre.BorderBrush = Brushes.Transparent;
            txtPublisher.BorderBrush = Brushes.Transparent;
            txtnational.BorderBrush = Brushes.Transparent;
            txtcopy.BorderBrush = Brushes.Transparent;
            dpDate.BorderBrush = Brushes.Transparent;
            rad_ebook.BorderBrush = Brushes.Transparent;
            rad_paper.BorderBrush = Brushes.Transparent;
            txtTitle.Text = "";
            txtAuthor.Text = "";
            rad_ebook.IsChecked = false;
            rad_paper.IsChecked = false;
            dpDate.SelectedDate = null;
            txtnational.Text = "";
            txtGenre.Text = "";
            cmbGenre.SelectedItem = null;
            txtPublisher.Text = "";
            txtcopy.Text = "";
            lbx_books.SelectedItem = null;
            Delete();
        }

        private void rad_ebook_Checked(object sender, RoutedEventArgs e)
        {
            rad_ebook.BorderBrush = Brushes.Green;
        }

        private void rad_paper_Checked(object sender, RoutedEventArgs e)
        {
            rad_paper.BorderBrush = Brushes.Green;
        }

        private void txtNatioal_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtnational.BorderBrush = Brushes.Green;
        }

        private void HideForm()
        {
            txtAuthor.IsEnabled = false;
            txtAuthor.FontWeight = FontWeights.Bold;
            txtAuthor.Foreground = Brushes.Black;
            txtGenre.IsEnabled = false;
            txtGenre.FontWeight = FontWeights.Bold;
            txtGenre.Foreground = Brushes.Black;
            txtPublisher.IsEnabled = false;
            txtPublisher.Foreground = Brushes.Black;
            txtPublisher.FontWeight = FontWeights.Bold;
            txtTitle.IsEnabled = false;
            txtTitle.FontWeight = FontWeights.Bold;
            txtTitle.Foreground = Brushes.Black;
            rad_ebook.IsEnabled = false;
            rad_ebook.Foreground = Brushes.Black;
            rad_ebook.FontWeight = FontWeights.Bold;
            rad_paper.Foreground = Brushes.Black;
            rad_paper.IsEnabled = false;
            rad_paper.FontWeight = FontWeights.Bold;
            txtnational.IsEnabled = false;
            txtnational.Foreground = Brushes.Black;
            txtnational.FontWeight = FontWeights.Bold;
            dpDate.IsEnabled = false;
            dpDate.Foreground = Brushes.Black;
            dpDate.FontWeight = FontWeights.Bold;
            cmbGenre.IsEnabled = false;
            cmbGenre.Foreground = Brushes.Black;
            cmbGenre.FontWeight = FontWeights.Bold;
            txtcopy.IsEnabled = false;
            txtcopy.Foreground = Brushes.Black;
            txtcopy.FontWeight = FontWeights.Bold;
        }
        private void ShowForm()
        {
            txtAuthor.IsEnabled = true;
            txtGenre.IsEnabled = true;
            txtPublisher.IsEnabled = true;
            txtTitle.IsEnabled = true;
            rad_ebook.IsEnabled = true;
            rad_paper.IsEnabled = true;
            txtnational.IsEnabled = true;
            dpDate.IsEnabled = true;
            cmbGenre.IsEnabled = true;
            txtcopy.IsEnabled = true;
        }

        private void Delete()
        {
            txtTitle.Text = "";
            txtTitle.BorderBrush = Brushes.Transparent;
            txtAuthor.Text = "";
            txtAuthor.BorderBrush = Brushes.Transparent;
            rad_ebook.IsChecked = false;
            rad_ebook.BorderBrush = Brushes.Transparent;
            rad_paper.IsChecked = false;
            rad_paper.BorderBrush = Brushes.Transparent;
            dpDate.SelectedDate = null;
            dpDate.BorderBrush = Brushes.Transparent;
            txtnational.Text = "";
            txtnational.BorderBrush = Brushes.Transparent;
            txtGenre.Text = "";
            txtGenre.BorderBrush = Brushes.Transparent;
            cmbGenre.SelectedItem = null;
            cmbGenre.BorderBrush = Brushes.Transparent;
            txtPublisher.Text = "";
            txtPublisher.BorderBrush = Brushes.Transparent;
            txtcopy.Text = "";
            txtcopy.BorderBrush = Brushes.Transparent;
        }

        private void txtCopy_TextChanged(object sender, TextChangedEventArgs e)
        {
            rad_ebook.BorderBrush = Brushes.Green;
        }

        private void HideCancel()
        {
            cancelButton.Visibility = Visibility.Collapsed;
        }
        private void ShowCancel()
        {
            cancelButton.Visibility = Visibility.Visible;
        }

        private void lbx_books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowChange();
            txtGenre.Text = "";
            //hide delete
            HideError();
            HideSave();
            HideCancel();
            ShowPlus();
            if (lbx_books.SelectedItem is ListBoxItem item &&
            item.Tag is Book selectedBook)
            {
                txtTitle.Text = selectedBook.Title;
                txtTitle.BorderBrush = Brushes.Transparent;
                txtAuthor.Text = selectedBook.Author;
                txtAuthor.BorderBrush = Brushes.Transparent;
                cmbGenre.Text = selectedBook.Genre;
                txtGenre.BorderBrush = Brushes.Transparent;
                txtPublisher.Text = selectedBook.Publisher;
                txtPublisher.BorderBrush = Brushes.Transparent;
                txtnational.Text = selectedBook.Nationality;
                txtnational.BorderBrush = Brushes.Transparent; ;
                txtcopy.Text = selectedBook.Copies.ToString();
                txtcopy.BorderBrush = Brushes.Transparent;

                rad_ebook.IsChecked = !selectedBook.Paper;
                rad_paper.IsChecked = selectedBook.Paper;

                dpDate.SelectedDate = new DateTime(
                    selectedBook.Year.Year,
                    selectedBook.Year.Month,
                    selectedBook.Year.Day
                );
                dpDate.BorderBrush = Brushes.Transparent;
            }
            
        }

        private void PlusButton_MouseEnter(object sender, MouseEventArgs e)
        {
            PlusButton.Background = Brushes.Transparent;
        }

        private void DeleteBook(Book book)
        {
            Delete();
            books.Remove(book);

            List<Book> Filteredbooks = filterBooks();
            {
                PrintSortedBooks(Filteredbooks, false);

            }
        }

        private void errorMsg_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}


// button ->pointer 
// törlés
// módosítás
// csúnya borderek eltűntetése
// reszponziv
// style
// angol
// comboboxnál megoldani, hgoy kitörlődjön amikor be van írva a másikba: odavissza
 /// menti-e módosításait mikor rányom egy másikra, illetve amikor a mégsére nyom
    /// - törlés és módosítás esetén hoverkor jelenjen meg szövegben az hogy nincs kiválasztva elem dőlt betűkkel (nem súlyos), ilyenkor NE változzon a kinézet hover esetén (plussz a kurzor se)
    /// módosításnál felülíródik a mentett dátum <>
    
    /// todo:  az ikonok működése reszponzívan
    /// 
