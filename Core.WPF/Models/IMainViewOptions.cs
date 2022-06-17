namespace Imagin.Core.Models
{
    public interface IMainViewOptions
    {
        bool LogEnable { get; }

        bool LogClearOnExit { get; }
        
        bool SaveWithDialog { get; }

        void Save();
    }
}