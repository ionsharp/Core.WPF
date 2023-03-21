using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Threading;
using System;
using System.Collections;
using System.Windows.Input;

namespace Imagin.Core.Models;

[Description("A queue of operations to run asynchronously."), Name(DefaultName), Serializable]
public class QueuePanel : DataPanel
{
    public static readonly ResourceKey TemplateKey = new();

    enum Category { Sort }

    public const string DefaultName = "Queue";

    [Hide]
    public override IList GroupNames => new StringCollection()
    {
        "None",
        nameof(Operation.Source),
        nameof(Operation.Status),
        nameof(Operation.Type),
    };

    [Hide]
    public override Uri Icon => Resource.GetImageUri(SmallImages.Queue);

    [Hide]
    public override IList SortNames => new StringCollection()
    {
        nameof(Operation.Added),
        nameof(Operation.Duration),
        nameof(Operation.Target),
        nameof(Operation.Progress),
        nameof(Operation.Speed),
        nameof(Operation.Size),
        nameof(Operation.SizeRead),
        nameof(Operation.Source),
        nameof(Operation.Status),
        nameof(Operation.Type),
    };

    [Hide]
    public override string TitleKey => DefaultName;

    public QueuePanel() : base() { }

    [Hide]
    public override ICommand ClearCommand => base.ClearCommand;

    [Hide]
    public override ICommand RefreshCommand => base.RefreshCommand;
}