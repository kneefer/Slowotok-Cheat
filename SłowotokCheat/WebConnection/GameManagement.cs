using SłowotokCheat.Models;
using SłowotokCheat.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Net;

namespace SłowotokCheat.WebConnection
{
    public class GameManagement : IDisposable, INotifyPropertyChanged
    {
        private const int TICK_INTERVAL_IN_MS = 1000;

        private int _intervalOfUpdatingStatus = 0;
        private int _intervalOfUpdatingGameTime = 0;
        private GameStatus _status;
        private DispatcherTimer _timer = new DispatcherTimer();
        private bool _answersSent = false;

        private TimeSpan _timeLeft;
        private TimeSpan _timeToGameEnd;
        private TimeSpan _timeToGetResults;

        public SlowotokWebActions WebActions { get; set; }
        public Board CurrentBoard { get; private set; }
        public GameStatus Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    TimeLeft = TimeSpan.FromMilliseconds(_status.Time);
                }
            }
        }
        public TimeSpan TimeLeft
        {
            get { return _timeLeft; }
            private set
            {
                if (value != _timeLeft)
                {
                    _timeLeft = value;
                    NotifyPropertyChanged("TimeLeft");
                }
            }
        }
        public TimeSpan TimeToGameEnd
        {
            get { return _timeToGameEnd; }
            private set
            {
                if (value != _timeToGameEnd)
                {
                    _timeToGameEnd = value;
                    NotifyPropertyChanged("TimeToGameEnd");
                }
            }
        }
        public TimeSpan TimeToGetResults
        {
            get { return _timeToGetResults; }
            private set
            {
                if (value != _timeToGetResults)
                {
                    _timeToGetResults = value;
                    NotifyPropertyChanged("TimeToGetResults");
                }
            }
        }

        public GameManagement()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(TICK_INTERVAL_IN_MS);
        }

        private async void _timer_Tick(object sender, EventArgs e)
        {
            TimeLeft -= _timer.Interval;
            TimeToGameEnd -= _timer.Interval;
            TimeToGetResults -= _timer.Interval;

            if (_intervalOfUpdatingStatus++ == 5)
            {
                UpdateStatus();
                _intervalOfUpdatingStatus = 0;
            }

            if(_intervalOfUpdatingGameTime++ == 10)
            {
                UpdateGameTime();
                _intervalOfUpdatingGameTime = 0;
            }

            if (TimeToGameEnd < TimeSpan.Zero && TimeLeft > TimeSpan.FromSeconds(25) && !_answersSent)
            {
                OnSendAnswerGotPossible();
                _answersSent = true;
            }

            if (TimeLeft < TimeSpan.FromSeconds(2))
            {
                _timer.Stop();
                await Task.Delay(2000);
                UpdateBoard();
                _timer.Start();
                _answersSent = false;
            }
        }

        private void UpdateGameTime()
        {
            
        }

        public async Task<AnswersResponse> SendAnswers(List<WordRecord> foundWords)
        {
            AnswersResponse response;

            if ((response = await WebActions.SendAnswers(foundWords)) == null)
                return null;

            TimeToGetResults = TimeSpan.FromMilliseconds(response.Time);
            return response;
        }

        private async void UpdateBoard()
        {
            Board response;
            if ((response = await WebActions.ReceiveStringAsync<Board>("play/board")) == null)
                return;
            CurrentBoard = response;
            TimeToGameEnd = TimeSpan.FromMilliseconds(CurrentBoard.Time);
            UpdateStatus();
            OnBoardChanged();
        }

        private async void UpdateStatus()
        {
            GameStatus response;
            if ((response = await WebActions.ReceiveStringAsync<GameStatus>("play/status")) == null)
                return;
             Status = response;
        }

        public void StartAutomation()
        {
            UpdateBoard();
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        public void StopAutomation()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
        }

        public event BoardChangedEventHandler BoardChanged;
        public event SendAnswerGotPossibleEventHandler SendAnswerGotPossible;
        
        private void OnBoardChanged()
        {
            if (BoardChanged != null)
                BoardChanged(this, new BoardEventArgs(CurrentBoard));
        }
        private void OnSendAnswerGotPossible()
        {
            if (SendAnswerGotPossible != null)
                SendAnswerGotPossible(this, new EventArgs());
        }
        public void Dispose()
        {
            StopAutomation();
            if (WebActions != null) WebActions.Dispose();
            WebActions = null;
        }

        #region Events Area

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
