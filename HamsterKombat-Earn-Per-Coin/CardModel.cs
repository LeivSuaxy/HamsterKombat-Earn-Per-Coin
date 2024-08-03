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
        private DateTime? expireDate;
        
        public DateTime? ExpireDate
        {
            get => expireDate;
            set { expireDate = value; }
        }
        // To buy time
        private DateTime? toBuyTime;
        
        public DateTime? ToBuyTime
        {
            get => toBuyTime;
            set { toBuyTime = value; }
        }
        
        // Constructor
        public CardModel(uint id, string name, double price, double earnPerHour, DateTime? toBuyTime = null,
            DateTime? expireDate = null)
        {
            ID = id;
            Name = name;
            Price = price;
            Earn_per_hour = earnPerHour;
            ExpireDate = expireDate;
            ToBuyTime = toBuyTime;
        }

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, Price: {Price}, Gain: {Earn_per_hour} \n";
        }

        public bool CanBuy()
        {
            if (DateTime.Now > toBuyTime)
            {
                ToBuyTime = null;
                return true;
            }

            return false;
        }
    }
}
