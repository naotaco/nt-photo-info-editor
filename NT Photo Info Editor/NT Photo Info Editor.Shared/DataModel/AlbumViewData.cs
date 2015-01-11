using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace NtPhotoInfoEditor.DataModel
{
    public class ContentViewData
    {
        public StorageFolder Folder { get; set; }
        public StorageFile File { get; set; }

        public BitmapImage Image { get; set; }
        public string Name { get; set; }
        public ContentType Type { get; set; }

        public int NumberOfPhotos { get; set; }
        public DateTime Created { get; set; }
        public string SubText1
        {
            get
            {
                switch (Type)
                {
                    case ContentType.Folder:
                        if (NumberOfPhotos > 0) { return NumberOfPhotos.ToString() + " photos."; }
                        else { return "No JPEG file."; }
                    case ContentType.Jpeg:
                        return Created.ToLocalTime().ToString();
                }
                return "";
            }
        }

        public int NumberOfFolders { get; set; }
        public string SubText2
        {
            get
            {
                switch (Type)
                {
                    case ContentType.Folder:
                        if (NumberOfFolders > 0) { return NumberOfFolders.ToString() + " folders."; }
                        else { return ""; }
                    case ContentType.Jpeg:
                        return "";
                }
                return "";
            }
        }
    }

    public enum ContentType
    {
        Folder,
        Jpeg,
        Other,
    }
}
