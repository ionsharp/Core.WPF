using System;
using System.Reflection;

namespace Imagin.Core.Reflection;

public class FieldModel : MemberModel<FieldInfo>
{
    public override bool IsWritable => !Member.IsInitOnly && Member.IsPublic;

    public override Type Type => Member.FieldType;

    public FieldModel(MemberModel parent, MemberSource source, MemberInfo member, MemberAttributes attributes, int depthIndex) : base(parent, source, member, attributes, depthIndex) { }

    protected override object GetValue(object input) => Member.GetValue(input);

    protected override void SetValue(object source, object value) => Member.SetValue(source, value);
}