using NtPhotoInfoEditor.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using Windows.Storage;
using Windows.UI.Core;

namespace NtPhotoInfoEditor.DataModel
{
    public class FolderInfo
    {
        public StorageFolder Self { get; set; }
        public List<FolderInfo> Children { get; set; }

        public List<ContentViewData> Contents { get; set; }
    }
}
