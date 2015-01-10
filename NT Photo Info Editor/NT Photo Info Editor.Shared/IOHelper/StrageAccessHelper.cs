using NtPhotoInfoEditor.DataModel;
using NtPhotoInfoEditor.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace NtPhotoInfoEditor.IOHelper
{
    public static class StorageAccessHelper
    {
        public static async Task<List<AlbumViewData>> ReadAlbums(List<StorageFolder> Albums)
        {
            var data = new List<AlbumViewData>();
            foreach (var album in Albums)
            {
                var property = await album.GetBasicPropertiesAsync();
                var items = await album.GetItemsAsync();
                var folders = await album.GetFoldersAsync();

                var viewData = new AlbumViewData() { Name = album.DisplayName, NumberOfFolders = folders.Count, NumberOfPhotos = items.Count };


                var thumbnail = new BitmapImage();
                if (items.Count > 0)
                {
                    try
                    {
                        var firstItem = await album.GetFileAsync(items[0].Name);
                        var thumb = await firstItem.GetThumbnailAsync(ThumbnailMode.ListView, 150);
                        thumbnail.SetSource(thumb);
                        viewData.Image = thumbnail;
                        thumb.Dispose();
                    }
                    catch (Exception e) { Logger.Log(e.StackTrace); }
                }

                data.Add(viewData);
            }
            return data;
        }
    }


}
