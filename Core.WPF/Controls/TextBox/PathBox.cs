using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Imagin.Core.Controls;

public class PathBox : TextBox
{
    #region Properties

    readonly PathBoxDropHandler DropHandler;

    public static readonly IValidate DefaultValidator = new LocalValidator();

    ///

    IValidate validator => ValidateHandler ?? DefaultValidator;

    ///

    public static readonly DependencyProperty BrowseButtonTemplateProperty = DependencyProperty.Register(nameof(BrowseButtonTemplate), typeof(DataTemplate), typeof(PathBox), new FrameworkPropertyMetadata(default(DataTemplate)));
    public DataTemplate BrowseButtonTemplate
    {
        get => (DataTemplate)GetValue(BrowseButtonTemplateProperty);
        set => SetValue(BrowseButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty BrowseButtonToolTipProperty = DependencyProperty.Register(nameof(BrowseButtonToolTip), typeof(string), typeof(PathBox), new FrameworkPropertyMetadata(default(string)));
    public string BrowseButtonToolTip
    {
        get => (string)GetValue(BrowseButtonToolTipProperty);
        set => SetValue(BrowseButtonToolTipProperty, value);
    }

    public static readonly DependencyProperty BrowseButtonVisibilityProperty = DependencyProperty.Register(nameof(BrowseButtonVisibility), typeof(bool), typeof(PathBox), new FrameworkPropertyMetadata(true));
    public bool BrowseButtonVisibility
    {
        get => (bool)GetValue(BrowseButtonVisibilityProperty);
        set => SetValue(BrowseButtonVisibilityProperty, value);
    }

    public static readonly DependencyProperty BrowseFileExtensionsProperty = DependencyProperty.Register(nameof(BrowseFileExtensions), typeof(Extensions), typeof(PathBox), new FrameworkPropertyMetadata(Extensions.Empty));
    [TypeConverter(typeof(ExtensionsTypeConverter))]
    public Extensions BrowseFileExtensions
    {
        get => (Extensions)GetValue(BrowseFileExtensionsProperty);
        set => SetValue(BrowseFileExtensionsProperty, value);
    }

    public static readonly DependencyProperty BrowseModeProperty = DependencyProperty.Register(nameof(BrowseMode), typeof(StorageDialogMode), typeof(PathBox), new FrameworkPropertyMetadata(StorageDialogMode.OpenFolder, OnBrowseModeChanged, OnBrowseModeCoerced));
    public StorageDialogMode BrowseMode
    {
        get => (StorageDialogMode)GetValue(BrowseModeProperty);
        set => SetValue(BrowseModeProperty, value);
    }
    static void OnBrowseModeChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<PathBox>().OnBrowseModeChanged(e.Convert<StorageDialogMode>());
    static object OnBrowseModeCoerced(DependencyObject i, object input) => input is StorageDialogMode mode && mode != StorageDialogMode.SaveFile ? input : throw new NotSupportedException();

    public static readonly DependencyProperty BrowseTitleProperty = DependencyProperty.Register(nameof(BrowseTitle), typeof(string), typeof(PathBox), new FrameworkPropertyMetadata(default(string)));
    public string BrowseTitle
    {
        get => (string)GetValue(BrowseTitleProperty);
        set => SetValue(BrowseTitleProperty, value);
    }

    public static readonly DependencyProperty CanBrowseProperty = DependencyProperty.Register(nameof(CanBrowse), typeof(bool), typeof(PathBox), new FrameworkPropertyMetadata(true));
    public bool CanBrowse
    {
        get => (bool)GetValue(CanBrowseProperty);
        set => SetValue(CanBrowseProperty, value);
    }

    public static readonly DependencyProperty CanValidateProperty = DependencyProperty.Register(nameof(CanValidate), typeof(bool), typeof(PathBox), new FrameworkPropertyMetadata(true, OnCanValidateChanged));
    public bool CanValidate
    {
        get => (bool)GetValue(CanValidateProperty);
        set => SetValue(CanValidateProperty, value);
    }
    static void OnCanValidateChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<PathBox>().OnCanValidateChanged(e.Convert<bool>());

    public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(PathBox), new FrameworkPropertyMetadata(Visibility.Visible));
    public Visibility IconVisibility
    {
        get => (Visibility)GetValue(IconVisibilityProperty);
        set => SetValue(IconVisibilityProperty, value);
    }

    public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(PathBox), new FrameworkPropertyMetadata(false));
    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        private set => SetValue(IsValidProperty, value);
    }

    public static readonly DependencyProperty ValidateHandlerProperty = DependencyProperty.Register(nameof(ValidateHandler), typeof(IValidate), typeof(PathBox), new FrameworkPropertyMetadata(null, OnValidateHandlerChanged));
    public IValidate ValidateHandler
    {
        get => (IValidate)GetValue(ValidateHandlerProperty);
        set => SetValue(ValidateHandlerProperty, value);
    }
    static void OnValidateHandlerChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<PathBox>().OnValidateHandlerChanged(e.Convert<IValidate>());

    public static readonly DependencyProperty ValidateTemplateProperty = DependencyProperty.Register(nameof(ValidateTemplate), typeof(DataTemplate), typeof(PathBox), new FrameworkPropertyMetadata(default(DataTemplate)));
    public DataTemplate ValidateTemplate
    {
        get => (DataTemplate)GetValue(ValidateTemplateProperty);
        set => SetValue(ValidateTemplateProperty, value);
    }

    public static readonly DependencyProperty ValidateToolTipProperty = DependencyProperty.Register(nameof(ValidateToolTip), typeof(string), typeof(PathBox), new FrameworkPropertyMetadata(default(string)));
    public string ValidateToolTip
    {
        get => (string)GetValue(ValidateToolTipProperty);
        set => SetValue(ValidateToolTipProperty, value);
    }

    #endregion

    #region PathBox

    public PathBox() : base()
    {
        this.RegisterHandler(OnLoaded, OnUnloaded);

        DropHandler = new(this);
        GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(this, DropHandler);
    }

    #endregion

    #region Methods

    void OnDriveInserted(RemovableDriveEventArgs e) => Validate();

    void OnDriveRemoved(RemovableDriveEventArgs e) => Validate();

    void OnLoaded()
    {
        RemovableDrive.Inserted -= OnDriveInserted; RemovableDrive.Removed -= OnDriveRemoved;
        RemovableDrive.Inserted += OnDriveInserted; RemovableDrive.Removed += OnDriveRemoved;
    }

    void OnUnloaded()
    {
        RemovableDrive.Inserted -= OnDriveInserted; RemovableDrive.Removed -= OnDriveRemoved;
    }

    void Validate() => SetCurrentValue(IsValidProperty, CanValidate ? validator.Validate(BrowseMode.Convert(), Text) : false);

    ///

    protected virtual void OnCanValidateChanged(ReadOnlyValue<bool> input) => Validate();
        
    protected virtual void OnBrowseModeChanged(ReadOnlyValue<StorageDialogMode> input) => Validate();

    protected virtual void OnValidateHandlerChanged(ReadOnlyValue<IValidate> input) => Validate();

    ///

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);
        Validate();
    }

    ///

    public void Browse()
    {
        Focus();
        if (StorageDialog.Show(out string path, BrowseTitle, BrowseMode, BrowseFileExtensions.Values, Text))
            SetCurrentValue(TextProperty, path);

        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }

    ICommand browseCommand;
    public ICommand BrowseCommand => browseCommand ??= new RelayCommand(Browse, () => CanBrowse);

    #endregion
}