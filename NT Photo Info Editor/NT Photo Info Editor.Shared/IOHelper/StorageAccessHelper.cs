using NtPhotoInfoEditor.DataModel;
using NtPhotoInfoEditor.PhotoUtil;
using NtPhotoInfoEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace NtPhotoInfoEditor.IOHelper
{
    public static class StorageAccessHelper
    {

        public static async Task<FolderInfo> ReadAllFoldersAsync(StorageFolder ParentFolder)
        {
            var folder = new FolderInfo();
            folder.Self = ParentFolder;
            folder.Contents = new List<ContentViewData>();
            var folders = new List<StorageFolder>(await ParentFolder.GetFoldersAsync());
            foreach (var f in folders)
            {
                folder.Contents.Add(await ReadFolderViewData(f));
            }
            return folder;
        }

        public static async Task<FolderInfo> ReadAllContentsAsync(StorageFolder ParentFolder)
        {        
            var items = await ParentFolder.GetFilesAsync();
            var folder = await ReadAllFoldersAsync(ParentFolder).ConfigureAwait(false);

            foreach (var item in items)
            {
                if (Definitions.PhotoFileExtensions.Contains(item.FileType))
                {
                    // add contents if supported photo file type
                    var viewData = new ContentViewData() { Name = item.Name, Type = ContentType.Jpeg, File = item, Created = new DateTime(0) };
                    folder.Contents.Add(viewData);

                    SequentialPhotoLoader.INSTANCE.Enqueue(new PhotoLoadTask()
                    {
                        Path = item.Path,
                        Loaded = async (file) =>
                            {
                                await SystemUtil.GetCurrentDispatcher().RunAsync(CoreDispatcherPriority.Normal, async () =>
                                {
                                    viewData.Created = file.DateCreated.DateTime;
                                    viewData.Image = await ReadThumbnailAsync(file);
                                    Logger.Log("Image set: " + file.Name);
                                });
                            }
                    });
                }
            }
            return folder;
        }

        static async Task<ContentViewData> ReadFolderViewData(StorageFolder folder)
        {
            var data = new List<ContentViewData>();
            var property = await folder.GetBasicPropertiesAsync();
            var items = await folder.GetItemsAsync();
            var folders = await folder.GetFoldersAsync();

            var viewData = new ContentViewData() { Name = folder.DisplayName, NumberOfFolders = folders.Count, NumberOfPhotos = await CountPhotoFile(folder), Type = ContentType.Folder, Folder = folder };

            if (items.Count > 0)
            {
                try
                {
                    viewData.Image = await ReadThumbnailAsync(await folder.GetFileAsync(items[0].Name));
                }
                catch (Exception e) { Logger.Log(e.StackTrace); }
            }
            return viewData;
        }

        static async Task<BitmapImage> ReadThumbnailAsync(StorageFile file)
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
            catch (Exception e) { Logger.Log(e.StackTrace); }
            Logger.Log("Return thumbnail: " + file.Name);
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
