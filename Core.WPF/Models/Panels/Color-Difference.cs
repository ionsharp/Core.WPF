using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Controls;

[Name("Difference"), Explicit, Serializable]
public class ColorDifferencePanel : Panel
{
    enum Category { A, B, Difference }

    [Category(Category.A), Name("Color"), Horizontal, Index(1), Show]
    public ByteVector4 Color1 { get => Get(ByteVector4.Black); set => Set(value); }

    [Category(Category.A), Name("Profile"), Index(0), Show]
    public GroupItemForm Profile1 { get => Get<GroupItemForm>(); set => Set(value); }

    [Category(Category.B), Name("Color"), Horizontal, Index(1), Show]
    public ByteVector4 Color2 { get => Get(ByteVector4.White); set => Set(value); }

    [Category(Category.B), Name("Profile"), Index(0), Show]
    public GroupItemForm Profile2 { get => Get<GroupItemForm>(); set => Set(value); }

    [Category(Category.Difference), ReadOnly, Show]
    public double Difference { get => Get(.0); set => Set(value); }

    public override Uri Icon => Resource.GetImageUri(SmallImages.Ruler);

    [Hide]
    public IGroupWriter Profiles { get => Get<IGroupWriter>(); set => Set(value); }

    public override string Title => "Difference";

    [Assign(typeof(CIE76ColorDifference), typeof(CIE94ColorDifference), typeof(CIEDE2000ColorDifference), typeof(CMCColorDifference), typeof(EuclideanColorDifference), typeof(JzCzhzDEzColorDifference))]
    [Pin(Pin.AboveOrLeft), Show]
    public IColorDifference Type { get => Get(new EuclideanColorDifference()); set => Set(value); }

    public ColorDifferencePanel() : base() { }

    public ColorDifferencePanel(IGroupWriter profiles) : this() => Profiles = profiles;

    void Update()
    {
        var profile1 = Profile1?.SelectedItem?.As<WorkingProfile>() ?? WorkingProfile.Default;
        var profile2 = Profile2?.SelectedItem?.As<WorkingProfile>() ?? WorkingProfile.Default;

        if (Type is CIE76ColorDifference || Type is CIE94ColorDifference || Type is CIEDE2000ColorDifference || Type is CMCColorDifference)
        {
            XColor.Convert(Color1).Convert(out RGB a);
            XColor.Convert(Color2).Convert(out RGB b);
            Difference = Type.ComputeDifference(a.To<Lab>(profile1), b.To<Lab>(profile2));
        }
        else if (Type is JzCzhzDEzColorDifference)
        {
            XColor.Convert(Color1).Convert(out RGB a);
            XColor.Convert(Color2).Convert(out RGB b);
            Difference = Type.ComputeDifference(a.To<LCHabj>(profile1), b.To<LCHabj>(profile2));
        }
        else if (Type is EuclideanColorDifference)
        {
            XColor.Convert(Color1).Convert(out RGB a);
            XColor.Convert(Color2).Convert(out RGB b);
            Difference = Type.ComputeDifference(a, b);
        }
    }

    void OnProfileChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => Update();

    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        switch (e.PropertyName)
        {
            case nameof(Color1):
            case nameof(Color2):
            case nameof(Type):
                Update(); break;

            case nameof(Profile1):
            case nameof(Profile2):
                e.OldValue.If<IPropertyChanged>(i => i.PropertyChanged -= OnProfileChanged);
                e.NewValue.If<IPropertyChanged>(i => i.PropertyChanged += OnProfileChanged);
                Update();
                break;

            case nameof(Profiles):
                Profile1 = new(Profiles, 0, 0);
                Profile2 = new(Profiles, 0, 0);
                break;
        }
    }
}