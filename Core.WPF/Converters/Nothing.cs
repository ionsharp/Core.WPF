namespace Imagin.Core.Converters
{
    public sealed class Nothing
    {
        public static readonly Nothing Do = new();

        Nothing() { }
    }
}