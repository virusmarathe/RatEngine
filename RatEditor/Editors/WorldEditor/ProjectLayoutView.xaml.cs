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
        public ProjectLayoutView()
        {
            InitializeComponent();
        }
        //private void OnAddSceneButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var vm = DataContext as Project;
        //    vm.AddScene("New Scene " + vm.Scenes.Count);
        //}
    }
}
