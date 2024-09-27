using System;
using StrategoDataAccess;
using Xunit;

namespace StrategoDataAccessTests
{
    public class DatabaseConnectionTests
    {
        [Fact]
        public void Test_GetPlayers()
        {
            using (var context = new StrategoEntities())
            {
                var players = context.Player.ToList();
                Assert.True(players.Count > 0);
            }
        }
    }
}
