using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;

namespace NtPhotoInfoEditor.Utils
{
    public class SystemUtil
    {
        public static string GetStringResource(string key)
        {
            return ResourceLoader.GetForCurrentView().GetString(key);
        }
    }
}
