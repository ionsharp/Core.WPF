using System;

namespace Imagin.Core.Media;

[Serializable]
public enum GradientType : int
{
    [Image(SmallImages.GradientAngle)]
    Angle = 0,
    [Name("Radial")]
    [Image(SmallImages.GradientRadial)]
    Circle = 1,
    [Image(SmallImages.GradientDiamond)]
    Diamond = 2,
    [Image(SmallImages.Gradient)]
    Linear = 3,
}