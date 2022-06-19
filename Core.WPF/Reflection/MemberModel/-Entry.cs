using Imagin.Core.Linq;
using System;
using System.Collections;

namespace Imagin.Core.Reflection
{
    public class EntryModel : MemberModel
    {
        public override bool CanWrite
            => true;

        public override Type DeclaringType
            => Source?.Instance.GetType();

        public override string DisplayName
            => Name.SplitCamel();

        public override Type Type
            => Value?.GetType();

        //...

        public EntryModel(MemberData data) : base(data, 0) { }

        //...

        protected override object GetValue(object input) => (input as IDictionary)[Name];

        protected override void SetValue(object input, object value) => (input as IDictionary)[Name] = value;

        //...

        public override void Subscribe() { }

        public override void Unsubscribe() { }
    }
}