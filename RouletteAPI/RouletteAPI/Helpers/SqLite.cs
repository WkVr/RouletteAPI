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
                CREATE TABLE [Spins]
                (
                    SpinID      INTEGER PRIMARY KEY AUTOINCREMENT,
                    GameID      INTEGER,
                    Value       INTEGER,
                    COLOR       VARCHAR(50),
                    FOREIGN KEY (GameID) REFERENCES Games(GameID)
                );";

                var data = await _dbContext.QueryAsync(sql);

                sql = @"
                CREATE TABLE Bets
                (   
                    BetID       INTEGER PRIMARY KEY AUTOINCREMENT,
                    GameID      INTEGER,
                    BetType     INTEGER,
                    BetAmount   INTEGER,
                    Values      VARCHAR(200),
                    SpinID      INTEGER,
                    FOREIGN KEY (GameID) REFERENCES Games(GameID),
                    FOREIGN KEY (SpinID) REFERENCES Spins(SpinID),
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

                var sql = @"INSERT INTO [Bets] (GameID, BetType, BetAmount, Values) VALUES(@GameId, @BetType, @BetAmount, @Values)";

                foreach (var bet in betList.Bets)
                {
                    await _dbContext.QueryAsync(sql, new { GameId = betList.GameId, BetType = (int)bet.BetType, BetAmount = bet.BetAmount, Values = bet.Values});
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
                    BetType,  
                    BetAmount,
                    Values,
                FROM
                    Bets
                WHERE 
                    GameId = @GameId";

            return await _dbContext.QueryAsync<Bet>(sql, new {GameId = gameId});
        }

        public async Task Spin(Spin spin)
        {
            _dbContext.Open();

            var sql = @"INSERT INTO [Spins] (GameID, Value, Color) VALUES(@GameId, @Value, @Color)";

            await _dbContext.QueryAsync(sql, new { GameId = spin.GameId, Value = spin.Value, Color = spin.Color});
        }

        public async Task<IEnumerable<Spin>> GetSpinList(int gameId)
        {
            _dbContext.Open();

            string sql = @"
                SELECT 
                    Value,
                    Color
                FROM
                    Spins
                WHERE 
                    GameId = @GameId";

            return await _dbContext.QueryAsync<Spin>(sql, new { GameId = gameId });
        }
    }
}
