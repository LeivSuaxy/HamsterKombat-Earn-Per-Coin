using System.Data.Entity.Infrastructure.Interception;
using System.Data.SQLite;

namespace HamsterKombat_Earn_Per_Coin
{
    internal class Controller
    {
        private List<CardModel> cards;
        private string dbname;
        private string stringConnection;
        private double money = double.MaxValue;
        private static Controller _singletonController;

        public double Money { get => money; set => money = value; }
        
        private Controller()
        {
            cards = new List<CardModel>();
            dbname = "cards.sqlite";
            stringConnection = $"Data Source={dbname};Version=3;";
            InitDatabaseProccess();
        }

        public static Controller GetInstance()
        {
            if (_singletonController == null)
            {
                _singletonController = new Controller();
            }

            return _singletonController;
        }

        #region DATABASES
        private void InitDatabaseProccess()
        {
            if (!ExistsDB())
            {
                CreateDatabase();
            }

            LoadCards();

        }

        private bool ExistsDB()
        {
            if (File.Exists(dbname))
            {
                return true;
            }

            return false;
        }
        
        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(dbname);

            using (var connection = new SQLiteConnection(stringConnection))
            {
                connection.Open();

                string query =  "CREATE TABLE IF NOT EXISTS cards (" +
                                "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                                "name TEXT NOT NULL," +
                                "price DOUBLE NOT NULL," +
                                "gain DOUBLE NOT NULL," +
                                "expiredate TEXT DEFAULT NULL," +
                                "tobuydate TEXT DEFAULT NULL," +
                                "active INTEGER DEFAULT 1" +
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
            using (var connection = new SQLiteConnection(stringConnection))
            {
                connection.Open();

                string query = "SELECT * FROM cards WHERE active=1 ORDER BY id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime? toBuyTime = reader["tobuydate"] != DBNull.Value
                                ? Convert.ToDateTime(reader["tobuydate"])
                                : (DateTime?)null;
                            
                            DateTime? expireDate = reader["expiredate"] != DBNull.Value
                                ? Convert.ToDateTime(reader["expiredate"])
                                : (DateTime?)null;
                            
                            CardModel card = new CardModel(
                                id: Convert.ToUInt32(reader["id"]),
                                name: reader["name"].ToString(),
                                price: Convert.ToDouble(reader["price"]),
                                earnPerHour: Convert.ToDouble(reader["gain"]),
                                toBuyTime: toBuyTime,
                                expireDate: expireDate
                            );

                            cards.Add(card);
                        }
                    }
                }
                connection.Close();
            }
        }
        #endregion

        #region CRUD
        public void InsertCard(string name, double price, double gain, DateTime? expireTime = null)
        {
            int lastid;
            using (var connection = new SQLiteConnection(stringConnection))
            {
                connection.Open();

                string query = "INSERT INTO cards (name, price, gain, expiredate) VALUES (@name, @price, @gain, @expiredate)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@gain", gain);
                    command.Parameters.AddWithValue("@expiredate", expireTime);
                    
                    command.ExecuteNonQuery();

                    lastid = (int)connection.LastInsertRowId;
                }
                connection.Close();
            }

            cards.Add(new CardModel(
                id: Convert.ToUInt32(lastid),
                name: name,
                price: price,
                earnPerHour: gain,
                expireDate: expireTime
                ));
        }

        public void UpdateCard(CardModel card)
        {
            // First code
            using (var connection = new SQLiteConnection(stringConnection))
            {
                connection.Open();

                string query = "UPDATE cards SET name = @name, price = @price, gain = @gain, expiredate=@expiredate, tobuydate=@tobuydate, active=@active WHERE id = @id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", card.Name);
                    command.Parameters.AddWithValue("@price", card.Price);
                    command.Parameters.AddWithValue("@gain", card.Earn_per_hour);
                    command.Parameters.AddWithValue("@id", card.ID);
                    command.Parameters.AddWithValue("@active", card.Active);
                    command.Parameters.AddWithValue("@expiredate", card.ExpireDate);
                    command.Parameters.AddWithValue("@tobuydate", card.ToBuyTime);

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

            cards[position] = card;
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
                if (card_for.CanBuy() && card_for.Earn_per_coin > best_gain && card_for.Price <= Money)
                {
                    best_gain = card_for.Earn_per_coin;
                    card = card_for;
                }
            }

            return card;
        }

        public string GetOrderedList()
        {
            List<CardModel> orderedList;
            if (Money == double.MaxValue)
            {
                orderedList = new List<CardModel>();
                
                foreach (CardModel card_for in cards)
                {
                    if (card_for.Active && card_for.CanBuy())
                    {
                        orderedList.Add(card_for);
                    }
                }

                for (int i = 0; i < orderedList.Count - 1; i++)
                {
                    for (int j = i + 1; j < orderedList.Count; j++)
                    {
                        if (orderedList[i].Earn_per_coin < orderedList[j].Earn_per_coin)
                        {
                            (orderedList[i], orderedList[j]) = (orderedList[j], orderedList[i]);
                        }
                    }
                }
            } else
            {
                orderedList = new List<CardModel>();

                foreach (CardModel card_for in cards)
                {
                    if(card_for.Price <= Money && card_for.Active && card_for.CanBuy())
                    {
                        orderedList.Add(card_for);
                    }
                }

                for (int i = 0; i < orderedList.Count - 1; i++)
                {
                    for (int j = i + 1; j < orderedList.Count; j++)
                    {
                        if (orderedList[i].Earn_per_coin < orderedList[j].Earn_per_coin)
                        {
                            (orderedList[i], orderedList[j]) = (orderedList[j], orderedList[i]);
                        }
                    }
                }
            }

            string text = string.Empty;

            foreach (CardModel card_for in orderedList)
            {
                text += $"ID: {card_for.ID}, Nombre: {card_for.Name}, AVG: {card_for.Earn_per_coin} \n";
            }

            return text;
        }

        public string GetStringCards()
        {
            string text = string.Empty;
            double total_gain = 0;
            double total_price = 0;
            double total_rate = 0;

            foreach (CardModel card in cards)
            {
                total_gain += card.Earn_per_hour;
                total_price += card.Price;
                total_rate += card.Earn_per_coin;
                text += card.ToString();
            }

            total_gain /= cards.Count;
            total_price /= cards.Count;
            total_rate /= cards.Count;
            double total_gain_per_second = total_gain / 60;

            string text2 = $"Ganancias promedio de cartas {total_gain} \n" +
                $"Precios promedios de cartas {total_price} \n" +
                $"Ganacia promedio por segundo prometida: {total_gain_per_second} \n" +
                $"W/R promedio: {total_rate} \n";

            return text + text2;
        }

        public string BuyOneEspecific(uint id, double newPrice, double newGain, DateTime? tobuytime = null)
        {
            if (id >= 0)
            {
                int position = -1;
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].ID == id)
                    {
                        position = i;
                        break;
                    }
                }

                if (position < 0)
                {
                    return "Error: El id de la carta no existe";
                }

                if (Money != double.MaxValue)
                {
                    Money -= cards[position].Price;
                }

                cards[position].Price = newPrice;
                cards[position].Earn_per_hour = newGain;
                cards[position].ToBuyTime = tobuytime;

                UpdateCard(cards[position]);

                return "Success: Update Correctly";
            }

            return "Error: Please provide a correct position";
        }

        public void DesactivateCard(uint id)
        {
            int position = -1;
            int index = 0;
            foreach (CardModel card in cards)
            {
                if (card.ID == id)
                {
                    position = index;
                    break;
                }
                index++;
            }

            if (position == -1)
            {
                throw new KeyNotFoundException("The id is incorrect");
            }
            cards[position].Active = false;
            this.UpdateCard(cards[position]);
        }

        public string FindCard(string str)
        {
            string text = string.Empty;

            foreach (var item in cards)
            {
                if(item.Name.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    text += item.ToString();
                }
            }
            return text;
        }

        public string GetSequenceToBuy(double allPrice)
        {
            List<CardModel> list = new List<CardModel>();

            foreach (CardModel card in cards)
            {
                if (card.Active)
                {
                    list.Add(card);
                }
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].Earn_per_coin < list[j].Earn_per_coin)
                    {
                        (list[i], list[j]) = (list[j], list[i]);
                    }
                }
            }

            // Este metodo recorre toda la lista buscando las mejores ofertas y va recomendando su compra hasta quedarte sin cambio
            string text = string.Empty;

            foreach(var item in list)
            {
                if(allPrice > item.Price)
                {
                    text += item.ToString();
                    allPrice -= item.Price;
                }
            }

            return text;
        }

        public void SecureSave()
        {
            List<CardModel> toUpdate = new List<CardModel>();
            
            foreach (var card in cards)
            {
                card.CheckExpireDate();
            }
            
        }

        // TODO Balance Control Panel
        #endregion
    }
}
