using GeoCoordTool.ViewModels;

namespace GeoCoordTool.Views
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
