using System.ComponentModel;

namespace Columns
{
    public class Box : INotifyPropertyChanged
    {
        private bool _collapsed;
        
        public Box(int type)
        {
            BoxType = type;
            Closing = false;
            Visited = false;
        }
        public int BoxType { get; }
        public bool Closing { get; set; }
        public bool Visited { get; set; }
        public bool Collapsed
        {
            get { return _collapsed; }
            set
            {
                if (!_collapsed)
                {
                    _collapsed = value;
                    NotifyPropertyChanged("Collapsed");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}