using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace TicTacToe;

class Program
{
    static async Task Main(string[] args)
    {
        var url = "http://localhost:8080/";

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
            
            for (int i = 1; i < 10001; i++)
            {
                var partida = await client.GetFromJsonAsync<Partida>($"partida/{i}");
                //ensure

                if (resultats.ContainsKey(partida.jugador1)&&resultats.ContainsKey(partida.jugador2))
                {
                    //comprovar files
                    var guanyador = 'a';
                    //p serveix per comprovar que en una partid només hi ha un guanyador
                    var p = 0;
                    
                    for (int j = 0; j < partida.tauler.Length; j++)
                    {
                        var fila = partida.tauler[j];
                        if (fila[0]==fila[1]&&fila[1]==fila[2])
                        {
                            guanyador = fila[0];
                            p++;
                        }
                        
                    }

                    //comprovar columnes manualment
                    if (guanyador == 'a')
                    {
                        for (int j = 0; j < partida.tauler.Length; j++)
                        {
                            if (partida.tauler[0][j]==partida.tauler[1][j]&&partida.tauler[1][j]==partida.tauler[2][j])
                            {
                                guanyador = partida.tauler[0][j];
                                p++;
                            }
                        }
                    }

                    //diagonal esquerra
                    if (guanyador == 'a')
                    {
                        if (partida.tauler[0][0]==partida.tauler[1][1]&&partida.tauler[2][2]==partida.tauler[1][1])
                        {
                            guanyador = partida.tauler[0][0];
                            p++;
                        }
                    }
                    
                    //diagonal dreta
                    if (guanyador == 'a')
                    {
                        if (partida.tauler[0][2]==partida.tauler[1][1]&&partida.tauler[2][0]==partida.tauler[1][1])
                        {
                            guanyador = partida.tauler[0][2];
                            p++;
                        }
                    }

                    if (p == 1)
                    {
                        if (guanyador == 'X')
                        {
                            resultats[partida.jugador2]++;
                        }
                        else if (guanyador == 'O')
                        {
                            resultats[partida.jugador1]++;
                        }
                    }
                }
            }
            //qin és el guanyador
            var mesGran = 0;
            var winner = "";
            bool esEmpat = false;
            
            foreach (var jugador in resultats)
            {
                if (jugador.Value == mesGran)
                {
                    esEmpat = true;
                    break;
                }
                
                if (jugador.Value > mesGran)
                {
                    mesGran = jugador.Value;
                    winner = jugador.Key;
                    esEmpat = false;
                }
            }

            foreach (var jugadors in participants)
            {
                if (winner == jugadors.Key && esEmpat!=true)
                {
                    Console.WriteLine($"El guanyador és {jugadors.Key} del país {jugadors.Value}!!!");
                }
            }
            
            if (esEmpat)
            {
                Console.WriteLine("Hi ha hagut un empat! No hi han guanyadors");
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
    }
    
}