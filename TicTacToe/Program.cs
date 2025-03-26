using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace TicTacToe;

class Program
{
    static void Main(string[] args)
    {
        var url = "http://localhost:8080";

        Uri uri = new Uri(url);

        using HttpClient client = new()
        {
            BaseAddress = uri
        };
        
        var resposta = client.GetFromJsonAsync<List<string>>("jugadors").Result;
        
        Regex rg = new Regex(@"participant.([A-Z]+\w+ [A-Z-'a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+)");
        
        Regex rga = new Regex(@"participant.([A-Z]+\w+ [A-Z-'a-z]+\w+).*desqualifica(da|t)"); //iterar novament la resposta per treure de diccionari els eliminats
        
        var participants = new Dictionary<string, string>();

        try
        {
            foreach (var frase in resposta)
            {
                Match match = rg.Match(frase);
                
                var nomJugador = match.Groups[1].Value;
                var pais = match.Groups[4].Value;
                participants.Add(nomJugador, pais);
                Console.WriteLine(nomJugador);
            }
            
            
            
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
    }
    
}