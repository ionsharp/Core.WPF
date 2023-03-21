using Imagin.Core.Input;
using System;

namespace Imagin.Core.Controls;

public abstract class PickerBox<T> : PickerBox
{
    public event EventHandler<EventArgs<T>> ValueChanged;

    protected abstract T DefaultValue { get; }

    public PickerBox() : base() { }

    protected virtual void OnValueChanged(ReadOnlyValue<T> input) => ValueChanged?.Invoke(this, new EventArgs<T>(input.New));

    protected abstract T GetValue();

    protected abstract void SetValue(T i);
}