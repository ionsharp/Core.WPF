using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Conversion;
using Imagin.Core.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Imagin.Core.Behavior;

public class ManualBindingBehavior : Behavior<DependencyObject>
{
    public DependencyProperty DataProperty { get; set; }

    static readonly DependencyPropertyKey ActualConverterKey = DependencyProperty.RegisterReadOnly(nameof(ActualConverter), typeof(IValueConverter), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public static readonly DependencyProperty ActualConverterProperty = ActualConverterKey.DependencyProperty;
    public IValueConverter ActualConverter
    {
        get => (IValueConverter)GetValue(ActualConverterProperty);
        private set => SetValue(ActualConverterKey, value);
    }

    public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register(nameof(Converter), typeof(Type), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnConverterChanged));
    public Type Converter
    {
        get => (Type)GetValue(ConverterProperty);
        set => SetValue(ConverterProperty, value);
    }
    static void OnConverterChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ManualBindingBehavior>().OnConverterChanged(e);

    public static readonly DependencyProperty ConverterParameterProperty = DependencyProperty.Register(nameof(ConverterParameter), typeof(object), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public object ConverterParameter
    {
        get => GetValue(ConverterParameterProperty);
        set => SetValue(ConverterParameterProperty, value);
    }

    public static readonly DependencyProperty ConverterSelectorProperty = DependencyProperty.Register(nameof(ConverterSelector), typeof(ConverterSelector), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public ConverterSelector ConverterSelector
    {
        get => (ConverterSelector)GetValue(ConverterSelectorProperty);
        set => SetValue(ConverterSelectorProperty, value);
    }

    public static readonly DependencyProperty ConverterSelectorKeyProperty = DependencyProperty.Register(nameof(ConverterSelectorKey), typeof(object), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnConverterSelectorKeyChanged));
    public object ConverterSelectorKey
    {
        get => (object)GetValue(ConverterSelectorKeyProperty);
        set => SetValue(ConverterSelectorKeyProperty, value);
    }
    static void OnConverterSelectorKeyChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ManualBindingBehavior>().OnConverterSelectorKeyChanged(e);

    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(BindingMode), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(BindingMode.OneWay, OnPropertyChanged));
    public BindingMode Mode
    {
        get => (BindingMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public static readonly DependencyProperty NotifyOnValidationErrorProperty = DependencyProperty.Register(nameof(NotifyOnValidationError), typeof(bool), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(false, OnPropertyChanged));
    public bool NotifyOnValidationError
    {
        get => (bool)GetValue(NotifyOnValidationErrorProperty);
        set => SetValue(NotifyOnValidationErrorProperty, value);
    }

    public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(PropertyPath), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public PropertyPath Path
    {
        get => (PropertyPath)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(nameof(Property), typeof(DependencyProperty), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public DependencyProperty Property
    {
        get => (DependencyProperty)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public object Source
    {
        get => (object)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(nameof(StringFormat), typeof(string), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public string StringFormat
    {
        get => (string)GetValue(StringFormatProperty);
        set => SetValue(StringFormatProperty, value);
    }

    public static readonly DependencyProperty UpdateSourceTriggerProperty = DependencyProperty.Register(nameof(UpdateSourceTrigger), typeof(UpdateSourceTrigger), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(UpdateSourceTrigger.Default, OnPropertyChanged));
    public UpdateSourceTrigger UpdateSourceTrigger
    {
        get => (UpdateSourceTrigger)GetValue(UpdateSourceTriggerProperty);
        set => SetValue(UpdateSourceTriggerProperty, value);
    }

    public static readonly DependencyProperty ValidatesOnNotifyDataErrorsProperty = DependencyProperty.Register(nameof(ValidatesOnNotifyDataErrors), typeof(bool), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(false, OnPropertyChanged));
    public bool ValidatesOnNotifyDataErrors
    {
        get => (bool)GetValue(ValidatesOnNotifyDataErrorsProperty);
        set => SetValue(ValidatesOnNotifyDataErrorsProperty, value);
    }

    public static readonly DependencyProperty ValidatesOnDataErrorsProperty = DependencyProperty.Register(nameof(ValidatesOnDataErrors), typeof(bool), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(false, OnPropertyChanged));
    public bool ValidatesOnDataErrors
    {
        get => (bool)GetValue(ValidatesOnDataErrorsProperty);
        set => SetValue(ValidatesOnDataErrorsProperty, value);
    }

    public static readonly DependencyProperty ValidationRulesProperty = DependencyProperty.Register(nameof(ValidationRules), typeof(ObservableCollection<ValidationRule>), typeof(ManualBindingBehavior), new FrameworkPropertyMetadata(null, OnPropertyChanged));
    public ObservableCollection<ValidationRule> ValidationRules
    {
        get => (ObservableCollection<ValidationRule>)GetValue(ValidationRulesProperty);
        set => SetValue(ValidationRulesProperty, value);
    }

    static void OnPropertyChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<ManualBindingBehavior>().OnPropertyChanged();

    public ManualBindingBehavior() : base() => SetCurrentValue(ValidationRulesProperty, new ObservableCollection<ValidationRule>());

    protected override void OnAttached() => OnPropertyChanged();
    
    protected virtual void OnConverterChanged(ReadOnlyValue<object> input)
    {
        ActualConverter = Conversion.Converter.Instances[Converter] as IValueConverter;
    }

    protected virtual void OnConverterSelectorKeyChanged(ReadOnlyValue<object> input)
    {
        ActualConverter = ConverterSelector?.Select(input.New);
    }

    protected virtual void OnPropertyChanged()
    {
        if (AssociatedObject != null)
        {
            var result = new Binding()
            {
                Converter
                    = ActualConverter,
                ConverterParameter
                    = ConverterParameter,
                Mode
                    = Mode,
                NotifyOnValidationError
                    = NotifyOnValidationError,
                Path
                    = Path,
                Source
                    = Source,
                StringFormat
                    = StringFormat,
                UpdateSourceTrigger
                    = UpdateSourceTrigger,
                ValidatesOnNotifyDataErrors
                    = ValidatesOnNotifyDataErrors,
                ValidatesOnDataErrors
                    = ValidatesOnDataErrors
            };
            ValidationRules.ForEach(i => result.ValidationRules.Add(i));

            AssociatedObject.Unbind(Property);
            AssociatedObject.Bind(Property, result);
        }
    }
}