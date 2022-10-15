using RatEditor.Components;
using RatEditor.GameProject;
using System;
using System.Collections.Generic;
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
            if ((sender as ListBox).SelectedItems.Count == 0)
            {
                GameEntityView.Instance.DataContext = null;
                return;
            }

            var entity = (sender as ListBox).SelectedItems[0];
            GameEntityView.Instance.DataContext = entity;
        }
    }
}
