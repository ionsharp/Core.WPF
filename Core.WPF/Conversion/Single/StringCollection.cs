using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Linq;
using System.Collections;
using System.Windows.Data;

namespace Imagin.Core.Conversion;

[ValueConversion(typeof(object), typeof(StringCollection))]
public class StringCollectionConverter : ValueConverter<object, StringCollection>
{
    public StringCollectionConverter() : base() { }

    protected override ConverterValue<StringCollection> ConvertTo(ConverterData<object> input)
    {
        var result = new StringCollection();
        if (input.Value is string c)
            c.Split(XArray.New(';'), System.StringSplitOptions.RemoveEmptyEntries).ForEach(i => result.Add(i.ToString())); 
            
        else if (input.Value is IEnumerable a)
            a.ForEach(i => result.Add(i.ToString()));

        else if (input.Value is IList b)
            b.ForEach(i => result.Add(i.ToString()));

        return result;
    }

    protected override ConverterValue<object> ConvertBack(ConverterData<StringCollection> input) => Nothing.Do;
}