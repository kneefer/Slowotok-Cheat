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
        private int _spanToWaitForNewBoard = 0;

        private DispatcherTimer _timer = new DispatcherTimer();
        private DispatcherTimer _newBoardWaitTimer = new DispatcherTimer();
        public SlowotokWebActions WebActions { get; set; }
        public Board CurrentBoard { get; private set; }

        private GameStatus _status;
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

        public GameManagement()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(TICK_INTERVAL_IN_MS);
            _newBoardWaitTimer.Interval = TimeSpan.FromSeconds(1);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            TimeLeft -= _timer.Interval;

            if (_intervalOfUpdatingStatus++ == 5)
            {
                UpdateStatus();
                _intervalOfUpdatingStatus = 0;
            }

            if (TimeLeft < TimeSpan.FromSeconds(2))
            {
                _timer.Stop();
                _newBoardWaitTimer.Tick += (b_sender, b_e) => {
                    if (_spanToWaitForNewBoard++ == 2)
                    {
                        _spanToWaitForNewBoard = 0;
                        _newBoardWaitTimer.Stop();
                        UpdateBoard();
                        _timer.Start();
                    }
                };
                _newBoardWaitTimer.Start();
            }
        }

        private void UpdateBoard()
        {
            CurrentBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<Board>(WebActions.Client.DownloadString("play/board"));
            TimeLeft = TimeSpan.FromMilliseconds(CurrentBoard.Time);
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

        public void OnBoardChanged()
        {
            if (BoardChanged != null)
                BoardChanged(this, new BoardChangedEventArgs(CurrentBoard));
        }
        public void Dispose()
        {
            StopAutomation();
            if (WebActions != null) WebActions.Dispose();
            WebActions = null;
        }
    }
}
