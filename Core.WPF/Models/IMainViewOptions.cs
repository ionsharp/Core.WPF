namespace Imagin.Core.Models
{
    public interface IMainViewOptions
    {
        bool LogEnabled { get; }

        bool LogClearOnExit { get; }
        
        bool SaveWithDialog { get; }

        void Save();
    }
}