using Imagin.Core.Colors;
using System;

namespace Imagin.Core.Controls;

[Serializable]
public class NamableProfile : Namable<WorkingProfile>
{
    [DisplayName("Profile")]
    public override WorkingProfile Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    public NamableProfile() : base() { }

    public NamableProfile(string name, WorkingProfile value) : base(name, value) { }
}