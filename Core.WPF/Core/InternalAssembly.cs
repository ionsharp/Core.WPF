namespace Imagin.Core
{
    public static class InternalAssembly
    {
        public const string AbsoluteImagePath 
            = "pack://application:,,,/" + Name + ";component/Images/";

        public const string AbsolutePath 
            = "pack://application:,,,/" + Name + ";component/";

        public const string Name = "Imagin.Core.WPF";

        public static class Space
        {
            public const string Core
                = "Imagin.Core";

            public const string Analytics
                = Core + ".Analytics";

            public const string Behavior
                = Core + ".Behavior";

            public const string Collections
                = Core + ".Collections";

            public const string CollectionsConcurrent
                = Core + ".Collections.Concurrent";

            public const string CollectionsObjectModel
                = Core + ".Collections.ObjectModel";

            public const string CollectionsSerialization
                = Core + ".Collections.Serialization";

            public const string Colors
                = Core + ".Colors";

            public const string Config
                = Core + ".Config";

            public const string Controls
                = Core + ".Controls";

            public const string Converters
                = Core + ".Converters";

            public const string Data
                = Core + ".Data";

            public const string Effects
                = Core + ".Effects";

            public const string Input
                = Core + ".Input";

            public const string Linq
                = Core + ".Linq";
            
            public const string Local
                = Core + ".Local";

            public const string LocalEngine
                = Core + ".Local.Engine";

            public const string LocalExtensions
                = Core + ".Local.Extensions";

            public const string LocalProviders
                = Core + ".Local.Providers";

            public const string Markup
                = Core + ".Markup";

            public const string Media
                = Core + ".Media";

            public const string MediaAnimation
                = Core + ".Media.Animation";

            public const string Models
                = Core + ".Models";

            public const string Numerics
                = Core + ".Numerics";
            
            public const string Reflection
                = Core + ".Reflection";

            public const string Storage
                = Core + ".Storage";

            public const string Text
                = Core + ".Text";

            public const string Time
                = Core + ".Time";
        }

        public const string Xml = "http://imagin.tech/imagin/wpf";
    }
}