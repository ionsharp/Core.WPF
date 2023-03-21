using Imagin.Core.Models;
using System.Windows;

namespace Imagin.Core.Controls;

public abstract class MainWindow : Window, IMainView
{
    public static readonly ResourceKey MainMenuKey = new();

    public readonly IMainViewModel Model;

    public MainWindow() : base()
    {
        Current.Add(this);

        Model = Current.Get<IMainViewModel>();
        Model.View = this;
    }
}