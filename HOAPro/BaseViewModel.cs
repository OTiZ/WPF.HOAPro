using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HOAPro
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        Lookup<string, string> DependentProperties;
        public BaseViewModel()
        {
            List<KeyValuePair<string, string>> _properties = new List<KeyValuePair<string, string>>();
            foreach (var property in GetType().GetProperties())
            {
                foreach (var d in (DependsUponAttribute[])property.GetCustomAttributes(typeof(DependsUponAttribute), true))
                {
                    _properties.Add(new KeyValuePair<string, string>(d.DependancyName, property.Name));
                }
            }

            DependentProperties = (Lookup<string, string>)_properties.ToLookup(p => p.Key, p => p.Value);

        }

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
                foreach (var prop in DependentProperties[property])
                    OnPropertyChanged(prop);

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        protected class DependsUponAttribute : Attribute
        {
            public string DependancyName { get; private set; }

            public DependsUponAttribute(string propertyName)
            {
                DependancyName = propertyName;
            }
        }

    }
}
