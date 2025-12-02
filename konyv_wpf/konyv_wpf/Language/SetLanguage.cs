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
                Delete();
                HideForm();
                HideError();
                HideRedBorder();
                ShowPlus();
                HideEveryButton();
            }

            else
            {
                dict.Source = new Uri("/Language/lang.EN.xaml", UriKind.Relative);
                Delete();
                HideForm();
                HideError();
                HideRedBorder();
                ShowPlus();
                HideEveryButton();
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);

        }
        public string T(string hu, string en)
        {
            return currentLanguage == "HU" ? hu : en;
        }

    }
}
