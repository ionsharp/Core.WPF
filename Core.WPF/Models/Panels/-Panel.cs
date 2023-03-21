using Imagin.Core.Controls;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using System;
using System.Timers;
using System.Xml.Serialization;

namespace Imagin.Core.Models;

[Serializable]
public abstract class Panel : Content
{
    public const string DefaultTemplateKey = "TemplateKey";

    #region Delegates

    internal delegate void SizeRequestHandler(Panel sender, double length);

    #endregion

    #region Events

    internal event SizeRequestHandler HeightRequested;

    internal event SizeRequestHandler WidthRequested;

    #endregion

    #region Fields

    public const SecondaryDocks DefaultDockPreference = SecondaryDocks.Center;

    public const System.Windows.Controls.Orientation DefaultOrientationPreference = System.Windows.Controls.Orientation.Horizontal;

    #endregion

    #region Properties

    [Hide]
    public object ActualTemplateKey => Try.Return(() => GetType().GetField(DefaultTemplateKey)?.GetValue(null));

    /// <summary>Gets whether or not the panel can be hidden.</summary>
    [Hide]
    public virtual bool CanHide { get; } = true;

    /// <summary>Gets whether or not the panel can live with other panels.</summary>
    [Hide]
    public virtual bool CanShare { get; } = true;

    /// <summary>Gets the direction in which the panel prefers to dock.</summary>
    [Hide]
    public virtual SecondaryDocks DockPreference { get; } = DefaultDockPreference;

    [Hide]
    public double Height { set => HeightRequested?.Invoke(this, value); }

    [Hide]
    public virtual Uri Icon => this.GetAttribute<ImageAttribute>()?.SmallImage.IfGet(i => new Uri(i, UriKind.Absolute));


    /// <summary>
    /// To do: Move logic to <see cref="DockControl"/>. Can't until everything is refactored to accomadate view models.
    /// </summary>
    [Hide, NonSerializable]
    public bool IsOptionsVisible { get => Get(false); set => Set(value); }

    [Hide, NonSerializable]
    public bool IsSelected { get => Get(false); set => Set(value); }

    [Hide, NonSerializable]
    public bool IsVisible { get => Get(true); set => Set(value); }

    [Hide]
    public virtual double MaxHeight { get; }

    [Hide]
    public virtual double MaxWidth { get; }

    [Hide]
    public virtual double MinHeight { get; }

    [Hide]
    public virtual double MinWidth { get; }

    [Hide]
    public string Name => GetType().Name;

    [Hide, NonSerializable]
    public ControlLength PinHeight { get => Get<ControlLength>(); set => Set(value); }

    [Hide, NonSerializable]
    public ControlLength PinWidth { get => Get<ControlLength>(); set => Set(value); }

    [Hide, NonSerializable]
    public double Progress { get => Get(.0); set => Set(value); }

    [Hide]
    public bool ProgressVisibility { get => Get(false); set => Set(value); }

    [Hide]
    public override string Title => this.GetName() is string result ? (" panel" is string suffix && result.EndsWith(suffix) ? result.Substring(0, result.Length - suffix.Length) : result) : null;

    [Hide]
    public virtual bool TitleLocalized => true;

    [Hide]
    public virtual bool TitleVisibility => true;

    [Hide, XmlIgnore]
    public override object ToolTip => this;

    [Hide]
    public virtual TopBottom ToolBarPlacement => TopBottom.Top;

    [Hide]
    public IDockViewModel ViewModel { get; set; }

    [Hide]
    public double Width { set => WidthRequested?.Invoke(this, value); }

    #endregion

    #region Panel

    public Panel() : base()
    {
        update = new(UpdateInterval, CanUpdate);
    }

    #endregion

    #region Methods

    [Hide]
    public T ActiveContent<T>() where T : Content => (T)ViewModel?.ActiveContent;

    [Hide]
    public T ActiveDocument<T>() where T : Document => (T)ViewModel?.ActiveDocument;

    [Hide]
    public T ActivePanel<T>() where T : Panel => (T)ViewModel?.ActivePanel;

    ///

    void OnActiveContentChanged(object sender, ChangedEventArgs<Content> e) => OnActiveContentChanged(e.NewValue);

    protected virtual void OnActiveContentChanged(Content input) { }

    void OnActiveDocumentChanged(object sender, ChangedEventArgs<Document> e) => OnActiveDocumentChanged(e.NewValue);

    protected virtual void OnActiveDocumentChanged(Document input) { }

    void OnActivePanelChanged(object sender, ChangedEventArgs<Panel> e) => OnActivePanelChanged(e.NewValue);

    protected virtual void OnActivePanelChanged(Panel input) { }

    void OnDocumentAdded(object sender, EventArgs<Document> e) => OnActiveDocumentChanged(e.Value);

    protected virtual void OnDocumentAdded(Document input) { }

    void OnDocumentRemoved(object sender, EventArgs<Document> e) => OnActiveDocumentChanged(e.Value);

    protected virtual void OnDocumentRemoved(Document input) { }

    ///

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Title))
            Update(() => ToolTip);
    }

    #endregion

    #region Subscribe/Unsubscribe

    public override void Subscribe()
    {
        base.Subscribe();
        update.Updated -= OnUpdate; update.Updated += OnUpdate;
        ViewModel.If(i =>
        {
            i.ActiveContentChanged += OnActiveContentChanged;
            i.ActiveDocumentChanged += OnActiveDocumentChanged;
            i.ActivePanelChanged += OnActivePanelChanged;

            i.DocumentAdded += OnDocumentAdded;
            i.DocumentRemoved += OnDocumentRemoved;
        });
    }

    public override void Unsubscribe()
    {
        base.Subscribe();
        update.Updated -= OnUpdate;
        ViewModel.If(i =>
        {
            i.ActiveContentChanged -= OnActiveContentChanged;
            i.ActiveDocumentChanged -= OnActiveDocumentChanged;
            i.ActivePanelChanged -= OnActivePanelChanged;

            i.DocumentAdded -= OnDocumentAdded;
            i.DocumentRemoved -= OnDocumentRemoved;
        });
    }

    #endregion

    #region Update

    Updatable update = null;

    protected virtual bool CanUpdate => false;

    protected virtual TimeSpan UpdateInterval => Updatable.DefaultInterval;

    void OnUpdate(object sender, ElapsedEventArgs e) => OnUpdate(e);

    protected virtual void OnUpdate(ElapsedEventArgs e) { }

    #endregion
}