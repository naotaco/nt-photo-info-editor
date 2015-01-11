using NtPhotoInfoEditor.DataModel;
using NtPhotoInfoEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace NtPhotoInfoEditor.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PhotoInfoPage : Page
    {
        public PhotoInfoPage()
        {
            this.InitializeComponent();
        }

        string Path;
        StorageFile File;
        private StatusBar statusBar = StatusBar.GetForCurrentView();
        PhotoPlaybackData PhotoData = new PhotoPlaybackData();

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ChangeProgressText("Loading file...");
            Path = (string)e.Parameter;
            Logger.Log("path: " + Path);
            File = await StorageFile.GetFileFromPathAsync(Path);

            using (var fileStream = await File.OpenStreamForReadAsync())
            {
                var image = new BitmapImage();
                image.SetSource(fileStream.AsRandomAccessStream());
                Image.Source = image;

                fileStream.Seek(0, SeekOrigin.Begin);
                try
                {
                    PhotoData.MetaData = NtImageProcessor.MetaData.JpegMetaDataParser.ParseImage(fileStream);
                    DetailInfoList.DataContext = PhotoData.EntryList;
                }
                catch (Exception ex) { Logger.Log(ex.StackTrace); }
            }
            HideProgress();
        }

        private async void HideProgress()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await statusBar.ProgressIndicator.HideAsync();
            });
        }

        private async void ChangeProgressText(string text)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                statusBar.ProgressIndicator.ProgressValue = null;
                statusBar.ProgressIndicator.Text = text;
                await statusBar.ProgressIndicator.ShowAsync();
            });
        }

    }
}
