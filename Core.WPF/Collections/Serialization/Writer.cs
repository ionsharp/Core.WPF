using Imagin.Core.Analytics;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Serialization;
using Imagin.Core.Storage;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Imagin.Core.Collections.Serialization
{
    public abstract class Writer<T> : ChangeCollection<T>, ILimit, ISerialize, IWriter
    {
        public string FilePath { get; private set; }

        Limit limit = default;
        public Limit Limit
        {
            get => limit;
            set
            {
                limit = value;
                limit.Coerce(this);
            }
        }

        /// <summary>
        /// Preserve an unreadable file by renaming it to something else. 
        /// This is useful if the file has something readable like XML.
        /// <see cref="XmlWriter{T}"/> is true by default so important data isn't lost forever.
        /// <see cref="BinaryWriter{T}"/> is false by default because it isn't readable so data loss risk is high.
        /// </summary>
        public virtual bool Preserve { get; set; } = true;

        public readonly string FileExtension;

        public readonly string SingleFileExtension;

        ///

        public Writer(string folderPath, string fileName, string fileExtension, string singleFileExtension, Limit limit) : base()
        {
            FilePath = $@"{folderPath}\{fileName}.{fileExtension}";
            FileExtension = fileExtension;
            SingleFileExtension = singleFileExtension;
            Limit = limit;
        }

        ///

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    limit.Coerce(this);
                    break;
            }
        }

        ///

        public abstract Result Deserialize(string filePath, out object result);

        public Result Deserialize(string filePath, out IEnumerable<T> items)
        {
            items = null;

            var result = Deserialize(filePath, out object i);
            if (i is IEnumerable<T> j)
            {
                items = j;
                return new Success();
            }

            return result;
        }

        ///

        public Result Serialize(object input) => Serialize(FilePath, input);

        public abstract Result Serialize(string filePath, object input);

        ///

        public Result Load()
        {
            var result = Deserialize(FilePath, out IEnumerable<T> i);
            if (result)
            {
                Clear();
                if (i?.Count() > 0)
                {
                    foreach (var j in i)
                        Add(j);
                }
            }

            //If loading fails (for any reason)
            if (!result)
            {
                //If unreadable files should be preserved
                if (Preserve)
                {
                    //Preserve the unreadable file by renaming it to a "clone" that does not yet exist
                    if (File.Long.Exists(FilePath))
                        Try.Invoke(() => File.Long.Move(FilePath, StoragePath.Clone(FilePath, StoragePath.DefaultCloneFormat, j => Storage.File.Long.Exists(j))));
                }
            }
            return result;
        }

        public Result Save()
        {
            return Serialize(this);
        }

        ///

        ICommand clearCommand;
        public ICommand ClearCommand => clearCommand ??= new RelayCommand(() => Clear(), () => Count > 0);

        ICommand exportCommand;
        public ICommand ExportCommand => exportCommand ??= new RelayCommand<T>(i => _ = Export(i), i => i is T);

        ICommand exportAllCommand;
        public ICommand ExportAllCommand => exportAllCommand ??= new RelayCommand(() => _ = Export(this), () => Count > 0);

        ICommand importCommand;
        public ICommand ImportCommand => importCommand ??= new RelayCommand(() => Import(), () => true);

        ///

        public async Task<Result> Export() => await Export(XArray.New<T>(this));

        public async Task<Result> Export(T i) => await Export(XArray.New(i));

        public async Task<Result> Export(IEnumerable<T> items)
        {
            var count = items.Count();
            if (count == 0)
                return new Error();

            var fileExtension = count == 1 ? SingleFileExtension : FileExtension;

            var path = string.Empty;
            if (StorageDialog.Show(out path, nameof(Export), StorageDialogMode.SaveFile, new[] { fileExtension }, FilePath))
            {
                var stuff = count == 1 ? (object)items.First() : items;
                return await Task.Run(() => Serialize(path, stuff));
            }

            return new Error(new OperationCanceledException());
        }

        ///

        public Result Import()
        {
            var e = 0;

            if (StorageDialog.Show(out string[] paths, nameof(Import), StorageDialogMode.OpenFile, new[] { SingleFileExtension, FileExtension }, FilePath))
            {
                if (paths?.Length > 0)
                {
                    foreach (var i in paths)
                    {
                        var result = Deserialize(i, out object j);

                        if (!result) { e++; continue; }

                        if (j is IEnumerable<T> list)
                            list.ForEach(k => Add(k));

                        else if (j is T item)
                            Add(item);
                    }
                }
            }

            return e == 0 ? new Success() : new Error(new Exception());
        }
    }
}