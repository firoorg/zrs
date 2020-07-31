namespace Zrs.Converters.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using NBitcoin;

    public sealed class UInt256 : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo culture, object value)
        {
            switch (value)
            {
                case string s:
                    return uint256.Parse(s);
                default:
                    throw new NotSupportedException(
                        $"Don't know how to convert {value.GetType()} to {typeof(uint256)}.");
            }
        }

        public override object ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo culture,
            object value,
            Type? destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            var v = (uint256)value;

            if (destinationType == typeof(string))
            {
                return v.ToString();
            }
            else
            {
                throw new NotSupportedException($"Don't know how to convert {typeof(uint256)} to {destinationType}.");
            }
        }
    }
}
