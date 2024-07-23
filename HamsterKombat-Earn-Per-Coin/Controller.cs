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
        private string dbname;
        private string stringConnection;


        public Controller()
        {
            this.cards = new List<CardModel>();
            this.dbname = "cards.sqlite";
            this.stringConnection = $"Data Source={dbname};Version=3;";
            this.InitDatabaseProccess();

        }

        #region DATABASES
        private void InitDatabaseProccess()
        {
            if (!this.ExistsDB())
            {
                CreateDatabase();
            }

            LoadCards();

        }

        private bool ExistsDB()
        {
            if (File.Exists(this.dbname))
            {
                return true;
            }

            return false;
        }
        
        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(this.dbname);

            using (var connection = new SQLiteConnection(this.stringConnection))
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

        private void LoadCards()
        {
            using (var connection = new SQLiteConnection(this.stringConnection))
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

                            this.cards.Add(card);
                        }
                    }
                }
                connection.Close();
            }
        }
        #endregion

        #region CRUD
        public void InsertCard(string name, double price, double gain)
        {
            using (var connection = new SQLiteConnection(this.stringConnection))
            {
                connection.Open();

                string query = "INSERT INTO cards (name, price, gain) VALUES (@name, @price, @gain)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@gain", gain);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void UpdateCard(CardModel card, int position)
        {
            using (var connection = new SQLiteConnection(this.stringConnection))
            {
                connection.Open();

                string query = "UPDATE cards SET name = @name, price = @price, gain = @gain WHERE id = @id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", card.Name);
                    command.Parameters.AddWithValue("@price", card.Price);
                    command.Parameters.AddWithValue("@gain", card.Earn_per_hour);
                    command.Parameters.AddWithValue("@id", card.ID);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            this.cards[position] = card;
        }
        #endregion
    }
}
