using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Columns
{
    public partial class GameWindow : INotifyPropertyChanged
    {
        private readonly double _boxWidth;
        private readonly double _boxHeight;
        private readonly Storyboard _downStoryboard;
        private ColumnItemsControl _currentColumn;
        private int _currentSpeedRatio = 1;
        private double _rowDropTime;
        private readonly object _storyBoardLock = new object();
        private int _lastScore;
        private int _completionCount;
        private string _newName;
        private bool _showOverlay;
        private bool _showOnlineScoresOverlay;
        private CanvasColumnItemsControl[] _columns;
        private bool _paused;

        public GameManager GameManager { get; }

        public GameWindow()
        {
            GameManager = new GameManager(8, 16, 8);
            Scoreboard = new ObservableCollection<ScoreboardEntry>(GameManager.Scoreboard);
            OnlineScoreboard = new ObservableCollection<ScoreboardEntry>(GameManager.OnlineScoreBoard);
            InitializeComponent();
            GameManager.PropertyChanged += GameManagerPropertyChanged;
            var canvasHeight = gameCanvas.Height;
            _boxWidth = gameCanvas.Width/GameManager.NumberOfColumns;
            _boxHeight = canvasHeight / GameManager.MaxRows;
            _rowDropTime = 0.5;
            _downStoryboard = new Storyboard();
            _downStoryboard.Completed += DownStoryboard_Completed;

            DataContext = this;
            Loaded += GameWindow_Loaded;
            KeyDown += GameWindowKeyDown;
            KeyUp += GameWindowKeyUp;
            startButton.Click += StartButtonClick;
            okButton.Click += OkButtonClick;
            okScoresButton.Click += OkScoresButtonClick;
            showOnlineScoresButton.Click += ShowOnlineScoresButtonClick;
            submitScoresButton.Click += SubmitScoresButtonClick;
        }

        public string NewName
        {
            get { return _newName;} set
            {
                if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, "^[A-Z0-9a-z _]*$"))
                {
                    _newName = value;
                    NotifyPropertyChanged("NewName");
                }else
                {
                    throw new ArgumentException("You can only use letters and numbers in your name");
                }
                
            }
        }
        public double ColumnHeight
        {
            get { return 30*4; }
        }
        public Visibility GameoverPopupVisibility
        {
            get { return _showOverlay ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility OnlineScoresPopupVisibility
        {
            get { return _showOnlineScoresOverlay ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility PausedPopupVisibility
        {
            get { return _paused ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility HasHighScoreVisibility
        {
            get { return GameManager.HasHighScore ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ObservableCollection<ScoreboardEntry> Scoreboard { get; }

        public ObservableCollection<ScoreboardEntry> OnlineScoreboard { get; }

        private void ReloadScores()
        {
            GameManager.OnlineScoreBoard.ReloadFromServer();
            OnlineScoreboard.Clear();
            foreach (ScoreboardEntry entry in GameManager.OnlineScoreBoard) OnlineScoreboard.Add(entry);
        }

        private void SubmitScoresButtonClick(object sender, RoutedEventArgs e)
        {
            GameManager.Scoreboard.SaveToServer();
            ReloadScores();
        }

        private void OkScoresButtonClick(object sender, RoutedEventArgs e)
        {
            _showOnlineScoresOverlay = false;
            NotifyPropertyChanged("OnlineScoresPopupVisibility");
        }

        private void ShowOnlineScoresButtonClick(object sender, RoutedEventArgs e)
        {
            ReloadScores();
            _showOnlineScoresOverlay = true;
            NotifyPropertyChanged("OnlineScoresPopupVisibility");
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            _showOverlay = false;
            NotifyPropertyChanged("GameoverPopupVisibility");
            GameManager.AddScore(NewName);
            startButton.IsEnabled = true;

        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void GameManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GameOver")
            {
                if (GameManager.GameOver)
                {
                    _showOverlay = true;
                }
                else
                {
                    startButton.IsEnabled = false;
                }
                NotifyPropertyChanged("GameoverPopupVisibility");
                NotifyPropertyChanged("HasHighScoreVisibility");

            }
            if (e.PropertyName == "Scoreboard")
            {
                Scoreboard.Clear();
                GameManager.Scoreboard.ForEach(score => Scoreboard.Add(score));
            }
        }

        private void GameWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (!GameManager.GameOver)
            {
                if (e.Key == Key.Left)
                    MoveLeft();
                if (e.Key == Key.Right)
                    MoveRight();
                if (e.Key == Key.Down)
                    SpeedUp();
                if (e.Key == Key.Up || e.Key == Key.LeftCtrl)
                    ColourRotate();
                if (e.Key == Key.P)
                {
                    if (_paused)
                    {
                        gameCanvas.Visibility = Visibility.Visible;
                        _paused = false; RestartDownAnimation();
                    }
                    else
                    {
                        gameCanvas.Visibility = Visibility.Hidden;
                        _paused = true;
                    }
                    NotifyPropertyChanged("PausedPopupVisibility");
                }
            }
        }

        private void GameWindowKeyUp(object sender, KeyEventArgs e)
        {
                if (e.Key == Key.Down)
                    SpeedUpCompleted();
        }
        
        private void StartGame()
        {
            GameManager.NewGame();
            gameCanvas.Children.Clear();
            _columns = new CanvasColumnItemsControl[GameManager.NumberOfColumns];
            for(int i = 0; i< GameManager.NumberOfColumns; i++)
            {
                _columns[i] = new CanvasColumnItemsControl(GameManager.GetColumn(i), _boxWidth, _boxHeight, GameManager.MaxRows, BoxAnimationsCompleted);
                gameCanvas.Children.Add(_columns[i]);
                _columns[i].SetValue(Canvas.LeftProperty, i * _boxWidth);
                _columns[i].SetValue(Canvas.TopProperty, -_boxHeight * GameManager.MaxRows);
            }
            StartColumnDrop();
        }

        private void GameWindow_Loaded(object obj, RoutedEventArgs args)
        {

            //StartGame();
            
        }
        private void MoveLeft()
        {
            if (GameManager.CanMoveLeft())
            {
                _currentColumn.Column.ColumnNumber--;
                _currentColumn.SetValue(Canvas.LeftProperty, _currentColumn.Column.ColumnNumber * _boxWidth);
            }
        }

        private void MoveRight()
        {
            if (GameManager.CanMoveRight())
            {
                _currentColumn.Column.ColumnNumber++;
                _currentColumn.SetValue(Canvas.LeftProperty, _currentColumn.Column.ColumnNumber * _boxWidth);
            }
        }

        private void SpeedUp()
        {
            lock (_storyBoardLock)
            {
                _currentSpeedRatio = 3;
                _downStoryboard.SetSpeedRatio(this, _currentSpeedRatio);
            }
        }

        private void SpeedUpCompleted()
        {
            lock (_storyBoardLock)
            {
                _currentSpeedRatio = 1;
            }
        }

        private void ColourRotate()
        {
            _currentColumn.Column.Rotate();
        }

        private void StartColumnDrop()
        {
               
                _currentColumn = new ColumnItemsControl(GameManager.NextColumn(), _boxWidth, _boxHeight);
                if (!GameManager.GameOver)
                {
                    if (GameManager.TotalScore > 1000) _rowDropTime = 0.35;
                    else if (GameManager.TotalScore > 500) _rowDropTime = 0.4;
                    else if (GameManager.TotalScore > 100) _rowDropTime = 0.45;

                    nextColumnDisplay.Children.Clear();
                    nextColumnDisplay.Children.Add(new ColumnItemsControl(GameManager.NextColumnDisplay, 30, 30));
                    gameCanvas.Children.Add(_currentColumn);
                    _currentColumn.SetValue(Canvas.LeftProperty, _boxWidth*_currentColumn.Column.ColumnNumber);
                    RestartDownAnimation();
                }

        }

        private void RestartDownAnimation()
        {
            lock (_storyBoardLock)
            {
                if (!_paused)
                {
                    _downStoryboard.Children.Clear();
                    double currentPosition = (GameManager.MaxRows - (_currentColumn.Column.RowNumber + 4))*_boxHeight;
                    _currentColumn.Column.RowNumber--;
                    double nextPosition = (GameManager.MaxRows - (_currentColumn.Column.RowNumber + 4))*_boxHeight;
                    var downAnimation = new DoubleAnimation(currentPosition, nextPosition,
                                                            new Duration(TimeSpan.FromSeconds(_rowDropTime)));
                    Storyboard.SetTarget(downAnimation, _currentColumn);
                    Storyboard.SetTargetProperty(downAnimation, new PropertyPath(Canvas.TopProperty));
                    _downStoryboard.Children.Add(downAnimation);
                    _downStoryboard.Begin(this, true);
                    _downStoryboard.SetSpeedRatio(this, _currentSpeedRatio);
                }
            }
        }

        private void DownStoryboard_Completed(object sender, EventArgs e)
        {
            lock (_storyBoardLock)
            {
                if (GameManager.HasFallCompleted())
                {
                    _completionCount = 0;
                    _lastScore = GameManager.FallCompleted();
                    gameCanvas.Children.Remove(_currentColumn);
                    gameCanvas.UpdateLayout();
                    if (_lastScore == 0) StartColumnDrop();
                }
                else
                {
                    RestartDownAnimation();
                }
            }
        }

        private void BoxAnimationsCompleted(object sender, EventArgs e)
        {
            lock (_storyBoardLock)
            {
                _completionCount++;
                if (_completionCount == _lastScore)
                {
                    _completionCount = 0;
                    _lastScore = GameManager.CheckForMatchesOnAllColumns();
                    if (_lastScore == 0) StartColumnDrop();
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
