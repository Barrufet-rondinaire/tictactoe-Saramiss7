using System.Net.Http.Json;
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
        Regex rg = new Regex(@"participant ([A-Z]+\w+ [A-Z-'-a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+)");
        Regex rg2 = new Regex(@"(.*(Ainhoa Ojeda)|participant ([A-Z]+\w+ [A-Z-'-a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+).*)");
        var jugadors = new Dictionary<string, string>();
        
        foreach (var respostes in resposta)
        {
            Match match = rg.Match(respostes);
            string nom = match.Groups[1].Value;
            string pais = match.Groups[4].Value;
            if (nom != "Ainhoa Ojeda")
            {
                jugadors.Add(nom, pais);
                Console.WriteLine($"Els jugadors és: {nom}\n Pais:{pais} ");
            }
        }
    }
}