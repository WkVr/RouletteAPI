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

                INSERT INTO [GAMES] (TableBet) VALUES(20);";

                await _dbContext.QueryAsync(sql);

                sql = @"
                CREATE TABLE [Bets]
                (   
                    ID          INTEGER PRIMARY KEY AUTOINCREMENT,
                    BetId       INTEGER,
                    GameID      INTEGER,
                    BetType     INTEGER,
                    BetAmount   INTEGER,
                    [Values]    VARCHAR(200),
                    SpinID      INTEGER,
                    FOREIGN KEY (GameID) REFERENCES Games(GameID)
                )";

                await _dbContext.QueryAsync(sql);

                sql = @"
                CREATE TABLE [Spins]
                (
                    SpinID      INTEGER PRIMARY KEY AUTOINCREMENT,
                    GameID      INTEGER,
                    Value       INTEGER,
                    COLOR       VARCHAR(50),
                    BetId       INTEGER UNIQUE,
                    FOREIGN KEY (GameID) REFERENCES Games(GameID),
                    FOREIGN KEY (BetID) REFERENCES Bets(BetID)
                );";

                var data = await _dbContext.QueryAsync(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> PlaceBet(BetList betList)
        {
            try
            {
                if (_dbContext == null)
                    CreateDatabase();

                _dbContext.Open();

                var betId = await _dbContext.QueryAsync(@"SELECT MAX(BetID) Max FROM [Bets]");

                var newBetId = betId.Select(x => x.Max).FirstOrDefault() ?? 1;

                var sql = @"INSERT INTO [Bets] (BetID, GameID, BetType, BetAmount, [Values]) VALUES(@BetID, @GameId, @BetType, @BetAmount, @Values)";

                foreach (var bet in betList.Bets)
                {
                    await _dbContext.QueryAsync(sql, new { BetID = newBetId, GameId = betList.GameId, BetType = (int)bet.BetType, BetAmount = bet.BetAmount, Values = bet.Values});
                };

                return newBetId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<Bet>> GetBetList(int gameId)
        {
            _dbContext.Open();

            string sql = @"
                SELECT 
                    BetId,
                    BetType,  
                    BetAmount,
                    [Values],
                    SpinId
                FROM
                    Bets
                WHERE 
                    GameId = @GameId";

            return await _dbContext.QueryAsync<Bet>(sql, new {GameId = gameId});
        }

        public async Task Spin(Spin spin)
        {
            _dbContext.Open();

            var sql = @"INSERT INTO [Spins] (GameID, Value, Color, BetID) VALUES(@GameId, @Value, @Color, @BetID)";

            await _dbContext.QueryAsync(sql, new { GameId = spin.GameId, Value = spin.Value, Color = spin.Color, BetID = spin.BetId});
        }

        public async Task<IEnumerable<Spin>> GetSpinList(int gameId)
        {
            _dbContext.Open();

            string sql = @"
                SELECT 
                    Value,
                    Color,
                    BetID
                FROM
                    Spins
                WHERE 
                    GameId = @GameId";

            return await _dbContext.QueryAsync<Spin>(sql, new { GameId = gameId });
        }

        public async Task<int> GetTableBet(int gameId)
        {
            _dbContext.Open();

            string sql = @"
                SELECT 
                    TableBet
                FROM
                    Games
                WHERE 
                    GameId = @GameId";

            var tableBet = await _dbContext.QueryAsync(sql, new { GameId = gameId });

            var selectedTableBet = tableBet.Select(x => x.TableBet).FirstOrDefault();
            return (int)selectedTableBet;
        }
    }
}
