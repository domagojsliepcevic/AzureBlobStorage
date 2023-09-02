using Azure.Storage.Blobs.Models;
using BlobStorage.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobStorage.ViewModel
{
    internal class ItemsViewModel
    {
        public const string ForwardSlash = "/";

        public ObservableCollection<string> Directories { get;}

        public ObservableCollection<BlobItem> Items { get;}

        public string Directory 
        {
            get => directory;
            set 
            { 
                directory = value;
                Refresh();
            }
        }
        public ItemsViewModel()
        {
            Items = new ObservableCollection<BlobItem>();
            Directories = new ObservableCollection<string>();
            Refresh();
        }

        private void Refresh()
        {
            Items.Clear();
            Directories.Clear();

            //fill directories
            Repository.Container.GetBlobs().ToList().ForEach(item =>
            {
                if (item.Name.Contains(ForwardSlash)) 
                {
                    string dir = item.Name.Substring(0,item.Name.LastIndexOf(ForwardSlash));

                    if (!Directories.Contains(dir))
                    {
                        Directories.Add(dir);
                    }

                    //root files
                    if (string.IsNullOrEmpty(Directory) && !item.Name.Contains(ForwardSlash))
                    {
                        Items.Add(item);
                    }
                    // directories
                    else if(!string.IsNullOrEmpty(Directory) && item.Name.StartsWith($"{Directory}{ForwardSlash}")) 
                    {
                        Items.Add(item);
                    }
                }
            });
        }

        //public async Task UploadAsync(string path, string dir)
        //{
        //    string filename = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);

        //    if (!string.IsNullOrEmpty(dir))
        //    {
        //        filename = $"{dir}{ForwardSlash}{filename}";
        //    }
        //   using (var fs = File.OpenRead(path)) 
        //    {
        //        await Repository.Container.GetBlobClient(filename).UploadAsync(fs, true);
        //    }
        //   Refresh();
        //}

        public async Task UploadAsync(string path, string dir)
        {
            string filename = Path.GetFileName(path); // Get just the file name

            // Extract the file extension from the filename
            string fileExtension = Path.GetExtension(filename);

            // Create or get the directory based on the file extension
            string directoryName = fileExtension.TrimStart('.'); // Remove the leading dot
            //if (!string.IsNullOrEmpty(dir))
            //{
            //    directoryName = $"{directoryName}";
            //}

            using (var fs = File.OpenRead(path))
            {
                // Combine the directory name and the original filename to form the new filename
                string newFilename = $"{directoryName}{ForwardSlash}{filename}";

                await Repository.Container.GetBlobClient(newFilename).UploadAsync(fs, true);
            }

            Refresh();
        }


        public async Task DownloadAsync(BlobItem item,string path)
        {
            
           using (var fs = File.OpenWrite(path)) 
            {
                await Repository.Container.GetBlobClient(item.Name).DownloadToAsync(fs);
            }

           Refresh();
        }
        public async Task DeleteAsync(BlobItem item)
        {
            await Repository.Container.GetBlobClient(item.Name).DeleteAsync();
            Refresh();
        }

        private string directory; 
    }
}
