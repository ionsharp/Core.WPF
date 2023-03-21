using Imagin.Core.Linq;
using Imagin.Core.Reflection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Imagin.Core.Controls;

public class MemberValuePresenter : ContentPresenter
{
    DispatcherTimer timer;

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(MemberValuePresenter), new FrameworkPropertyMetadata(Orientation.Vertical));
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty MemberStyleProperty = DependencyProperty.Register("MemberStyle", typeof(Enum), typeof(MemberValuePresenter), new FrameworkPropertyMetadata(null, OnMemberStyleChanged));
    static void OnMemberStyleChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => i.As<MemberValuePresenter>().OnMemberStyleChanged(e);
    protected virtual void OnMemberStyleChanged(ReadOnlyValue<Enum> input)
    {
        if (IsLoaded)
            UpdateTimer();
    }

    public MemberValuePresenter() : base()
    {        
        this.RegisterHandler(OnLoaded, OnUnloaded);
        this.Bind(MemberStyleProperty, $"{nameof(Content)}.{nameof(MemberModel.Style)}", this);
    }

    void OnTick(object sender, EventArgs e)
    {
        Content.If<MemberModel>(i =>
        {
            if (i.Properties[MemberModel.PropertyNames.IsEditingText] is bool isEditing && !isEditing)
                i.Update(() => i.Value);
        });
    }

    void InsertTimer()
    {
        timer = new();
        timer.Interval = TimeSpan.FromSeconds(GetValue(MemberStyleProperty).GetAttribute<UpdateAttribute>().Seconds);
        timer.Tick += OnTick;
        timer.Start();
    }

    void RemoveTimer()
    {
        if (timer != null)
        {
            timer.Stop();
            timer.Tick -= OnTick;
            timer = null;
        }
    }

    void UpdateTimer()
    {
        if (GetValue(MemberStyleProperty)?.HasAttribute<UpdateAttribute>() == true)
        {
            InsertTimer();
        }
        else RemoveTimer();
    }

    void OnLoaded()
    {
        UpdateTimer();
    }

    void OnUnloaded()
    {
        RemoveTimer();
    }
}