namespace HamsterKombat_Earn_Per_Coin;

/*
 * This class is to control time for cards
 *
 * Example:
 * 2 Hours remaining to buy again the card
 * Just Type: 2h and the software will take the 2 just like a number and h just like HOUR
 * 'h' = Hour
 * 'm' = Minute
 * 's' = Second
 */
public class TimesControl
{
    private readonly char[] admitibleChar = {'h', 'm', 's'};
    
    public DateTime GetDateForString(string str)
    {
        if (str is null or " " or "")
        {
            throw new InvalidDataException("Esta cadena de texto no es valida");
        }

        if (str[0] is '0' || !char.IsDigit(str[0]))
        {
            throw new InvalidDataException("The first number cannot be 0 and need to be a number");
        }

        string resultStringNumber = string.Empty;
        int number;
        int pointer = 0;
        // Take the number
        while (pointer < str.Length && char.IsDigit(str[pointer]))
        {
            resultStringNumber += str[pointer++];
        }

        int.TryParse(resultStringNumber, out number);

        DateTime futureDate = DateTime.Now;
        
        //if (!(pointer + 1 < str.Length)) throw new InvalidDataException("Please provide a valid hour format: h, m, s");
        
        if(!Array.Exists(admitibleChar, element => element == str[pointer])) 
            throw new InvalidDataException("Please the provided format is not correct: h, m or s");
        
        // If is H is Hour
        if (str[pointer] == 'h') futureDate = futureDate.AddHours(number);
        
        // If is M is minute
        if (str[pointer] == 'm') futureDate = futureDate.AddMinutes(number);
        
        // If is S is seconds
        if (str[pointer] == 's') futureDate = futureDate.AddSeconds(number);
        
        Console.WriteLine(DateTime.Now);
        Console.WriteLine(number);
        Console.WriteLine(futureDate);
        Console.WriteLine(str[pointer]);
        
        // Convert DateTime to ISO8601
        // yyyy-MM-dd HH:mm:ss

        //string time = futureDate.ToString("yyyy-MM-dd HH:mm:ss");
        
        return futureDate;
    }
}