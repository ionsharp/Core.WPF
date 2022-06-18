using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Storage;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Imagin.Core.Conversion
{
    [ValueConversion(typeof(object[]), typeof(string))]
    public class FileSizeMultiConverter : MultiConverter<string>
    {
        public static FileSizeMultiConverter Default { get; private set; } = new FileSizeMultiConverter();
        FileSizeMultiConverter() { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 2)
            {
                if (values[1] is FileSizeFormat format)
                {
                    if (values[0] is long a)
                        return new FileSize(a).ToString(format);

                    if (values[0] is ulong b)
                        return new FileSize(b).ToString(format);
                }
            }
            return string.Empty;
        }
    }
}