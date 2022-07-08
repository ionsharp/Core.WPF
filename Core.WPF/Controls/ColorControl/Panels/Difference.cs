using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Numerics;
using System;
using System.Runtime.CompilerServices;

namespace Imagin.Core.Controls;

[DisplayName("Difference"), Explicit, Serializable]
public class ColorDifferencePanel : Panel
{
    enum Category { Color1, Color2 }

    ByteVector4 color1 = ByteVector4.Black;
    [Category(Category.Color1), DisplayName("Color"), Horizontal, Index(1), Visible]
    public ByteVector4 Color1
    {
        get => color1;
        set => this.Change(ref color1, value);
    }

    GroupItemModel profile1 = null;
    [Category(Category.Color1), DisplayName("Profile"), Index(0), Visible]
    public GroupItemModel Profile1
    {
        get => profile1;
        set
        {
            profile1.If(i => i.PropertyChanged -= OnProfileChanged);
            this.Change(ref profile1, value);
            profile1.If(i => i.PropertyChanged += OnProfileChanged);
        }
    }
    
    ByteVector4 color2 = ByteVector4.White;
    [Category(Category.Color2), DisplayName("Color"), Horizontal, Index(1), Visible]
    public ByteVector4 Color2
    {
        get => color2;
        set => this.Change(ref color2, value);
    }

    GroupItemModel profile2 = null;
    [Category(Category.Color2), DisplayName("Profile"), Index(0), Visible]
    public GroupItemModel Profile2
    {
        get => profile2;
        set
        {
            profile2.If(i => i.PropertyChanged -= OnProfileChanged);
            this.Change(ref profile2, value);
            profile2.If(i => i.PropertyChanged += OnProfileChanged);
        }
    }

    double difference = 0;
    [Above, ReadOnly, Visible]
    public double Difference
    {
        get => difference;
        private set => this.Change(ref difference, value);
    }

    public override Uri Icon => Resources.InternalImage(Images.Ruler);

    public override string Title => "Difference";

    IColorDifference type = new EuclideanColorDifference();
    [Assignable(typeof(CIE76ColorDifference), typeof(CIE94ColorDifference), typeof(CIEDE2000ColorDifference), typeof(CMCColorDifference), typeof(EuclideanColorDifference), typeof(JzCzhzDEzColorDifference))]
    [Index(-1), Visible]
    public IColorDifference Type
    {
        get => type;
        set => this.Change(ref type, value);
    }

    public ColorDifferencePanel(Collections.Serialization.IGroupWriter profiles) : base() 
    {
        Profile1 = new(profiles, 0, 0);
        Profile2 = new(profiles, 0, 0);
    }

    void Update()
    {
        var profile1 = Profile1?.SelectedItem?.As<WorkingProfileModel>()?.Value ?? WorkingProfile.Default;
        var profile2 = Profile2?.SelectedItem?.As<WorkingProfileModel>()?.Value ?? WorkingProfile.Default;

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

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Color1):
            case nameof(Color2):
            case nameof(Profile1):
            case nameof(Profile2):
            case nameof(Type):
                Update(); break;
        }
    }
}