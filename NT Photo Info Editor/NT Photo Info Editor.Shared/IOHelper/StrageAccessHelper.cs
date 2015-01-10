using NtPhotoInfoEditor.DataModel;
using NtPhotoInfoEditor.PhotoUtil;
using NtPhotoInfoEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace NtPhotoInfoEditor.IOHelper
{
    public static class StorageAccessHelper
    {
        public static async Task<List<ContentViewData>> CreateContentListFromFolder(StorageFolder ParentFolder)
        {
            var data = new List<ContentViewData>();
            var Children = await ParentFolder.GetFoldersAsync();
            foreach (var folder in Children)
            {
                data.Add(await ReadFolder(folder));
            }
            return data;
        }

        static async Task<ContentViewData> ReadFolder(StorageFolder folder)
        {
            var data = new List<ContentViewData>();
            var property = await folder.GetBasicPropertiesAsync();
            var items = await folder.GetItemsAsync();
            var folders = await folder.GetFoldersAsync();

            var viewData = new ContentViewData() { Name = folder.DisplayName, NumberOfFolders = folders.Count, NumberOfPhotos = await CountPhotoFile(folder), Type = ContentType.Folder };

            if (items.Count > 0)
            {
                try
                {
                    viewData.Image = await ReadAsThumbnail(await folder.GetFileAsync(items[0].Name));
                }
                catch (Exception e) { Logger.Log(e.StackTrace); }
            }
            return viewData;
        }

        static async Task<ContentViewData> ReadFile(StorageFile file)
        {
            var viewData = new ContentViewData() { Name = file.Name, Type = ContentType.Jpeg, Created = file.DateCreated.DateTime };
            try
            {
                viewData.Image = await ReadAsThumbnail(file);
            }
            catch (IOException ex) { Logger.Log(ex.StackTrace); }
            catch (Exception ex) { Logger.Log(ex.StackTrace); }
            return viewData;
        }

        static async Task<BitmapImage> ReadAsThumbnail(StorageFile file)
        {
            Logger.Log("Getting thumbnail: " + file.Name);
            var thumbnail = new BitmapImage();
            try
            {
                using (var thumb = await file.GetThumbnailAsync(ThumbnailMode.ListView, 150))
                {
                    thumbnail.SetSource(thumb);
                    thumb.Dispose();
                }
            }
            catch (IOException e) { Logger.Log(e.StackTrace); }
            return thumbnail;
        }

        static async Task<int> CountPhotoFile(StorageFolder folder)
        {
            IReadOnlyList<StorageFile> items = new List<StorageFile>();
            int itemCount = 0;
                
            try
            {
                items = await folder.GetFilesAsync();
                foreach (var item in items)
                {
                    if (Definitions.PhotoFileExtensions.Contains(item.FileType)) { itemCount++; }
                }
            }
            catch (IOException ex)
            {
                Logger.Log(ex.StackTrace);
                return 0;
            }
            return itemCount;
        }
    }


}
