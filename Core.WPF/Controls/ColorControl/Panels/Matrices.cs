using Imagin.Core.Collections.Serialization;
using Imagin.Core.Input;
using Imagin.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imagin.Core.Controls;

[DisplayName("Matrices"), Explicit, Serializable]
public class ColorMatricesPanel : GroupPanel<MatrixModel>
{
    enum Category { Add }

    public override Uri Icon => Resources.InternalImage(Images.Matrix);

    public override string ItemName => "matrix";

    public override string Title => "Matrices";

    public ColorMatricesPanel(IGroupWriter input) : base(input) { }

    protected override MatrixModel GetNewItem() => new($"Untitled {ItemName}", "", Matrix.Zero3x3);

    ICommand addChromacityMatrixCommand;
    [Category(nameof(Category.Add)), DisplayName("Add chromacity matrix")]
    [Image(Images.XYZRound)]
    [Index(1), Tool, Visible]
    public ICommand AddChromacityMatrixCommand => addChromacityMatrixCommand ??= new RelayCommand(() =>
    {
        var oldItem = new ChromacityMatrixModel($"Untitled {ItemName}", "", Matrix.Zero3x3);
        var newItem = new GroupValueModel(Groups, oldItem, SelectedGroupIndex);

        MemberWindow.ShowDialog($"New {ItemName}", newItem, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; i.NameColumnVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);
        if (result == 0)
        {
            if (newItem.GroupIndex >= 0)
                Groups[newItem.GroupIndex].Add((MatrixModel)newItem.Value);
        }
    });
}