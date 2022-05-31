using Imagin.Core.Collections.Generic;
using Imagin.Core.Colors;
using Imagin.Core.Data;
using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using Imagin.Core.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace Imagin.Core.Media;

/// <summary>
/// A normalized [0, 1] color with three components.
/// </summary>
public class ObservableColor : Base
{
    #region Properties

    readonly Handle handle = false;

    readonly Action<Color> OnChanged;

    //...

    Color actualColor = System.Windows.Media.Colors.White;
    [Format(ColorFormat.TextBox), DisplayName("Color")]
    [Featured, Label(false), Visible]
    public Color ActualColor
    {
        get => actualColor;
        set => this.Change(ref actualColor, value);
    }

    //...

    Components component = Components.X;
    [Index(1), Label(false), Tool]
    public Components Component
    {
        get => component;
        set => this.Change(ref component, value);
    }

    [Hidden]
    public IList Models
    {
        get
        {
            var result = new ObservableCollection<Type>();
            ColorVector.Type.ForEach(i => result.Add(i.Value));
            return result;
        }
    }

    int model = 0;
    [Label(false)]
    [Localize(false)]
    [Index(0)]
    [Source(nameof(Models))]
    [Style(Int32Style.Index)]
    [Tool]
    public virtual int Model
    {
        get => model;
        set => this.Change(ref model, value);
    }

    [Hidden]
    public Type ModelType
    {
        get => Model >= 0 ? (Type)Models[Model] : null;
        set
        {
            if (value != null)
            {
                var i = 0;
                foreach (var j in Models)
                {
                    if (j.Equals(value))
                    {
                        Model = i;
                        break;
                    }
                    i++;
                }
            }
        }
    }

    WorkingProfiles profile = WorkingProfiles.sRGB;
    [Index(-1), Label(false), Localize(false), Tool]
    public WorkingProfiles Profile
    {
        get => profile;
        set => this.Change(ref profile, value);
    }

    //...

    [PropertyTrigger(nameof(Controls.MemberModel.DisplayName), nameof(NameX))]
    [Index(0), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayX
    {
        get => GetDisplayValue(0);
        set => SetDisplayValue(value, 0);
    }

    [PropertyTrigger(nameof(Controls.MemberModel.DisplayName), nameof(NameY))]
    [Index(1), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayY
    {
        get => GetDisplayValue(1);
        set => SetDisplayValue(value, 1);
    }

    [PropertyTrigger(nameof(Controls.MemberModel.DisplayName), nameof(NameZ))]
    [Index(2), UpdateSourceTrigger(UpdateSourceTrigger.LostFocus), Visible]
    public string DisplayZ
    {
        get => GetDisplayValue(2);
        set => SetDisplayValue(value, 2);
    }

    //...

    [Hidden]
    public string NameX => $"({ColorVector.GetComponent(ModelType, 0).Symbol}) {ColorVector.GetComponent(ModelType, 0).Name}";

    [Hidden]
    public string NameY => $"({ColorVector.GetComponent(ModelType, 1).Symbol}) {ColorVector.GetComponent(ModelType, 1).Name}";

    [Hidden]
    public string NameZ => $"({ColorVector.GetComponent(ModelType, 2).Symbol}) {ColorVector.GetComponent(ModelType, 2).Name}";

    //...

    [Hidden]
    public Vector Maximum => new(ColorVector.GetComponent(ModelType, 0).Maximum, ColorVector.GetComponent(ModelType, 1).Maximum, ColorVector.GetComponent(ModelType, 2).Maximum);

    [Hidden]
    public Vector Minimum => new(ColorVector.GetComponent(ModelType, 0).Minimum, ColorVector.GetComponent(ModelType, 1).Minimum, ColorVector.GetComponent(ModelType, 2).Minimum);

    [Hidden]
    public Vector Value => new(x, y, z);

    //...

    One x = default;
    [Hidden]
    public One X
    {
        get => x;
        set => this.Change(ref x, value);
    }

    One y = default;
    [Hidden]
    public One Y
    {
        get => y;
        set => this.Change(ref y, value);
    }

    One z = default;
    [Hidden]
    public One Z
    {
        get => z;
        set => this.Change(ref z, value);
    }

    #endregion

    #region ObservableColor

    public ObservableColor(Color defaultColor, Action<Color> onChanged = null) : base()
    {
        ActualColor = defaultColor; OnChanged = onChanged;
    }

    #endregion

    #region Methods

    string GetDisplayValue(int index)
    {
        One result = default;
        switch (Component)
        {
            case Components.X:
                var a = new One[] { z, x, y };
                result = a[index];
                break;
            case Components.Y:
                var b = new One[] { x, z, y };
                result = b[index];
                break;
            case Components.Z:
                var c = new One[] { x, y, z };
                result = c[index];
                break;
        }

        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);
        return aRange.Convert(bRange.Minimum, bRange.Maximum, result).Round(2).ToString();
    }

    void SetDisplayValue(string input, int index)
    {
        var aRange = new DoubleRange(0, 1);
        var bRange = new DoubleRange(Minimum[index], Maximum[index]);

        var result = (One)bRange.Convert(aRange.Minimum, aRange.Maximum, input?.Double() ?? 0);
        switch (Component)
        {
            case Components.X:
                switch (index)
                {
                    case 0: Z = result; break;
                    case 1: X = result; break;
                    case 2: Y = result; break;
                }
                break;
            case Components.Y:
                switch (index)
                {
                    case 0: X = result; break;
                    case 1: Z = result; break;
                    case 2: Y = result; break;
                }
                break;
            case Components.Z:
                switch (index)
                {
                    case 0: X = result; break;
                    case 1: Y = result; break;
                    case 2: Z = result; break;
                }
                break;
        }

        switch (index)
        {
            case 0: this.Changed(() => DisplayX); break;
            case 1: this.Changed(() => DisplayY); break;
            case 2: this.Changed(() => DisplayZ); break;
        }
    }

    //... ActualColor <Binding> [X, Y, Z]

    /// <summary>
    /// Converts from <see cref="Model"/> to <see cref="RGB"/> based on <see cref="Component"/>.
    /// </summary>
    void ConvertTo()
    {
        handle.SafeInvoke(() =>
        {
            Vector3<double> result = default;
            if (Component == Components.X)
                result = new(z, x, y);

            if (Component == Components.Y)
                result = new(x, z, y);

            if (Component == Components.Z)
                result = new(x, y, z);

            var xyz = ColorVector.New(ModelType, result);
            var rgb = xyz.ToRGB(WorkingProfile.Default.sRGB);

            ActualColor = XColor.Convert(rgb);
        });
    }

    /// <summary>
    /// Converts from <see cref="RGB"/> to <see cref="Model"/> based on <see cref="Component"/>.
    /// </summary>
    void ConvertFrom()
    {
        handle.SafeInvoke((Action)(() =>
        {
            var rgb = Xrgb.Convert(ActualColor);

            var result = ColorVector.New(ModelType, rgb, WorkingProfile.Default.sRGB).Value;
            var xyz = new Vector3<double>(result[(int)0] / 255, result[(int)1] / 255, result[(int)2] / 255);

            switch (Component)
            {
                case Components.X: X = xyz.Y; Y = xyz.Z; Z = xyz.X; break;
                case Components.Y: X = xyz.X; Y = xyz.Z; Z = xyz.Y; break;
                case Components.Z: X = xyz.X; Y = xyz.Y; Z = xyz.Z; break;
            }
        }));
    }

    //...

    public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(Component):
                this.Changed(() => DisplayX);
                this.Changed(() => DisplayY);
                this.Changed(() => DisplayZ);

                this.Changed(() => NameX);
                this.Changed(() => NameY);
                this.Changed(() => NameZ);
                break;

            case nameof(Model):
                this.Changed(() => ModelType);
                goto case nameof(Component);

            case nameof(X):
                switch (Component)
                {
                    case Components.X: this.Changed(() => DisplayY); break;
                    case Components.Y: this.Changed(() => DisplayX); break;
                    case Components.Z: this.Changed(() => DisplayX); break;
                }
                //ConvertTo();
                break;

            case nameof(Y):
                switch (Component)
                {
                    case Components.X: this.Changed(() => DisplayZ); break;
                    case Components.Y: this.Changed(() => DisplayZ); break;
                    case Components.Z: this.Changed(() => DisplayY); break;
                }
                //ConvertTo();
                break;

            case nameof(Z):
                switch (Component)
                {
                    case Components.X: this.Changed(() => DisplayX); break;
                    case Components.Y: this.Changed(() => DisplayY); break;
                    case Components.Z: this.Changed(() => DisplayZ); break;
                }
                //ConvertTo();
                break;

            case nameof(ActualColor):
                //OnChanged?.Invoke(ActualColor);
                //ConvertFrom();
                break;

            case nameof(Profile):
                break;
        }
    }

    #endregion
}