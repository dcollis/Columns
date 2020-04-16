using System.ComponentModel;

namespace Columns
{
    public class GameManager : INotifyPropertyChanged
    {
        private readonly int _numberOfBlockTypes;
        private int _totalScore;
        private Column[] _columns;
        private bool _gameOver = true;
        private FallingColumn _currentColumn;

        public GameManager(int columns, int maxRows, int numberOfBlockTypes)
        {
            MaxRows = maxRows;
            NumberOfColumns = columns;
            _numberOfBlockTypes = numberOfBlockTypes;
            Scoreboard = Scoreboard.Load("Columns.Scoreboard.txt");
            OnlineScoreBoard = new Scoreboard();
        }

        public Scoreboard OnlineScoreBoard { get; }
        public Scoreboard Scoreboard { get; }
        public FallingColumn NextColumnDisplay { get; private set; }

        public int TotalScore
        {
            get { return _totalScore; }
            set { _totalScore = value; NotifyPropertyChanged("TotalScore"); }
        }

        public bool GameOver
        {
            get { return _gameOver; }
            set
            {
                _gameOver = value;
                NotifyPropertyChanged("GameOver");
            }
        }

        public bool HasHighScore
        {
            get { return Scoreboard.IsHighScore(_totalScore); }
        }

        public int NumberOfColumns { get; }
        public int MaxRows { get; }

        public void AddScore (string name)
        {
            Scoreboard.Add(new ScoreboardEntry(name, TotalScore));
            Scoreboard.Save();
            NotifyPropertyChanged("Scoreboard");
        }

        public Column GetColumn(int i)
        {
            return _columns[i];
        }

        public void NewGame()
        {
            GameOver = false;
            _columns = new Column[NumberOfColumns];
            for (int i = 0; i < NumberOfColumns; i++ )
            {
                _columns[i] = new Column();
            }
            NextColumnDisplay = GenerateFallingColumn();
            _totalScore = 0;
        }
       
        public FallingColumn NextColumn()
        {
            _currentColumn = NextColumnDisplay;
            if(_columns[_currentColumn.ColumnNumber].Boxes.Count >= MaxRows) GameOver = true;
            NextColumnDisplay = GenerateFallingColumn();
            return _currentColumn;
        }

        public int FallCompleted()
        {
            for (int i = 0; i < _currentColumn.Boxes.Length; i++)
            {
                 _columns[_currentColumn.ColumnNumber].Boxes.Add(_currentColumn.Boxes[_currentColumn.Boxes.Length - 1 - i]);
            }
            return CheckForMatchesOnAllColumns();
        }
        
        public bool HasFallCompleted()
        {

            if (_currentColumn.RowNumber == 0 || _columns[_currentColumn.ColumnNumber].Boxes.Count >= _currentColumn.RowNumber) return true;
            return false;
        }

        public bool CanMoveLeft()
        {

            if (_currentColumn.ColumnNumber == 0 || _columns[_currentColumn.ColumnNumber - 1].Boxes.Count - 1 >= _currentColumn.RowNumber) return false;
            return true;
        }

        public bool CanMoveRight()
        {
            if (_currentColumn.ColumnNumber == NumberOfColumns - 1 || _columns[_currentColumn.ColumnNumber + 1].Boxes.Count - 1 >= _currentColumn.RowNumber) return false;
            return true;
        }

        public int CheckForMatchesOnAllColumns()
        {
            int score = 0;
            foreach (Column column in _columns)
            {
                for (int i = column.Boxes.Count - 1; i >= 0; i--)
                {
                    if (column.Boxes[i].Collapsed)
                        column.Boxes.RemoveAt(i);
                }
            }

            for (int i = 0; i < NumberOfColumns; i++ )
            {
                for (int j = 0; j < _columns[i].Boxes.Count; j++)
                {
                    Box currentBox = _columns[i].Boxes[j];
                    if (!currentBox.Collapsed)
                    {
                        score += StartCheck(i, j);
                    }
                }
            }
            TotalScore = TotalScore + score;
            return score;
        }

        private FallingColumn GenerateFallingColumn()
        {
            var column = new FallingColumn(_numberOfBlockTypes)
            {
                ColumnNumber = NumberOfColumns / 2,
                RowNumber = MaxRows
            };
            return column;
        }

        private int StartCheck(int column, int row)
        {
            int score = 0;
            Box currentBox = _columns[column].Boxes[row];
            currentBox.Visited = true;
            int level = 1;
            if (column < NumberOfColumns - 1)
            {
                if (row < _columns[column].Boxes.Count - 1 && _columns[column].Boxes[row + 1].BoxType == currentBox.BoxType)
                {
                    level++;
                }
                score += CheckSurroundingBoxes(column + 1, row, currentBox.BoxType, level);
                if (score == 0) level = 1;
            }
            if (row < _columns[column].Boxes.Count - 1)
            {
                if (column < NumberOfColumns - 1 && row < _columns[column+1].Boxes.Count -1 && _columns[column + 1].Boxes[row].BoxType == currentBox.BoxType)
                {
                    level++;
                }
            
                score += CheckSurroundingBoxes(column, row + 1, currentBox.BoxType, level);
            }
            if (score > 0)
            {
                currentBox.Collapsed = true; score++;
            }
            currentBox.Visited = false;
            return score;
        }

        private int CheckSurroundingBoxes(int column, int row, int type, int level)
        {
            int score = 0;
            if (_columns[column].Boxes.Count > row)
            {
                Box currentBox = _columns[column].Boxes[row];
                if (currentBox.BoxType == type && !currentBox.Collapsed && !currentBox.Visited)
                {
                    currentBox.Visited = true;
                    if (column > 0) score += CheckSurroundingBoxes(column - 1, row, type, level+1);
                    if (column < NumberOfColumns - 1) score += CheckSurroundingBoxes(column + 1, row, type, level+1);
                    if (row < _columns[column].Boxes.Count - 1) score += CheckSurroundingBoxes(column, row + 1, type, level+1);
                    if (row > 0) score += CheckSurroundingBoxes(column, row - 1, type, level+1);
                    if (level > 1 || score > 0)
                    {
                        currentBox.Collapsed = true;
                        score++;
                    }
                    currentBox.Visited = false;
                }
            }
            return score;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
