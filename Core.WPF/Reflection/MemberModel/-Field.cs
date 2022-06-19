using System;
using System.Reflection;

namespace Imagin.Core.Reflection
{
    public class FieldModel : MemberModel
    {
        public override bool CanWrite 
            => true;

        new public FieldInfo Member 
            => (FieldInfo)base.Member;

        public override Type Type 
            => Member.FieldType;

        //...

        public FieldModel(MemberData data, int depthIndex) : base(data, depthIndex) { }

        //...

        protected override object GetValue(object input) => Member.GetValue(input);

        protected override void SetValue(object source, object value) => Member.SetValue(source, value);
    }
}