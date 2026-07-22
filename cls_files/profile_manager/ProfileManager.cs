using System.Runtime.InteropServices;
using BattlePlayers;

namespace ProfileManagerComponents
{
    public class ProfileManager
    {
        private UserProfile CreateProfileObject(string id, string name, bool isBot)
        {
            return new UserProfile(id, name, isBot);
        }


        //These two methods are just for testing purposes, this will be updated to load save data etc.
        public UserProfile CreateUserProfile()
        {
            return CreateProfileObject("BSS10", "Blue Shirt Studio", false);
        }

        public UserProfile CreateBotProfile()
        {
            return CreateProfileObject("Bot101", "ALT", true);
        }
    }
}