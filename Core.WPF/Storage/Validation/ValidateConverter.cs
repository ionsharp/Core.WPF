using Imagin.Core.Controls;
using Imagin.Core.Converters;
using Imagin.Core.Linq;
using System;
using System.Globalization;
using System.Windows.Data;

using static Imagin.Core.Numerics.M;

namespace Imagin.Core.Storage
{
    public class ValidateConverter : MultiConverter<bool>
    {
        public static ValidateConverter Default { get; private set; } = new ValidateConverter();
        ValidateConverter() { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length >= 2)
            {
                if (values[0] is string path)
                {
                    if (values[1] is StorageWindowModes mode)
                    {
                        var types = mode.Convert();
                        if (values[2] is IValidate validator)
                            return validator.Validate(types, path);

                        return PathBox.DefaultValidator.Validate(types, path);
                    }
                }
            }
            return Binding.DoNothing;
        }
    }
}