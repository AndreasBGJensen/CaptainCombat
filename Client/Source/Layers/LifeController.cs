
using CaptainCombat.Client.NetworkEvent;
using CaptainCombat.Client.Scenes;
using CaptainCombat.Client.Source.Layers.Events;
using CaptainCombat.Common;
using CaptainCombat.Common.Singletons;
using System.Collections.Generic;
using System.Threading;

namespace CaptainCombat.Client.Layers {


    class LifeController {

        private Dictionary<uint, int> lives = new Dictionary<uint, int>();

        public Dictionary<uint, int> Lives { get {
                var copy = new Dictionary<uint, int>();
                foreach (var pair in lives) copy.Add(pair.Key, pair.Value);
                return copy;
            }
        }

        private bool settingWinner = false;
        
        public Player Winner { get; set; } = null;
        public bool WinnerFound { get => Winner != null; }

        private Thread uploadWinnerThread;
        private Thread downloadWinnerThread;

        private EventController eventController;


        public LifeController(EventController eventController)
        {
            this.eventController = eventController;
        }
            

        public void Start(){

            // Set initial lives
            foreach (var client in GameInfo.Current.Clients)
                lives[client.Id] = Settings.LIVES;

            // Listen for life lost events
            eventController.AddListener<LifeLostEvent>((e) => {
                lives[e.Sender]--;
                
                // Check if client is winner
                var opponentsAlive = 0;
                foreach (var pair in lives)
                    if (pair.Key != Connection.Instance.User_id)
                        if (pair.Value > 0)
                            opponentsAlive++;

                if (opponentsAlive == 0)
                    UploadWinner((uint)Connection.Instance.User_id);

                return true;
            });
            
            // Start listening for winners
            ListenForWinner();
        }

        
        public void Stop() {
            downloadWinnerThread?.Abort();
            uploadWinnerThread?.Abort();
        }


        private void UploadWinner(uint winnerId) {
            // To ensure that the client isn't attempting to set
            // 2 winners at the same time
            if (settingWinner) return;
            settingWinner = true;

            uploadWinnerThread = new Thread((e) => {
                Connection.Instance.LobbySpace.Get("winner-lock");
                Connection.Instance.LobbySpace.Put("winner", (int)winnerId);
            });
            uploadWinnerThread.Priority = ThreadPriority.Highest;
            uploadWinnerThread.Start();
        }


        private void ListenForWinner() {
            downloadWinnerThread = new Thread((e) => {
                var tuple = Connection.Instance.LobbySpace.Query("winner", typeof(int));
                Winner = GameInfo.Current.GetPlayer((uint)(int)tuple[1]);
            });
            downloadWinnerThread.Priority = ThreadPriority.Highest;
            downloadWinnerThread.Start();
        }


        /// <summary>
        /// Decrements the local client's lives
        /// </summary>
        public int DecrementLocalLives() {
            var clientId = (uint)Connection.Instance.User_id;
            if (lives[clientId] == 0) return 0;
            lives[clientId]--;

            // Send life lost event
            foreach (var client in GameInfo.Current.Clients)
                if (!client.IsLocal)
                    eventController.Send(new LifeLostEvent(client.Id));

            return lives[clientId];
        }


        public int GetClientLives(uint clientId) {
            return lives[clientId];
        }


        public int GetLocalClientLives() {
            return GetClientLives((uint)Connection.Instance.User_id);
        }

    }
}
