using NtPhotoInfoEditor.DataModel;
using NtPhotoInfoEditor.Utils;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
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

            Path = (string)e.Parameter;
            Logger.Log("path: " + Path);
            File = await StorageFile.GetFileFromPathAsync(Path);

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            DetailInfoList.DataContext = PhotoData.EntryList;

            await LoadImage(File);
        }

        private async Task LoadImage(StorageFile file)
        {
            ChangeProgressText("Loading file...");

            using (var fileStream = await file.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                var image = new BitmapImage();
                image.SetSource(fileStream.AsRandomAccessStream());
                Image.Source = image;

                fileStream.Seek(0, SeekOrigin.Begin);
                try
                {
                    PhotoData.MetaData = await NtImageProcessor.MetaData.JpegMetaDataParser.ParseImageAsync(fileStream);
                    HideProgress();
                }
                catch (Exception ex) { Logger.Log(ex.StackTrace); }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
            e.Handled = true;
        }

        private async void HideProgress()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Logger.Log("HideProgress.");
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

        private void DetailInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) { return; }

            var selected = e.AddedItems.ElementAt(0) as EntryViewData;
            Logger.Log("Selection changed: " + selected.MetadataKey + " " + selected.Name);
        }

    }
}
