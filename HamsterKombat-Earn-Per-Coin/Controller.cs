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
            int lastid;
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

                    lastid = (int)connection.LastInsertRowId;
                }
                connection.Close();
            }

            cards.Add(new CardModel(
                    id: Convert.ToUInt32(lastid),
                    name: name,
                    price: price,
                    earn_per_hour: gain
                ));
        }

        public void UpdateCard(CardModel card)
        {
            // First code
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
            // End First Code

            int position = 0;
            // Second code
            foreach (CardModel card_for in cards)
            {
                if (card_for.ID == card.ID)
                {
                    break;
                }
                position++;
            }

            this.cards[position] = card;
            // End Second code
            
        }

        public void DeleteCard(CardModel card)
        {
            // First code
            using (var connection = new SQLiteConnection(this.stringConnection))
            {
                connection.Open();

                string query = "DELETE FROM cards WHERE id = @id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", card.ID);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            // End First Code

            int position = 0;
            // Second code
            foreach (CardModel card_for in cards)
            {
                if (card_for.ID == card.ID)
                {
                    break;
                }
                position++;
            }

            this.cards.RemoveAt(position);
            // End Second code
        }
        #endregion

        #region FUNCTIONS
        public CardModel GetBestBuy()
        {
            CardModel card = null;
            double best_gain = 0;

            foreach (CardModel card_for in cards)
            {
                if (card_for.Earn_per_coin > best_gain)
                {
                    best_gain = card_for.Earn_per_coin;
                    card = card_for;
                }
            }

            return card;
        }

        public string GetOrderedList()
        {
            List<CardModel> orderedList = this.cards;

            for(int i = 0;  i < orderedList.Count-1; i++)
            {
                for(int j = i+1; j < orderedList.Count; j++)
                {
                    if (orderedList[i].Earn_per_coin < orderedList[j].Earn_per_coin)
                    {
                        CardModel temp = orderedList[i];
                        orderedList[i] = orderedList[j];
                        orderedList[j] = temp;
                    }
                }
            }

            string text = string.Empty;

            foreach (CardModel card_for in orderedList)
            {
                text += $"Nombre: {card_for.Name}, AVG: {card_for.Earn_per_coin} \n";
            }

            return text;
        }

        public string GetStringCards()
        {
            string text = string.Empty;

            foreach (CardModel card in cards)
            {
                text += card.ToString();
            }

            return text;
        }

        public string BuyOneEspecific(int position, double newPrice, double newGain)
        {
            if (position >= 0 && position <= cards.Count)
            {
                CardModel cardModified = cards.ElementAt(position);

                if (cardModified is not null)
                {
                    cardModified.Price = newPrice;
                    cardModified.Earn_per_hour = newGain;
                    this.UpdateCard(cardModified);
                }
                return "Error: The card is null";
            }

            return "Error: Please provide a correct position";
        }

        // TODO Balance Control Panel
        #endregion
    }
}
