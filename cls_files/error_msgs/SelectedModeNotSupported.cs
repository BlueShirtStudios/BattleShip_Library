namespace BattleExceptions
{
    public class ModeNotSupportedError : Exception
    {
        public ModeNotSupportedError(string msg) : base(msg) { }
    }
}