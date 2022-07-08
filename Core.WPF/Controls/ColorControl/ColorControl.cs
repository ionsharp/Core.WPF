using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Config;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
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

    readonly ColorControlOptions options = null;

    #endregion

    #region ColorControl
    
    public readonly ColorAnalysisPanel AnalysisPanel;

    public readonly ColorPanel ColorPanel;

    public readonly ColorDifferencePanel DifferencePanel;
    
    public readonly ColorHarmonyPanel HarmonyPanel;

    public readonly ColorMatrixPanel MatrixPanel;

    public ColorControl() : base()
    {
        this.RegisterHandler(OnLoaded, OnUnloaded);

        SetCurrentValue(DocumentsProperty, new DocumentCollection());

        options = new ColorControlOptions($@"{ApplicationProperties.GetFolderPath(DataFolders.Documents)}\{nameof(ColorControl)}\Options.data", Documents);
        options.Load(out BaseSavable newOptions);
        options = newOptions as ColorControlOptions;

        var panels = new PanelCollection();

        ColorsPanel = new ColorsPanel(options.Colors);
        ColorsPanel.Selected += OnColorSelected;
        panels.Add(ColorsPanel);

        SetCurrentValue(OptionsPanelProperty, new OptionsPanel());

        panels.Add(OptionsPanel);

        ColorPanel = new ColorPanel();
        panels.Add(ColorPanel);

        ColorPanel.PropertyChanged += OnColorPanelChanged;

        panels.Add(new ColorChromacityPanel());

        AnalysisPanel = new ColorAnalysisPanel(options.Profiles);
        panels.Add(AnalysisPanel);

        DifferencePanel = new ColorDifferencePanel(options.Profiles);
        panels.Add(DifferencePanel);

        HarmonyPanel = new ColorHarmonyPanel(this);
        panels.Add(HarmonyPanel);

        IlluminantsPanel = new ColorIlluminantsPanel(options.Illuminants);
        panels.Add(IlluminantsPanel);

        MatrixPanel = new ColorMatrixPanel(options.Matrices);
        panels.Add(MatrixPanel);

        ProfilesPanel = new ColorProfilesPanel(options.Profiles);
        panels.Add(ProfilesPanel);

        Panels = panels;
        SetCurrentValue(OptionsProperty, options);
    }

    #endregion

    #region Methods

    void OnColorPanelChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ColorPanel.ComponentNormalize):
                Documents.ForEach<ColorDocument>(i => i.Color.Normalize = ColorPanel.ComponentNormalize);
                break;

            case nameof(ColorPanel.ComponentPrecision):
                Documents.ForEach<ColorDocument>(i => i.Color.Precision = ColorPanel.ComponentPrecision);
                break;
        }
    }

    void OnColorSaved(object sender, EventArgs<Color> e)
    {
        ColorsPanel?.SelectedGroup.If(i =>
        {
            e.Value.Convert(out ByteVector4 j);
            i.Add(j);
        });
    }

    void OnColorSelected(object sender, EventArgs<ByteVector4> e)
        => ActiveDocument.If(i => i.Color.ActualColor = Color.FromArgb(e.Value.A, e.Value.R, e.Value.G, e.Value.B));

    void OnDocumentsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems[0] is ColorDocument a)
                {
                    a.ColorSaved += OnColorSaved;
                    a.Color.Normalize = ColorPanel.ComponentNormalize;
                    a.Color.Precision = ColorPanel.ComponentPrecision;
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                e.OldItems[0].As<ColorDocument>().ColorSaved -= OnColorSaved;
                break;
        }
    }

    void OnHarmonySaved(object sender, EventArgs<Color[]> e)
    {
        if (e.Value?.Length > 0)
        {
            var selectedGroup = new GroupItemWithNoValue(Options.Colors, ColorsPanel.SelectedGroupIndex);
            MemberWindow.ShowDialog("Add colors to", selectedGroup, out int result, i => { i.GroupName = MemberGroupName.None; i.HeaderVisibility = Visibility.Collapsed; i.NameColumnVisibility = Visibility.Collapsed; }, Buttons.SaveCancel);
            if (result == 0)
            {
                if (selectedGroup.GroupIndex >= 0)
                {
                    e.Value.ForEach(i =>
                    {
                        i.Convert(out ByteVector4 j);
                        ColorsPanel.Groups[selectedGroup.GroupIndex].Add(j);
                    });
                }
            }
        }
    }

    //...

    void OnLoaded()
    {
        Documents.CollectionChanged += OnDocumentsChanged;
        Documents.ForEach(i => i.As<ColorDocument>().ColorSaved += OnColorSaved);

        ColorPanel.If(i => { i.PropertyChanged -= OnColorPanelChanged; i.PropertyChanged += OnColorPanelChanged; });

        HarmonyPanel.Saved -= OnHarmonySaved; HarmonyPanel.Saved += OnHarmonySaved;
    }

    void OnUnloaded()
    {
        Documents.CollectionChanged -= OnDocumentsChanged;
        Documents.ForEach(i => i.As<ColorDocument>().ColorSaved -= OnColorSaved);

        ColorPanel.If(i => i.PropertyChanged -= OnColorPanelChanged);

        HarmonyPanel.Saved -= OnHarmonySaved;
    }

    //...

    protected virtual void OnActiveDocumentChanged(Value<ColorDocument> input)
    {
        ActiveDocumentChanged?.Invoke(this, new(input.New));
    }

    protected virtual void OnOptionsChanged(IColorControlOptions input)
    {
        if (input != null)
        {
            input.OnLoaded(this);

            ColorsPanel
                ?.Update(input.Colors);
            IlluminantsPanel
                ?.Update(input.Illuminants);
            MatrixPanel
                ?.Update(input.Matrices);
            ProfilesPanel
                ?.Update(input.Profiles);

            AnalysisPanel.Profile = new(input.Profiles, 0, 0);

            DifferencePanel.Profile1 = new(input.Profiles, 0, 0);
            DifferencePanel.Profile2 = new(input.Profiles, 0, 0);

            Documents.ForEach<ColorDocument>(i => i.Color.ProfileGroups = input.Profiles);
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
    public ICommand SaveOldColorCommand => saveOldColorCommand ??= new RelayCommand(() => ColorsPanel?.SelectedGroup.If(i =>
    {
        ActiveDocument.OldColor.Convert(out ByteVector4 j);
        i.Add(j);
    }), 
    () => ActiveDocument != null);

    ICommand saveNewColorCommand;
    public ICommand SaveNewColorCommand => saveNewColorCommand ??= new RelayCommand(() => ColorsPanel?.SelectedGroup.If(i =>
    {
        ActiveDocument.Color.ActualColor.Convert(out ByteVector4 j);
        i.Add(j);
    }), 
    () => ActiveDocument != null);

    #endregion
}