using Imagin.Core.Collections.ObjectModel;
using Imagin.Core.Conversion;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Reflection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Imagin.Core.Models;

[Name("Random"), Image(SmallImages.Refresh), Serializable]
public class RandomPanel : Panel
{
    public static readonly ResourceKey TemplateKey = new();

    enum Category { Characters }

    #region Properties

    [Pin(Pin.AboveOrLeft), Category(Category.Characters)]
    [Header, HideName, Index(1), Lock, Placeholder("Characters")]
    [Width(200)]
    public string Characters { get => Get(""); set => Set(value); }

    [Option, StringStyle(StringStyle.Tokens, ' ')]
    public string CustomCharacters { get => Get(""); set => Set(value); }

    [Pin(Pin.AboveOrLeft), Category(Category.Characters)]
    [CollectionStyle(ItemCommand = nameof(AddCharactersCommand)), Header, HideName, Image(SmallImages.Plus), Lock, Index(0), Style(BooleanStyle.ToggleButton)]
    public StringCollection CustomCharactersList
    {
        get
        {
            var result = new StringCollection();
            result.Add(Core.Text.Characters.Lower);
            result.Add(Core.Text.Characters.Numbers);
            result.Add(Core.Text.Characters.Special);
            result.Add(Core.Text.Characters.Upper);
            CustomCharacters.Split(Array<char>.New(' '), StringSplitOptions.RemoveEmptyEntries).ForEach(i => result.Add(i));
            return result;
        }
    }

    [Lock, Header]
    public bool Distinct { get => Get(false); set => Set(value); }

    [Name("FontFamily"), Option]
    public FontFamily FontFamily { get => GetFrom(new FontFamily("Calibri"), Converter.Get<FontFamilyToStringConverter>()); set => SetFrom(value, Converter.Get<FontFamilyToStringConverter>()); }

    [Name("FontSize"), Option]
    [Range(8.0, 72.0, 1.0, Style = RangeStyle.Both)]
    public double FontSize { get => Get(16.0); set => Set(value); }

    [Hide]
    public bool Generating { get => Get(false); set => Set(value); }

    [Pin(Pin.BelowOrRight), CollectionStyle(ItemCommand = nameof(FillCommand)), Header, HideName, Image(SmallImages.Clock), Style(BooleanStyle.ToggleButton)]
    public StringCollection History { get => Get(new StringCollection()); set => Set(value); }

    [Name("HistoryLimit"), Option]
    [Range(0, 64, 1, Style = RangeStyle.Both)]
    public int HistoryLimit { get => Get(20); set => Set(value); }

    [Header, Lock, Range((uint)1, uint.MaxValue, (uint)1, Style = RangeStyle.UpDown)]
    public uint Length { get => Get((uint)50); set => Set(value); }

    [Hide]
    public string Text { get => Get(""); set => Set(value); }

    [Name("TextAlignment")]
    [Option]
    public Alignment TextAlignment { get => Get(Alignment.Center); set => Set(value); }

    #endregion

    #region RandomPanel

    public RandomPanel() : base() { }

    #endregion

    #region Methods

    async Task Generate()
    {
        Generating = true;

        if (Length > 0)
        {
            await Dispatch.BeginInvoke(() => Text = string.Empty);

            var result = new StringBuilder();
            var length = (int)Length;

            var characters = Characters;
            await Task.Run(() => result.Append(Core.Numerics.Random.String(characters, length, length)));

            await Dispatch.BeginInvoke(() =>
            {
                var finalResult = result.ToString();
                finalResult = Distinct ? string.Concat(finalResult.Distinct()) : finalResult;

                Text = finalResult;

                if (HistoryLimit > 0)
                {
                    if (History.Count == HistoryLimit)
                        History.RemoveAt(HistoryLimit - 1);

                    History.Insert(0, finalResult);
                }
            });
        }

        Generating = false;
    }

    public override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);
        if (e.PropertyName == nameof(CustomCharacters))
        {
            var old = string.Empty;

            var result = string.Empty;
            foreach (var i in CustomCharacters)
            {
                if (i != ' ')
                {

                    if (!old.Contains(i))
                    {
                        result += i;
                        old += i;
                    }
                }
                else
                {
                    result += i;
                    old = string.Empty;
                }
            }

            e.NewValue = result;
        }
    }
    public override void OnPropertyChanged(PropertyEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(CustomCharacters))
            Update(() => CustomCharactersList);
    }

    #endregion

    #region Commands

    ICommand addCharactersCommand;
    [Hide]
    public ICommand AddCharactersCommand
        => addCharactersCommand ??= new RelayCommand<string>(i => Characters = $"{Characters}{i}", i => true);

    ICommand clearHistoryCommand;
    [Hide]
    public ICommand ClearHistoryCommand 
        => clearHistoryCommand ??= new RelayCommand(() => History.Clear(), () => History.Count > 0);

    ICommand copyCommand;
    [Hide]
    public ICommand CopyCommand
        => copyCommand ??= new RelayCommand(() => System.Windows.Clipboard.SetText(Text), () => !Text.NullOrEmpty());

    ICommand fillCommand;
    [Hide]
    public ICommand FillCommand 
        => fillCommand ??= new RelayCommand<string>(i => Text = i, i => !i.NullOrEmpty());

    ICommand generateCommand;
    [Header, Pin(Pin.BelowOrRight), Content("Generate"), Image(SmallImages.Refresh), Index(int.MaxValue), Show, Style(ButtonStyle.Default)]
    public ICommand GenerateCommand 
        => generateCommand ??= new RelayCommand(() => _ = Generate(), () => Characters.Length > 0 && Length > 0);

    #endregion
}