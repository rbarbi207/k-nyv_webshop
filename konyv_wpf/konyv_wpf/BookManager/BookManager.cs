using konyv_wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace konyv_wpf
{
    partial class MainWindow : Window
    {
        private void IfEnglishNewBook()
        {
            int j = books.Count;
            if (currentLanguage == "HU")
            {
                if (cmbGenre.SelectedItem == null)
                {
                    if (rad_paper.IsChecked == true)
                    {
                        Genres.Add(txtGenre.Text);
                        GenreIsEnglish = txtGenre.Text;
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            Genre = GenreIsEnglish,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = true,
                            Nationality = txtnational.Text,
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                    else
                    {
                        Genres.Add(txtGenre.Text);
                        GenreIsEnglish = txtGenre.Text;
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            Genre = GenreIsEnglish,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = false,
                            Nationality = txtnational.Text,
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                }
                else
                {
                    if (rad_paper.IsChecked == true)
                    {
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            Genre = cmbGenre.SelectedItem.ToString(),
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = true,
                            Nationality = txtnational.Text,
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                    else
                    {
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
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                }

            }
            else
            {
                if (cmbGenre.SelectedItem == null)
                {
                    if (rad_paper.IsChecked == true)
                    {
                        Genre_En.Add(txtGenre.Text);
                        GenreIsEnglish = txtGenre.Text;
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            GenreEn = GenreIsEnglish,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = true,
                            Nationality = txtnational.Text,
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                    else
                    {
                        Genre_En.Add(txtGenre.Text);
                        GenreIsEnglish = txtGenre.Text;
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            GenreEn = GenreIsEnglish,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = false,
                            Nationality = txtnational.Text,
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                }
                else
                {
                    if (rad_paper.IsChecked == true)
                    {
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            GenreEn = cmbGenre.SelectedItem.ToString(),
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = true,
                            Nationality = txtnational.Text,
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                    else
                    {
                        books.Add(new Book()
                        {
                            Id = j + 1,
                            Title = txtTitle.Text,
                            Author = txtAuthor.Text,
                            GenreEn = cmbGenre.SelectedItem.ToString()!,
                            Publisher = txtPublisher.Text,
                            Year = DateOnly.FromDateTime(dpDate.SelectedDate!.Value),
                            Copies = 1,
                            Paper = false,
                            Nationality = txtnational.Text,
                            DateEdited = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, 0)
                        });
                    }
                }
            }
        }

        private void UpdateGenreComboBox()
        {
            if (currentLanguage == "HU")
                cmbGenre.ItemsSource = Genres;
            else
                cmbGenre.ItemsSource = Genre_En;
        }

        private void loadBooks(string filename)
        {
            string json = File.ReadAllText(jsonPath);
            books = JsonConvert.DeserializeObject<List<Book>>(json)!;

            foreach (Book book in books)
            {

                Genres.Add(book.Genre);
                Genre_En.Add(book.GenreEn); 
            }
   

            PrintSortedBooks(books, false);
            Genres = Genres.Distinct().ToList();
            cmbGenre.ItemsSource = Genres;
            Genre_En = Genre_En.Distinct().ToList();         }

        // --- Listbox
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

                panel.Children.Add(lbl_title);
                Label lbl_date = new Label();
                // dátum
               
                if (book.DateEdited != null)
                {
                    lbl_date.Name = "lbl_date";
                    lbl_date.Content = T("módosítva: ", "edited: ") + book.DateEdited;
                }
                else 
                {
                    
                    lbl_date.Name = "lbl_date";
                    book.DateEdited = DateTime.Now;
                    lbl_date.Content = T("frissítve: ", "Last loaded: ") + book.DateEdited.ToString();
                 
                }
                lbl_date.FontSize = 12;
                lbl_date.HorizontalAlignment = HorizontalAlignment.Right;
                lbl_date.Margin = new Thickness(0, -5, 0, 0);
                lbl_date.FontStyle = FontStyles.Italic;

                panel.Children.Add(lbl_date);
                item.Content = panel;
                item.Tag = book;
                item.HorizontalAlignment = HorizontalAlignment.Stretch;
                item.Width = rct_list.Width - 110;
                item.Cursor = Cursors.Hand;
                item.Padding = new Thickness(5, 0, 5, 0);
                lbx_books.Items.Add(item);
            }

            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(books, Formatting.Indented));
        }
        private List<Book> makeSortList(List<Book> books, bool clicked)
        {
            List<Book> sortedBooks = new List<Book>();
            lbx_books.Items.Clear();
            //sortedBooks = sort(books);

            if (clicked)
            {
                bool foundNotNull = false;
                foreach(Book book in books)
                {
                    if (book.DateEdited != null)
                    {
                        foundNotNull = true;
                    }
                }
                
                if (!foundNotNull) // ha van benne 0
                {
                    if (ascending) // ha növekvő akkor 
                    {

                        sortedBooks = books.OrderByDescending(book => book.DateEdited).ThenBy(book => book.Title).ToList();
                        ascending = false;
                    }
                    else
                    {
                    sortedBooks = books.OrderBy(book => book.Title).ToList();
                        ascending = true;
                    }
                }
                else
                {
                    if (ascending)
                    {
                        sortedBooks = books.OrderBy(book => book.DateEdited).ThenBy(book => book.Title).ToList();
                        ascending = false;
                    }
                    else
                    {
                        ascending = true;
                        sortedBooks = books.OrderByDescending(book => book.DateEdited).ThenBy(book=> book.Title).ToList(); 
                    }

                }
            }
            else
            {
                if (!ascending)
                {
                    sortedBooks = books.OrderBy(book => book.DateEdited).ThenBy(book => book.Title).ToList();
                }
                else
                {
                    sortedBooks = books.OrderByDescending(book => book.DateEdited).ThenBy(book => book.Title).ToList();
                }
            }
            return sortedBooks;
            //if (clicked)
            //{
            //    if (!sorted)
            //    {
            //        sorted = true;
            //    }
            //    else
            //    {
            //        sortedBooks.Reverse();
            //        sorted = false;
            //    }
            //}
            //else
            //{
            //    if (!sorted)
            //    { 
            //        sortedBooks.Reverse();
            //    }
            //}
            //return sortedBooks;
        }
        private List<Book> sort(List<Book> books)
        {
           
            return books.OrderBy(book => book.DateEdited).ThenBy(book => book.Title).ToList();
        }


      
        private Book searchForSelectedItem()
        {
            Book _book = books[0];
            foreach (Book book in books)
            {
                if (txtTitle.Text.ToLower() == book.Title.ToLower() && txtAuthor.Text.ToLower() == book.Author.ToLower() &&
                        ((cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre) || (cmbGenre.SelectedItem?.ToString() == book.GenreEn || txtGenre.Text == book.GenreEn)) && //
                        (rad_ebook.IsChecked != book.Paper || rad_paper.IsChecked == book.Paper) &&
                        DateOnly.FromDateTime(dpDate.SelectedDate!.Value) == book.Year && txtnational.Text == book.Nationality)
                {
                    _book = book;
                }
            }

            return _book;
        }


        // --- Searchbar
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
        
        
        string CapitalizeWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(text.ToLower());
        }
    }
}
