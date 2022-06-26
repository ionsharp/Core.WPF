using Imagin.Core.Collections.Generic;
using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Colors;
using Imagin.Core.Config;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Controls;

public partial class ColorControl : Control
{
    public event DefaultEventHandler<ColorDocument> ActiveDocumentChanged;

    public static readonly ResourceKey<PanelTemplateSelector> PanelTemplateSelectorKey = new();

    #region Properties

    static readonly DependencyPropertyKey ColorsPanelKey = DependencyProperty.RegisterReadOnly(nameof(ColorsPanel), typeof(ColorsPanel), typeof(ColorControl), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ColorsPanelProperty = ColorsPanelKey.DependencyProperty;
    public ColorsPanel ColorsPanel
    {
        get => (ColorsPanel)GetValue(ColorsPanelProperty);
        private set => SetValue(ColorsPanelKey, value);
    }

    public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register(nameof(ActiveDocument), typeof(ColorDocument), typeof(ColorControl), new FrameworkPropertyMetadata(null, OnActiveDocumentChanged));
    public ColorDocument ActiveDocument
    {
        get => (ColorDocument)GetValue(ActiveDocumentProperty);
        set => SetValue(ActiveDocumentProperty, value);
    }
    static void OnActiveDocumentChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ColorControl>().OnActiveDocumentChanged(e);

    public static readonly DependencyProperty DocumentsProperty = DependencyProperty.Register(nameof(Documents), typeof(DocumentCollection), typeof(ColorControl), new FrameworkPropertyMetadata(null));
    public DocumentCollection Documents
    {
        get => (DocumentCollection)GetValue(DocumentsProperty);
        set => SetValue(DocumentsProperty, value);
    }

    static readonly DependencyPropertyKey IlluminantsPanelKey = DependencyProperty.RegisterReadOnly(nameof(IlluminantsPanel), typeof(ColorIlluminantsPanel), typeof(ColorControl), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty IlluminantsPanelProperty = IlluminantsPanelKey.DependencyProperty;
    public ColorIlluminantsPanel IlluminantsPanel
    {
        get => (ColorIlluminantsPanel)GetValue(IlluminantsPanelProperty);
        private set => SetValue(IlluminantsPanelKey, value);
    }

    public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(nameof(Options), typeof(IColorControlOptions), typeof(ColorControl), new FrameworkPropertyMetadata(null, OnOptionsChanged));
    public IColorControlOptions Options
    {
        get => (IColorControlOptions)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }
    static void OnOptionsChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ColorControl>().OnOptionsChanged(e.NewValue as IColorControlOptions);

    public static readonly DependencyProperty OptionsPanelProperty = DependencyProperty.Register(nameof(OptionsPanel), typeof(OptionsPanel), typeof(ColorControl), new FrameworkPropertyMetadata(null));
    public OptionsPanel OptionsPanel
    {
        get => (OptionsPanel)GetValue(OptionsPanelProperty);
        set => SetValue(OptionsPanelProperty, value);
    }

    static readonly DependencyPropertyKey PanelsKey = DependencyProperty.RegisterReadOnly(nameof(Panels), typeof(PanelCollection), typeof(ColorControl), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty PanelsProperty = PanelsKey.DependencyProperty;
    public PanelCollection Panels
    {
        get => (PanelCollection)GetValue(PanelsProperty);
        private set => SetValue(PanelsKey, value);
    }

    static readonly DependencyPropertyKey ProfilesPanelKey = DependencyProperty.RegisterReadOnly(nameof(ProfilesPanel), typeof(ColorProfilesPanel), typeof(ColorControl), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ProfilesPanelProperty = ProfilesPanelKey.DependencyProperty;
    public ColorProfilesPanel ProfilesPanel
    {
        get => (ColorProfilesPanel)GetValue(ProfilesPanelProperty);
        private set => SetValue(ProfilesPanelKey, value);
    }

    public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(ColorControl), new FrameworkPropertyMetadata(null));
    public ICommand SaveCommand
    {
        get => (ICommand)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    readonly ColorControlOptions defaultOptions = null;

    #endregion

    #region ColorControl

    public ColorControl() : base()
    {
        this.RegisterHandler(OnLoaded, OnUnloaded);

        ColorControlOptions.Load($@"{ApplicationProperties.GetFolderPath(DataFolders.Documents)}\{nameof(ColorControl)}\Options.data", out defaultOptions);

        SetCurrentValue(DocumentsProperty,
            new DocumentCollection());

        var panels = new PanelCollection();

        ColorsPanel = new ColorsPanel(defaultOptions.Colors);
        panels.Add(ColorsPanel);

        SetCurrentValue(OptionsPanelProperty, new OptionsPanel());

        panels.Add(OptionsPanel);
        panels.Add(new ColorPanel());
        panels.Add(new ColorAnalysisPanel());
        panels.Add(new ColorChromacityPanel());
        panels.Add(new ColorHarmonyPanel(this));

        IlluminantsPanel = new ColorIlluminantsPanel(defaultOptions.Illuminants);
        panels.Add(IlluminantsPanel);

        ProfilesPanel = new ColorProfilesPanel(defaultOptions.Profiles);
        panels.Add(ProfilesPanel);

        Panels = panels;
        SetCurrentValue(OptionsProperty, defaultOptions);
    }

    #endregion

    #region Methods

    void OnColorSaved(object sender, EventArgs<Color> e)
    {
        ColorsPanel?.SelectedGroup.If(i => i.Add(e.Value));
    }

    void OnColorSelected(object sender, EventArgs<StringColor> e)
        => ActiveDocument.If(i => i.Color.ActualColor = e.Value);

    void OnDocumentsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems[0].As<ColorDocument>().ColorSaved += OnColorSaved;
                break;

            case NotifyCollectionChangedAction.Remove:
                e.OldItems[0].As<ColorDocument>().ColorSaved -= OnColorSaved;
                break;
        }
    }

    void OnLoaded()
    {
        Documents.CollectionChanged += OnDocumentsChanged;
        Documents.ForEach(i => i.As<ColorDocument>().ColorSaved += OnColorSaved);
    }

    void OnUnloaded()
    {
        Documents.CollectionChanged -= OnDocumentsChanged;
        Documents.ForEach(i => i.As<ColorDocument>().ColorSaved -= OnColorSaved);
    }

    protected virtual void OnActiveDocumentChanged(Value<ColorDocument> input)
    {
        ActiveDocumentChanged?.Invoke(this, new(input.New));
    }

    protected virtual void OnOptionsChanged(IColorControlOptions input)
    {
        if (input != null)
        {
            input.OnLoaded(this);
            ColorsPanel?.Update(input.Colors);
            ColorsPanel.Selected += OnColorSelected;

            IlluminantsPanel?.Update(input.Illuminants);

            ProfilesPanel?.Update(input.Profiles);
        }
    }

    public static ListCollectionView GetModels()
    {
        var models = new ObservableCollection<NamableCategory<Type>>();
        Colour.Types.ForEach(i => models.Add(new(i.Name, i.Name.Substring(0, 1).ToUpper(), i)));

        var result = new ListCollectionView(models);
        result.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription("Category", System.ComponentModel.ListSortDirection.Ascending));
        result.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

        return result;
    }

    #endregion

    #region Commands

    ICommand saveOldColorCommand;
    public ICommand SaveOldColorCommand => saveOldColorCommand ??= new RelayCommand(() => ColorsPanel?.SelectedGroup.If<GroupCollection<StringColor>>(i => i.Add(ActiveDocument.OldColor)), () => ActiveDocument != null);

    ICommand saveNewColorCommand;
    public ICommand SaveNewColorCommand => saveNewColorCommand ??= new RelayCommand(() => ColorsPanel?.SelectedGroup.If<GroupCollection<StringColor>>(i => i.Add(ActiveDocument.Color.ActualColor)), () => ActiveDocument != null);

    #endregion
}