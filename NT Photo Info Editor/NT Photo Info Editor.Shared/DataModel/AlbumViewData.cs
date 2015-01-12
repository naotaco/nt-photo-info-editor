using NtPhotoInfoEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace NtPhotoInfoEditor.DataModel
{
    public class ContentViewData : INotifyPropertyChanged
    {
        public ContentType Type { get; set; }

        StorageFolder _Folder;
        public StorageFolder Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
                NotifyChangedOnUI("Folder");
            }
        }

        StorageFile _File;
        public StorageFile File
        {
            get { return _File; }
            set
            {
                _File = value;
                NotifyChangedOnUI("File");
            }
        }

        BitmapImage _Image;
        public BitmapImage Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
                NotifyChangedOnUI("Image");
            }
        }

        string _Name;
        public string Name { get { return _Name; }
            set {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyChangedOnUI("Name");
                }
            }
        }

        int _NumberOfPhotos;
        public int NumberOfPhotos { get { return _NumberOfPhotos; }
            set {
                if (_NumberOfPhotos != value)
                {
                    _NumberOfPhotos = value;
                    NotifyChangedOnUI("NumberOfPhotos");
                    NotifyChangedOnUI("SubText1");
                }
            }
        }

        DateTime _Created;
        public DateTime Created { get { return _Created; }
            set
            {
                _Created = value;
                NotifyChangedOnUI("Created");
                NotifyChangedOnUI("SubText1");
            }
        }

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
                        if (Created.Ticks == 0) { return "--"; }
                        return Created.ToLocalTime().ToString();
                }
                return "";
            }
        }

        int _NumberOfFolders;
        public int NumberOfFolders { get { return _NumberOfFolders; }
            set
            {
                if (_NumberOfFolders != value)
                {
                    _NumberOfFolders = value;
                    NotifyChangedOnUI("NumberOfFolders");
                    NotifyChangedOnUI("SubText2");
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected async void NotifyChangedOnUI(string name, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            var dispatcher = SystemUtil.GetCurrentDispatcher();
            if (dispatcher == null) { return; }

            await dispatcher.RunAsync(priority, () =>
            {
                NotifyChanged(name);
            });
        }

        protected void NotifyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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
