using FacialDetectionAgent.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FacialDetectionAgent.Views
{
    /// <summary>
    /// Interaction logic for VideoSourceConfigView.xaml
    /// </summary>
    public partial class VideoSourceConfigView : UserControl
    {
        public VideoSourceConfigView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                await ((VideoSourceConfigViewModel)DataContext).StartStream();
            }
        }

        private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                await ((VideoSourceConfigViewModel)DataContext).StopStream();
            }
        }
    }
}
