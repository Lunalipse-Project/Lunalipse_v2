using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace UpdatePacker
{
    public class WatchedFile : INotifyPropertyChanged
    {
        private ModifiedState _State;
        public string FileName;
        public string AbsolutePath;
        [XmlElementAttribute(DataType = "dateTime")]
        public DateTime LastModify;

        public ModifiedState State
        {
            get => _State;
            set
            {
                _State = value;
                NotifyPropertyChanged("ModifyTypeIdentifier");
                NotifyPropertyChanged("ModifyTypeColor");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public string ModifyTypeIdentifier
        {
            get
            {
                switch(_State)
                {
                    case ModifiedState.ADD:
                        return "++";
                    case ModifiedState.DELETED:
                        return "--";
                    case ModifiedState.CHANGED:
                        return "**";
                    case ModifiedState.RUN:
                        return ">>";
                    default:
                        return string.Empty;
                }
            }
        }
        
        [XmlIgnore]
        public SolidColorBrush ModifyTypeColor
        {
            get
            {
                switch (_State)
                {
                    case ModifiedState.ADD:
                        return new SolidColorBrush(Color.FromArgb(255, 212, 237, 218));
                    case ModifiedState.DELETED:
                        return new SolidColorBrush(Color.FromArgb(255, 248, 215, 218));
                    case ModifiedState.CHANGED:
                        return new SolidColorBrush(Color.FromArgb(255, 255, 243, 205));
                    case ModifiedState.RUN:
                        return new SolidColorBrush(Color.FromArgb(255, 204, 229, 255));
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        [XmlIgnore]
        public string ModifiedFileName
        {
            get => FileName;
        }
        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FileTreeCache
    {
        public string RootDirectory;
        public List<WatchedFile> watchedFiles;
    }

    public enum ModifiedState
    {
        ADD, //++
        CHANGED, //**
        DELETED, //--
        RUN,  //>>
        NOCHANGE //
    }
}
