using FacialDetectionAgent.ViewModels;
using FacialDetectionAgent.Views;
using System.Windows;

namespace FacialDetectionAgent
{
    /// <summary>
    /// Interaction logic for VideoSourceConfigWindow.xaml
    /// </summary>
    public partial class VideoSourceConfigWindow : Window
    {
        public VideoSourceConfigWindow(VideoSourceConfigViewModel viewModel)
        {
            InitializeComponent();

            SourceConfig.DataContext = viewModel;
        }
    }
}
