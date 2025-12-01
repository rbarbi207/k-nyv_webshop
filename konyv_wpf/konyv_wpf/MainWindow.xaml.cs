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
        private List<string> errors = new List<string>();

        public static string currentLanguage = "HU";
        public bool sorted = true;
        public bool exists = false; 
        private bool hasBeenDone = false;
        public Book? matchingbook = null;
        ListBoxItem? item = null;
        public int idNumber = 0;
        public Book? lastClicked = null;

        public MainWindow()
        {
            InitializeComponent();
            loadBooks("books.json");
            //languageSelect();

            HideError();
            ShowPlus();
            HideSave();
            //*
            HideForm();
            br_Clear.Visibility = Visibility.Collapsed;
            br_Modify.Visibility = Visibility.Hidden;
            br_Delete.Visibility = Visibility.Hidden;
        
        }


        // ---- Még kell??

            

                /// false -> alap: legkorábbi <summary>
                /// kiválasztott elem eltárolása ? 
                // Új könyv hozzáadás biztos mentés
                // Mentés mégse
                // Könyv Módosítás biztos mentés

                // + valahogy legyen cancel hogyha selecteltünk itemet a listboxba
                // + csak elindításkor vagyi fúmenőbe lehessen nyelvet változtatni
                // + MessasageBox gombfeliratainak nyelv változtatása

/// felugró ablak 

        // ---------- Language 
        //private void languageSelect()
        //{
        //    MessageBoxResult result = MessageBox.Show("Átvált angol fordításra?", "Will you turn on English translation?", MessageBoxButton.YesNo);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        currentLanguage = "HU";
        //    }
        //    else
        //    {
        //        currentLanguage = "EN";
        //    }
        //        languageSwitch();
        //}

/// az alatta lévő függvényből másolva (Mousedown) 
        //private void languageSwitch()
        //{
        //    ResourceDictionary dict = new ResourceDictionary();
        //    if (currentLanguage == "HU")
        //    {
        //        currentLanguage = "EN";
        //        lbl_lang.Content = "English";
        //        SetLanguage("en");
        //    }
        //    else
        //    {
        //        currentLanguage = "HU";
        //        lbl_lang.Content = "Magyar";
        //        SetLanguage("hu");
        //    }
        //    PrintSortedBooks(books, false);
        //}

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
        private void tbx_searchbar_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            tbx_searchbar.Text = " ";

            PrintSortedBooks(books, false);
        }
        //*


        // ---------- Elemek change funkciója - form rész
        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            rct_title.Stroke = Brushes.Transparent;
            rct_title.StrokeThickness = 0;

            if (txtTitle.Text.Trim() != "" && txtTitle.IsEnabled == true)
            {
                tick1.Visibility = Visibility.Visible;
            }
            else
            {
                tick1.Visibility = Visibility.Collapsed;
            }
        }
        //*
        private void txtAuthor_TextChanged(object sender, TextChangedEventArgs e)
        {
            rct_author.Stroke = Brushes.Transparent;
            rct_author.StrokeThickness = 0;

            if (txtAuthor.Text.Trim() != "" && txtAuthor.IsEnabled == true)
            {
                tick2.Visibility = Visibility.Visible;
            }
            else
            {
                tick2.Visibility = Visibility.Collapsed;
            }
        }
        //*
        private void cmbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            if (txtGenre.Text.Trim() != "")
            {
                txtGenre.BorderBrush = Brushes.Transparent;
                cmbGenre.SelectedItem = null;
                tick0.Visibility = Visibility.Visible;
            }
            else
            {
                tick0.Visibility = Visibility.Collapsed;
            }
        }
        private void dpDate_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            rct_Date.Stroke = Brushes.Transparent;
            rct_Date.StrokeThickness = 0;
       
        }
        private void txtPublisher_TextChanged(object sender, TextChangedEventArgs e)
        {
            rct_publisher.Stroke = Brushes.Transparent;
            rct_publisher.StrokeThickness = 0;
            if (txtPublisher.Text.Trim() != "" && txtPublisher.IsEnabled == true)
            {
                tick3.Visibility = Visibility.Visible;
            }
            else
            {
                tick3.Visibility = Visibility.Collapsed;
            }
        }
        private void rad_ebook_Checked(object sender, RoutedEventArgs e)
        {
            rct_ebook.Stroke = Brushes.Transparent;
            rct_ebook.StrokeThickness = 0;


            rad_ebook.FontWeight = FontWeights.Bold;
            rad_paper.FontWeight = FontWeights.Normal;

            txtcopy.IsEnabled = false;
            txtcopy.Text = "-";
        }
        private void rad_paper_Checked(object sender, RoutedEventArgs e)
        {
            rct_ebook.Stroke = Brushes.Transparent;
            rct_ebook.StrokeThickness = 0;
            rad_ebook.FontWeight = FontWeights.Normal;
            rad_paper.FontWeight = FontWeights.Bold;

            txtcopy.IsEnabled = true;
            if (lbx_books.SelectedItem is ListBoxItem item &&
           item.Tag is Book selectedBook)
            {
                txtcopy.Text = selectedBook.Copies.ToString();
            }
        }
        private void txtNatioal_TextChanged(object sender, TextChangedEventArgs e)
        {
            rct_national.Stroke = Brushes.Transparent;
            rct_national.StrokeThickness = 0;
            if (txtnational.Text.Trim() != "" && txtnational.IsEnabled == true)
            {
                tick4.Visibility = Visibility.Visible;
            }
            else
            {
                tick4.Visibility = Visibility.Collapsed;
            }
        }
        private void txtCopy_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            tick5.Visibility = Visibility.Collapsed;
                if (string.IsNullOrWhiteSpace(txtcopy.Text))
            {
                HideError();
                rct_copy.Stroke = Brushes.Transparent;
                rct_copy.StrokeThickness = 0;
                return;
            }
            int n;
            if (!int.TryParse(txtcopy.Text, out n) && rad_ebook.IsChecked == false)
            {

                errors.Clear();
                errors.Add(T("A példányszámnak számnak kell lennie!", "The number of copies must be a number!"));
                ShowError();
            }
            else if (n < 0 && rad_ebook.IsChecked == false)
            {

                errors.Clear();
                errors.Add(T("A példányszámnak minimum 1-nek kell lennie!", "The number of copies must be at least 1!"));
                ShowError();
            }
            else
            {
                errors.Clear();
                HideError();
             
                rct_copy.Stroke = Brushes.Transparent;
                rct_copy.StrokeThickness = 0;
            }

            if (txtcopy.IsEnabled == true && errors.Count == 0)
            {
                tick5.Visibility = Visibility.Visible;
            }
            else
            {
                tick5.Visibility = Visibility.Collapsed;
            }
        }



        private void lbx_books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

                if (rad_ebook.IsChecked == true)
                {
                    txtcopy.Text = "-";
                    txtcopy.IsEnabled = false;
                }

                dpDate.SelectedDate = new DateTime(
                    selectedBook.Year.Year,
                    selectedBook.Year.Month,
                    selectedBook.Year.Day
                );
                dpDate.BorderBrush = Brushes.Transparent;
                idNumber = selectedBook.Id;
            }
            ShowChange();
            txtGenre.Text = "";

            item = (ListBoxItem)lbx_books.SelectedItem;


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
            if (hasBeenDone && matchingbook != null)
            {
                int i = books.IndexOf(matchingbook);
                if (i < 0 && matchingbook.Id != 0)
                {
                    i = books.FindIndex(b => b.Id == matchingbook.Id);
                }
                if (i >= 0)
                {
                    books[i].Copies += int.Parse(txtcopy.Text);
                    books[i].DateEdited = DateTime.Now;
                    File.WriteAllText(jsonPath, JsonConvert.SerializeObject(books, Formatting.Indented));

                    // frissítjük a megjelenítést és UI-t
                    HideError();
                    PrintSortedBooks(books, false);
                    ShowPlus();
                    Delete();
                    exists = false;
                    hasBeenDone = false;
                    matchingbook = null;
                }
            }
            else
            {
                bool valid = true;
                int copy = 0;
                errors.Clear();




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
                    // ez így jó? 
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
                        (cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre) &&
                        (rad_ebook.IsChecked != book.Paper || rad_paper.IsChecked == book.Paper) &&
                        DateOnly.FromDateTime(dpDate.SelectedDate!.Value) == book.Year && txtnational.Text == book.Nationality && (txtcopy.Text != "" && copy > 0))
                    {
                        matchingbook = book;
                        exists = true;
                        errors.Add(T("Már létezik ilyen könyv a nyílvántartásban, hozzáadja ezt a feljegyzést a példányszámhoz?", "This book already exists in the registry. Do you want to add this entry to its copy count?"));
                        ShowError();
                        hasBeenDone = true;
                        ShowSave();
                        return;
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

                File.WriteAllText(jsonPath,
                JsonConvert.SerializeObject(books, Formatting.Indented));

                HideError();
                HideForm();
                PrintSortedBooks(books, false);
                ShowPlus();
                HideSave();
                hasBeenDone = false;
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

        }
        private void br_Delete_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
            (string)Application.Current.Resources["msg_delete_question"],
            (string)Application.Current.Resources["msg_confirmation"],
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {

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
                    File.WriteAllText(jsonPath, JsonConvert.SerializeObject(books, Formatting.Indented));


                    HideSave();
                    HideCancel();
                    ShowPlus();
                    Delete();
                    HideForm();
                    PrintSortedBooks(books, false);
                }

            }


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




    }
}

