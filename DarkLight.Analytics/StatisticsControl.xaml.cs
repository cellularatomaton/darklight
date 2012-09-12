using System.Globalization;
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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DarkLight.Analytics
{
    /// <summary>
    /// Interaction logic for StatisticsControl.xaml
    /// </summary>
    public partial class StatisticsControl : UserControl
    {
        public StatisticsControl()
        {
            InitializeComponent();
        }

        public PrimativeTypeStatisticsModel PrimativeTypeStatisticsModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ComplexTypeStatisticsModel<object> ComplexTypeStatisticsModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
