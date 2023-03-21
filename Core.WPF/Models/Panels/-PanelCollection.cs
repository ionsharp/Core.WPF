using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Linq;
using System.Collections.Specialized;

namespace Imagin.Core.Models;

public class PanelCollection : ObservableCollection<Panel>
{
    public IDockViewModel ViewModel { get => this.Get<IDockViewModel>(); private set => this.Set(value); }

    public PanelCollection(IDockViewModel viewModel) : base() => ViewModel = viewModel;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems?.ForEach<Panel>(i => { i.ViewModel = ViewModel; i.Unsubscribe(); i.Subscribe(); });
                break;

            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Reset:
                e.OldItems?.ForEach<Panel>(i => { i.ViewModel = null; i.Unsubscribe(); });
                break;

            case NotifyCollectionChangedAction.Replace:
                break;
        }
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(ViewModel))
            this.ForEach(i => i.ViewModel = (IDockViewModel)e.NewValue);
    }
}