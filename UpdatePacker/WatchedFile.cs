using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UpdatePacker
{
    public class WatchedFile
    {
        public ModifiedState State;
        public string FileName;
        public string AbsolutePath;

        public string ModifyTypeIdentifier
        {
            get
            {
                switch(State)
                {
                    case ModifiedState.ADD:
                        return "++";
                    case ModifiedState.DELETED:
                        return "--";
                    case ModifiedState.REPLACE:
                        return "**";
                    case ModifiedState.RUN:
                        return ">>";
                    default:
                        return string.Empty;
                }
            }
        }

        public SolidColorBrush ModifyTypeColor
        {
            get
            {
                switch (State)
                {
                    case ModifiedState.ADD:
                        return new SolidColorBrush(Color.FromArgb(255, 212, 237, 218));
                    case ModifiedState.DELETED:
                        return new SolidColorBrush(Color.FromArgb(255, 248, 215, 218));
                    case ModifiedState.REPLACE:
                        return new SolidColorBrush(Color.FromArgb(255, 255, 243, 205));
                    case ModifiedState.RUN:
                        return new SolidColorBrush(Color.FromArgb(255, 204, 229, 255));
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        public string ModifiedFileName
        {
            get => FileName;
        }
    }

    public enum ModifiedState
    {
        ADD, //++
        REPLACE, //**
        DELETED, //--
        RUN  //>>
    }
}
