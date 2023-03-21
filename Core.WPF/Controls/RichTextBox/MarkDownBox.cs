using System.Windows;
using System.Windows.Controls;
using Imagin.Core.Text;

namespace Imagin.Core.Controls;

public class MarkDownBox : TextBox
{
    public static readonly DependencyProperty BulletOrderedProperty = DependencyProperty.Register(nameof(BulletOrdered), typeof(Bullets), typeof(MarkDownBox), new FrameworkPropertyMetadata(Bullets.NumberPeriod));
    public Bullets BulletOrdered
    {
        get => (Bullets)GetValue(BulletOrderedProperty);
        set => SetValue(BulletOrderedProperty, value);
    }

    public static readonly DependencyProperty BulletUnorderedProperty = DependencyProperty.Register(nameof(BulletUnordered), typeof(Bullets), typeof(MarkDownBox), new FrameworkPropertyMetadata(Bullets.Square));
    public Bullets BulletUnordered
    {
        get => (Bullets)GetValue(BulletUnorderedProperty);
        set => SetValue(BulletUnorderedProperty, value);
    }

    public static readonly DependencyProperty FontScaleProperty = DependencyProperty.Register(nameof(FontScale), typeof(double), typeof(MarkDownBox), new FrameworkPropertyMetadata(1.0));
    public double FontScale
    {
        get => (double)GetValue(FontScaleProperty);
        set => SetValue(FontScaleProperty, value);
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(MarkDownBox), new FrameworkPropertyMetadata(null));
    public object Source
    {
        get => (object)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly DependencyProperty TextWrapProperty = DependencyProperty.Register(nameof(TextWrap), typeof(TextWrapping), typeof(MarkDownBox), new FrameworkPropertyMetadata(TextWrapping.Wrap));
    public TextWrapping TextWrap
    {
        get => (TextWrapping)GetValue(TextWrapProperty);
        set => SetValue(TextWrapProperty, value);
    }

    public MarkDownBox() : base() { }
}