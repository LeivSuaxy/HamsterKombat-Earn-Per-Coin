namespace HamsterKombat_Earn_Per_Coin
{
    internal class CardModel
    {
        // ID
        private uint id;
        public uint ID { get { return id; } private set { id = value; } }
        
        // Name
        private string name;
        public string Name { get { return name; } private set { name = value; } }

        // Price
        private double price;

        public double Price { get { return price; } set { price = value; } }

        // Ear-per-Hour
        private double earn_per_hour;
        
        public double Earn_per_hour { get { return earn_per_hour; } set { earn_per_hour = value; } }

        // Ear-per-Coin
        public double Earn_per_coin { get { return (Earn_per_hour / Price); } }

        // Is Active?
        private bool active = true;

        public bool Active { get => active; set { active = value; } }

        // Needed
        // To Expire Date
        // To buy time
        
        // Constructor
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
