using System;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Columns
{
    /// <summary>
    /// Interaction logic for Column.xaml
    /// </summary>
    public partial class CanvasColumnItemsControl
    {
        public Column Column { get; }
        public double BoxWidth { get; }
        public double BoxHeight { get; }
        public double CurrentProgress { get; set; }
        public double ColumnHeight { get; set; }
        public CanvasColumnItemsControl(Column column, double width, double height, int maxRows, EventHandler lastAnimationCompleted) : base()
        {
            Column = column;
            BoxWidth = width;
            BoxHeight = height - 2;
            CurrentProgress = 0;
            ColumnHeight = height * (maxRows * 2);
            Resources.Add("Height", BoxHeight);
            InitializeComponent();
            DataContext = this;
            ((Storyboard)Resources["boxAnimation"]).Completed += lastAnimationCompleted;
        }
    }
}
