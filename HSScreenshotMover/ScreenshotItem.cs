using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HSScreenshotMover
{
    public class ScreenshotItem : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
        bool _isMoved = false;
        String _path, _filename;

        public bool isMoved
        {
            get
            {
                return _isMoved;
            }
            private set
            {
                _isMoved = value;
                RaisePropertyChanged();
                RaisePropertyChanged("canMove");
            }
        }

        public bool canMove
        {
            get
            {
                return !_isMoved;
            }
        }

        public String FileName
        {
            get
            {
                return _filename;
            }
        }

        public String FullPath
        {
            get
            {
                return _path;
            }
        }

        public ScreenshotItem(String path)
        {
            _path = path;
            _filename = System.IO.Path.GetFileName(path);
        }

        public bool MoveFile(String newpath)
        {
            try
            {
                String target = System.IO.Path.Combine(newpath, _filename);
                System.IO.File.Move(_path, target);
                _path = target;
                isMoved = true;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ScreenshotList
    {
        public ObservableCollection<ScreenshotItem> Moved
        {
            get;
            private set;
        }

        public ObservableCollection<ScreenshotItem> Unmoved
        {
            get;
            private set;
        }

        public ScreenshotList()
        {
            Moved = new ObservableCollection<ScreenshotItem>();
            Unmoved = new ObservableCollection<ScreenshotItem>();
        }

        public bool MoveFile(String newpath, ScreenshotItem file)
        {
            if (Unmoved.Contains(file))
            {
                if (file.MoveFile(newpath))
                {
                    Unmoved.Remove(file);
                    Moved.Add(file);
                    return true;
                }
            }
            return false;
        }

        public ScreenshotItem AddFile(String path)
        {
            var item = new ScreenshotItem(path);
            Unmoved.Add(item);
            return item;
        }
    }
}
