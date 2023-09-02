using Azure.Storage.Blobs.Models;
using BlobStorage.ViewModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlobStorage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly ItemsViewModel itemsViewModel;

        public MainWindow()
        {
            InitializeComponent();
            itemsViewModel = new ItemsViewModel();
            Init();
        }

        private void Init()
        {
            CbDirectories.ItemsSource = itemsViewModel.Directories;
            LbItems.ItemsSource = itemsViewModel.Items;
        }

        private void LbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = LbItems.SelectedItem as BlobItem;
        }

        private void CbDirectories_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                itemsViewModel.Directory = CbDirectories.Text.Trim();
                CbDirectories.SelectedItem = itemsViewModel.Directory;
            }
        }

        private void CbDirectories_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (itemsViewModel.Directories.Contains(CbDirectories.Text))
            {
                itemsViewModel.Directory = CbDirectories.Text;
                CbDirectories.SelectedItem = itemsViewModel.Directory;
            }
        }

        //private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new OpenFileDialog();
        //    if (dialog.ShowDialog() == true)
        //    {
        //        await itemsViewModel.UploadAsync(dialog.FileName, CbDirectories.Text);

        //    }
        //    CbDirectories.Text = itemsViewModel.Directory;
        //}

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            // Set the file filter to allow only image files (e.g., .jpg, .png, .gif)
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tiff;*.svg|All Files|*.*";
            if (dialog.ShowDialog() == true)
            {
                // Get the selected file's extension to use as a directory
                string selectedFileExtension = System.IO.Path.GetExtension(dialog.FileName);

                // Call the UploadAsync method with the selected file and the extracted extension
                await itemsViewModel.UploadAsync(dialog.FileName, selectedFileExtension);
            }

            CbDirectories.Text = itemsViewModel.Directory;
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (!(LbItems.SelectedItem is BlobItem item))
            {
                return;
            }
            var dialog = new SaveFileDialog() 
            {
                FileName = item.Name.Substring(item.Name.LastIndexOf(ItemsViewModel.ForwardSlash) +1 )
            };
            if (dialog.ShowDialog() == true)
            {
                await itemsViewModel.DownloadAsync(item,dialog.FileName);

            }
            CbDirectories.Text = itemsViewModel.Directory;
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(LbItems.SelectedItem is BlobItem item))
            {
                return;
            }
            
            await itemsViewModel.DeleteAsync(item);
            CbDirectories.Text = itemsViewModel.Directory;
        }
    }
}
