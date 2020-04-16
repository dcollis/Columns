using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Columns
{
    /// <summary>
    /// Interaction logic for Column.xaml
    /// </summary>
    public partial class ColumnItemsControl : ItemsControl, INotifyPropertyChanged
    {
        
        public ColumnItemsControl(FallingColumn column, double width, double height) : base()
        {
            InitializeComponent();
            Column = column;
            BoxWidth = width;
            BoxHeight = height -2;
            CurrentProgress = 0;
            ColumnHeight = height*Column.Boxes.Length;
            DataContext = this;
            
        }
        public FallingColumn Column { get; }
        public double BoxWidth { get; }
        public double BoxHeight { get; }
        public double CurrentProgress { get; set; }
        public double ColumnHeight { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
