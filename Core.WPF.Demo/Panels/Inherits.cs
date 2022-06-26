using Imagin.Core;
using Imagin.Core.Linq;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Demo
{
    [Explicit]
    public class InheritsPanel : Panel
    {
        Element element;
        public Element Element
        {
            get => element;
            set => this.Change(ref element, value);
        }

        public override Uri Icon => Resources.InternalImage(Images.Info);
        
        public override string Title => "Inherits";

        ObservableCollection<Type> types = new();
        public ObservableCollection<Type> Types
        {
            get => types;
            set => this.Change(ref types, value);
        }

        public InheritsPanel() : base() { }

        public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Element):
                    types.Clear();
                    var type = Element?.Type;
                    if (type != null)
                    {
                        types.Add(type);
                        type.Inheritance()?.ForEach(i => types.Add(i));
                    }
                    break;
            }
        }
    }
}