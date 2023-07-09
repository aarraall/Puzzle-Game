using Main.Scripts.Game.EventHandler;

namespace Main.Scripts.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public enum State
        {
            Loading,
            Home,
            Level,
            Pause,
            Quit
        }
        public GameEventHandler EventHandler;
        public State GameState { get; private set; }
        
        
        public override void Init()
        {
            base.Init();
            EventHandler = new GameEventHandler();
            SetState(State.Loading);
        }

        protected override void Dispose()
        {
            base.Dispose();
        }

        public void SetState(State state)
        {
            GameState = state;
            switch (state)
            {
                case State.Loading:
                    OnLoading();
                    break;
                case State.Home:
                    OnHome();
                    break;
                case State.Level:
                    OnLevel();
                    break;
                case State.Pause:
                    OnPause();
                    break;
                case State.Quit:
                    OnQuit();
                    break;
            }
        }

        private void OnLoading()
        {
            //Do stuff
            SetState(State.Home);
        }
        private void OnHome()
        {
        }
        private void OnLevel()
        {
            //let user play and listen goals, on goal is reached, carry user to home

        }
        private void OnPause()
        {
            //Do stuff

        }
        private void OnQuit()
        {
            //Do stuff

        }

        private void Reset()
        {
        }
    }
}
