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
        private string _title;
        private string _author { get; set; }
        private DateTime _year { get; set; }
        private bool _paper { get; set; }
        private string _genre { get; set; }
        private int _copies { get; set; }

        private DateTime _dateEdited { get; set; }
        public string Title { get { return _title; } set { _title = value; } }
        public string Author { get { return _author; } set { _author = value; }}

        public DateTime Year { get { return _year; } set { _year = value; }}

        public bool Paper { get { return _paper; } set { _paper = value; }}

        public string Genre{ get { return _genre; } set { _genre = value; }}

        public int Copies{ get { return _copies; } set { _copies = value; }}

        public DateTime DateEdited{ get { return _dateEdited; } set { _dateEdited = value; }}

    }

}

