using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Columns
{

    public class ColumnBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class Column : ColumnBase
    {
        public ObservableCollection<Box> Boxes { get; } = new ObservableCollection<Box>();
    }

    public class FallingColumn : ColumnBase
    {
        public int ColumnNumber { get; set; }

        public int RowNumber { get; set; }

        public FallingColumn(int numberOfBoxTypes)
        {
            Random gen = new Random();
            Boxes[0] = new Box(gen.Next(0, numberOfBoxTypes - 1));
            Boxes[1] = new Box(gen.Next(0, numberOfBoxTypes - 1));
            Boxes[2] = new Box(gen.Next(0, numberOfBoxTypes - 1));
            Boxes[3] = new Box(gen.Next(0, numberOfBoxTypes - 1));
        }

        public Box[] Boxes { get; private set; } = new Box[4];

        public void Rotate()
        {
            Box[] temp = new Box[Boxes.Length];
            temp[0] = Boxes[3];
            for (int i = 0; i < Boxes.Length - 1; i++) temp[i + 1] = Boxes[i];
            Boxes = temp;
            NotifyPropertyChanged("Boxes");
        }

    }
}
