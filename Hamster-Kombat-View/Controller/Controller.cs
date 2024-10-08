﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using View.Model.CardModel;
using System.IO;


namespace View.Controller
{
    internal class Controller
    {
        private List<CardModel> cards;
        private string dbname;
        private string stringConnection;
        private double money = double.MaxValue;

        public double Money { get => money; set => money = value; }


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

                string query = "SELECT * FROM cards WHERE active=1 ORDER BY id";

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

                string query = "UPDATE cards SET name = @name, price = @price, gain = @gain, active=@active WHERE id = @id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", card.Name);
                    command.Parameters.AddWithValue("@price", card.Price);
                    command.Parameters.AddWithValue("@gain", card.Earn_per_hour);
                    command.Parameters.AddWithValue("@id", card.ID);
                    command.Parameters.AddWithValue("@active", card.Active);

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
                if (card_for.Earn_per_coin > best_gain && card_for.Price <= Money)
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
            if (this.Money == double.MaxValue)
            {
                orderedList = new List<CardModel>();
                
                foreach (CardModel card_for in cards)
                {
                    if (card_for.Active)
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
                            CardModel temp = orderedList[i];
                            orderedList[i] = orderedList[j];
                            orderedList[j] = temp;
                        }
                    }
                }
            } else
            {
                orderedList = new List<CardModel>();

                foreach (CardModel card_for in this.cards)
                {
                    if(card_for.Price <= this.Money && card_for.Active)
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
                            CardModel temp = orderedList[i];
                            orderedList[i] = orderedList[j];
                            orderedList[j] = temp;
                        }
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
            double total_gain = 0;
            double total_price = 0;

            foreach (CardModel card in cards)
            {
                total_gain = card.Earn_per_hour;
                total_price = card.Price;
                text += card.ToString();
            }

            total_gain /= cards.Count;
            total_price /= cards.Count;

            string text2 = $"Ganancias promedio de cartas {total_gain} \n" +
                $"Precios promedios de cartas {total_price} \n";

            return text + text2;
        }

        public string BuyOneEspecific(uint id, double newPrice, double newGain)
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

                if (this.Money != double.MaxValue)
                {
                    this.Money -= cards[position].Price;
                }

                cards[position].Price = newPrice;
                cards[position].Earn_per_hour = newGain;

                this.UpdateCard(cards[position]);

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
                        CardModel temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
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

        // TODO Balance Control Panel
        #endregion
    }
}
