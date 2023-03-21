using Imagin.Core.Collections.Generic;
using Imagin.Core.Conversion;
using Imagin.Core.Reflection;
using System;
using System.Windows.Data;

namespace Imagin.Core.Effects;

[Serializable]
public class BinaryEffectCollection : BinaryValue<EffectCollection, ObjectDictionary, BinaryEffectCollection.EffectCollectionConverter>
{
    [ValueConversion(typeof(EffectCollection), typeof(ObjectDictionary))]
    public class EffectCollectionConverter : Conversion.ValueConverter<EffectCollection, ObjectDictionary>
    {
        public EffectCollectionConverter() : base() { }

        protected override ConverterValue<EffectCollection> ConvertBack(ConverterData<ObjectDictionary> input)
        {
            return new EffectCollection();

            var result = new EffectCollection();
            result.LoadMembers(input.Value);
            return result;
        }

        protected override ConverterValue<ObjectDictionary> ConvertTo(ConverterData<EffectCollection> input)
            => new ObjectDictionary(); //input.Value.SaveMembers()
    }

    public BinaryEffectCollection() : this(null) { }

    public BinaryEffectCollection(EffectCollection value) : base(value) { }
}