using konyv_webshop;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace konyv_webshop
{
    //// plusbutton fgv.
    //// savebutton fgv.
    //// textboxokra padding = 2,5 (xaml)
    /// borderes buttonok (xaml)
    /// erroros rectangle (xaml)
    /// listbox fgv. a másiknak kitörlése
    /// keresés duplaklikk -> megoldani


    /// stílus, reszpo, nyelv
    //kép -> majd kiválasztani az eredeti projektben a pathet
    /// todo:  az ikonok működése reszponzívan
    public partial class MainWindow : Window
    {
        private readonly string jsonPath =
        System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\books.json"));
        public bool sorted = false;
        private List<konyv_webshop.Book> books = new();
        public List<string> Genres { get; set; } = new();
        private List<string> errors = new List<string>();
        public bool exists = false;
        public Book? matchingbook = null;
        private bool hasBeenDone = false;
        public string errorMessage = "";
        private bool hasShownError = false;

        private int index = -1;
        public MainWindow()
        {
            InitializeComponent();
            loadBooks("books.json");
            HideError();
            ShowPlus();
            HideSave();
            HideForm();
            br_Clear.Visibility = Visibility.Collapsed;
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
                lbl_title.Content = CapitalizeWords(displayTitle);
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
            string json = File.ReadAllText(jsonPath);
            books = JsonConvert.DeserializeObject<List<Book>>(json)!;
            foreach (Book book in books)
            {
                book.DateEdited = DateTime.Now;
                Genres.Add(book.Genre);
            }
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
            rct_author.Stroke = Brushes.Transparent;
            rct_author.StrokeThickness = 0;
        }

        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            rct_title.Stroke = Brushes.Transparent;
            rct_title.StrokeThickness = 0;
        }
        private void cmbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtGenre.Text = "";
            rct_genre.Stroke = Brushes.Transparent;
            rct_genre.StrokeThickness = 0;
        }
        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (lbx_books.SelectedItem != null)
            {
                HideForm();
                errors.Add("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?");
            
            }
            else
            {
                ShowForm();
                HideError();

            }
            ShowSave();
            ShowCancel();
            HidePlus();
            return;
        }

        private void txtGenre_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtGenre.Text != "")
            {
                rct_genre.Stroke = Brushes.Transparent;
                rct_genre.StrokeThickness = 0;
                cmbGenre.SelectedItem = null;

            }
        }

        private void ShowError()
        {
            errorMsg.Text = "";
            errorMsg.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF24142F")!;

            if (errors.Contains("A mentéshez ki kell töltenie minden mezőt!"))
            {
                errorMsg.Text = "A mentéshez ki kell töltenie minden mezőt!";
            }
            else
            {

                errorMsg.Text = string.Join("\n", errors);
            }
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
            br_Clear.Visibility = Visibility.Visible;
            br_newBook.Visibility = Visibility.Visible;
            PlusButton.Visibility = Visibility.Visible;
        }
        private void HidePlus()
        {
            br_newBook.Visibility = Visibility.Collapsed;
            PlusButton.Visibility = Visibility.Collapsed;
        }


        // Mentés mégse
        private void ShowSave()
        {
            br_Clear.Visibility= Visibility.Collapsed;
            br_Save.Visibility = Visibility.Visible;
            br_Cancel.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Visible;
        }
        private void HideSave()
        {
            br_Save.Visibility = Visibility.Collapsed;
            br_Cancel.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;
        }

        private void dpDate_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            rct_Date.Stroke = Brushes.Transparent;
            rct_Date.StrokeThickness = 0;
        }
    
        private void ShowChange()
        {
            br_newBook.Visibility = Visibility.Visible;
            PlusButton.Visibility = Visibility.Visible;
        }
        private void HideChange()
        {
            br_newBook.Visibility = Visibility.Collapsed;
            PlusButton.Visibility = Visibility.Collapsed;
        }

        private void txtPublisher_TextChanged(object sender, TextChangedEventArgs e)
        {
            rct_publisher.Stroke = Brushes.Transparent;
            rct_publisher.StrokeThickness = 0;
        }

        private Book? searchformatchingbook()
        {
            foreach (var book in books)
            {
                if (txtTitle.Text.ToLower() == book.Title.ToLower() && txtAuthor.Text.ToLower() == book.Author.ToLower() &&
                    (cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre) &&
                    (rad_ebook.IsChecked != book.Paper || rad_paper.IsChecked == book.Paper) &&
                    DateOnly.FromDateTime(dpDate.SelectedDate!.Value) == book.Year && txtnational.Text == book.Nationality && txtcopy.Text != "")
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
       /// mentés
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            /// ha van már egy könyv és először nyom rá a mentésre
            if (errors.Count > 0 && hasShownError == false)
            {
                ShowError();

                errors.Clear();
            }
            // ha van könyv, és másodszorra nyom a mentésre
            if (errors.Count == 0 && searchformatchingbook() != null && hasShownError)
            {
                lbx_books.SelectedItem = null;
                index = -1;
                matchingbook = searchformatchingbook();
                int indexOfBook = books.IndexOf(matchingbook!);
                books[indexOfBook].Copies++;
                books[indexOfBook].DateEdited = DateTime.Now;
                File.WriteAllText(jsonPath,JsonConvert.SerializeObject(books, Formatting.Indented));
                ShowPlus();
                HideSave();
                Delete();
                PrintSortedBooks(books, false);
            }
            else if (lbx_books.SelectedItem == null)
            {
                bool valid = true;
                int copy = 0;
                if (txtTitle.Text.Trim() == "")
                {
                    
                    rct_title.Stroke = Brushes.Red;
                    rct_title.StrokeThickness = 1;
                    //errors.Add("Kérem adja meg a könyv címét! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");

                }
                if (txtAuthor.Text.Trim() == "")
                {
            
                    rct_author.Stroke = Brushes.Red;
                    rct_author.StrokeThickness = 1;
                    //errors.Add("Kérem adja meg a könyv szerzőjét! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");

                }
                if ((cmbGenre.SelectedItem == null && txtGenre.Text.Trim() == ""))
                {
                    rct_genre.Stroke = Brushes.Red;
                    rct_genre.StrokeThickness = 1;


                    //errors.Add("Kérem adja meg a könyv műfaját! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");
                }
                if (txtPublisher.Text.Trim() == "")
                {
                    rct_publisher.Stroke = Brushes.Red;
                    rct_publisher.StrokeThickness = 1;
                    //errors.Add("Kérem adja meg a könyv kiadóját! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");
                }
                if (rad_ebook.IsChecked == false && rad_paper.IsChecked == false)
                {
                    rct_ebook.Stroke = Brushes.Red;
                    rct_ebook.StrokeThickness = 1;

                    //rad_ebook.BorderBrush = Brushes.Red;
                    //rad_paper.BorderBrush = Brushes.Red;
                    //errors.Add("Kérem adja meg hogy a könyv ebbok vagy papír! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");

                }
                if (dpDate.SelectedDate == null)
                {
                    rct_Date.StrokeThickness = 1;
                    rct_Date.Stroke = Brushes.Red;
                    //errors.Add("Kérem adja meg a könyv kiadási dátumát! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");
                }
                if (txtnational.Text.Trim() == "")
                {
                    rct_national.Stroke = Brushes.Red;
                    rct_national.StrokeThickness = 1;
                    //errors.Add("Kérem adja meg hogy milyen nyelven íródott a könyv! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");
                }
                if (txtcopy.Text.Trim() == "")
                {
                    rct_copy.Stroke = Brushes.Red;
                    rct_copy.StrokeThickness = 1;
                    //errors.Add("Kérem adja meg a példányszámot! ");
                    errors.Add("A mentéshez ki kell töltenie minden mezőt!");
                }
                if (!int.TryParse(txtcopy.Text, out copy) && txtcopy.Text.Trim() != "")
                {
                    rct_copy.Stroke = Brushes.Red;
                    rct_copy.StrokeThickness = 1;
                    if (!errors.Contains("A példányszámnak számnak kell lennie!"))
                    {
                    errors.Add("A példányszámnak számnak kell lennie!");

                    }
                    
                }
                if (copy <= 0 && txtcopy.Text.Trim() != "" && int.TryParse(txtcopy.Text, out copy))
                {
                    rct_copy.Stroke = Brushes.Red;
                    rct_copy.StrokeThickness = 1;
                    if (!errors.Contains("A példányszámnak minimum 1-nek kell lennie!"))
                    {
                        errors.Add("A példányszámnak minimum 1-nek kell lennie!");

                    }
                }
                if (errors.Count > 0)
                {
                    valid = false;
                    if (errors.Contains("A mentéshez ki kell töltenie minden mezőt!"))
                    {
                        List<string> OriginalErrors = new List<string>();
                        foreach(string error in errors)
                        {
                            if (error != "A mentéshez ki kell töltenie minden mezőt!")
                            {
                                OriginalErrors.Add(error);
                            }
                        }
                        errors = OriginalErrors;
                        errors.Add("A mentéshez ki kell töltenie minden mezőt!");
                    }
                    if (index == -1 && SaveButton.Visibility == Visibility.Visible && !IsEmptyAll())
                    {
                        MessageBox.Show("");
                    }
                    ShowError();
                }
/// bemásolni   /// könyv mentése
                if (valid && lbx_books.SelectedItem == null)
                {
                    string genre;
                    bool paper;
                    if (cmbGenre.SelectedItem == null)
                    {
                        genre = txtGenre.Text;
                        Genres.Add(txtGenre.Text);
                    }
                    else if (txtGenre.Text == "")
                    {
                        genre = cmbGenre.SelectedItem.ToString()!;
                    }
                    if (rad_paper.IsChecked == true)
                    {
                        paper = true;
                    }
                    else
                    {
                        paper = false;
                    }
                    books.Add(new Book()
                    {
                        Id = books.Count,
                        Title = CapitalizeWords(txtTitle.Text),
                        Author = CapitalizeWords(txtAuthor.Text),
                        Genre = CapitalizeWords(cmbGenre.SelectedItem?.ToString() ?? txtGenre.Text),
                        Publisher = CapitalizeWords(txtPublisher.Text),
                        Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                        Copies = 1,
                        Paper = paper,
                        Nationality = CapitalizeWords(txtnational.Text),
                        DateEdited = DateTime.Now
                    });


                    File.WriteAllText("books.json",JsonConvert.SerializeObject(books, Formatting.Indented));

                    PrintSortedBooks(books, false);

                    HideForm();
                    Delete();
                }
///////                
            }
            errors.Clear();
            hasShownError = true;

        }
       /// mégse
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            lbx_books.SelectionChanged -= lbx_books_SelectionChanged;
            HideError();
            HideForm();
            ShowPlus();
            HideSave();
            errors.Clear();
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
            index = -1;
            Delete();
            hasBeenDone = false;
            matchingbook = null;
            lbx_books.SelectionChanged += lbx_books_SelectionChanged;
        }

        private void rad_ebook_Checked(object sender, RoutedEventArgs e)
        {


            rct_ebook.Stroke = Brushes.Transparent;
            rct_ebook.StrokeThickness = 0;


            rad_ebook.FontWeight = FontWeights.Bold;
            rad_paper.FontWeight = FontWeights.Normal;
        }

        private void rad_paper_Checked(object sender, RoutedEventArgs e)
        {

            rct_ebook.Stroke = Brushes.Transparent;
            rct_ebook.StrokeThickness = 0;
            rad_ebook.FontWeight = FontWeights.Normal;
            rad_paper.FontWeight = FontWeights.Bold;
        }

        private void txtNatioal_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            rct_national.Stroke = Brushes.Transparent;
            rct_national.StrokeThickness = 0;
        }

        private void HideForm()
        {
          
            txtAuthor.IsEnabled = false;
            txtAuthor.FontWeight = FontWeights.Bold;
            txtAuthor.Foreground = Brushes.Black;
            txtGenre. IsEnabled = false;
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
            rad_paper.Foreground = Brushes.Black;
            rad_paper.IsEnabled = false;
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
            br_Clear.Visibility = Visibility.Collapsed;
        
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

        //private void HideCancel()
        //{
        //    cancelButton.Visibility = Visibility.Collapsed;
        //}
        private void ShowCancel()
        {
            cancelButton.Visibility = Visibility.Visible;
        }

        private void txtCopy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtcopy.Text))
            {
                HideError();
                rct_copy.Stroke = Brushes.Transparent;
                rct_copy.StrokeThickness = 0;
                return;
            }
            int n;
            if (!int.TryParse(txtcopy.Text, out n)) {
                //txtcopy.BorderBrush = Brushes.Red;
                errors.Clear();
                errors.Add("A példányszámnak számnak kell lennie!");
                ShowError();
            }
            else if (n < 0)
            {
                //txtcopy.BorderBrush = Brushes.Red;
                errors.Clear();
                errors.Add("A példányszámnak 0-nál nagyobbnak kell lennie!");
                ShowError();
            }
            else
            {
                errors.Clear();
                rct_copy.Stroke = Brushes.Transparent;
                rct_copy.StrokeThickness = 0;
            }
        }

        private bool IsEmptyAll()
        {
            bool empty = false;
            if (txtAuthor.Text == "" && txtTitle.Text == "" && txtcopy.Text == "" && dpDate.Text == "" && rad_ebook.IsChecked == false && rad_ebook.IsChecked== false && cmbGenre.SelectedItem == null && txtGenre.Text == "" && txtPublisher.Text == "" && txtnational.Text == "")
            {
                empty = true;
            }

            return empty;
        }
        private void lbx_books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ///// nem volt ezelőtt a change előtt item selectelve -> azt jelenti, hogy plussz gomb megnyomása esetén a felhasználó dolgozott a felületen
            //if (index == -1 && SaveButton.Visibility == Visibility.Visible && !IsEmptyAll())
            //{
            //    hasShownError = false;
            //    MessageBox.Show("Ha tovább lép, elfognak veszni a mentetlen adatok.", "Biztosan tovább lép?");
            //}

            lbx_books.Visibility = Visibility.Visible;
            hasShownError = false;
            errors.Clear();
            HideForm();
            HideError();
            HideSave();
            ShowPlus();

            if (lbx_books.SelectedItem == null)
            {
                HideError();
                HideSave();
                return;
            }


            else if(lbx_books.SelectedItem is ListBoxItem item &&
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
            ShowChange();
            txtGenre.Text = "";

            index = lbx_books.Items.IndexOf(lbx_books.SelectedItem);

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

        string CapitalizeWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(text.ToLower());
        }

        private void br_Clear_MouseDown(object sender, RoutedEventArgs e)
        {
            lbx_books.SelectedItem = null;
            index = -1;
            Delete();
        }
    }
}

// törlés
// módosítás
// reszponziv
// style
// angol
/// módosításnál felülíródik a mentett dátum <>
/// menti-e módosításait mikor rányom egy másikra, illetve amikor a mégsére nyom

   


