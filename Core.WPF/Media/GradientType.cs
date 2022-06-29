using System;

namespace Imagin.Core.Media;

[Serializable]
public enum GradientType : int
{
    [Image(Images.GradientAngle)]
    Angle = 0,
    [DisplayName("Radial")]
    [Image(Images.GradientRadial)]
    Circle = 1,
    [Image(Images.GradientDiamond)]
    Diamond = 2,
    [Image(Images.Gradient)]
    Linear = 3,
}