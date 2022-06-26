using Imagin.Core.Analytics;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Imagin.Core.Reflection;

public partial class MemberModel
{
    public IList SourceCollection => Value as IList;

    public ObservableCollection<ListItemModel> Items { get; private set; }

    public int CollectionLength
    {
        get => SourceCollection.Count;
        set
        {
            Resize(value);
            this.Changed(() => CollectionLength);
        }
    }

    int selectedIndex = -1;
    public int SelectedIndex
    {
        get => selectedIndex;
        set => this.Change(ref selectedIndex, value);
    }

    //...

    object CreateItem(Type type)
    {
        type ??= ItemType;
        return type == typeof(string) ? "" : type.Create<object>();
    }

    void CreateItem(int index, object i)
    {
        ListItemModel result = new(this, new(Parent, new(new MemberPathSource(i)), null, new(null)));
        if (index == -1)
            Items.Add(result);

        else Items.Insert(index, result);

        result.DepthIndex = 0;
        result.UpdateValue();

        result.RefreshHard();
        result.Subscribe();
    }

    object GetSelectedItem()
        => SourceCollection is IList i && i.Count > SelectedIndex && SelectedIndex >= 0 ? i[SelectedIndex] : null;

    //...

    protected virtual void InsertAbove(int index, Type type)
        => Try.Invoke(() => CreateItem(type).If(i => SourceCollection.Insert(index == -1 ? 0 : index, i)), e => Log.Write<MemberModel>(e));

    protected virtual void InsertBelow(int index, Type type) 
        => Try.Invoke(() => CreateItem(type).If(i =>
        {
            var newIndex = index + 1;
            if (index != -1 && newIndex < SourceCollection.Count)
                SourceCollection.Insert(newIndex, i);

            else SourceCollection.Add(i);
        }), e => Log.Write<MemberModel>(e));

    void Resize(int length)
    {
        if (length == SourceCollection?.Count)
            return;

        Try.Invoke(() =>
        {
            if (length == 0)
                SourceCollection.Clear();

            else if (length > SourceCollection.Count)
            {
                var j = length - SourceCollection.Count;
                for (var i = 0; i < j; i++)
                    SourceCollection.Add(CreateItem(null));
            }
            else
            {
                var j = SourceCollection.Count - length;
                for (var i = SourceCollection.Count - 1; i >= length; i--)
                    SourceCollection.RemoveAt(i);
            }
        },
        e => Log.Write<MemberModel>(e));
    }

    //...

    ICommand insertAboveCommand;
    public ICommand InsertAboveCommand
        => insertAboveCommand
        ??= new RelayCommand<Type>(i => InsertAbove(SelectedIndex, i),
            i => SourceCollection != null);

    ICommand insertBelowCommand;
    public ICommand InsertBelowCommand
        => insertBelowCommand
        ??= new RelayCommand<Type>(i => InsertBelow(SelectedIndex, i),
            i => SourceCollection != null);

    ICommand moveDownCommand;
    public ICommand MoveDownCommand
        => moveDownCommand
        ??= new RelayCommand(() => Try.Invoke(() => SourceCollection.MoveDown(SelectedIndex, true)),
            () => GetSelectedItem() != null);

    ICommand moveUpCommand;
    public ICommand MoveUpCommand
        => moveUpCommand
        ??= new RelayCommand(() => Try.Invoke(() => SourceCollection.MoveUp(SelectedIndex, true)),
            () => GetSelectedItem() != null);

    ICommand removeCommand;
    public ICommand RemoveCommand
        => removeCommand
        ??= new RelayCommand(() => Try.Invoke(() => SourceCollection.RemoveAt(SelectedIndex)),
            () => GetSelectedItem() != null);

    ICommand resetCommand;
    public ICommand ResetCommand
        => resetCommand
        ??= new RelayCommand(() => Try.Invoke(() => GetSelectedItem().If<IReset>(i => i.Reset())),
            () => GetSelectedItem() is IReset);
}