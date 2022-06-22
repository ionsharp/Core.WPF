using Imagin.Core.Collections.Serialization;
using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Reflection;
using System;
using System.Linq;
using System.Windows.Input;

namespace Imagin.Core.Models
{
    [MemberVisibility(MemberVisibility.Explicit, MemberVisibility.Explicit)]
    [Serializable]
    public class ColorsPanel : GroupPanel<StringColor>
    {
        public static readonly ResourceKey TemplateKey = new();

        [field: NonSerialized]
        public event EventHandler<EventArgs<StringColor>> Selected;

        //...

        public override Uri Icon => Resources.InternalImage(Images.Colors);

        public override string Title => "Colors";

        Func<StringColor> Background;

        Func<StringColor> Foreground;

        //...

        public ColorsPanel(IGroupWriter input, Func<StringColor> background, Func<StringColor> foreground) : base()
        {
            Initialize(input, background, foreground);
        }

        //...

        public void Initialize(IGroupWriter input, Func<StringColor> background, Func<StringColor> foreground)
        {
            Background = background;
            Foreground = foreground;

            if (input != null)
            {
                Groups = input as GroupWriter<StringColor>;
                if (!Groups.Contains(i => i is PrimaryColors))
                    Groups.Add(new PrimaryColors());

                if (!Groups.Contains(i => i is SecondaryColors))
                    Groups.Add(new SecondaryColors());

                if (!Groups.Contains(i => i is TertiaryColors))
                    Groups.Add(new TertiaryColors());

                if (!Groups.Contains(i => i is QuaternaryColors))
                    Groups.Add(new QuaternaryColors());

                if (!Groups.Contains(i => i is QuinaryColors))
                    Groups.Add(new QuinaryColors());

                if (SelectedGroupIndex == -1)
                    SelectedGroupIndex = 0;
            }
        }

        protected virtual void OnSelected(StringColor input) => Selected?.Invoke(this, new EventArgs<StringColor>(input));

        ICommand addBackgroundCommand;
        public ICommand AddBackgroundCommand => addBackgroundCommand ??= new RelayCommand
        (
            () => SelectedGroup.Add(new StringColor(Background())),
            () => Background != null && Groups != null && SelectedGroup != null
        );

        ICommand addForegroundCommand;
        public ICommand AddForegroundCommand => addForegroundCommand ??= new RelayCommand
        (
            () => SelectedGroup.Add(new StringColor(Foreground())),
            () => Foreground != null && Groups != null && SelectedGroup != null
        );

        ICommand cloneColorCommand;
        public ICommand CloneColorCommand => cloneColorCommand ??= new RelayCommand<StringColor>(i => SelectedGroup.Insert(SelectedGroup.IndexOf(i), new StringColor(i.Value)), i => i != null);

        ICommand deleteColorCommand;
        public ICommand DeleteColorCommand => deleteColorCommand ??= new RelayCommand<StringColor>(i => SelectedGroup.Remove(i), i => i != null);

        ICommand selectColorCommand;
        public ICommand SelectColorCommand => selectColorCommand ??= new RelayCommand<StringColor>(i => OnSelected(i), i => i != null);
    }
}