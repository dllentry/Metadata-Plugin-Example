using CPPCli;
using FacialDetectionAgent.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacialDetectionAgent
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        public MainUserControl(MainUserControlViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item.Content is DataSource)
            {
                var model = new VideoSourceConfigViewModel(item.Content as DataSource);
                var window = new VideoSourceConfigWindow(model);

                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Show();
            }
        }
    }
}
