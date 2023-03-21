namespace Imagin.Core;

public static class AssemblyPath
{
    public const string Core
        = "Imagin.Core";

    public const string Analytics
        = Core + $".{nameof(Imagin.Core.Analytics)}";

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

    public const string Markup
        = Core + $".{nameof(Imagin.Core.Markup)}";

    public const string Models
        = Core + $".{nameof(Imagin.Core.Models)}";

    public const string Numerics
        = Core + $".{nameof(Imagin.Core.Numerics)}";

    public const string Media
        = Core + $".{nameof(Imagin.Core.Media)}";

    public const string Reflection
        = Core + $".{nameof(Imagin.Core.Reflection)}";

    public const string Storage
        = Core + $".{nameof(Imagin.Core.Storage)}";

    public const string Validation
        = Core + $".{nameof(Imagin.Core.Validation)}";
}