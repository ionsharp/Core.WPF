using Imagin.Core.Linq;
using Imagin.Core.Models;
using System;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Imagin.Core.Config;

[Description("Inspect the logical/visual tree of the main window.")]
[Image(SmallImages.Tree), Name("Inspect"), Serializable]
public class InspectExtension : PanelExtension
{
    public override string Author => nameof(Imagin);

    public override string Description => this.GetDescription();

    public override string Icon => this.GetAttribute<ImageAttribute>().SmallImage;

    public override string Name => this.GetName();

    public override string Uri => null;

    public override Version Version => new Version(1, 0, 0, 0);

    [NonSerialized]
    InspectPanel panel;
    [Hide]
    public override Panel Panel => panel;

    public override IExtensionResources Resources => null;

    public InspectExtension() : base()
    {
        panel = new();
    }

    void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        //Log.Write<InspectPanel>($"OriginalSource = {(e.OriginalSource?.GetType().FullName ?? "null")}, Source = {(e.Source?.GetType().FullName ?? "null")}");
    }

    [OnDeserialized]
    void OnDeserialized(StreamingContext context)
    {
        panel = new();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        Current.Get<BaseApplication>().MainWindow.PreviewMouseDown -= OnPreviewMouseDown;
    }

    public override void OnEnabled()
    {
        base.OnEnabled();
        panel.Source.Add(new(panel, Current.Get<BaseApplication>().MainWindow, ElementTypes.Visual));
        Current.Get<BaseApplication>().MainWindow.PreviewMouseDown += OnPreviewMouseDown;
    }
}