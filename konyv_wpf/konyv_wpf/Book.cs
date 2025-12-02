using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace konyv_wpf
{
    public class Book
    {
        private int id { get; set; }
        private string author { get; set; }
        private string title { get; set; }
        private string genre { get; set; }
        private string genre_en { get; set; }
        private string publisher { get; set; }
        private DateOnly year { get; set; }
        private int copies { get; set; }
        private bool paper { get; set; }
        private string nationality { get; set; }
        private DateTime dateEdited { get; set; }

        public int Id { get { return id; } set { id = value; } }
        public string Author { get { return author; } set { author = value; } }
        public string Title { get { return title; } set { title = value; } }
        public string Genre { get { return genre; } set { genre = value; } }
        public string GenreEn { get { return genre_en; } set { genre_en = value; } }
        public string Publisher { get { return publisher; } set { publisher = value; } }
        public DateOnly Year { get { return year; } set { year = value; } }
        public int Copies { get { return copies; } set { copies = value; } }
        public bool Paper { get { return paper; } set { paper = value; } }
        public string Nationality { get { return nationality; } set { nationality = value; } }
        public DateTime DateEdited { get { return dateEdited; } set { dateEdited = value; } }

        public override string ToString()
        {
            return $"{id}, {author}, {title}, {genre}, {publisher}, {year}, {copies}, {paper}, {nationality}";
        }
    }
}

