using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DarkLight.Utilities
{
    public class AdjustableProperty : INotifyPropertyChanged
    {
        public const string _header = "-- Select Property Name --";

        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (value != _propertyName)
                {
                    _propertyName = value;
                    NotifyPropertyChanged("PropertyName");
                }
            }
        }

        private Type _propertyType;
        public Type PropertyType
        {
            get { return _propertyType; }
            set
            {
                if (value != _propertyType)
                {
                    _propertyType = value;
                    SetMinMax();
                    NotifyPropertyChanged("PropertyType");
                }
            }
        }

        private bool _selected;
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

        private decimal _currentValue;
        public decimal CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (value != _currentValue)
                {
                    _currentValue = value;
                    NotifyPropertyChanged("CurrentValue");
                }
            }
        }
        
        private decimal _min = decimal.MinValue;
        public decimal Min
        {
            get { return _min; }
            set
            {
                if (value != _min)
                {
                    _min = value;
                    NotifyPropertyChanged("Min");
                }
            }
        }

        private decimal _max = decimal.MaxValue;
        public decimal Max
        {
            get { return _max; }
            set
            {
                if (value != _max)
                {
                    _max = value;
                    NotifyPropertyChanged("Max");
                }
            }
        }

        private void SetMinMax()
        {
            if (_propertyType == typeof(int))
            {
                Min = Convert.ToDecimal(int.MinValue);
                Max = Convert.ToDecimal(int.MaxValue);
            }
            else if (_propertyType == typeof(double))
            {
                Min = Convert.ToDecimal(double.MinValue);
                Max = Convert.ToDecimal(double.MaxValue);
            }
        }

        public List<decimal> GetRange(int numberIntervals)
        {
            decimal range = _max - _min;
            decimal step = range / numberIntervals;
            return Enumerable.ToList<decimal>(Range.Decimal(_min, _max, step));
        }

        public void SetCurrentValue(object instance)
        {
            if (_propertyName != _header)
            {
                var responseType = instance.GetType();
                var propertyInfo = responseType.GetProperty(PropertyName);
                if (_propertyType == typeof (int))
                {
                    var val = Convert.ToInt32(CurrentValue);
                    propertyInfo.SetValue(instance, val, null);
                }
                else if (_propertyType == typeof (decimal))
                {
                    var val = CurrentValue;
                    propertyInfo.SetValue(instance, val, null);
                }
                else if (_propertyType == typeof (double))
                {
                    var val = Convert.ToDouble(CurrentValue);
                    propertyInfo.SetValue(instance, val, null);
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

    //public class AdjustableInteger : INotifyPropertyChanged
    //{
    //    private string _propertyName;
    //    public string PropertyName
    //    {
    //        get { return _propertyName; }
    //        set
    //        {
    //            if (value != _propertyName)
    //            {
    //                _propertyName = value;
    //                NotifyPropertyChanged("PropertyName");
    //            }
    //        }
    //    }

    //    private bool _checked;
    //    public bool Checked
    //    {
    //        get { return _checked; }
    //        set
    //        {
    //            if (value != _checked)
    //            {
    //                _checked = value;
    //                NotifyPropertyChanged("Checked");
    //            }
    //        }
    //    }

    //    private int _value;
    //    public int Value
    //    {
    //        get { return _value; }
    //        set
    //        {
    //            if (value != _value)
    //            {
    //                _value = value;
    //                NotifyPropertyChanged("Value");
    //            }
    //        }
    //    }

    //    private int _min = int.MinValue;
    //    public int Min
    //    {
    //        get { return _min; }
    //        set
    //        {
    //            if (value != _min)
    //            {
    //                _min = value;
    //                NotifyPropertyChanged("Min");
    //            }
    //        }
    //    }

    //    private int _max = int.MaxValue;
    //    public int Max
    //    {
    //        get { return _max; }
    //        set
    //        {
    //            if (value != _max)
    //            {
    //                _max = value;
    //                NotifyPropertyChanged("Max");
    //            }
    //        }
    //    }

    //    public List<int> GetRange(int numberIntervals)
    //    {
    //        int range = _max - _min;
    //        int step = range / numberIntervals;
    //        return Range.Int32(_min, _max, step).ToList();
    //    }

    //    public void BindValueToResponse(Response response)
    //    {
    //        var responseType = response.GetType();
    //        var propertyInfo = responseType.GetProperty(PropertyName);
    //        propertyInfo.SetValue(response, _value, null);
    //    }

    //    #region INotifyPropertyChanged
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected void NotifyPropertyChanged(String info)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(info));
    //        }
    //    }
    //    #endregion
    //}

    //public class AdjustableDouble : INotifyPropertyChanged
    //{
    //    private string _propertyName;
    //    public string PropertyName
    //    {
    //        get { return _propertyName; }
    //        set
    //        {
    //            if (value != _propertyName)
    //            {
    //                _propertyName = value;
    //                NotifyPropertyChanged("PropertyName");
    //            }
    //        }
    //    }

    //    private bool _checked;
    //    public bool Checked
    //    {
    //        get { return _checked; }
    //        set
    //        {
    //            if (value != _checked)
    //            {
    //                _checked = value;
    //                NotifyPropertyChanged("Checked");
    //            }
    //        }
    //    }

    //    private double _value;
    //    public double Value
    //    {
    //        get { return _value; }
    //        set
    //        {
    //            if (value != _value)
    //            {
    //                _value = value;
    //                NotifyPropertyChanged("Value");
    //            }
    //        }
    //    }

    //    private double _min = double.MinValue;
    //    public double Min
    //    {
    //        get { return _min; }
    //        set
    //        {
    //            if (value != _min)
    //            {
    //                _min = value;
    //                NotifyPropertyChanged("Min");
    //            }
    //        }
    //    }

    //    private double _max = double.MaxValue;
    //    public double Max
    //    {
    //        get { return _max; }
    //        set
    //        {
    //            if (value != _max)
    //            {
    //                _max = value;
    //                NotifyPropertyChanged("Max");
    //            }
    //        }
    //    }

    //    public List<double> GetRange(int numberIntervals)
    //    {
    //        double range = _max - _min;
    //        double step = range / numberIntervals;
    //        return Range.Double(_min, _max, step).ToList();
    //    }

    //    public void BindValueToResponse(Response response)
    //    {
    //        var responseType = response.GetType();
    //        var propertyInfo = responseType.GetProperty(PropertyName);
    //        propertyInfo.SetValue(response, _value, null);
    //    }

    //    #region INotifyPropertyChanged
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected void NotifyPropertyChanged(String info)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(info));
    //        }
    //    }
    //    #endregion
    //}

    //public class AdjustableDecimal : INotifyPropertyChanged
    //{
    //    private string _propertyName;
    //    public string PropertyName
    //    {
    //        get { return _propertyName; }
    //        set
    //        {
    //            if (value != _propertyName)
    //            {
    //                _propertyName = value;
    //                NotifyPropertyChanged("PropertyName");
    //            }
    //        }
    //    }

    //    private bool _checked;
    //    public bool Checked
    //    {
    //        get { return _checked; }
    //        set
    //        {
    //            if (value != _checked)
    //            {
    //                _checked = value;
    //                NotifyPropertyChanged("Checked");
    //            }
    //        }
    //    }

    //    private decimal _value;
    //    public decimal Value
    //    {
    //        get { return _value; }
    //        set
    //        {
    //            if (value != _value)
    //            {
    //                _value = value;
    //                NotifyPropertyChanged("Value");
    //            }
    //        }
    //    }

    //    private decimal _min = decimal.MinValue;
    //    public decimal Min
    //    {
    //        get { return _min; }
    //        set
    //        {
    //            if (value != _min)
    //            {
    //                _min = value;
    //                NotifyPropertyChanged("Min");
    //            }
    //        }
    //    }

    //    private decimal _max = decimal.MaxValue;
    //    public decimal Max
    //    {
    //        get { return _max; }
    //        set
    //        {
    //            if (value != _max)
    //            {
    //                _max = value;
    //                NotifyPropertyChanged("Max");
    //            }
    //        }
    //    }

    //    public List<decimal> GetRange(int numberIntervals)
    //    {
    //        decimal range = _max - _min;
    //        decimal step = range / numberIntervals;
    //        return Range.Decimal(_min, _max, step).ToList();
    //    }

    //    public void BindValueToResponse(Response response)
    //    {
    //        var responseType = response.GetType();
    //        var propertyInfo = responseType.GetProperty(PropertyName);
    //        propertyInfo.SetValue(response, _value, null);
    //    }

    //    #region INotifyPropertyChanged
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected void NotifyPropertyChanged(String info)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(info));
    //        }
    //    }
    //    #endregion
    //}
}
