
using System.Windows;

using ComplianceDashBoard.Client.Model;

namespace ComplianceDashBoard.Client.View
{
    /// <summary>
    /// Interaction logic for NavDashboardSettingVisual.xaml
    /// </summary>
    public partial class ComplianceDashBoardSettingVisual : Window
    {

        ComplianceDashBoardSettingsViewModel _viewModel;
        ComplianceDashBoardVisual _view;

        public ComplianceDashBoardSettingVisual(ComplianceDashBoardSettingsViewModel ViewModel, ComplianceDashBoardVisual View)
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            this._view = View;
            this._viewModel = ViewModel;

           
          
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
           
          
        }

       
    }
}
