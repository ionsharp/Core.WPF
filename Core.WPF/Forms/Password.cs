using Imagin.Core.Data;
using Imagin.Core.Numerics;

namespace Imagin.Core;

[Categorize(false), ViewSource(ShowHeader = false)]
public class PasswordForm : Form
{
    [Hide]
    public PasswordType Type { get; private set; }

    [Name("Password"), VisibilityTrigger(nameof(Type), Operators.Equal, PasswordType.Default)]
    public string Password { get => Get(""); set => Set(value); }

    [Name("Pattern"), VisibilityTrigger(nameof(Type), Operators.Equal, PasswordType.Pattern)]
    public Int32LineCollection PasswordPattern { get => Get<Int32LineCollection>(new()); set => Set(value); }

    [Name("Pin"), VisibilityTrigger(nameof(Type), Operators.Equal, PasswordType.Pin)]
    public int PasswordPin { get => Get(0); set => Set(value); }

    public PasswordForm(PasswordType type) : base()
    {
        Type = type;
    }
}