using Imagin.Core.Linq;
using System;
using System.Reflection;

namespace Imagin.Core.Reflection
{
    public class ListObjectModel : MemberModel
    {
        public readonly ListMemberModel Parent;

        public int ParentIndex => Parent.Items.IndexOf(this);

        //...

        public override bool CanWrite => !Parent.IsReadOnly;

        public override bool IsReadOnly => !CanWrite;

        public override MemberInfo Member => Parent.Member;

        public override Type Type => Parent?.ItemType;

        //...

        public ListObjectModel(ListMemberModel parent, MemberData data) : base(data, 0)
        {
            Parent = parent;
        }

        //...

        protected override void SetValue(object source, object value) 
            => ParentIndex.If(i => i > -1 && Parent.ActualValue?.Count > i, i => Parent.ActualValue[i] = value);

        protected override object GetValue(object input)
        {
            object result = null;
            ParentIndex.If(i => i > -1 && Parent.ActualValue?.Count > i, i => result = Parent.ActualValue[i]);
            return result;
        }
    }
}