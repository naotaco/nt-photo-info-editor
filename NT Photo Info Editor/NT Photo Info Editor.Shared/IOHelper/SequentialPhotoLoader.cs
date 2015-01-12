using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Windows.Storage;
using System.IO;
using NtPhotoInfoEditor.Utils;
using System.Threading.Tasks;

namespace NtPhotoInfoEditor.IOHelper
{
    public class SequentialPhotoLoader
    {
        public static SequentialPhotoLoader INSTANCE = new SequentialPhotoLoader();
        private SequentialPhotoLoader() { }
        Queue<PhotoLoadTask> Queue = new Queue<PhotoLoadTask>();
        bool IsRunning = false;

        public async void Enqueue(PhotoLoadTask task)
        {
            Queue.Enqueue(task);
            Logger.Log("Enqueue: " + task.Path);
            if (!IsRunning)
            {
                await LoadSinglePhoto().ConfigureAwait(false);
            }
        }

        public void Clear()
        {
            Queue.Clear();
        }

        private async Task LoadSinglePhoto()
        {
            if (Queue.Count == 0) { return; }

            IsRunning = true;
            var task = Queue.Dequeue();
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(task.Path);
                if (task.Loaded != null) { task.Loaded.Invoke(file); };
            }
            catch (IOException ex) { Logger.Log(ex.StackTrace); }
            if (Queue.Count == 0)
            {
                IsRunning = false;
            }
            else
            {
                await LoadSinglePhoto().ConfigureAwait(false);
            }
        }
    }

    public class PhotoLoadTask
    {
        public string Path { get; set; }
        public Action<StorageFile> Loaded;
    }
}
