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

namespace konyv_wpf
{
    partial class MainWindow : Window
    {
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

                // dátum
                Label lbl_date = new Label();
                lbl_date.Name = "lbl_date";
                lbl_date.Content = T("módosítva: ", "edited: ") + book.DateEdited;
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
        private Book searchForSelectedItem()
        {
            Book _book = books[0];
            foreach (Book book in books)
            {
                if (txtTitle.Text.ToLower() == book.Title.ToLower() && txtAuthor.Text.ToLower() == book.Author.ToLower() &&
                        (cmbGenre.SelectedItem?.ToString() == book.Genre || txtGenre.Text == book.Genre) &&
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
