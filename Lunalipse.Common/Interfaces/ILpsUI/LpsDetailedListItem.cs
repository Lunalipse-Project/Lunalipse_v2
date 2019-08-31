using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.ILpsUI
{
    public class LpsDetailedListItem : INotifyPropertyChanged
    {
        string ddesc;
        public string DetailedIcon { get; set; }
        public string DetailedDescription
        {
            get => ddesc;
            set
            {
                ddesc = value;
                NotifyPropertyChanged();
            }
        }
        public string I18NDescription { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
