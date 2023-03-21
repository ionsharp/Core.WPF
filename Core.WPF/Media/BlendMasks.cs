using System;

namespace Imagin.Core.Media;

[Serializable]
public enum BlendMasks : int
{
    None = 0,
    Clip = 1,
    DeepPunch = 2,
    ShallowPunch = 3
}