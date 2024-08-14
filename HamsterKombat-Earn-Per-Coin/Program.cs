// See https://aka.ms/new-console-template for more information
using HamsterKombat_Earn_Per_Coin;

Controller controller = Controller.GetInstance();
Console.WriteLine("Hola bienvenidos a la calculadora de ganancia para Hamnster Kombat");
int selected = 0;
bool bucle = true;
TimesControl timesControl = new TimesControl();

while (bucle)
{
    if (controller.Money != double.MaxValue)
    {
        Console.WriteLine($"Monedero: {controller.Money}");
    }
    Console.WriteLine("Digite la posicion de la opcion que desea:");
    Console.WriteLine("1-Ver cartas");
    Console.WriteLine("2-Ver cartas ordenadas");
    Console.WriteLine("3-Insertar carta");
    Console.WriteLine("4-Eliminar carta");
    Console.WriteLine("5-Ver la mejor oferta");
    Console.WriteLine("6-Comprar una carta");
    Console.WriteLine("7-Establecer monedero");
    Console.WriteLine("8-Desactivar carta");
    Console.WriteLine("9-Encontrar carta");
    Console.WriteLine("10-Secuencia de compra");
    Console.WriteLine("11-Salir");
    selected = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

    switch (selected)
    {
        case 1:
            Console.WriteLine(controller.GetStringCards());
            break;
        case 2:
            Console.WriteLine(controller.GetOrderedList());
            break;
        case 3:
            int option = 0;
            string name = string.Empty;
            double price = 0;
            double gain = 0;

            Console.WriteLine("Por favor inserte el nombre de la carta: ");
            name = Console.ReadLine();
            Console.WriteLine("Por favor inserte el precio de la carta: ");
            while (!double.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
            }
            Console.WriteLine("Por favor inserte las ganancias por hora adicional de la carta: ");
            while (!double.TryParse(Console.ReadLine(), out gain))
            {
                Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
            }

            Console.WriteLine("Tiene tiempo de expiracion? (Digite 1 para si 0 para no)");
            option = int.Parse(Console.ReadLine());
            DateTime? expireTime = null;
            
            if (option == 1)
            {
                string time = string.Empty;
                Console.WriteLine("Digite cuanto tiempo en el formato de horas, minutos o segundos (h, m, s) Example (2h = 2 horas) : ");
                time = Console.ReadLine();
                try
                {
                    expireTime = timesControl.GetDateForString(time);
                }
                catch (InvalidDataException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            controller.InsertCard(name, price, gain, expireTime);
            break;
        case 5:
            double newPrice = 0;
            double newGain = 0;
            CardModel obtained = controller.GetBestBuy();
            Console.WriteLine(obtained.Name);
            Console.WriteLine("Desea comprarla (1 Si, 0 No): ");
            option = int.Parse(Console.ReadLine());
            if (option == 1)
            {
                Console.WriteLine("Digite el nuevo precio: ");
                while (!double.TryParse(Console.ReadLine(), out newPrice))
                {
                    Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
                }
                Console.WriteLine("Digite la nueva ganacia: ");
                while (!double.TryParse(Console.ReadLine(), out newGain))
                {
                    Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
                }
             
                obtained.Price = newPrice;
                obtained.Earn_per_hour = newGain;

                Console.WriteLine("Tiempo impuesto para comprar? 1 Si, 0 No: ");
                option = int.Parse(Console.ReadLine());
                if (option == 1)
                {
                    DateTime? toBuyTime = null;
                    string time = string.Empty;
                    Console.WriteLine("Digite el tiempo en el formato tL donde t = Numero de horas y L = h,m,s: ");
                    time = Console.ReadLine();
                    try
                    {
                        toBuyTime = timesControl.GetDateForString(time);
                        obtained.ToBuyTime = toBuyTime;
                    }
                    catch (InvalidDataException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                }
                
                controller.UpdateCard(obtained);
                Console.WriteLine("Compra satisfecha");
            }
            
            break;
        case 6:
            uint id = 0;
            DateTime? toBuyTimes = null;
            
            Console.WriteLine(controller.GetStringCards());
            Console.WriteLine("Digite el id de la carta que quiera comprar");
            id = uint.Parse(Console.ReadLine());
            Console.WriteLine("Digite el nuevo precio: ");
            while (!double.TryParse(Console.ReadLine(), out newPrice))
            {
                Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
            }
            Console.WriteLine("Digite la nueva ganacia: ");
            while (!double.TryParse(Console.ReadLine(), out newGain))
            {
                Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
            }
            
            Console.WriteLine("Tiempo impuesto para comprar? 1 Si, 0 No: ");
            option = int.Parse(Console.ReadLine());
            if (option == 1)
            {
                string time = string.Empty;
                Console.WriteLine("Digite el tiempo en el formato tL donde t = Numero de horas y L = h,m,s: ");
                time = Console.ReadLine();
                try
                {
                    toBuyTimes = timesControl.GetDateForString(time);
                }
                catch (InvalidDataException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            Console.WriteLine(controller.BuyOneEspecific(id, newPrice, newGain, toBuyTimes));
            break;
        case 7:
            double money_temp;
            Console.WriteLine("Digite la cantidad que destina a comprar: ");
            while (!double.TryParse(Console.ReadLine(), out money_temp))
            {
                Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
            }

            controller.Money = money_temp;
            break;
        case 8:
            Console.WriteLine(controller.GetStringCards());
            Console.WriteLine("Digite el id de la carta que quiera comprar");
            id = uint.Parse(Console.ReadLine());

            try
            {
                controller.DesactivateCard(id);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            break;
        case 9:
            string find = string.Empty;
            Console.WriteLine("Introduce un nombre a buscar: ");
            find = Console.ReadLine();

            Console.WriteLine(controller.FindCard(find));
            break;
        case 10:
            Console.WriteLine("Digite la cantidad que destina a comprar: ");
            while (!double.TryParse(Console.ReadLine(), out money_temp))
            {
                Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
            }

            Console.WriteLine(controller.GetSequenceToBuy(money_temp));
            break;
        case 11:
            Console.WriteLine("Exiting...");
            controller.SecureSave();
            bucle = false;
            break;
        default:
            Console.WriteLine("End round");
            break;
    }
}