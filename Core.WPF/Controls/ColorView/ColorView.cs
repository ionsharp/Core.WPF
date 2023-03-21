using Imagin.Core.Analytics;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Controls;

public partial class ColorView : Control
{
    public static readonly ResourceKey PanelTemplateSelectorKey = new();

    ///

    #region Properties

    public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register(nameof(ActiveDocument), typeof(ColorDocument), typeof(ColorView), new FrameworkPropertyMetadata(null));
    public ColorDocument ActiveDocument
    {
        get => (ColorDocument)GetValue(ActiveDocumentProperty);
        set => SetValue(ActiveDocumentProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(DockViewModel), typeof(ColorView), new FrameworkPropertyMetadata(null, OnViewModelChanged));
    public DockViewModel ViewModel
    {
        get => (DockViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
    static void OnViewModelChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ColorView>().OnViewModelChanged(e);

    public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(nameof(Options), typeof(ColorViewOptions), typeof(ColorView), new FrameworkPropertyMetadata(null));
    public ColorViewOptions Options
    {
        get => (ColorViewOptions)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(ColorView), new FrameworkPropertyMetadata(null));
    public ICommand SaveCommand
    {
        get => (ICommand)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    #endregion

    #region ColorView

    public ColorView() : base()
    {
        this.RegisterHandler(OnLoaded, OnUnloaded);

        //var defaultOptions = new ColorViewOptions();
        //defaultOptions.Load(out ColorViewOptions existingOptions);
        //SetCurrentValue(OptionsProperty, existingOptions ?? defaultOptions);

        //SetCurrentValue(ViewModelProperty, new DockViewModel(Options));
    }

    #endregion

    #region Methods

    void OnColorPanelChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ColorPanel.ComponentNormalize):
                ViewModel.Documents.ForEach<ColorDocument>(i => i.Normalize = ViewModel.Panels.FirstOrDefault<ColorPanel>().ComponentNormalize);
                break;

            case nameof(ColorPanel.ComponentPrecision):
                ViewModel.Documents.ForEach<ColorDocument>(i => i.Precision = ViewModel.Panels.FirstOrDefault<ColorPanel>().ComponentPrecision);
                break;
        }
    }

    void OnColorSaved(object sender, EventArgs<Color> e)
    {
        ViewModel?.Panels.FirstOrDefault<ColorsPanel>()?.SelectedGroup.If(i =>
        {
            e.Value.Convert(out ByteVector4 j);
            i.Add("", j);
        });
    }

    void OnColorSelected(object sender, EventArgs<ByteVector4> e)
        => ActiveDocument.If(i => i.NewColor = Color.FromArgb(e.Value.A, e.Value.R, e.Value.G, e.Value.B));

    void OnDocumentsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems[0] is ColorDocument a)
                {
                    a.ColorSaved += OnColorSaved;
                    a.Profiles = Options.Profiles;
                    a.Normalize = ViewModel.Panels.FirstOrDefault<ColorPanel>().ComponentNormalize;
                    a.Precision = ViewModel.Panels.FirstOrDefault<ColorPanel>().ComponentPrecision;
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                e.OldItems[0].As<ColorDocument>().ColorSaved -= OnColorSaved;
                break;
        }
    }

    void OnHarmonyColorPicked(object sender, EventArgs<Color> e) => ActiveDocument.If(i => i.NewColor = e.Value);

    void OnHarmonySaved(object sender, EventArgs<Color[]> e)
    {
        if (e.Value?.Length > 0)
        {
            var selectedGroup = new GroupIndexForm(Options.Colors, ViewModel.Panels.FirstOrDefault<ColorsPanel>().SelectedGroupIndex);
            Dialog.ShowObject("Add colors to", selectedGroup, Resource.GetImageUri(SmallImages.Plus), i =>
            {
                if (i == 0)
                {
                    if (selectedGroup.GroupIndex >= 0)
                    {
                        e.Value.ForEach(i =>
                        {
                            i.Convert(out ByteVector4 j);
                            ViewModel.Panels.FirstOrDefault<ColorsPanel>().Groups[selectedGroup.GroupIndex].Add("", j);
                        });
                    }
                }
            }, 
            Buttons.SaveCancel);
        }
    }

    ///

    void OnLoaded()
    {
        if (ViewModel != null)
        {
            ViewModel.Documents.CollectionChanged += OnDocumentsChanged;
            ViewModel.Documents.ForEach(i => i.As<ColorDocument>().ColorSaved += OnColorSaved);

            ViewModel.Panels.FirstOrDefault<ColorPanel>().If(i => { i.PropertyChanged -= OnColorPanelChanged; i.PropertyChanged += OnColorPanelChanged; });
            ViewModel.Panels.FirstOrDefault<ColorHarmonyPanel>().Saved -= OnHarmonySaved; ViewModel.Panels.FirstOrDefault<ColorHarmonyPanel>().Saved += OnHarmonySaved;
        }
    }

    void OnUnloaded()
    {
        if (ViewModel != null)
        {
            ViewModel.Documents.CollectionChanged -= OnDocumentsChanged;
            ViewModel.Documents.ForEach(i => i.As<ColorDocument>().ColorSaved -= OnColorSaved);

            ViewModel.Panels.FirstOrDefault<ColorPanel>().If(i => i.PropertyChanged -= OnColorPanelChanged);
            ViewModel.Panels.FirstOrDefault<ColorHarmonyPanel>().Saved -= OnHarmonySaved;
        }
    }

    ///

    void SaveTo(Color input)
    {
        if (ViewModel != null)
        {
            var item = new GroupValueForm<ByteVector4>(ViewModel.Panels.FirstOrDefault<ColorsPanel>().Groups, input, ViewModel.Panels.FirstOrDefault<ColorsPanel>().SelectedGroupIndex);

            Dialog.ShowObject($"Save color", item, Resource.GetImageUri(SmallImages.Save), i =>
            {
                if (i == 0)
                {
                    if (item.GroupIndex > -1)
                    {
                        var a = (Color)item.Value;
                        a.Convert(out ByteVector4 b);

                        ViewModel.Panels.FirstOrDefault<ColorsPanel>().Groups[item.GroupIndex].Add("", b);
                    }
                }
            },
            Buttons.SaveCancel);
        }
    }

    ///

    IEnumerable<Models.Panel> GetPanels()
    {
        yield return new ColorPanel();
        yield return new ColorsPanel();
        yield return new ColorAnalysisPanel();
        yield return new ColorChromacityPanel();
        yield return new ColorDifferencePanel();
        yield return new ColorHarmonyPanel();
        yield return new ColorIlluminantsPanel();
        yield return new ColorMatricesPanel();
        yield return new ColorProfilesPanel();
    }

    ///

    protected virtual void OnViewModelChanged(ReadOnlyValue<DockViewModel> input)
    {
        input.Old.If(i =>
        {
            i.Panels.FirstOrDefault<ColorPanel>().PropertyChanged -= OnColorPanelChanged;
            i.Panels.FirstOrDefault<ColorsPanel>().Selected -= OnColorSelected;

            i.Panels.FirstOrDefault<ColorHarmonyPanel>().Picked -= OnHarmonyColorPicked;
        });
        input.New.If(i =>
        {
            GetPanels().ForEach(j =>
            {
                i.Panels.Add(j);
                j.ViewModel = input.New;
            });

            var options = (ColorViewOptions)input.New.Options;
            ViewModel.Panels.FirstOrDefault<ColorsPanel>()
                .Groups = options.Colors;
            ViewModel.Panels.FirstOrDefault<ColorIlluminantsPanel>()
                .Groups = options.Illuminants;
            ViewModel.Panels.FirstOrDefault<ColorMatricesPanel>()
                .Groups = options.Matrices;
            ViewModel.Panels.FirstOrDefault<ColorProfilesPanel>()
                .Groups = options.Profiles;

            ViewModel.Panels.FirstOrDefault<ColorAnalysisPanel>()
                .Profiles = options.Profiles;
            ViewModel.Panels.FirstOrDefault<ColorAnalysisPanel>()
                .Profile = new(options.Profiles, 0, 0);

            ViewModel.Panels.FirstOrDefault<ColorDifferencePanel>()
                .Profiles = options.Profiles;
            ViewModel.Panels.FirstOrDefault<ColorDifferencePanel>()
                .Profile1 = new(options.Profiles, 0, 0);
            ViewModel.Panels.FirstOrDefault<ColorDifferencePanel>()
                .Profile2 = new(options.Profiles, 0, 0);

            ViewModel.Documents.ForEach<ColorDocument>(j => j.Profiles = options.Profiles);

            i.Panels.FirstOrDefault<ColorPanel>().PropertyChanged += OnColorPanelChanged;
            i.Panels.FirstOrDefault<ColorsPanel>().Selected += OnColorSelected;

            i.Panels.FirstOrDefault<ColorHarmonyPanel>().Picked += OnHarmonyColorPicked;

            i.Documents.ForEach<ColorDocument>(j => j.Profiles = options.Profiles);
        });
    }

    #endregion

    #region Commands

    ICommand copyColorCommand;
    public ICommand CopyColorCommand => copyColorCommand ??= new RelayCommand<Color>(i => Copy.Set(i), i => i != null);

    ICommand copyHexadecimalCommand;
    public ICommand CopyHexadecimalCommand => copyHexadecimalCommand ??= new RelayCommand<Color>(i =>
    {
        i.Convert(out ByteVector4 result);
        Clipboard.SetText(result.ToString(false));
    }, 
    i => i != null);

    ICommand pasteOldColorCommand;
    public ICommand PasteOldColorCommand => pasteOldColorCommand ??= new RelayCommand(() => ActiveDocument.OldColor = Copy.Get<Color>(), () => ActiveDocument != null && Copy.Contains(typeof(Color)));

    ICommand pasteNewColorCommand;
    public ICommand PasteNewColorCommand => pasteNewColorCommand ??= new RelayCommand(() => ActiveDocument.NewColor = Copy.Get<Color>(), () => ActiveDocument != null && Copy.Contains(typeof(Color)));

    ICommand saveOldColorCommand;
    public ICommand SaveOldColorCommand => saveOldColorCommand ??= new RelayCommand(() => ViewModel.Panels.FirstOrDefault<ColorsPanel>()?.SelectedGroup.If(i =>
    {
        ActiveDocument.OldColor.Convert(out ByteVector4 j);
        i.Add("", j);
    }),
    () => ActiveDocument != null);

    ICommand saveOldColorToCommand;
    public ICommand SaveOldColorToCommand 
        => saveOldColorToCommand ??= new RelayCommand(() => ViewModel.Panels.FirstOrDefault<ColorsPanel>()?.SelectedGroup.If(i => SaveTo(ActiveDocument.OldColor)), () => ActiveDocument != null);

    ICommand saveNewColorCommand;
    public ICommand SaveNewColorCommand => saveNewColorCommand ??= new RelayCommand(() => ViewModel.Panels.FirstOrDefault<ColorsPanel>()?.SelectedGroup.If(i =>
    {
        ActiveDocument.NewColor.Convert(out ByteVector4 j);
        i.Add("", j);
    }),
    () => ActiveDocument != null);

    ICommand saveNewColorToCommand;
    public ICommand SaveNewColorToCommand
        => saveNewColorToCommand ??= new RelayCommand(() => ViewModel.Panels.FirstOrDefault<ColorsPanel>()?.SelectedGroup.If(i => SaveTo(ActiveDocument.NewColor)), () => ActiveDocument != null);

    #endregion
}