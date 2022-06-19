namespace Imagin.Core;

public static class AssemblyProperties
{
    public const string Name = "Imagin.Core.WPF";

    //...

    public const string AbsolutePath
        = "pack://application:,,,/" + Name + ";component/";

    public const string AbsoluteImagePath 
        = "pack://application:,,,/" + Name + ";component/Images/";

    public static class Path
    {
        public const string Core
            = "Imagin.Core";

        public const string Analytics
            = Core + $".{nameof(Imagin.Core.Analytics)}";

        public const string Animation
            = Core + $".{nameof(Imagin.Core.Animation)}";

        public const string Behavior
            = Core + $".{nameof(Imagin.Core.Behavior)}";

        public const string Collections
            = Core + $".{nameof(Imagin.Core.Collections)}";

        public const string CollectionsConcurrent
            = Core + $".{nameof(Imagin.Core.Collections)}.{nameof(Imagin.Core.Collections.Concurrent)}";

        public const string CollectionsObjectModel
            = Core + $".{nameof(Imagin.Core.Collections)}.{nameof(Imagin.Core.Collections.ObjectModel)}";

        public const string CollectionsSerialization
            = Core + $".{nameof(Imagin.Core.Collections)}.{nameof(Imagin.Core.Collections.Serialization)}";

        public const string Colors
            = Core + $".{nameof(Imagin.Core.Colors)}";

        public const string Config
            = Core + $".{nameof(Imagin.Core.Config)}";

        public const string Controls
            = Core + $".{nameof(Imagin.Core.Controls)}";

        public const string Conversion
            = Core + $".{nameof(Imagin.Core.Conversion)}";

        public const string Data
            = Core + $".{nameof(Imagin.Core.Data)}";

        public const string Effects
            = Core + $".{nameof(Imagin.Core.Effects)}";

        public const string Input
            = Core + $".{nameof(Imagin.Core.Input)}";

        public const string Linq
            = Core + $".{nameof(Imagin.Core.Linq)}";

        public const string Local
            = Core + $".{nameof(Imagin.Core.Local)}";

        public const string LocalEngine
            = Core + $".{nameof(Imagin.Core.Local)}.{nameof(Imagin.Core.Local.Engine)}";

        public const string LocalExtensions
            = Core + $".{nameof(Imagin.Core.Local)}.{nameof(Imagin.Core.Local.Extensions)}";

        public const string LocalProviders
            = Core + $".{nameof(Imagin.Core.Local)}.{nameof(Imagin.Core.Local.Providers)}";

        public const string Markup
            = Core + $".{nameof(Imagin.Core.Markup)}";

        public const string Models
            = Core + $".{nameof(Imagin.Core.Models)}";

        public const string Numerics
            = Core + $".{nameof(Imagin.Core.Numerics)}";

        public const string Paint
            = Core + $".{nameof(Imagin.Core.Paint)}";

        public const string Reflection
            = Core + $".{nameof(Imagin.Core.Reflection)}";

        public const string Storage
            = Core + $".{nameof(Imagin.Core.Storage)}";
    }

    public const string Xml = "http://imagin.tech/imagin/wpf";
}