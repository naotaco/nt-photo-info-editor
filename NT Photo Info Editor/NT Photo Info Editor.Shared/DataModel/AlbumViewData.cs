using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace NtPhotoInfoEditor.DataModel
{
    public class ContentViewData
    {
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
                        else { return "No photo."; }
                    case ContentType.Other:
                        return Created.ToString();
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
                    case ContentType.Other:
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
