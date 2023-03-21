using System.Collections;

namespace Imagin.Core.Models;

[Categorize(false), ViewSource(ShowHeader = false)]
public class ReadOnlySelectionForm : ViewModel
{
    readonly IList itemSource;
    [Hide]
    public IList ItemSource => itemSource;

    [Pin(Pin.AboveOrLeft), HideName]
    [Int32Style(Int32Style.Index, nameof(ItemSource), nameof(IName.Name))]
    public int SelectedIndex { get => Get(0); set => Set(value); }

    [Editable, HideName, ReadOnly]
    public object SelectedItem { get => Get<object>(); private set => Set(value); }

    public ReadOnlySelectionForm(IList itemSource) : base()
    {
        this.itemSource = itemSource;
        Update(() => SelectedIndex);
    }

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(SelectedIndex):
                if (SelectedIndex >= 0 && SelectedIndex < ItemSource.Count)
                    SelectedItem = ItemSource[SelectedIndex];

                break;
        }
    }
}