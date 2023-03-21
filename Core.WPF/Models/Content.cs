using System;
using System.Xml.Serialization;

namespace Imagin.Core.Models;

[Serializable]
public abstract class Content : LockableViewModel, ISubscribe, IUnsubscribe
{
    [Hide, XmlIgnore]
    public virtual bool CanFloat { get => Get(true); set => Set(value); }

    [Hide, XmlIgnore]
    [NonSerializable]
    public bool IsBusy
    {
        get => Get(false, false);
        set
        {
            Set(value, false);
            Update(() => IsNotBusy);
        }
    }

    [Hide]
    public bool IsNotBusy => !IsBusy;

    [Hide, XmlIgnore]
    public virtual string Title { get; }

    [Hide, XmlIgnore]
    public virtual object ToolTip { get; }

    ///

    [Hide]
    public virtual void Subscribe() { }

    [Hide]
    public virtual void Unsubscribe() { }
}