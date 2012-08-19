using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TradeLink.Common;

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
}