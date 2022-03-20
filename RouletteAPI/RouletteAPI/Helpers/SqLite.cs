using System.Data.SQLite;
using Dapper;
using RouletteAPI.Models;
using System.Linq;

namespace RouletteAPI.Helpers
{
    public class SqLite : ISqLite
    {
        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\Roulette.sqlite"; }
        }

        public static SQLiteConnection _dbContext
        {
            get { return new SQLiteConnection("Data Source=" + DbFile); }
        }

        public SqLite()
        {
            if (!File.Exists(DbFile))
            {
                _ = _dbContext;
                CreateDatabase();
            }
        }

        private async Task CreateDatabase()
        {
            try
            {
                _dbContext.Open();

                string sql = @"
                CREATE TABLE [Games]
                (
                    GameID      INTEGER PRIMARY KEY AUTOINCREMENT,
                    TableBet    INTEGER
                );

                INSERT INTO [GAMES] (TableBet) VALUES(20);
                INSERT INTO [GAMES] (TableBet) VALUES(10);
                INSERT INTO [GAMES] (TableBet) VALUES(30);
                INSERT INTO [GAMES] (TableBet) VALUES(50);"
                    ;

                await _dbContext.QueryAsync(sql);

                sql = @"
                SELECT * FROM [Games]";

                var data = await _dbContext.QueryAsync(sql);

                sql = @"
                CREATE TABLE Bets
                (   
                    BetID       INTEGER PRIMARY KEY AUTOINCREMENT,
                    GameID      INTEGER,
                    BetType     INTEGER,
                    BetAmount   INTEGER,
                    FOREIGN KEY (GameID) REFERENCES Games(GameID)
                )";

                await _dbContext.QueryAsync(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> PlaceBet(BetList betList)
        {
            try
            {
                if (_dbContext == null)
                    CreateDatabase();

                _dbContext.Open();

                var sql = @"INSERT INTO [Bets] (GameID, BetType, BetAmount) VALUES(@GameId, @BetType, @BetAmount)";

                foreach (var bet in betList.Bets)
                {
                    await _dbContext.QueryAsync(sql, new { GameId = betList.GameId, BetType = (int)bet.BetType, BetAmount = bet.BetAmount});
                };
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Bet>> GetBetList(int gameId)
        {
            _dbContext.Open();

            string sql = @"
                SELECT 
                    BetType  
                    BetAmount
                FROM
                    Bets
                WHERE 
                    GameId = @GameId";

            return await _dbContext.QueryAsync<Bet>(sql, new {GameId = gameId});
        }
    }
}
