using Imagin.Core.Colors;
using System;

namespace Imagin.Core.Controls;

[Serializable]
public class WorkingProfileModel : Namable<WorkingProfile>, IDescription
{
    string description;
    public string Description
    {
        get => description;
        set => this.Change(ref description, value);
    }

    [Copy, DisplayName("Profile")]
    public override WorkingProfile Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    public WorkingProfileModel() : base() { }

    public WorkingProfileModel(string name, string description, WorkingProfile value) : base(name, value) => Description = description;
}