using RatEditor.Components;
using RatEditor.GameProject;
using RatEditor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RatEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        private int _entityID = 0;

        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vm = button.DataContext as Scene;

            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = $"Empty Game Entity {_entityID}" });
            _entityID++;
        }

        private void OnGameEntity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameEntityView.Instance.DataContext = null;
            var listBox = sender as ListBox;
            if (e.AddedItems.Count > 0)
            {
                GameEntityView.Instance.DataContext = listBox.SelectedItems[0]; // temp single view
            }

            var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
            var prevSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction("Change selection",
                () => // undo 
                {
                    listBox.UnselectAll();
                    prevSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },
                () => // redo
                {
                    listBox.UnselectAll();
                    newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                }));
        }
    }
}
