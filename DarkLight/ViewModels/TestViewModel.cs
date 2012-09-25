using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using DarkLight.Views;

namespace DarkLight.ViewModels
{
    public class TestViewModel : Conductor<Screen>.Collection.OneActive
    {
        string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set 
            { 
                message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public bool CanSayHello
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public void SayHello()
        {
            Message = "Hello " + Name + "!";
        }

        public TestViewModel()
        {
            //AttachView(new TestView(), ViewLocator.DefaultContext);
            //ViewLocator.InitializeComponent(
            //    GetView(ViewLocator.DefaultContext));
        }
    }
}
