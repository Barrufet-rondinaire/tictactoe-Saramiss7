using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;

namespace TicTacToe;

class Program
{
    static async Task Main()
    {
        /*POST: El guanyador del campionat són tots els que hagin guanyat més partides
        Es va eliminar algun participant … Les partides on participi el jugador eliminat no s’han de comptar
        */
        
        var url = "http://localhost:8080/";
        Uri uri = new (url);

        using HttpClient client = new() //Estableix la connexió
        {
            BaseAddress = uri
        };

        var resposta = await client.GetFromJsonAsync<List<string>>("jugadors");// Canviar el get a on vols que és conecti: uri / És conecta amb l'adreça

        //GET: S’ha perdut la llista de participants i l’hem de recuperar: saber els participants i de quin país són.
        Regex rg = new Regex(@"participant ([A-Z]+\w+ [A-Z-'a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+)");
        Regex desquali = new Regex(@"participant ([A-Z]+\w+ [A-Z-'-a-z]+\w+).*(desqualifica(da|t))");
        var jugadors = new Dictionary<string, string>();
        var punts = new Dictionary<string, int> ();
        var desqualificats = new List<string>();
        
        //Llista desqualificats
        foreach (var respostes in resposta)
        {
            Match match = desquali.Match(respostes);
            string nom = match.Groups[1].Value;
            desqualificats.Add(nom);
        }
        //Llista jugadors
        foreach (var respostes in resposta)
        {
            Match match = rg.Match(respostes);
            string nom = match.Groups[1].Value;
            string pais = match.Groups[4].Value;
            if (!desqualificats.Contains(nom))
            {
                jugadors.Add(nom, pais);
                punts.Add(nom, 0);
                //Console.WriteLine($"Els jugadors és: {nom}\n Pais:{pais} ");
            }
        }
        
        /*foreach (var jugador in jugadors)
        {
            Console.WriteLine($"El nom: {jugador.Key}, el pais: {jugador.Value}");
        }*/
        
        //for per revisar totes les 10000 partides de la competició amb el GET a dintre
        for (var i = 1; i <= 10000; i = i++)
        {
            var partida = await client.GetFromJsonAsync<Resposta>($"partida/{i}");
            //Console.WriteLine(partida.ToString());
            
            //switch amb 8 ifs per comprovar resultats de cada partida
            string un = partida.tauler[0];
            string dos = partida.tauler[1];
            string tres = partida.tauler[2];
            
            var O = "";
            var X = "";
            if (punts.ContainsKey(partida.jugador1) && punts.ContainsKey(partida.jugador2))
            {
                O = partida.jugador1;
                X = partida.jugador2;
                //Console.WriteLine($"{partida}");
                if (un[0] == 'X' && un[1] == 'X' && un[2] == 'X' || dos[0] == 'X' && dos[1] == 'X' && dos[2] == 'X' ||
                    tres[0] == 'X' && tres[1] == 'X' && tres[2] == 'X' ||
                    un[0] == 'X' && dos[0] == 'X' && tres[0] == 'X' ||
                    un[1] == 'X' && dos[1] == 'X' && tres[1] == 'X' || un[2] == 'X' && dos[2] == 'X' && tres[2] == 'X')
                {
                    punts[$"{X}"]++;
                }
                else if (un[0] == 'O' && un[1] == 'O' && un[2] == 'O' ||
                         dos[0] == 'O' && dos[1] == 'O' && dos[2] == 'O' ||
                         tres[0] == 'O' && tres[1] == 'O' && tres[2] == 'O' ||
                         un[0] == 'O' && dos[0] == 'O' && tres[0] == 'O' ||
                         un[1] == 'O' && dos[1] == 'O' && tres[1] == 'O' ||
                         un[2] == 'O' && dos[2] == 'O' && tres[2] == 'O')
                {
                    punts[$"{O}"]++;
                }
                //Diagonals
                else if (un[0] == 'X' && dos[1] == 'X' && tres[2] == 'X' ||
                         un[2] == 'X' && dos[1] == 'X' && tres[0] == 'X')
                {
                    punts[$"{X}"]++;
                }
                else if (un[0] == 'O' && dos[1] == 'O' && tres[2] == 'O' ||
                         un[2] == 'O' && dos[1] == 'O' && tres[0] == 'O')
                {
                    punts[$"{O}"]++;
                }
                else
                {
                    //Console.WriteLine("Empat");
                }
            }
            i++;
        }
        foreach (var punt in punts)
        {
            Console.WriteLine($"Nom: {punt.Key}, punts: {punt.Value}");
        }
        var guanyador = "";
        var mesgran = 0;
        bool empat = false;
        foreach (var punt in punts)
        {
            if (punt.Value == mesgran)
            {
                empat = true;
            }
            
            if (punt.Value > mesgran)
            {
                guanyador = punt.Key;
                mesgran = punt.Value;
                empat = false;
            }
        }
        
        foreach (var jugador in jugadors)
        {
            if (jugador.Key == guanyador && empat == false)
            {
                Console.WriteLine($"El guanyador és: {jugador.Key} del país: {jugador.Value}");
            }

            if (empat)
            {
                Console.WriteLine($"Empat");
            }
        }
    }
}