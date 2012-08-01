using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DarkLight.Utilities;
using TradeLink.API;
using TradeLink.AppKit;

namespace DarkLight.Analytics.Models
{
    public class OptimizationConfigurationModel : INotifyPropertyChanged
    {
        

        private AdjustableProperty _adjustableProperty1;
        public AdjustableProperty AdjustableProperty1
        {
            get { return _adjustableProperty1; }
            set
            {
                if (value != _adjustableProperty1)
                {
                    _adjustableProperty1 = value;
                    NotifyPropertyChanged("AdjustableProperty1");
                }
            }
        }

        private PlottableValue _selectedPlottableValue;
        public PlottableValue SelectedPlottableValue
        {
            get { return _selectedPlottableValue; }
            set
            {
                if (value != _selectedPlottableValue)
                {
                    _selectedPlottableValue = value;
                    NotifyPropertyChanged("SelectedPlottableValue");
                }
            }
        }

        int _numberUniformSamples = 10;
        public int NumberUniformSamples
        {
            get { return _numberUniformSamples; }
            set
            {
                if (value != _numberUniformSamples)
                {
                    _numberUniformSamples = value;
                    NotifyPropertyChanged("NumberUniformSamples");

                }
            }
        }

        private PlayTo _selectedPlayToValue = PlayTo.End;
        public PlayTo SelectedPlayToValue
        {
            get { return _selectedPlayToValue; }
            set
            {
                if (value != _selectedPlayToValue)
                {
                    _selectedPlayToValue = value;
                    NotifyPropertyChanged("SelectedPlayToValue");
                }
            }
        }

        public IEnumerable<PlayTo> PlayToValues
        {
            get
            {
                var values = Enum.GetValues(typeof(PlayTo)).Cast<PlayTo>();
                SelectedPlayToValue = PlayTo.End;
                return values;
            }
        }

        Response _selectedResponse;
        public Response SelectedResponse
        {
            get { return _selectedResponse; }
            set
            {
                if(value != _selectedResponse)
                {
                    _selectedResponse = value;
                    NotifyPropertyChanged("SelectedResponse");
                    
                }
            }
        }

        ObservableCollection<AdjustableProperty> _adjustableProperties = new ObservableCollection<AdjustableProperty>();
        public ObservableCollection<AdjustableProperty> AdjustableProperties
        {
            get { return _adjustableProperties; }
            set
            {
                if (value != _adjustableProperties)
                {
                    _adjustableProperties = value;
                    NotifyPropertyChanged("AdjustableProperties");
                }
            }
        }

        ObservableCollection<PlottableValue> _plottableValues = new ObservableCollection<PlottableValue>();
        public ObservableCollection<PlottableValue> PlottableValues
        {
            get { return _plottableValues; }
            set
            {
                if (value != _plottableValues)
                {
                    _plottableValues = value;
                    NotifyPropertyChanged("PlottableValues");
                }
            }
        }

        public void PopulateAdjustableProperties()
        {
            if (_selectedResponse != null)
            {
                _adjustableProperties.Clear();
                var responseType = _selectedResponse.GetType();
                var adjustablePropertyList = PlottingUtilities.GetAllAdjustableProperties(responseType);
                var headerItem = new AdjustableProperty
                {
                    PropertyName = AdjustableProperty._header,
                };
                AdjustableProperty1 = headerItem;
                AdjustableProperties.Add(headerItem);
                foreach (var adjustableProperty in adjustablePropertyList)
                {
                    AdjustableProperties.Add(adjustableProperty);
                }
            }
        }

        public void PopulatePlottableValues()
        {
            PlottableValues.Clear();
            var plottableType = typeof(Results);
            var plottableList = PlottingUtilities.GetAllPlottableValues(plottableType);
            foreach (var plottableValue in plottableList)
            {
                PlottableValues.Add(plottableValue);
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
