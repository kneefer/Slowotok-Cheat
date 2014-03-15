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
        private DispatcherTimer _timer = new DispatcherTimer();
        public SlowotokWebActions WebActions { get; set; }
        public Board CurrentBoard { get; private set; }
        public GameStatus Status { get; set; }

        public GameManagement()
        {
            _timer.Tick += (sender, e) => e.ToString();
        }

        private void UpdateBoard()
        {
            CurrentBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<Board>(WebActions.Client.DownloadString("play/board"));
        }

        private void UpdateStatus()
        {
            Status = Newtonsoft.Json.JsonConvert.DeserializeObject<GameStatus>(WebActions.Client.DownloadString("play/status"));
        }

        public void StartAutomation()
        {
            UpdateStatus();
        }

        public void StopAutomation()
        {

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
