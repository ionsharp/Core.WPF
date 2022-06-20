using Imagin.Core.Analytics;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Colors;
using Imagin.Core.Linq;
using Imagin.Core.Media;
using Imagin.Core.Models;
using Imagin.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Imagin.Core.Controls
{
    [DisplayName("Color control options")]
    [Serializable]
    public class ColorControlOptions : Base, IColorControlOptions, ILayout, ISerialize
    {
        enum Category { Component, Layouts, Window }

        #region Properties

        bool autoSaveLayout = true;
        [Category(Category.Layouts)]
        [DisplayName("Auto save")]
        public bool AutoSaveLayout
        {
            get => autoSaveLayout;
            set => this.Change(ref autoSaveLayout, value);
        }

        [Hidden]
        [field: NonSerialized]
        public ColorControl ColorControl { get; private set; }

        IGroupWriter IColorControlOptions.Colors => colors;
        [field: NonSerialized]
        GroupWriter<StringColor> colors;
        [Hidden]
        public GroupWriter<StringColor> Colors
        {
            get => colors;
            set => this.Change(ref colors, value);
        }

        bool componentNormalize = false;
        [Category(Category.Component)]
        [DisplayName("Normalize")]
        public bool ComponentNormalize
        {
            get => componentNormalize;
            set => this.Change(ref componentNormalize, value);
        }

        int componentPrecision = 2;
        [Category(Category.Component)]
        [DisplayName("Precision")]
        public int ComponentPrecision
        {
            get => componentPrecision;
            set => this.Change(ref componentPrecision, value);
        }

        [Hidden]
        public string FilePath { get; private set; }

        string layout = string.Empty;
        [Hidden]
        public virtual string Layout
        {
            get => layout;
            set => this.Change(ref layout, value);
        }

        [NonSerialized]
        Layouts layouts = null;
        [Category(Category.Layouts)]
        [DisplayName("Layout")]
        public virtual Layouts Layouts
        {
            get => layouts;
            set => this.Change(ref layouts, value);
        }

        ObservableCollection<Namable<WorkingProfile>> IColorControlOptions.Profiles => profiles;
        [field: NonSerialized]
        ObservableCollection<Namable<WorkingProfile>> profiles = new();
        [Hidden]
        public ObservableCollection<Namable<WorkingProfile>> Profiles
        {
            get => profiles;
            set => this.Change(ref profiles, value);
        }

        [Category(Category.Window)]
        [DisplayName("Panels")]
        public PanelCollection Panels => ColorControl?.Panels;

        #endregion

        #region ColorControlOptions

        public ColorControlOptions() : base() 
            => XList.ForEach<PropertyInfo>(typeof(WorkingProfiles).GetProperties(BindingFlags.Public | BindingFlags.Static), i => profiles.Add(new(i.GetDisplayName(), (WorkingProfile)i.GetValue(null))));

        public ColorControlOptions(string filePath) : this() => FilePath = filePath;

        #endregion

        #region Methods

        public static Result Load(string filePath, out ColorControlOptions data)
        {
            var result = BinarySerializer.Deserialize(filePath, out object options);
            data = options as ColorControlOptions ?? new ColorControlOptions(filePath);
            return result;
        }

        public Result Deserialize(string filePath, out object data) => BinarySerializer.Deserialize(filePath, out data);

        //...

        public Result Save() => Serialize(this);

        public Result Serialize(object data) => Serialize(FilePath, data);

        public Result Serialize(string filePath, object data)
        {
            OnSaved();
            return BinarySerializer.Serialize(filePath, data);
        }

        //...

        public IEnumerable<Uri> GetDefaultLayouts()
        {
            yield return Resources.Uri(AssemblyProperties.Name, "Controls/ColorControl/Layouts/Default.xml");
        }

        public void OnLoaded(ColorControl colorPicker)
        {
            ColorControl = colorPicker;

            Colors = new GroupWriter<StringColor>($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\ColorControl", "Colors", "data", "colors", new Collections.Limit(250, Collections.Limit.Actions.RemoveFirst));
            Colors.Load();

            Layouts = new Layouts($@"{Config.ApplicationProperties.GetFolderPath(Config.DataFolders.Documents)}\ColorControl\Layouts", GetDefaultLayouts());
            Layouts.Update(layout);
            Layouts.Refresh();
        }

        public void OnSaved()
        {
            Layout = Layouts.Layout;
            Colors.Save();
        }

        #endregion
    }
}