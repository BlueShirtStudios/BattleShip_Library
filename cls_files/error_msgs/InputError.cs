namespace BattleExceptions
{
    public class InputException : Exception
    {
        public object ExpectedObject { get; private set; }
        public object RecievedObject { get; private set; }

        public InputException(object ex, object re, string msg) : base(msg)
        {
            ExpectedObject = ex;
            RecievedObject = re;
        }

        
    }
}