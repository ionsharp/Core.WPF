using Imagin.Core.Controls;
using Imagin.Core.Reflection;
using System.ComponentModel;
using System.Windows;

namespace Imagin.Core.Linq;

public static class XMemberControl
{
    #region (readonly) ActualSource

    static readonly DependencyPropertyKey ActualSourceKey = DependencyProperty.RegisterAttachedReadOnly("ActualSource", typeof(object), typeof(XMemberControl), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ActualSourceProperty = ActualSourceKey.DependencyProperty;
    public static object GetActualSource(IMemberControl i) => (object)i.GetValue(ActualSourceProperty);

    #endregion

    #region GroupDirection

    public static readonly DependencyProperty GroupDirectionProperty = DependencyProperty.RegisterAttached("GroupDirection", typeof(ListSortDirection), typeof(XMemberControl), new FrameworkPropertyMetadata(ListSortDirection.Ascending, OnGroupDirectionChanged));
    public static ListSortDirection GetGroupDirection(IMemberControl i) => (ListSortDirection)i.GetValue(GroupDirectionProperty);
    public static void SetGroupDirection(IMemberControl i, ListSortDirection input) => i.SetValue(GroupDirectionProperty, input);
    static void OnGroupDirectionChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => GetMembers(i as IMemberControl).ApplyGroup(GetGroupName(i as IMemberControl));

    #endregion

    #region GroupName

    public static readonly DependencyProperty GroupNameProperty = DependencyProperty.RegisterAttached("GroupName", typeof(MemberGroupName), typeof(XMemberControl), new FrameworkPropertyMetadata(MemberGroupName.Category, OnGroupNameChanged));
    public static MemberGroupName GetGroupName(IMemberControl i) => (MemberGroupName)i.GetValue(GroupNameProperty);
    public static void SetGroupName(IMemberControl i, MemberGroupName input) => i.SetValue(GroupNameProperty, input);
    static void OnGroupNameChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => GetMembers(i as IMemberControl).ApplyGroup(GetGroupName(i as IMemberControl));

    #endregion

    #region (ReadOnly) Members

    static readonly DependencyPropertyKey MembersKey = DependencyProperty.RegisterAttachedReadOnly("Members", typeof(MemberCollection), typeof(XMemberControl), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty MembersProperty = MembersKey.DependencyProperty;
    public static MemberCollection GetMembers(IMemberControl i) => (MemberCollection)i.GetValue(MembersProperty);

    #endregion

    #region SortDirection

    public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.RegisterAttached("SortDirection", typeof(ListSortDirection), typeof(XMemberControl), new FrameworkPropertyMetadata(ListSortDirection.Ascending, OnSortDirectionChanged));
    public static ListSortDirection GetSortDirection(IMemberControl i) => (ListSortDirection)i.GetValue(SortDirectionProperty);
    public static void SetSortDirection(IMemberControl i, ListSortDirection input) => i.SetValue(SortDirectionProperty, input);
    static void OnSortDirectionChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => GetMembers(i as IMemberControl).ApplySort();

    #endregion

    #region SortName

    public static readonly DependencyProperty SortNameProperty = DependencyProperty.RegisterAttached("SortName", typeof(MemberSortName), typeof(XMemberControl), new FrameworkPropertyMetadata(MemberSortName.DisplayName, OnSortNameChanged));
    public static MemberSortName GetSortName(IMemberControl i) => (MemberSortName)i.GetValue(SortNameProperty);
    public static void SetSortName(IMemberControl i, MemberSortName input) => i.SetValue(SortNameProperty, input);
    static void OnSortNameChanged(DependencyObject i, DependencyPropertyChangedEventArgs e) => GetMembers(i as IMemberControl).ApplySort();

    #endregion

    #region Source

    public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(object), typeof(XMemberControl), new FrameworkPropertyMetadata(null, OnSourceChanged));
    public static object GetSource(IMemberControl i) => (object)i.GetValue(SourceProperty);
    public static void SetSource(IMemberControl i, object input) => i.SetValue(SourceProperty, input);
    static void OnSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e)
    {
        if (i is IMemberControl control)
        {
            if (GetMembers(control) == null)
                control.SetValue(MembersKey, new MemberCollection(GetGroupName(control), new MemberSortComparer(control)));

            var members = GetMembers(control);

            if (e.NewValue is MemberSourceFilter filter)
            {
                control.SetValue(ActualSourceKey, filter.Source);
                members.Load(filter.Source, filter);
            }
            else
            {
                control.SetValue(ActualSourceKey, e.NewValue);
                members.Load(e.NewValue);
            }
        }
    }

    #endregion
}