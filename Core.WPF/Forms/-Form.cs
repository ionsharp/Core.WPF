namespace Imagin.Core;

public abstract class Form : Base
{
    public Form() : base() { }
}

[Categorize(false), ViewSource(ShowHeader = false)]
public class NameForm : Form
{
    public string Name { get => Get(Namable.DefaultName); set => Set(value); }

    public NameForm() : this(Namable.DefaultName) { }

    public NameForm(string name) : base() => Name = name;
}