namespace Imagin.Core.Media;

public sealed class ExifTag
{
    public int Id { get; private set; }

    public string Description { get; private set; }

    public string Name { get; private set; }

    public string Value { get; set; }

    public ExifTag(int id, string name, string description) { Id = id; Name = name; Description = description; }

    public override string ToString() => string.Format("{0} ({1}) = {2}", Description, Name, Value);
}