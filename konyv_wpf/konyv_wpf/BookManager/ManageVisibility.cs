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
            txtAuthor.Foreground = Brushes.Black;
            txtGenre.IsEnabled = false;
            txtGenre.Foreground = Brushes.Black;
            txtPublisher.IsEnabled = false;
            txtPublisher.Foreground = Brushes.Black;
            txtTitle.IsEnabled = false;
            txtTitle.Foreground = Brushes.Black;
            rad_ebook.IsEnabled = false;
            rad_ebook.Foreground = Brushes.Black;
            rad_paper.Foreground = Brushes.Black;
            rad_paper.IsEnabled = false;
            txtnational.IsEnabled = false;
            txtnational.Foreground = Brushes.Black;
            dpDate.IsEnabled = false;
            dpDate.Foreground = Brushes.Black;
            cmbGenre.IsEnabled = false;
            txtcopy.IsEnabled = false;
            txtcopy.Foreground = Brushes.Black;
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
            if (rad_ebook.IsChecked == true)
            {
                txtcopy.Text = "-";
            txtcopy.IsEnabled = false;

            }
            else
            {
                txtcopy.IsEnabled = true;
            }
        }
        private void HideRedBorder()
        {
            rct_author.Stroke = Brushes.Transparent;
            rct_genre.Stroke = Brushes.Transparent;
            tick0.Text = null;
            rct_publisher.Stroke = Brushes.Transparent;
            rct_title.Stroke = Brushes.Transparent;
            rct_ebook.Stroke = Brushes.Transparent;
            rct_national.Stroke = Brushes.Transparent;
            rct_Date.Stroke = Brushes.Transparent;
            rct_copy.Stroke = Brushes.Transparent;
        }



        private void HideEveryButton()
        {
            br_Cancel.Visibility = Visibility.Collapsed;
            br_Save.Visibility = Visibility.Collapsed;
            br_Clear.Visibility = Visibility.Collapsed;
            br_Modify.Visibility = Visibility.Collapsed;
            br_Delete.Visibility = Visibility.Collapsed;    

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
            if (rad_ebook.IsChecked == false)
            txtcopy.IsEnabled = true;
        }
    }
}
