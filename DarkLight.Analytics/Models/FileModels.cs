using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TradeLink.Common;
using System.Linq;
using DarkLight.Utilities;

namespace DarkLight.Analytics.Models
{
    public class ResponseLibraryList : ObservableCollection<string>
    {
        public const string _header = "--Select Response--";

        private string _selectedResponse = "";
        public string SelectedResponse
        {
            get
            {
                return _selectedResponse;
            }

            set
            {
                if (value != _selectedResponse)
                {
                    _selectedResponse = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SelectedResponse"));
                }
            }
        }

        private string _fileName = "";
        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                if (value != _fileName)
                {
                    _fileName = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FileName"));
                }
            }
        }

        public ResponseLibraryList()
        {
            // Insert code required on object creation below this point.
        }

        public void LoadResponseListFromFileName(string fileName)
        {
            FileName = fileName;
            Clear();
            List<string> responseList = Util.GetResponseList(fileName);
            SelectedResponse = _header;
            Add(_header);
            foreach (var response in responseList)
            {
                Add(response);
            }
        }
    }

    public class TickDataFileList : ObservableCollection<FileModel>
    {
        private string _tickDataDirectory = "";
        public string TickDataDirectory
        {
            get
            {
                return _tickDataDirectory;
            }

            set
            {
                if (value != _tickDataDirectory)
                {
                    _tickDataDirectory = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TickDataDirectory"));
                }
            }
        }

        public TickDataFileList()
        {
        }

        public void LoadPathFromFileName(string fileName)
        {
            var path = System.IO.Path.GetDirectoryName(fileName);
            LoadPath(path, fileName);
        }

        public void LoadPath(string path, string fileName = "")
        {
            var files = System.IO.Directory.GetFiles(path, "*.TIK");
            Clear();
            TickDataDirectory = path;
            foreach (var file in files)
            {
                var fileModel = new FileModel
                {
                    Checked = (file == fileName),
                    LongFileName = file,
                    ShortFileName = System.IO.Path.GetFileName(file),
                };
                Add(fileModel);
            }
        }
    }

    public class FileModel : INotifyPropertyChanged
    {
        private string _shortFileName = "";
        public string ShortFileName
        {
            get { return _shortFileName; }
            set
            {
                if(value != _shortFileName)
                {
                    _shortFileName = value;
                    NotifyPropertyChanged("ShortFileName");
                }
            }
        }

        private string _longFileName = "";
        public string LongFileName
        {
            get { return _longFileName; }
            set
            {
                if (value != _longFileName)
                {
                    _longFileName = value;
                    NotifyPropertyChanged("LongFileName");
                }
            }
        }

        private bool _checked = false;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    NotifyPropertyChanged("Checked");
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }

    public class SymbolModel : INotifyPropertyChanged
    {
        private string _symbol = string.Empty;
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (value != _symbol)
                {
                    _symbol = value;
                    NotifyPropertyChanged("Symbol");
                }
            }
        }

        private bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    NotifyPropertyChanged("Selected");
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }

    public class TickFileModel : INotifyPropertyChanged
    {
        private SymbolModel _symbol;
        public SymbolModel Symbol
        {
            get { return _symbol; }
            set
            {
                if (value != _symbol)
                {
                    _symbol = value;
                    NotifyPropertyChanged("Symbol");
                }
            }
        }

        private FileModel _file;
        public FileModel File
        {
            get { return _file; }
            set
            {
                if (value != _file)
                {
                    _file = value;
                    NotifyPropertyChanged("File");
                }
            }
        }

        private DateTime _dateForFile;
        public DateTime DateForFile
        {
            get { return _dateForFile; }
            set
            {
                if (value != _dateForFile)
                {
                    _dateForFile = value;
                    NotifyPropertyChanged("DateForFile");
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }

    public class TickDataModel : INotifyPropertyChanged
    {
        private static string tikFilePattern = @"^.*\d{8}\.TIK$";
        private Regex tikFileRegex = new Regex(tikFilePattern);

        private string _tickDataDirectory = "";
        public string TickDataDirectory
        {
            get
            {
                return _tickDataDirectory;
            }

            set
            {
                if (value != _tickDataDirectory)
                {
                    _tickDataDirectory = value;
                    NotifyPropertyChanged("TickDataDirectory");
                }
            }
        }

        /// <summary>
        /// Symbol models are used to show which symbols are available in the given tick data directory.
        /// </summary>
        private ObservableCollection<SymbolModel> _symbolModels = new ObservableCollection<SymbolModel>();
        public ObservableCollection<SymbolModel> SymbolModels
        {
            get { return _symbolModels; }
            set
            {
                if (value != _symbolModels)
                {
                    _symbolModels = value;
                    NotifyPropertyChanged("SymbolModels");
                }
            }
        }

        /// <summary>
        /// Tick file models contain all the data necessary to access the tick files.
        /// </summary>
        private List<TickFileModel> _tickFileModels = new List<TickFileModel>();
        public List<TickFileModel> TickFileModels
        {
            get { return _tickFileModels; }
            set
            {
                if (value != _tickFileModels)
                {
                    _tickFileModels = value;
                    NotifyPropertyChanged("TickFileModels");
                }
            }
        }

        private ObservableCollection<DateTime> _availableDates;
        public ObservableCollection<DateTime> AvailableDates
        {
            get { return _availableDates; }
            set
            {
                if (value != _availableDates)
                {
                    _availableDates = value;
                    NotifyPropertyChanged("AvailableDates");
                }
            }
        }

        private DateTime _firstDate;
        public DateTime FirstDate
        {
            get { return _firstDate; }
            set
            {
                if (value != _firstDate)
                {
                    _firstDate = value;
                    NotifyPropertyChanged("FirstDate");
                }
            }
        }

        private DateTime _lastDate;
        public DateTime LastDate
        {
            get { return _lastDate; }
            set
            {
                if (value != _lastDate)
                {
                    _lastDate = value;
                    NotifyPropertyChanged("LastDate");
                }
            }
        }

        public void SetDates()
        {
            var selectedSymbols = SymbolModels.Where(s => s.Selected).ToList();
            var displayDates = TickFileModels.Join(
                selectedSymbols,
                tickFileModel => tickFileModel.Symbol.Symbol,
                symbolModel => symbolModel.Symbol,
                (model, symbolModel) => model.DateForFile).Distinct().ToList();

            // Set min and max dates:
            if (displayDates.Count != 0)
            {
                FirstDate = displayDates.Min();
                LastDate = displayDates.Max();
            }
            else if(TickFileModels.Count != 0)
            {
                FirstDate = TickFileModels.Select(m => m.DateForFile).Min();
                LastDate = TickFileModels.Select(m => m.DateForFile).Max();
            }
            AvailableDates = new ObservableCollection<DateTime>(displayDates);
        }

        public void LoadPath(string path)
        {
            TickFileModels.Clear();
            // Load tick files:
            LoadFiles(path);
            // Get unique symbols:
            var symbolModels =
                TickFileModels.Select(m => m.Symbol).Distinct((s1, s2) => s1.Symbol == s2.Symbol).ToList();
            SymbolModels = new ObservableCollection<SymbolModel>(symbolModels);
            SetDates();
        }

        private void LoadFiles(string path)
        {
            var files = System.IO.Directory.GetFiles(path, "*.TIK");
            TickDataDirectory = path;
            
            // Load any tick files in this directory:
            foreach (var file in files)
            {
                var shortName = System.IO.Path.GetFileName(file);
                if( tikFileRegex.IsMatch(shortName) )
                {
                    var tickInfo = TickFileNameInfo.GetTickFileInfoFromShortName(shortName);

                    //var name = System.IO.Path.GetFileNameWithoutExtension(shortName);
                    //var dateString = name.Substring(name.Length - 8);
                    //var symString = name.Substring(0, name.Length - 8);
                    //var year = Convert.ToInt32(dateString.Substring(0, 4));
                    //var month = Convert.ToInt32(dateString.Substring(4, 2));
                    //var day = Convert.ToInt32(dateString.Substring(6, 2));
                    //var date = new DateTime(year, month, day);

                    var date = new DateTime(tickInfo.Year,tickInfo.Month,tickInfo.Day);

                    var fileModel = new FileModel
                    {
                        LongFileName = file,
                        ShortFileName = shortName,
                    };

                    var symbolModel = new SymbolModel
                    {
                        Symbol = tickInfo.Symbol,
                        Selected = false,
                    };
                    symbolModel.PropertyChanged += (sender, args) =>
                    {
                        if(args.PropertyName == "Selected")
                        {
                           SetDates();
                        }
                    };

                    var tickFileModel = new TickFileModel
                    {
                        DateForFile = date,
                        File = fileModel,
                        Symbol = symbolModel,
                    };
                    TickFileModels.Add(tickFileModel);
                }
            }

            // Recursively load any tick files in subdirectories:
            var directories = System.IO.Directory.GetDirectories(path);
            foreach (var _directory in directories)
            {
                LoadFiles(_directory);
            }
        }

        

        //public IEnumerable<string> GetFilePaths(DateTime? firstDate, DateTime? lastDate)
        //{
        //    IEnumerable<string> paths;
        //    if (firstDate.HasValue && lastDate.HasValue)
        //    {
        //        var lowerBound = firstDate.Value.Ticks;
        //        var upperBound = lastDate.Value.Ticks;
        //        paths = TickFileModels
        //            .Where(file => lowerBound < file.DateForFile.Ticks && file.DateForFile.Ticks < upperBound)
        //            .Select(file => file.File.LongFileName);
        //    }
        //    else
        //    {
        //        paths = new List<string> { };
        //    }
        //    return paths;
        //}

        public List<List<string>> GetGroupedFilePaths(DateTime? firstDate, DateTime? lastDate)
        {
            List<List<string>> groups = new List<List<string>>{};
            if (firstDate.HasValue && lastDate.HasValue)
            {
                var lowerBound = firstDate.Value.Ticks;
                var upperBound = lastDate.Value.Ticks;
                var selectedSymbols = SymbolModels.Where(s => s.Selected).ToList();
                var fileModels = TickFileModels
                    .Where(file => lowerBound < file.DateForFile.Ticks && file.DateForFile.Ticks < upperBound);
                var symbolFilteredFileModels = fileModels.Join(
                    selectedSymbols,
                    outerModel => outerModel.Symbol.Symbol,
                    innerModel => innerModel.Symbol,
                    (outerModel, innerModel) => outerModel);
                var distinctDates = fileModels
                    .Distinct((model1, model2) => model1.DateForFile.Date == model2.DateForFile.Date)
                    .Select(model => model.DateForFile);
                foreach (var _dateTime in distinctDates)
                {
                    var dateGroup = symbolFilteredFileModels
                        .Where(model => model.DateForFile.Date == _dateTime.Date)
                        .Select(model => model.File.LongFileName).ToList();
                    groups.Add(dateGroup);
                }
            }
            return groups;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }

    public class TickFileNameInfo
    {
        public string Symbol { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public static TickFileNameInfo GetTickFileInfoFromShortName(string shortName)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(shortName);
            var dateString = name.Substring(name.Length - 8);
            var symString = name.Substring(0, name.Length - 8);
            var year = Convert.ToInt32(dateString.Substring(0, 4));
            var month = Convert.ToInt32(dateString.Substring(4, 2));
            var day = Convert.ToInt32(dateString.Substring(6, 2));

            return new TickFileNameInfo
            {
                Symbol = symString,
                Day = day,
                Month = month,
                Year = year,
            };
        }
    }
}