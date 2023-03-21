using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Linq;
using System;
using System.Collections.Specialized;

namespace Imagin.Core.Models;

[Serializable]
public class DocumentCollection : ObservableCollection<Document>
{
    public DocumentCollection() : base() { }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems?.ForEach<Document>(i => { i.Unsubscribe(); i.Subscribe(); });
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Reset:
                e.OldItems?.ForEach<Document>(i => { i.Unsubscribe(); });
                break;

            case NotifyCollectionChangedAction.Replace:
                break;
        }
    }
}