namespace BattlePlayers
{
    //Interface Defining our structure for the user
    public interface IUser
    {
        UserProfile Profile { get; }
        CurrentGameData GameData { get; }
    }

    //Controlled Implementation of the interface in a base player class which is parent to all players
    public abstract class BasePlayer : IUser
    {
        public UserProfile Profile { get; }
        public CurrentGameData GameData { get; } = new();

        protected BasePlayer(UserProfile profile)
        {
            Profile = profile;
        }
    }

}