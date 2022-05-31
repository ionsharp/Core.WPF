using Imagin.Core.Input;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Colors;
using Imagin.Core.Models;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Imagin.Core.Controls
{
    [MemberVisibility(Property: MemberVisibility.Explicit)]
    [Serializable]
    public class ColorDocument : Document
    {
        public static readonly System.Windows.Media.Color DefaultOldColor = System.Windows.Media.Colors.Black;

        public static readonly System.Windows.Media.Color DefaultNewColor = System.Windows.Media.Colors.White;

        public static readonly Type DefaultModel = typeof(HSB);

        [field: NonSerialized]
        public event DefaultEventHandler<System.Windows.Media.Color> ColorSaved;

        //...

        byte alpha = 255;
        public byte Alpha
        {
            get => alpha;
            set => this.Change(ref alpha, value);
        }

        [field: NonSerialized]
        ObservableColor color = null;
        public ObservableColor Color
        {
            get => color;
            set => this.Change(ref color, value);
        }

        public override object Icon => Color.ActualColor;

        StringColor oldColor = DefaultOldColor;
        public System.Windows.Media.Color OldColor
        {
            get => oldColor.Value;
            set => this.Change(ref oldColor, new StringColor(value));
        }

        public override string Title => $"#{Color.ActualColor.Hexadecimal()}";

        public override object ToolTip => Color.ActualColor;

        //...

        /// <summary>
        /// Alpha <> NewColor <> ObservableColor
        /// </summary>
        public ColorDocument() : this(DefaultNewColor, DefaultModel) { }

        public ColorDocument(System.Windows.Media.Color color, Type model) : base()
        {
            Color = new(color);
            Color.ModelType = model;
            Color.ActualColor = color;
        }

        //...

        public override void Subscribe()
        {
            base.Subscribe();
            Color.PropertyChanged += OnColorChanged;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            Color.PropertyChanged -= OnColorChanged;
        }

        public override void Save() { }

        //...

        void OnColorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Changed(() => Color);
        }

        public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Alpha):
                    Color.If(i => i.ActualColor = Color.ActualColor.A(Alpha));
                    break;

                case nameof(Color):
                    this.Changed(() => Title);
                    this.Changed(() => ToolTip);
                    break;
            }
        }

        //...

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext input) { }

        //...

        [field: NonSerialized]
        ICommand revertCommand;
        [DisplayName("Revert")]
        [Hidden(false)]
        [Index(2)]
        public ICommand RevertCommand => revertCommand ??= new RelayCommand(() =>
        {
            var oldColor = OldColor;
            OldColor = Color.ActualColor;
            Color.ActualColor = oldColor;
        },
        () => true);

        [field: NonSerialized]
        ICommand saveCommand;
        [DisplayName("Save")]
        [Hidden(false)]
        [Index(3)]
        new public ICommand SaveCommand => saveCommand ??= new RelayCommand(() => OldColor = Color.ActualColor, () => true);

        [field: NonSerialized]
        ICommand saveColorCommand;
        [DisplayName("SaveColor")]
        [Hidden(false)]
        [Index(3)]
        public ICommand SaveColorCommand => saveColorCommand ??= new RelayCommand(() => ColorSaved?.Invoke(this, new(Color.ActualColor)), () => true);
    }
}