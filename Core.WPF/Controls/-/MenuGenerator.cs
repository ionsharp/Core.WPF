using Imagin.Core.Conversion;
using Imagin.Core.Data;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Markup;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Core.Controls;

public class MenuGenerator
{
    #region Constants

    public const string DefaultCategory = "General";

    ///

    static readonly Func<MenuItemAttribute, string> GetCategory = new(i =>
    {
        var j = i.Category;
        if (j is Enum k)
            return k.GetDisplayName() ?? $"{k}";

        return j?.ToString() ?? DefaultCategory;
    });

    static readonly Func<MenuItemAttribute, int> GetCategoryIndex = new(i =>
    {
        if (i.CategoryIndex is int j)
            return j;

        return 0;
    });

    static readonly Func<MenuItemAttribute, int> GetSubCategory = new(i =>
    {
        if (i.SubCategory is int j)
            return j;

        return 0;
    });

    static readonly Func<MenuItemAttribute, object, string> GetHeader = new((i, j) => i.Header ?? (j is MemberInfo k ? k.Name : j?.ToString()));

    static readonly Func<MenuItemAttribute, int> GetIndex = new(i => i.Index);

    #endregion

    #region Properties

    public readonly MenuBase Control;

    #endregion

    #region MenuGenerator

    public MenuGenerator(MenuBase control) : base() => Control = control;

    #endregion

    #region Methods

    #region Private

    /// Format

    void FormatItem(SetterBaseCollection setters, object source, MenuItemCollectionAttribute attribute)
    {
        /*
        <Setter Property="Linq:XToolTip.Header" Value="{Data:NameAttributeBinding}"/>
        <Setter Property="Linq:XToolTip.HeaderIcon" Value="{Data:ImageAttributeBinding Type=Image}"/>
        */

        if (attribute.ItemCheckable)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.IsCheckableProperty,
                Value = true
            });
            setters.Add(new Setter()
            {
                Property = MenuItem.IsCheckedProperty,
                Value = new Binding() { Path = new(attribute.ItemCheckablePath), Mode = (BindingMode)attribute.ItemCheckableMode }
            });
        }

        //Command
        if (attribute.ItemCommandName != null)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.CommandProperty,
                Value = source.GetPropertyValue(attribute.ItemCommandName)
            });
        }

        if (attribute.ItemCommandParameterPath != null)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.CommandParameterProperty,
                Value = new Binding(attribute.ItemCommandParameterPath)
            });
        }

        //Header
        if (attribute.ItemHeaderBinding != null)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.HeaderProperty,
                Value = attribute.ItemHeaderBinding.Create<object>()
            });
        }
        else if (attribute.ItemHeaderPath != null)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.HeaderProperty,
                Value = attribute.ItemHeaderConverter.Implements<IValueConverter>() ? new Bind(attribute.ItemHeaderPath) { Convert = attribute.ItemHeaderConverter } : new LocalBinding(attribute.ItemHeaderPath)
            });
        }

        //Icon
        if (attribute.ItemIcon is SmallImages image)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.IconProperty,
                Value = Resource.GetImageUri($"{image}.png", AssemblyType.Core).GetImage()
            });
        }
        else if (attribute.ItemIconPath != null)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.IconProperty,
                Value = new Binding(attribute.ItemIconPath)
            });
            if (attribute.ItemIconTemplateSource != null)
            {
                setters.Add(new Setter()
                {
                    Property = XMenuItem.IconTemplateProperty,
                    Value = new DynamicResourceExtension(attribute.ItemIconTemplateSource.GetField(attribute.ItemIconTemplateKey).GetValue(null))
                });
            }
        }

        //InputGestureText
        if (attribute.ItemInputGestureTextPath != null)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.InputGestureTextProperty,
                Value = new Bind(attribute.ItemInputGestureTextPath) { Convert = attribute.ItemInputGestureTextConverter }
            });
            if (attribute.ItemInputGestureTextTemplateSource != null)
            {
                setters.Add(new Setter()
                {
                    Property = XMenuItem.InputGestureTextTemplateProperty,
                    Value = new DynamicResourceExtension(attribute.ItemInputGestureTextTemplateSource.GetField(attribute.ItemInputGestureTextTemplateKey).GetValue(null))
                });
            }
        }

        //ToolTip
        if (attribute.ItemToolTipPath != null)
        {
            setters.Add(new Setter()
            {
                Property = MenuItem.ToolTipProperty,
                Value = new Bind(attribute.ItemToolTipPath)
            });
            if (attribute.ItemToolTipTemplateSource != null)
            {
                setters.Add(new Setter()
                {
                    Property = XElement.ToolTipTemplateProperty,
                    Value = new DynamicResourceExtension(attribute.ItemToolTipTemplateSource.GetField(attribute.ItemToolTipTemplateKey).GetValue(null))
                });
            }
        }
    }

    void FormatTemplate(SetterBaseCollection setters, DependencyProperty contentProperty, DependencyProperty contentTemplateProperty, string path, Type templateSource, string templateKey)
    {
        if (path != null)
        {
            setters.Add(new Setter()
            {
                Property = contentProperty,
                Value = new Binding(path)
            });
            if (templateSource != null)
            {
                setters.Add(new Setter()
                {
                    Property = contentTemplateProperty,
                    Value = new DynamicResourceExtension(templateSource.GetField(templateKey).GetValue(null))
                });
            }
        }
    }

    /// Item

    DataTrigger GetTrigger(object source, MenuItemCollectionAttribute attribute)
    {
        var result = new DataTrigger() { Binding = new Bind() { Convert = typeof(IsConverter), ConverterParameter = attribute.ItemType }, Value = true };
        FormatItem(result.Setters, source, attribute);
        return result;
    }

    MenuItem GetCollection(object source, object member, MenuItemCollectionAttribute attribute, IEnumerable collection, CompositeCollection composite, Style style)
    {
        if (attribute.ItemType != null && (collection is ICollection || collection is ListCollectionView))
        {
            if (attribute.IsInline)
            {
                style.Triggers.Add(GetTrigger(source, attribute));

                var newCollection = collection is ListCollectionView ? (ListCollectionView)collection : new ListCollectionView((IList)collection);

                newCollection.GroupDescriptions.Clear();
                newCollection.SortDescriptions.Clear();
                
                if (attribute.GroupName != null)
                {
                    newCollection.GroupDescriptions.Add(new PropertyGroupDescription(attribute.GroupName));
                    newCollection.SortDescriptions.Add(new System.ComponentModel.SortDescription(attribute.GroupName, attribute.GroupDirection));
                }

                if (attribute.SortName != null)
                {
                    newCollection.SortDescriptions.Add(new System.ComponentModel.SortDescription(attribute.SortName, attribute.SortDirection));
                }

                newCollection.Refresh();

                composite.Add(GetNothing(newCollection));
                composite.Add(new CollectionContainer() { Collection = newCollection });
            }
            else
            {
                var itemSource = new CompositeCollection() { GetNothing(collection), new CollectionContainer() { Collection = collection } };

                var itemStyle = GetDefaultStyle();
                itemStyle.Triggers.Add(GetTrigger(source, attribute));

                var result = GetItem(source, member, attribute);
                result.ItemsSource = collection;

                if (attribute.SortSource != null)
                {
                    var sortSource = Try.Return(() => source.GetPropertyValue(attribute.SortSource));

                    result.Bind(XItemsControl.SortDirectionProperty,
                        attribute.SortDirectionPath,
                        sortSource,
                        BindingMode.OneWay);
                    result.Bind(XItemsControl.SortNameProperty,
                        attribute.SortNamePath,
                        sortSource,
                        BindingMode.OneWay);
                }

                void group()
                {
                    result.SetResourceReference(XItemsControl.GroupStyleProperty, XItemsControl.MenuGroupStyleKey);

                    XItemsControl.SetGroupContainerStyle(result, itemStyle);
                    XItemsControl.SetGroupsItself(result, collection is ListCollectionView);
                }

                if (attribute.GroupSource != null)
                {
                    var groupSource = Try.Return(() => source.GetPropertyValue(attribute.GroupSource));

                    result.Bind(XItemsControl.GroupDirectionProperty,
                        attribute.GroupDirectionPath,
                        groupSource,
                        BindingMode.OneWay);
                    result.Bind(XItemsControl.GroupNameProperty,
                        attribute.GroupNamePath,
                        groupSource,
                        BindingMode.OneWay);

                    group();
                }
                else if (collection is ListCollectionView view)
                {
                    group();
                }
                else result.ItemContainerStyle = itemStyle;
                return result;
            }
        }
        return null;
    }

    MenuItem GetItem(object source, object member, MenuItemAttribute attribute) => new MenuItem() { Style = GetStyle(source, member, attribute) };

    MenuItem GetItem(object source, object member, MenuItemAttribute attribute, Type type)
    {
        var result = GetItem(source, member, attribute);

        var styleResource = GetDefaultStyle();

        var composite = new CompositeCollection();

        Func<MemberInfo, bool> a = new(i => i.GetAttribute<MenuItemAttribute>() is MenuItemAttribute j && j.Parent?.Equals(member) == true);

        Dictionary<MenuItemAttribute, object> allMembers = new();

        var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field | MemberTypes.Method | MemberTypes.Property, false)
            .Where(i => i.DeclaringType == type && a(i));

        members.ForEach(i => allMembers.Add(i.GetAttribute<MenuItemAttribute>(), i));

        if (member != null)
        {
            var subLevelMenuType = type.GetAttribute<MenuAttribute>()?.Types
                .FirstOrDefault(i => i.GetAttribute<MenuAttribute>()?.Parent?.Equals(member) == true);

            subLevelMenuType?.GetEnumValues().ForEach<Enum>(i => allMembers.Add(i.GetAttribute<MenuItemAttribute>(), i));
        }

        var categories = allMembers.Select(i => i.Key)
            .OrderBy(i => i.Category == null ? 0 : 1)
            .ThenBy(GetCategoryIndex).ThenBy(GetCategory).ThenBy(GetSubCategory)
            .ThenBy(GetIndex).ThenBy(i => GetHeader(i, allMembers[i]))
            .GroupBy(GetCategory);

        var categoryIndex = 0;
        foreach (var category in categories)
        {
            if (category.Key != DefaultCategory)
            {
                if (categories.Count() > 1)
                {
                    var separator = new Separator();
                    XSeparator.SetHeader(separator, category.Key);
                    composite.Add(separator);
                }
            }

            var subCategories = new List<string>();
            category.ForEach(x =>
            {
                var y = $"{x.SubCategory}";
                if (!subCategories.Contains(y))
                    subCategories.Add(y);
            });

            var index = 0;
            subCategories.ForEach(subCategory =>
            {
                if (index > 0)
                    composite.Add(new Separator());

                var subCategoryMembers = category.Where(i => $"{i.SubCategory}".Equals(subCategory));
                subCategoryMembers.ForEach(attribute =>
                {
                    if (allMembers[attribute] is MemberInfo member)
                    {
                        var value = source.GetMemberValue(member);

                        MenuItem item = null;

                        if (attribute is MenuItemCollectionAttribute a)
                        {
                            item = GetCollection(source, member, a, value as IEnumerable, composite, styleResource);
                        }

                        else if (attribute is MenuItemAttribute b)
                        {
                            if (source.GetMemberValue(member) is ICommand command)
                            {
                                if (attribute.CanSlide)
                                {
                                    /*
                                    item = new SliderMenuItem();
                                    item.Bind(SliderMenuItem.ValueProperty, attribute.SlidePath, source, BindingMode.TwoWay);

                                    var e = attribute.SlideMinimum;
                                    while (e <= attribute.SlideMaximum)
                                    {
                                        var f = new MenuItem() { Header = attribute.SlideHeader.F(e), Style = GetDefaultStyle() };
                                        SliderMenuItem.SetValue(f, e);
                                        SliderMenuItem.SetSteps(f, attribute.SlideStep);
                                        item.Items.Add(f);
                                        e += attribute.SlideMaximum / attribute.SlideCut;
                                    }
                                    */
                                    item = GetItem(source, member, b);
                                    item.HorizontalContentAlignment = HorizontalAlignment.Stretch;

                                    var stack = new StackPanel();

                                    var slider = new Slider() { Maximum = attribute.SlideMaximum, Minimum = attribute.SlideMinimum, TickFrequency = attribute.SlideTick };
                                    slider.Bind(Slider.ValueProperty, attribute.SlidePath, source, BindingMode.TwoWay);

                                    stack.Children.Add(new TextBlock() { Text = attribute.Header }); stack.Children.Add(slider);
                                    item.Header = stack;
                                }
                                else
                                {
                                    item = GetItem(source, member, b);
                                    item.Command = command;

                                    if (attribute.HideIfDisabled)
                                        item.Bind(UIElement.VisibilityProperty, nameof(FrameworkElement.IsEnabled), item, BindingMode.OneWay, new System.Windows.Controls.BooleanToVisibilityConverter());
                                }
                            }
                        }

                        if (item != null)
                            composite.Add(item);
                    }
                    else
                    {
                        composite.Add(GetItem(source, allMembers[attribute], attribute, type));
                    }
                });

                index++;
            });

            categoryIndex++;
        }


        result.ItemsSource = composite;
        result.Resources.Add(typeof(MenuItem), styleResource);
        return result;
    }

    MenuItem GetNothing(IEnumerable collection)
    {
        var result = new MenuItem() { FontStyle = FontStyles.Italic, Header = "None" };
        result.Bind(MenuItem.VisibilityProperty, new CompareInt32Binding(nameof(IList.Count), CompareBinding.Types.Equal, 0) { Result = CompareBinding.Results.Visibility, Source = collection });
        return result;
    }

    /// Style

    Style GetDefaultStyle() => new Style(typeof(MenuItem), (Style)Control.FindResource(typeof(MenuItem)));

    Style GetStyle(object source, object member, MenuItemAttribute attribute)
    {
        var result = GetDefaultStyle();

        //Header
        result.Setters.Add(new System.Windows.Setter()
        {
            Property = MenuItem.HeaderProperty,
            Value = new LocExtension(attribute.Header ?? (member is MemberInfo m ? m.Name : member?.ToString()) ?? "")
        });

        //Icon
        if (attribute.Icon is SmallImages image)
        {
            result.Setters.Add(new System.Windows.Setter()
            {
                Property = MenuItem.IconProperty,
                Value = Resource.GetImageUri(image)?.GetImage()
            });
        }
        else if (attribute.Icon is string imageString)
        {
            result.Setters.Add(new System.Windows.Setter()
            {
                Property = MenuItem.IconProperty,
                Value = Resource.GetImageUri(imageString, AssemblyType.Current)?.GetImage()
            });
        }
        else FormatTemplate(result.Setters, MenuItem.IconProperty, XMenuItem.IconTemplateProperty, attribute.IconPath, attribute.IconTemplateSource, attribute.IconTemplateKey);

        //InputGestureText
        if (attribute.InputGestureText != null)
        {
            result.Setters.Add(new System.Windows.Setter()
            {
                Property = MenuItem.InputGestureTextProperty,
                Value = attribute.InputGestureText
            });
        }
        else FormatTemplate(result.Setters, MenuItem.InputGestureTextProperty, XMenuItem.InputGestureTextTemplateProperty, attribute.InputGestureTextPath, attribute.InputGestureTextSource, attribute.InputGestureTextKey);

        return result;
    }

    #endregion

    public void Load(object source)
    {
        Control.Items.Clear();

        if (source == null)
            return;

        var sourceType = source.GetType();

        List<Type> sourceTypes = new() { sourceType };
        sourceType.EachBaseType(sourceTypes.Add);

        ItemsControl control = null;
        MenuItem other = null;

        var count = sourceTypes.Count();
        for (var index = 0; index < count; index++)
        {
            var type = sourceTypes.ElementAt(index);

            var topLevel = type.GetAttribute<MenuAttribute>()?.Types.FirstOrDefault(i => i.GetAttribute<MenuAttribute>()?.Parent == null);
            if (topLevel == null)
                continue;

            control = Control as ItemsControl;
            if (Control is Menu && type.GetAttribute<MenuAttribute>()?.Float == true)
            {
                other ??= new MenuItem() { Header = "Other", Icon = Resource.GetImageUri(SmallImages.TriangleDown) };
                control = other;
            }

            var members = new Dictionary<MenuItemAttribute, object>();

            topLevel?.GetEnumValues()
                .ForEach(i => members.Add(i.As<Enum>().GetAttribute<MenuItemAttribute>(), i));

            type.GetMembers(BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field | MemberTypes.Method | MemberTypes.Property, false)
                .Where(i => i.DeclaringType == type)
                .Where(i => i.GetAttribute<MenuItemAttribute>() is MenuItemAttribute j && j.Parent == null)
                .ForEach(i =>
                {
                    var j = i.GetAttribute<MenuItemAttribute>();
                    if (!members.ContainsKey(j))
                        members.Add(j, i);
                });

            var attributes
                = members.Select(i => i.Key);

            if (!attributes.Any())
                continue;

            attributes = attributes
                .OrderBy(i => i.Index).ThenBy(i => i.Header ?? (members[i] is MemberInfo j ? j.Name : members[i].ToString()));

            int added = 0;
            foreach (MenuItemAttribute attribute in attributes)
            {
                var member = members[attribute];

                MenuItem item = null;

                //Top level [method]
                if (member is MethodInfo method)
                {
                    item = GetItem(source, member, attribute);
                    item.Command = new RelayCommand(() => method.Invoke(source, null));
                }
                //Top level [command]
                else if (member is MemberInfo info)
                {
                    if (source.GetMemberValue(info) is ICommand command)
                    {
                        item = GetItem(source, member, attribute);
                        item.Command = command;

                        attribute.HideIfDisabled.If(true, 
                            () => item.Bind(UIElement.VisibilityProperty, nameof(FrameworkElement.IsEnabled), item, BindingMode.OneWay, new System.Windows.Controls.BooleanToVisibilityConverter()));
                    }
                    else if (source.GetMemberValue(info) is IEnumerable enumerable)
                    {
                        item = GetCollection(source, member, attribute as MenuItemCollectionAttribute, enumerable, null, null);
                    }
                }
                //Top level [item]
                else
                {
                    item = GetItem(source, member, attribute, type);
                }

                //Add item
                if (item != null)
                {
                    added++;
                    control.Items.Add(item);
                }
            }

            //Add separator
            if (added > 0)
            {
                if (index < count - 1)
                    control.Items.Add(new Separator());
            }
        }

        //Remove last separator
        if (control?.Items.Last() is Separator)
            control.Items.RemoveLast();

        if (other != null)
            Control.Items.Add(other);
    }

    #endregion
}