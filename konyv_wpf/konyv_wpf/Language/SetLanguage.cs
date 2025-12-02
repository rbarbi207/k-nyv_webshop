using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace konyv_wpf
{
    public partial class MainWindow : Window
    {
        private void SetLanguage(string lang)
        {
            ResourceDictionary dict = new ResourceDictionary();

            if (lang == "hu")
            {
                dict.Source = new Uri("/Language/lang.HU.xaml", UriKind.Relative);
            }

            else
            {
                dict.Source = new Uri("/Language/lang.EN.xaml", UriKind.Relative);
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);

            UpdateGenreComboBox();

            Delete();
            HideForm();
            HideError();
            HideRedBorder();
            ShowPlus();
            HideEveryButton();
        }
        public string T(string hu, string en)
        {
            return currentLanguage == "HU" ? hu : en;
        }

    }
}
