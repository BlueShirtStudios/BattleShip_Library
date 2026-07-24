namespace BattleEventArgs
{
    public class ErrorEventArgs : EventArgs
    {
        public string Msg { get; private set; }
        public Exception Error { get; private set; }

        public ErrorEventArgs(string cMsg, Exception cError)
        {
            Msg = cMsg;
            Error = cError;
        }
    }
}