using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace konyv_wpf
{
    public partial class MainWindow : Window
    {

        // Mentés mégse
        // Könyv módosítás biztos mentés


        // --- FORM
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
            //rad_ebook.FontWeight = FontWeights.Bold;
            rad_paper.Foreground = Brushes.Black;
            rad_paper.IsEnabled = false;
            //rad_paper.FontWeight = FontWeights.Bold;
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
            rad_paper.FontWeight = FontWeights.Normal;
            rad_ebook.BorderBrush = Brushes.Transparent;
            rad_paper.IsChecked = false;
            rad_ebook.FontWeight = FontWeights.Normal;
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


        // --- CancelButton
        private void HideCancel()
        {
            cancelButton.Visibility = Visibility.Collapsed;
        }
        private void ShowCancel()
        {
            cancelButton.Visibility = Visibility.Visible;
        }


        // -- plusButton
        private void ShowChange()
        {
            br_newBook.Visibility = Visibility.Visible;
            PlusButton.Visibility = Visibility.Visible;
        }


        // -- cancelButton + saveButton
        private void ShowSave()
        {
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


        // --- Hide buttons
        private void ShowPlus()
        {   
            br_Modify.Visibility = Visibility.Collapsed;
            br_Delete.Visibility = Visibility.Collapsed;
            br_Clear.Visibility = Visibility.Collapsed;
            if (lbx_books.SelectedItem != null)
            {
                br_Modify.Visibility = Visibility.Visible;
                br_Delete.Visibility = Visibility.Visible;
                br_Clear.Visibility = Visibility.Visible;
            }
            br_newBook.Visibility = Visibility.Visible;
            PlusButton.Visibility = Visibility.Visible;
        }
        private void HidePlus()
        {
            br_Modify.Visibility = Visibility.Collapsed;
            br_Delete.Visibility = Visibility.Collapsed;
            br_Clear.Visibility = Visibility.Collapsed;
            br_newBook.Visibility = Visibility.Collapsed;
            PlusButton.Visibility = Visibility.Collapsed;
        }


        // --- ErrorMsg
        private void ShowError()
        {
            errorMsg.Text = "";
            errorMsg.Foreground = Brushes.Red;
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



        private void ShowCopy()
        {
            txtcopy.IsEnabled = true;
        }
    }
}
