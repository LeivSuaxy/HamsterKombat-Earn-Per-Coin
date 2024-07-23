﻿// See https://aka.ms/new-console-template for more information
using HamsterKombat_Earn_Per_Coin;

Controller controller = new Controller();
Console.WriteLine("Hola bienvenidos a la calculadora de ganancia para Hamnster Kombat");
int selected = 0;
double Money = double.MaxValue;

while (true)
{
    if (Money != double.MaxValue)
    {
        Console.WriteLine($"Monedero: {Money}");
    }
    Console.WriteLine("Digite la posicion de la opcion que desea:");
    Console.WriteLine("1-Ver cartas");
    Console.WriteLine("2-Ver cartas ordenadas");
    Console.WriteLine("3-Insertar carta");
    Console.WriteLine("4-Eliminar carta");
    Console.WriteLine("5-Ver la mejor oferta");
    Console.WriteLine("6-Comprar una carta");
    Console.WriteLine("7-Establecer monedero");
    Console.WriteLine("8-Salir");
    selected = int.Parse(Console.ReadLine());

    switch (selected)
    {
        case 1:
            Console.WriteLine(controller.GetStringCards());
            break;
        case 2:
            Console.WriteLine(controller.GetOrderedList(Money));
            break;
        case 3:
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

            controller.InsertCard(name, price, gain);
            break;
        case 5:
            int option = 0;
            double newPrice = 0;
            double newGain = 0;
            CardModel obtained = controller.GetBestBuy(Money);
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
                Money -= obtained.Price;
                obtained.Price = newPrice;
                obtained.Earn_per_hour = newGain;

                controller.UpdateCard(obtained);
                Console.WriteLine("Compra satisfecha");
            }
            
            break;
        case 6:
            uint id = 0;
            
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

            Console.WriteLine(controller.BuyOneEspecific(id, newPrice, newGain, Money));
            break;
        case 7:
            Console.WriteLine("Digite la cantidad que destina a comprar: ");
            while (!double.TryParse(Console.ReadLine(), out Money))
            {
                Console.WriteLine("Por favor inserte un valor correcto (Ejemplo: 3,43): ");
            }
            break;
        default:
            Console.WriteLine("End round");
            break;
    }
}