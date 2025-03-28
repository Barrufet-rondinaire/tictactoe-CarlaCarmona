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
        var resultats = new Dictionary<string, int>();

        try
        {
            foreach (var frase in resposta)
            {
                Match match = rg.Match(frase);

                var nomJugador = match.Groups[1].Value;
                var pais = match.Groups[4].Value;
                participants.Add(nomJugador, pais);
                resultats.Add(nomJugador, 0);
                Console.WriteLine(nomJugador);
            }

            foreach (var frase in resposta)
            {
                Match match = rga.Match(frase);

                var desqualificat = match.Groups[1].Value;
                participants.Remove(desqualificat);
                resultats.Remove(desqualificat);
            }
            
            for (int i = 0; i < 10000; i++)
            {
                var partida = client.GetFromJsonAsync<Partida>($"/partida/{i}").Result;

                if (resultats.ContainsKey(partida.jugador1)&&resultats.ContainsKey(partida.jugador2))
                {
                    //comprovar files
                    var guanyador = 'a'; 
                    for (int j = 0; j < partida.tauler.Length; j++)
                    {
                        var fila = partida.tauler[j];
                        if (fila[0]==fila[1]&&fila[1]==fila[2])
                        {
                            guanyador = fila[0];
                        }
                    }

                    if (guanyador == 'X')
                    {
                        resultats[partida.jugador2]++;
                    }
                    else if (guanyador == 'O')
                    {
                        resultats[partida.jugador1]++;
                    }
                    
                    /*for (int j = 0; j < partida.tauler.Length; j++)
                    {
                        var columna = partida.tauler[];
                        
                        if (columna[0]==columna[1]&&columna[1]==columna[2])
                        {
                            guanyador = columna[0];
                        }
                    }*/

                    //comprovar columnes manualment
                    for (int j = 0; j < partida.tauler.Length; j++)
                    {
                        if (partida.tauler[0][j]==partida.tauler[1][j]&&partida.tauler[1][j]==partida.tauler[2][j])
                        {
                            guanyador = columna[0]; //arreglar
                        }
                    }
                }
            }
            //fer un for que faci 10000 peticions (get) a servidor per les partides
            //un switch que comprovi el resultat a cada partida
            //diccionari punts que té clau x i clau 0 valor es va sumant punts[X]++ if (un[0]== un[1]...) var s = 
            
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
    }
    
}