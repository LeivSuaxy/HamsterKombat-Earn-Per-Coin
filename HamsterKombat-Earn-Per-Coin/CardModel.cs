using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HamsterKombat_Earn_Per_Coin
{
    internal class CardModel
    {
        uint id;
        string name;
        double price;
        double earn_per_hour;

        public CardModel(uint id, string name, double price, double earn_per_hour)
        {
            this.id = id;
            this.name = name;
            this.price = price;
            this.earn_per_hour = earn_per_hour;
        }

        public uint GetId() => id;

        public string GetName() => name;

        public double GetPrice() => price;

        public double GetEarn_per_hour() => earn_per_hour;

        public double GetEarn_per_coint() => (earn_per_hour / price);

        public void SetPrice(double price) { this.price = price; }

        public void SetEarn_per_hour(double earn_per_hour) { this.earn_per_hour = earn_per_hour; }


    }
}
