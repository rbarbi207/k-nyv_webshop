using konyv_wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Resources;
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

namespace konyv_wpf
{

    public partial class MainWindow : Window
    {
        private readonly string jsonPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\books.json"));
        
        private Brush color = new SolidColorBrush(Color.FromRgb(199, 26, 26));
        
        private List<konyv_wpf.Book> books = new();
        public List<string> Genres = new();
        public List<string> Genre_En = new();
        private List<string> errors = new List<string>();
        private List<char> chars = ['0','1','2','3','4','5','6','7','8','9','*','{',']','}',']','(',')','#','@','˘','^','ˇ','~','°','"','˙','´','¸','¤','_','/','×','$','÷','|','=','+'];
        private bool ascending = true;
        private bool hasBeenDone = false;
        public static string currentLanguage = "HU";
        public bool sorted = true;
        public bool exists = false; 
        public bool modificationClicked = false;
        public int idNumber = 0;
        ListBoxItem? item = null;
        public Book? matchingbook = null;
        public Book? lastClicked = null;
        public Book? tobeModified = null;
        public string? GenreIsEnglish = null;
        public MainWindow()
        {
            InitializeComponent();
            loadBooks("books.json");
            HideError();
            ShowPlus();
            HideSave();
            HideForm();
            br_Clear.Visibility = Visibility.Collapsed;
            br_Modify.Visibility = Visibility.Hidden;
            br_Delete.Visibility = Visibility.Hidden;
        
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                e.Handled = true; 
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth < 720)
            {
                br_Clear.VerticalAlignment = VerticalAlignment.Top;
                br_Clear.Margin = new Thickness(0, 40, 10, 20);
                br_newBook.Width = 80;
                br_Delete.Margin = new Thickness(0, 0, 100, 20);
                br_Delete.Width = 95;
                br_Modify.VerticalAlignment = VerticalAlignment.Top;
                br_Modify.Margin = new Thickness(0, 40, 100, 20);
            }
            else
            {
                br_Clear.VerticalAlignment = VerticalAlignment.Bottom;
                br_Clear.Margin = new Thickness(0, 0, 245, 20);
                br_newBook.Width = 50;
                br_Delete.Margin = new Thickness(0, 0, 65, 20);
                br_Delete.Width = 75;
                br_Modify.VerticalAlignment = VerticalAlignment.Bottom;
                br_Modify.Margin = new Thickness(0, 0, 145, 20);
            }
        }

        private async void ShowToast(string resourceKey)
        {
            ToastText.Text = (string)FindResource(resourceKey);
            ToastPopUp.Visibility = Visibility.Visible;

            await Task.Delay(1000); 

            ToastPopUp.Visibility = Visibility.Collapsed;
        }

        // ---------- Language  
        private void lbl_lang_MouseEnter(object sender, MouseEventArgs e)
        {
            lbl_lang.Opacity = 0.4;
            txtb_lang.Opacity = 0.4;
        }
        private void lbl_lang_MouseLeave(object sender, MouseEventArgs e)
        {
            lbl_lang.Opacity = 0.6;
            txtb_lang.Opacity = 0.6;
        }
        private void lbl_lang_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();
            if (currentLanguage == "HU")
            {
                currentLanguage = "EN";
                lbl_lang.Content = "English";
                SetLanguage("en");
            }
            else
            {
                currentLanguage = "HU";
                lbl_lang.Content = "Magyar";
                SetLanguage("hu");
            }
            PrintSortedBooks(books, false);
        }
        private void txtb_lang_MouseEnter(object sender, MouseEventArgs e)
        {
            lbl_lang.Opacity = 0.4;
            txtb_lang.Opacity = 0.4;
        }
        private void txtb_lang_MouseLeave(object sender, MouseEventArgs e)
        {
            lbl_lang.Opacity = 0.6;
            txtb_lang.Opacity = 0.6;
        }
        private void txtb_lang_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();
            if (currentLanguage == "HU")
            {
                currentLanguage = "EN";
                lbl_lang.Content = "English";
                SetLanguage("en");
            }
            else
            {
                currentLanguage = "HU";
                lbl_lang.Content = "Magyar";
                SetLanguage("hu");
            }
            PrintSortedBooks(books, false);
        }



        // ---------- Listbox Manual Update
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

        

        // ---------- Searchbar
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
            else if(tbx_searchbar.Text == null)
            {
                Delete();
            }
            else
            {
                lbx_books.Items.Clear();
                ListBoxItem item = new ListBoxItem();
                item.Content = T("Nincs találat ezen a néven!", "No results found under this name!");
                item.HorizontalAlignment = HorizontalAlignment.Center;
                item.IsHitTestVisible = false;
                item.Focusable = false;
                lbx_books.Items.Add(item);
            }
            HideForm();

        }
        private void tbx_searchbar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Keyboard.ClearFocus();
        }
        private void tbx_searchbar_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            tbx_searchbar.Text = "";
           

        }


        // ---------- Elemek change funkciója - form rész
        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg a könyv címét! ", "Please enter a book title! "));
            ShowError();

            //// bármi változik akkor ez is változik 
            modificationClicked = false;
            rct_title.Stroke = Brushes.Transparent;
            bool IsError = false;
            foreach (char C in chars)
            {
                if (txtTitle.Text.Contains(C))
                {
                    IsError = true;
                    break;
                }
            }
            if (IsError)
            {
                errors.Add(T("A cím nem tartalmazhat speciális karaktereket!", "The title can't contain special characters!"));
                ShowError();
                tick1.Visibility = Visibility.Collapsed;
                rct_title.Stroke = Brushes.Red;
            }
            if (txtTitle.Text.Trim() != "" && txtTitle.IsEnabled == true && IsError == false)
            {
                tick1.Visibility = Visibility.Visible;
            }
            if (txtTitle.Text.Trim() == "")
            {
                tick1.Visibility = Visibility.Collapsed;
                errors.Remove(T("A cím nem tartalmazhat speciális karaktereket!", "The title can't contain special characters!"));
                ShowError();
            }
        }
        private void txtAuthor_TextChanged(object sender, TextChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg a könyv szerzőjét! ", "Please enter the author of the book!"));
            ShowError();

            bool IsError = false;
            rct_author.Stroke = Brushes.Transparent;
            foreach (char C in chars)
            {
                if (txtAuthor.Text.Contains(C))
                {
                    IsError = true;
                    break;
                }
            }
            if (IsError)
            {
                errors.Add(T("A szerző nem tartalmazhat speciális karaktereket!", "The author can't contain special characters!"));
                ShowError();
                tick2.Visibility = Visibility.Collapsed;
                rct_author.Stroke = Brushes.Red;
            }
            if (txtAuthor.Text.Trim() != "" && txtAuthor.IsEnabled == true && IsError == false)
            {
                tick2.Visibility = Visibility.Visible;
            }
            if (txtAuthor.Text.Trim() == "")
            {
                tick2.Visibility = Visibility.Collapsed;
                errors.Remove(T("A szerző nem tartalmazhat speciális karaktereket!", "The author can't contain special characters!"));
                ShowError();
            }
        }
        private void cmbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg a könyv műfaját! ", "Please enter the genre of the book!"));
            ShowError();

            txtGenre.Text = "";
            rct_genre.Stroke = Brushes.Transparent;
            rct_genre.StrokeThickness = 0;
            if (cmbGenre.IsEnabled == true)
            {
                tick0.Visibility = Visibility.Visible;

            }
            else
            {
                tick0.Visibility = Visibility.Collapsed;
            }

        }
        private void txtGenre_TextChanged(object sender, TextChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg a könyv műfaját! ", "Please enter the genre of the book!"));
            ShowError();

            bool IsError = false;
            foreach (char C in chars)
            {
                if (txtGenre.Text.Contains(C))
                {
                    IsError = true;
                    break;
                }
            }
            if (IsError)
            {
                errors.Add(T("A műfaj nem tartalmazhat speciális karaktereket!", "The genre field can't contain special characters!"));
                tick0.Visibility = Visibility.Collapsed;
                ShowError();
                rct_genre.Stroke = Brushes.Red;
            }
            if (txtGenre.Text.Trim() != "" && txtGenre.IsEnabled == true && IsError == false)
            {
                tick3.Visibility = Visibility.Visible;
                rct_genre.Stroke = Brushes.Transparent;
                cmbGenre.SelectedItem = null;
            }
            if (txtGenre.Text.Trim() == "")
            {
                txtGenre.BorderBrush = Brushes.Transparent;
                tick0.Visibility = Visibility.Collapsed;
                errors.Remove(T("A műfaj nem tartalmazhat speciális karaktereket!", "The genre field can't contain special characters!"));
                ShowError();
            }
        }
        private void dpDate_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg a könyv kiadási dátumát! ", "Please enter the book's publication date!"));
            ShowError();

            rct_Date.Stroke = Brushes.Transparent;
        }
        private void txtPublisher_TextChanged(object sender, TextChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg a könyv kiadóját! ", "Please enter the publisher of the book!"));
            ShowError();

            bool IsError = false;
            rct_publisher.Stroke = Brushes.Transparent;
            foreach (char C in chars)
            {
                if (txtPublisher.Text.Contains(C))
                {
                    IsError = true;
                    break;
                }
            }
            if (IsError)
            {
                errors.Add(T("A kiadó neve nem tartalmazhat speciális karaktereket!", "The publisher's name field can't contain special characters!"));
                rct_publisher.Stroke = Brushes.Red;
                tick3.Visibility = Visibility.Collapsed;
                ShowError();
            }
            if (txtPublisher.Text.Trim() != "" && txtPublisher.IsEnabled == true && IsError == false)
            {
                tick3.Visibility = Visibility.Visible;
                rct_publisher.Stroke = Brushes.Transparent;
                rct_publisher.StrokeThickness = 0;
            }
            if (txtPublisher.Text.Trim() == "")
            {
                tick3.Visibility = Visibility.Collapsed;
                rct_publisher.Stroke = Brushes.Transparent;
                errors.Remove(T("A kiadó neve nem tartalmazhat speciális karaktereket!", "The publisher's name field can't contain special characters!"));
                ShowError();
            }
        }
        private void rad_ebook_Checked(object sender, RoutedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg hogy a könyv ebbok vagy papír! ", "Please select whether the book is an ebook or paperback!"));
            ShowError();
            rct_ebook.Stroke = Brushes.Transparent;
            rct_ebook.StrokeThickness = 0;

            txtcopy.IsEnabled = false;
            txtcopy.Text = "-";
        }
        private void rad_paper_Checked(object sender, RoutedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg hogy a könyv ebbok vagy papír! ", "Please select whether the book is an ebook or paperback!"));
            ShowError();

            rct_ebook.Stroke = Brushes.Transparent;
            rct_ebook.StrokeThickness = 0;

            /// ha a title szerkeszthető akkor a copy is 
            if(txtTitle.IsEnabled == true){
                txtcopy.IsEnabled = true;

            }
            if (lbx_books.SelectedItem is ListBoxItem item &&
           item.Tag is Book selectedBook)
            {
                txtcopy.Text = selectedBook.Copies.ToString();
            }
            if (txtcopy.Text == "-1")
            {
                txtcopy.Text = "-";
            }
        }
        private void txtNatioal_TextChanged(object sender, TextChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg hogy milyen nyelven íródott a könyv! ", "Please enter the language in which the book was written!"));
            ShowError();
            rct_national.Stroke = Brushes.Transparent;
            bool IsError = false;
            foreach(char C in chars)
            {
                if (txtnational.Text.Contains(C))
                {
                    IsError = true;
                    break;
                }
            }
            if (IsError)
            {
                rct_national.Stroke = Brushes.Red;
                errors.Add(T("A nemzetiség nem tartalmazhat speciális karaktereket!", "The nationality field can't contain special characters!"));
                ShowError();
                tick4.Visibility = Visibility.Collapsed;
            }
            if (txtnational.Text.Trim() != "" && txtnational.IsEnabled == true && IsError == false)
            {
                tick4.Visibility = Visibility.Visible;
            }
            if (txtnational.Text.Trim() == "")
            {
                tick4.Visibility = Visibility.Collapsed;
                errors.Remove(T("A nemzetiség nem tartalmazhat speciális karaktereket!", "The nationality field can't contain special characters!"));
                ShowError();
            }
        }
        private void txtCopy_TextChanged(object sender, TextChangedEventArgs e)
        {
            errors.Remove(T("Kérem adja meg a példányszámot! ", "Please enter the number of copies!"));
            ShowError();
            tick5.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(txtcopy.Text))
            {
                rct_copy.Stroke = Brushes.Transparent;
                errors.Remove(T("A példányszámnak minimum 1-nek kell lennie!", "The number of copies must be at least 1!"));
                errors.Remove(T("A példányszámnak számnak kell lennie!", "The number of copies must be a number!"));
                ShowError();
                return;
            }
            int n;
            if (!int.TryParse(txtcopy.Text, out n) && rad_ebook.IsChecked == false)
            {
                rct_copy.Stroke = Brushes.Red;
                errors.Add(T("A példányszámnak számnak kell lennie!", "The number of copies must be a number!"));
                ShowError();
            }
            else if (n < 1 && rad_ebook.IsChecked == false)
            {
                rct_copy.Stroke = Brushes.Red;
                errors.Add(T("A példányszámnak minimum 1-nek kell lennie!", "The number of copies must be at least 1!"));
                ShowError();
            }
            else
            {
                errors.Remove(T("A példányszámnak minimum 1-nek kell lennie!", "The number of copies must be at least 1!"));
                rct_copy.Stroke = Brushes.Transparent;
            }

            if (txtcopy.IsEnabled == true && errors.Count == 0)
            {
                errors.Remove(T("A példányszámnak minimum 1-nek kell lennie!", "The number of copies must be at least 1!"));
                tick5.Visibility = Visibility.Visible;
            }
            else
            {
                tick5.Visibility = Visibility.Collapsed;
            }
            ShowError();
        }
        private void lbx_books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            UpdateGenreComboBox();
            lbx_books.Visibility = Visibility.Visible;
            txtcopy.IsEnabled = false;
            errors.Clear();
            HideForm();
            HideError();
            HideSave();
            ShowPlus();
            //ShowCopy();


            if (lbx_books.SelectedItem == null)
            {
                HideError();
                HideSave();
                return;
            }


            else if (lbx_books.SelectedItem is ListBoxItem item &&
            item.Tag is Book selectedBook)
            {
                lastClicked = selectedBook;
                txtTitle.Text = selectedBook.Title;
                txtTitle.BorderBrush = Brushes.Transparent;
                txtAuthor.Text = selectedBook.Author;
                txtAuthor.BorderBrush = Brushes.Transparent;
                if (currentLanguage == "HU") 
                {
                    if (string.IsNullOrWhiteSpace(selectedBook.Genre))
                    {
                        GenreEnglish.IsOpen = true;
                        cmbGenre.IsEnabled = true;
                        cmbGenre.Text = selectedBook.GenreEn;
                    }
                    else
                    {
                        cmbGenre.Text = selectedBook.Genre;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(selectedBook.GenreEn))
                    {
                        GenreEnglish.IsOpen = true;
                        cmbGenre.IsEnabled = true;
                        cmbGenre.Text = selectedBook.Genre;
                    }
                    else
                    {
                        cmbGenre.Text = selectedBook.GenreEn;
                    }
                } 
                txtGenre.BorderBrush = Brushes.Transparent;
                txtPublisher.Text = selectedBook.Publisher;
                txtPublisher.BorderBrush = Brushes.Transparent;
                txtnational.Text = selectedBook.Nationality;
                txtnational.BorderBrush = Brushes.Transparent; ;
                txtcopy.Text = selectedBook.Copies.ToString();
                txtcopy.BorderBrush = Brushes.Transparent;

                rad_ebook.IsChecked = !selectedBook.Paper;
                rad_paper.IsChecked = selectedBook.Paper;

                if (rad_ebook.IsChecked == true)
                {
                    txtcopy.Text = "-";
                    txtcopy.IsEnabled = false;
                }

                dpDate.SelectedDate = selectedBook.Year.ToDateTime(TimeOnly.MinValue);
                dpDate.BorderBrush = Brushes.Transparent;
                idNumber = selectedBook.Id;
            }
            ShowChange();
            txtGenre.Text = "";

            item = (ListBoxItem)lbx_books.SelectedItem;


        }
        private void lbx_books_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            /// listboxra kattintva az e.originalsource megadja azt az elemet amire kattintottunk a listboxon belülre 
            /// a container from element pedig visszaadja azt az itemet amiben benne volt az a dolog amit az egerünkkel elértünk
            var item = ItemsControl.ContainerFromElement(
                lbx_books,
                e.OriginalSource as DependencyObject /// dependencyobject -> a vizuális elemek innen származnak le
            ) as ListBoxItem;


            if (item != null && item.IsSelected)
            {

                e.Handled = true;


                item.IsSelected = false;

                Delete();
                HideForm();
            }
        }



        // Button eventek
        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            HidePlus();
            if (lbx_books.SelectedItem != null)
            {
                HideForm();
                ShowCopy();
                errors.Add(T("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?", "This book already exists in the registry. Do you want to add this entry to its copy count?"));
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
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            HidePlus();
            ShowError();
            if (hasBeenDone && matchingbook != null && !modificationClicked && errors.Count == 0)
            {
                int i = books.IndexOf(matchingbook);
                if (i < 0 && matchingbook.Id != 0)
                {
                    i = books.FindIndex(b => b.Id == matchingbook.Id);
                }
                if (i >= 0)
                {
                    books[i].Copies += int.Parse(txtcopy.Text);
                    books[i].DateEdited = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    DateTime.Now.Second, 0
                    );
                    books = books.OrderBy(book => book.Id).ToList();
                    File.WriteAllText(jsonPath, JsonConvert.SerializeObject(books, Formatting.Indented));

                    // frissítjük a megjelenítést és UI-t
                    HideError();
                    PrintSortedBooks(books, false);
                    ShowPlus();
                    Delete();
                    exists = false;
                    hasBeenDone = false;
                    matchingbook = null;
                    MyPopupNew.IsOpen = true;
                }
            }
            else if (hasBeenDone && matchingbook != null && modificationClicked && errors.Count == 0)
            {

                int i = books.IndexOf(matchingbook);
                if (i < 0 && matchingbook.Id != 0)
                {
                    i = books.FindIndex(b => b.Id == matchingbook.Id);
                }
                if (i >= 0)
                {
                    if (rad_ebook.IsChecked == true)
                    {
                        books[i].Copies = -1;
                    }
                    else
                    {
                        books[i].Copies = int.Parse(txtcopy.Text);

                    }
                    books[i].DateEdited = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    DateTime.Now.Second, 0
                    );
                    books = books.OrderBy(book => book.Id).ToList();
                    File.WriteAllText(jsonPath, JsonConvert.SerializeObject(books, Formatting.Indented));

                    // frissítjük a megjelenítést és UI-t
                    HideError();
                    PrintSortedBooks(books, false);
                    ShowPlus();
                    exists = false;
                    hasBeenDone = false;
                    matchingbook = null;
                    MyPopupModify.IsOpen = true;
                }
            }
            else if (errors.Count == 0)
            {
                bool valid = true;
                int copy = 0;

                if (txtTitle.Text.Trim() == "")
                {
                    valid = false;
                    rct_title.Stroke = color;
                    rct_title.StrokeThickness = 1;
                    errors.Add(T("Kérem adja meg a könyv címét! ", "Please enter a book title! "));

                }
                if (txtAuthor.Text.Trim() == "")
                {
                    valid = false;
                    rct_author.Stroke = color;
                    rct_author.StrokeThickness = 1;
                    errors.Add(T("Kérem adja meg a könyv szerzőjét! ", "Please enter the author of the book!"));
                }
                if ((cmbGenre.SelectedItem == null && txtGenre.Text.Trim() == ""))
                {
                    valid = false;
                    rct_genre.Stroke = color;
                    rct_genre.StrokeThickness = 1;
                    errors.Add(T("Kérem adja meg a könyv műfaját! ", "Please enter the genre of the book!"));
                }
                if (txtPublisher.Text.Trim() == "")
                {
                    valid = false;
                    rct_publisher.Stroke = color;
                    rct_publisher.StrokeThickness = 1;
                    errors.Add(T("Kérem adja meg a könyv kiadóját! ", "Please enter the publisher of the book!"));
                }
                if (rad_ebook.IsChecked == false && rad_paper.IsChecked == false)
                {
                    valid = false;
                    rct_ebook.Stroke = color;
                    rct_ebook.StrokeThickness = 1;
                    errors.Add(T("Kérem adja meg hogy a könyv ebbok vagy papír! ", "Please select whether the book is an ebook or paperback!"));

                }
                if (dpDate.SelectedDate == null)
                {
                    valid = false;
                    rct_Date.StrokeThickness = 1;
                    rct_Date.Stroke = color;
                    errors.Add(T("Kérem adja meg a könyv kiadási dátumát! ", "Please enter the book's publication date!"));

                }
                if (txtnational.Text.Trim() == "")
                {
                    valid = false;
                    rct_national.Stroke = color;
                    rct_national.StrokeThickness = 1;
                    errors.Add(T("Kérem adja meg hogy milyen nyelven íródott a könyv! ", "Please enter the language in which the book was written!"));
                }

                if (txtcopy.Text.Trim() == "" && rad_ebook.IsChecked == false)
                {
                    valid = false;
                    rct_copy.Stroke = color;
                    rct_copy.StrokeThickness = 1;
                    errors.Add(T("Kérem adja meg a példányszámot! ", "Please enter the number of copies!"));

                }
                if (!int.TryParse(txtcopy.Text, out copy) && txtcopy.Text.Trim() != "")
                {
                    if (rad_ebook.IsChecked == false)
                    {
                        rct_copy.Stroke = color;
                        rct_copy.StrokeThickness = 1;
                        errors.Add(T("A példányszámnak számnak kell lennie!", "The number of copies must be a number!"));
                    }

                }
                if (copy <= 0 && txtcopy.Text.Trim() != "" && int.TryParse(txtcopy.Text, out copy) && rad_ebook.IsChecked == false)
                {
                    rct_copy.Stroke = color;
                    rct_copy.StrokeThickness = 1;
                    errors.Add(T("A példányszámnak minimum 1-nek kell lennie!", "The number of copies must be at least 1!"));
                }
                foreach (var book in books)
                {
                    if (txtTitle.Text.ToLower() == book.Title.ToLower() && txtAuthor.Text.ToLower() == book.Author.ToLower() &&
                        (((cmbGenre.SelectedItem?.ToString() == book.Genre) || (txtGenre.Text == book.Genre)) || ((cmbGenre.SelectedItem?.ToString() == book.GenreEn) || (txtGenre.Text == book.GenreEn))) && //
                        ((book.Paper && rad_paper.IsChecked == true) || (!book.Paper && rad_ebook.IsChecked == true)) &&
                        DateOnly.FromDateTime(dpDate.SelectedDate!.Value) == book.Year && txtnational.Text == book.Nationality && (((book.Paper && rad_paper.IsChecked == true) && (txtcopy.Text != "" && copy > 0)) || ((!book.Paper && rad_ebook.IsChecked == true) && (txtcopy.Text == "-"))))
                    {
                        if (rad_ebook.IsChecked == true)
                        {
                            AlreadyExists.IsOpen = true;
                            HideError();
                        }
                        else
                        {
                            matchingbook = book;
                            exists = true;
                            errors.Add(T("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?", "This book already exists in the registry. Do you want to add this entry to its copy count?"));
                            ShowError();
                            hasBeenDone = true;
                            ShowSave();
                        }
                            return;
                    }
                    else if (valid == true && (txtTitle.Text.ToLower() != book.Title.ToLower() &&
                        txtAuthor.Text.ToLower() != book.Author.ToLower() &&
                        (((cmbGenre.SelectedItem?.ToString() != book.Genre) || (txtGenre.Text.ToLower() != book.Genre)) || ((cmbGenre.SelectedItem?.ToString() != book.GenreEn) || (txtGenre.Text.ToLower() != book.GenreEn))) && //
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
                if (exists == false && valid == true && !modificationClicked && errors.Count == 0)
                {
                    MyPopupNew.IsOpen = true;
                    return;
                }
                
                if (modificationClicked && valid == true && errors.Count == 0)
                {
                    MyPopupModify.IsOpen = true;
                    modificationClicked = false;
                    return;
                }
                
                ShowPlus();
                HideSave();
                matchingbook = null;
            }

            hasBeenDone = true;

        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            lbx_books.SelectionChanged -= lbx_books_SelectionChanged;
            HideError();
            errors.Clear();
            HideForm();
            ShowPlus();
            HideEveryButton();
            HideSave();
            // kell if hogyha hozzáadásnál van 
            rct_title.Stroke = Brushes.Transparent;
            rct_title.StrokeThickness = 0;

            rct_author.Stroke = Brushes.Transparent;
            rct_author.StrokeThickness = 0;

            rct_genre.Stroke = Brushes.Transparent;
            rct_genre.StrokeThickness = 0;

            rct_publisher.Stroke = Brushes.Transparent;
            rct_publisher.StrokeThickness = 0;

            rct_Date.Stroke = Brushes.Transparent;
            rct_Date.StrokeThickness = 0;

            rct_national.Stroke = Brushes.Transparent;
            rct_national.StrokeThickness = 0;

            rct_copy.Stroke = Brushes.Transparent;
            rct_copy.StrokeThickness = 0;

            rct_ebook.Stroke = Brushes.Transparent;
            rct_ebook.StrokeThickness = 0;

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
            
            hasBeenDone = false;
            matchingbook = null;
            lbx_books.SelectionChanged += lbx_books_SelectionChanged;
        }
        private void br_Clear_MouseDown(object sender, RoutedEventArgs e)
        {
            lbx_books.SelectedItem = null;
            Delete();
            HideEveryButton();
        }
        private void br_Modify_MouseDown(object sender, RoutedEventArgs e)
        {
            HidePlus();

            cancelButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
            br_Modify.Visibility = Visibility.Collapsed;
            br_Delete.Visibility = Visibility.Collapsed;
            br_Cancel.Visibility = Visibility.Visible;
            br_Save.Visibility = Visibility.Visible;

            ShowForm();
            tobeModified = searchForSelectedItem();
            modificationClicked = true;
        }
        private void br_Delete_MouseDown(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
        }



        // ---------- PopUpok
        private void PopupOk_Click(object sender, RoutedEventArgs e)
        {
            Book toBeDeleted = searchForSelectedItem();

            List<konyv_wpf.Book> newBooks = new List<Book>();
            foreach (konyv_wpf.Book book in books)
            {

                if (toBeDeleted.Id != book.Id)
                {
                    newBooks.Add(book);

                }
            }
            books = newBooks;
            books = books.OrderBy(book =>book.Id).ToList();
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(books, Formatting.Indented));


            HideSave();
            HideCancel();
            lbx_books.SelectedItem = null;
            ShowPlus();
            Delete();
            HideForm();
            PrintSortedBooks(books, false);


            MyPopup.IsOpen = false;
            ShowToast("DeleteText");
        }
        private void PopupCancel_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }
        private void PopupOk_ClickExists(object sender, RoutedEventArgs e)
        {
            HideEveryButton();
            Delete();
            ShowPlus();
            AlreadyExists.IsOpen = false;
        }
        private void PopupCancel_ClickNewRUS(object sender, RoutedEventArgs e)
        {
            MyPopupNew.IsOpen = false;
        }
        private void PopupOk_ClickNewRUS(object sender, RoutedEventArgs e)
        {
            books = books.OrderBy(book => book.Id).ToList();
            File.WriteAllText(jsonPath,
            JsonConvert.SerializeObject(books, Formatting.Indented));
            IfEnglishNewBook();
            ShowToast("TextNewBook");
            MyPopupNew.IsOpen = false;
            Delete();
            HideForm();
        }
        private void PopupCancel_ClickModifyRUS(object sender, RoutedEventArgs e)
        {
            MyPopupModify.IsOpen = false;
        }
        private void PopupOk_ClickModifyRUS(object sender, RoutedEventArgs e)
        {
            if (cmbGenre.SelectedItem == null)
            {
                if (currentLanguage == "HU")
                {
                    Genres.Add(txtGenre.Text);
                    tobeModified!.Genre = txtGenre.Text;
                }
                else
                {
                    Genre_En.Add(txtGenre.Text);
                    tobeModified!.GenreEn = txtGenre.Text;
                }
            }
            else
            {
                if (currentLanguage == "HU")
                {
                    tobeModified!.Genre = cmbGenre.SelectedItem.ToString()!;
                }
                else
                {
                    tobeModified!.GenreEn = cmbGenre.SelectedItem.ToString()!;
                }
            }
            if (rad_paper.IsChecked == true)
            {
                tobeModified!.Paper = true;
                tobeModified!.Copies = int.Parse(txtcopy.Text);
            }
            else
            {
                tobeModified!.Copies = -1;
                tobeModified!.Paper = false;
            }
            tobeModified.Title = txtTitle.Text;
            tobeModified.Author = txtAuthor.Text;
            tobeModified.Publisher = txtPublisher.Text;
            tobeModified.Year = DateOnly.FromDateTime(dpDate.SelectedDate.Value);
            tobeModified.Nationality = txtnational.Text;
            tobeModified.DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0);


            books[books.IndexOf(tobeModified)] = tobeModified;
            PrintSortedBooks(books, false);
            ShowToast("ModifyText");
            MyPopupModify.IsOpen = false;
            Delete();
            HideForm();

            books = books.OrderBy(book => book.Id).ToList();
            File.WriteAllText(jsonPath,
            JsonConvert.SerializeObject(books, Formatting.Indented));
        }
        private void PopupOk_ClickGenreEn(object sender, RoutedEventArgs e)
        {
            if (lastClicked == null)
                return;

            UpdateGenreComboBox();
            if (currentLanguage == "HU")
            {
                cmbGenre.ItemsSource = Genre_En;   
                cmbGenre.SelectedItem = lastClicked.GenreEn;
            }
            else
            {
                cmbGenre.ItemsSource = Genres; 
                cmbGenre.SelectedItem = lastClicked.Genre;
            }
            GenreEnglish.IsOpen = false;
        }
        private void PopupNo_ClickGenreEn(object sender, RoutedEventArgs e)
        {
            cmbGenre.Text = "";
            GenreEnglish.IsOpen = false;
        }
        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
            AlreadyExists.IsOpen = false;
            GenreEnglish.IsOpen = false;
            MyPopupModify.IsOpen = false;
            MyPopupNew.IsOpen = false;
        }
    }
}

