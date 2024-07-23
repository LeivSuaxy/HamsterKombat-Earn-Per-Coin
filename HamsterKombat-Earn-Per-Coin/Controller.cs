using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace HamsterKombat_Earn_Per_Coin
{
    internal class Controller
    {
        private List<CardModel> cards;

        public Controller()
        {
            cards = new List<CardModel>();
            this.InitDatabaseProccess();

        }

        #region DATABASES
        private void InitDatabaseProccess()
        {
            string dbname = "cards.sqlite";

            if (!this.ExistsDB(dbname))
            {
                CreateDatabase(dbname);
            }

            LoadCards(dbname);

        }

        private bool ExistsDB(string dbname)
        {
            if (File.Exists(dbname))
            {
                return true;
            }

            return false;
        }
        
        private void CreateDatabase(string dbname)
        {
            SQLiteConnection.CreateFile(dbname);

            using (var connection = new SQLiteConnection(this.GetConnectionString(dbname)))
            {
                connection.Open();

                string query =  "CREATE TABLE IF NOT EXISTS cards (" +
                                "id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "name TEXT," +
                                "price DOUBLE," +
                                "gain DOUBLE" +
                                ");";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private void LoadCards(string dbname)
        {
            using (var connection = new SQLiteConnection(this.GetConnectionString(dbname)))
            {
                connection.Open();

                string query = "SELECT * FROM cards ORDER BY id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CardModel card = new CardModel(
                                id: Convert.ToUInt32(reader["id"]),
                                name: reader["name"].ToString(),
                                price: Convert.ToDouble(reader["price"]),
                                earn_per_hour: Convert.ToDouble(reader["gain"])
                            );

                            cards.Add(card);
                        }
                    }
                }
                connection.Close();
            }
        }

        private string GetConnectionString(string dbname) => $"Data Source={dbname};Version=3;";

        #endregion
        #region CRUD

        #endregion
    }
}
