using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SousChef.Models
{
    public class Converter : INotifyPropertyChanged
    {
        public string UnitGroup { get; set; }
        public string Units { get; set; }
        public string LeftIcon { get; set; }
        //public string RightIcon { get; set; }
        public string LeftName { get; set; }
        public string RightName { get; set; }
        public string LeftToRight { get; set; }
        public string RightToLeft { get; set; }

        private string rightIcon;

        public string RightIcon
        {
            get
            {
                return rightIcon;
            }
            set
            {
                rightIcon = value;
                RaisePropertyChangeEvent(nameof(RightIcon));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChangeEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
