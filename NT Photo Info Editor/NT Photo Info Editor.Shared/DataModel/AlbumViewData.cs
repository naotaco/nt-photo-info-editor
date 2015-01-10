using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace NtPhotoInfoEditor.DataModel
{
    public class AlbumViewData
    {
        public BitmapImage Image { get; set; }
        public string Name { get; set; }

        public int NumberOfPhotos { get; set; }
        public string NumberOfPhotosString
        {
            get { return NumberOfPhotos.ToString() + " photos."; }
        }

        public int NumberOfFolders { get; set; }
        public string NumberOfFoldersString
        {
            get { return NumberOfFolders.ToString() + " folders."; }
        }
    }
}
