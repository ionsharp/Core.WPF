namespace Imagin.Core.Controls
{
    public interface ISuggest
    {
        string Convert(object input);

        bool Handle(object input, string text);
    }
}