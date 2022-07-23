using System;
using System.Reflection;

namespace Imagin.Core.Reflection;

public class PropertyModel : MemberModel<PropertyInfo>
{
    public override bool IsWritable => Member.CanWrite && Member.GetSetMethod(true) != null;

    public override Type Type => Member.PropertyType;

    public PropertyModel(MemberModel parent, MemberSource source, MemberInfo member, MemberAttributes attributes, int depthIndex) : base(parent, source, member, attributes, depthIndex) { }

    protected override void SetValue(object source, object value) => Member.SetValue(source, value, null);

    protected override object GetValue(object input) => Member.GetValue(input);
}