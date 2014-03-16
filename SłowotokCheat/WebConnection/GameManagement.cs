using SłowotokCheat.Models;
using SłowotokCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SłowotokCheat.WebConnection
{
    public class GameManagement : IDisposable
    {
        private const int TICK_INTERVAL_IN_MS = 1000;

        private int _intervalOfUpdatingStatus = 0;
        private GameStatus _status;
        private DispatcherTimer _timer = new DispatcherTimer();
        private bool _answersSent = false;

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
        public TimeSpan TimeLeft { get; private set; }
        public TimeSpan TimeToGameEnd { get; private set; }
        public TimeSpan TimeToGetResults { get; set; }

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

            if (TimeToGameEnd < TimeSpan.FromSeconds(-1) && TimeLeft > TimeSpan.FromSeconds(25) && !_answersSent)
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

        public async Task<AnswersResponse> SendAnswers(List<WordRecord> foundWords)
        {
            AnswersResponse response = await WebActions.SendAnswers(foundWords);
            TimeToGetResults = TimeSpan.FromMilliseconds(response.Time);

            return response;
        }

        private void UpdateBoard()
        {
            CurrentBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<Board>(WebActions.Client.DownloadString("play/board"));
            TimeToGameEnd = TimeSpan.FromMilliseconds(CurrentBoard.Time);
            UpdateStatus();
            OnBoardChanged();
        }

        private void UpdateStatus()
        {
            Status = Newtonsoft.Json.JsonConvert.DeserializeObject<GameStatus>(WebActions.Client.DownloadString("play/status"));
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
    }
}
