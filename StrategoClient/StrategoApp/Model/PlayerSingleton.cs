using StrategoApp.LogInService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.Model
{
    public class PlayerSingleton
    {
        private static PlayerSingleton _playerInstance;
        public Player Player { get; private set; }
        public static PlayerSingleton Instance
        {
            get
            {
                if (_playerInstance == null)
                {
                    _playerInstance = new PlayerSingleton();
                }
                return _playerInstance;
            }
        }

        public void LogIn(Player player)
        {
            Player = player;
        }
        public void LogOut()
        {
            Player = null;
        }
        public bool IsLoggedIn()
        {
            return Player != null;
        }
    }
}
