namespace Imagin.Core.Input
{
    public interface IKeySelectComparer
    {
        bool Compare(object input, string query);
    }
}