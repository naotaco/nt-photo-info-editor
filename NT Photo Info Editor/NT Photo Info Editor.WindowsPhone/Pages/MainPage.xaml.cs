using NtPhotoInfoEditor.Common;
using NtPhotoInfoEditor.Pages;
using NtPhotoInfoEditor.DataModel;
using NtPhotoInfoEditor.IOHelper;
using NtPhotoInfoEditor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NtPhotoInfoEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        private NavigationHelper navigationHelper;

        readonly StorageFolder RootFolder = KnownFolders.PicturesLibrary;
        FolderInfo RootFolderInfo;
        private StatusBar statusBar = StatusBar.GetForCurrentView();

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.munimuni

            Init();

            navigationHelper.OnNavigatedTo(e);
            ChangeProgressText("Reading files...");
            RootFolderInfo = await StorageAccessHelper.ReadAllContentsAsync(RootFolder).ConfigureAwait(false);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var FirstPivotItem = CreateFolderPivotItem();
                (FirstPivotItem.Content as ListView).ItemsSource = RootFolderInfo.Contents;
                PivotRoot.Items.Add(FirstPivotItem);
                HideProgress();
            });
        }

        void Init()
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (PivotRoot.SelectedIndex == 0)
            {
                return;
            }

            PivotRoot.SelectedIndex--;
            e.Handled = true;
        }

        async void AlbumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeProgressText("Reading files...");

            var selectedPivotIndex = (((sender as ListView).Parent as PivotItem).Parent as Pivot).SelectedIndex;
            var selectedItemIndex = (sender as ListView).SelectedIndex;
            Logger.Log("pivot: " + selectedPivotIndex + " item: " + selectedItemIndex);

            if (selectedItemIndex >= (((sender as ListView).ItemsSource) as List<ContentViewData>).Count) { return; }
            var selectedContent = (((sender as ListView).ItemsSource) as List<ContentViewData>).ElementAt(selectedItemIndex);
            Logger.Log("selected content: " + selectedContent.Name);

            switch (selectedContent.Type)
            {
                case ContentType.Folder:
                    await OpenFolder(selectedContent.Folder, selectedPivotIndex);
                    break;
                case ContentType.Jpeg:
                    OpenPhoto(selectedContent.File.Path);
                    break;
            }
            HideProgress();
        }

        private async Task OpenFolder(StorageFolder storageFolder, int currentLevel)
        {
            for (int i = currentLevel + 1; i < PivotRoot.Items.Count; i++)
            {
                PivotRoot.Items.RemoveAt(i);
            }

            var pItem = CreateFolderPivotItem();
            PivotRoot.Items.Add(pItem);
            PivotRoot.SelectedIndex = currentLevel + 1;

            var fInfo = await StorageAccessHelper.ReadAllContentsAsync(storageFolder);
            (pItem.Content as ListView).ItemsSource = fInfo.Contents;

        }

        private void OpenPhoto(string path)
        {
            Frame.Navigate(typeof(PhotoInfoPage), path);
        }


        PivotItem CreateFolderPivotItem()
        {
            var item = new PivotItem();
            item.Margin = new Thickness(0);
            var list = new ListView();
            list.ItemTemplate = (DataTemplate)App.Current.Resources["ContentTemplate"];
            list.SelectionMode = ListViewSelectionMode.Single;
            list.SelectionChanged += AlbumList_SelectionChanged;
            item.Content = list;
            return item;
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
