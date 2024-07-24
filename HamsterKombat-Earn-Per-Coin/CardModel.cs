using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HamsterKombat_Earn_Per_Coin
{
    internal class CardModel
    {
        private uint id;

        public uint ID { get { return id; } private set { id = value; } }

        private string name;

        public string Name { get { return name; } private set { name = value; } }

        private double price;

        public double Price { get { return price; } set { price = value; } }

        private double earn_per_hour;

        public double Earn_per_hour { get { return earn_per_hour; } set { earn_per_hour = value; } }

        public double Earn_per_coin { get { return (Earn_per_hour / Price); } }

        private bool active = true;

        public bool Active { get => active; set { active = value; } }

        public CardModel(uint id, string name, double price, double earn_per_hour)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Earn_per_hour = earn_per_hour;
        }

        public override string ToString()
        {
            return $"ID: {this.ID}, Name: {this.Name}, Price: {this.Price}, Gain: {this.Earn_per_hour} \n";
        }
    }
}
