using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DarkLight.Analytics.Models;
using TradeLink.Common;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace DarkLight.Analytics
{
    /// <summary>
    /// Interaction logic for TickFileControl.xaml
    /// </summary>
    public partial class TickFileControl : UserControl
    {
        private static TickDataModel _tickDataModel = new TickDataModel();

        public TickFileControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _tickDataModel;
            _tickDataModel.LoadPath(Properties.Settings.Default.TickDataDirectory);
            _tickDataModel.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == "AvailableDates")
                {
                    var dates = _tickDataModel.AvailableDates.OrderBy(d => d.Ticks).ToList();
                    BlackoutDatePicker(FirstDatePicker, dates);
                    BlackoutDatePicker(LastDatePicker, dates);
                }
            };
        }

        private void BlackoutDatePicker(DatePicker picker, List<DateTime> dates)
        {
            for (int i = 0; i < dates.Count - 1; i++)
            {
                var firstDate = dates[i];
                var nextDate = dates[i + 1];
                if (firstDate.AddDays(1).Date != nextDate.Date)
                {
                    picker.BlackoutDates.Add
                    (
                        new CalendarDateRange(
                            firstDate.AddDays(1),
                            nextDate.AddDays(-1)
                        )
                    );
                }
            }
        }

        private void DataDirButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the FolderBrowserDialog.
            var _folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = _folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var folderName = _folderBrowserDialog.SelectedPath;
                _tickDataModel.LoadPath(folderName);
            }
        }
    }
}
