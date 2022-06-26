using Imagin.Core;
using Imagin.Core.Models;
using Imagin.Core.Reflection;
using System;
using System.Collections.ObjectModel;

namespace Demo
{
    [Explicit]
    public class TestPanel : Panel
    {
        public override Uri Icon => Resources.InternalImage(Images.Phone);

        Test selectedItem;
        public Test SelectedItem
        {
            get => selectedItem;
            set => this.Change(ref selectedItem, value);
        }

        ObservableCollection<Test> tests = new()
        {
            //new StringConditionTest()
        };
        public ObservableCollection<Test> Tests
        {
            get => tests;
            set => this.Change(ref tests, value);
        }

        public override string Title => "Test";

        public TestPanel() : base() { }
    }
}