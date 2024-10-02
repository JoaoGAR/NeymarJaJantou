using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static void Main(string[] args)
    {
        var json = JObject.Parse(File.ReadAllText("config.json"));
        string apiKey = json["HuggingFaceApiKey"].ToString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        string clientNow = DateTime.Now.ToString("HH:mm");
        Console.WriteLine($"It is {clientNow}");

        string resposta = Ask(clientNow);
        Console.WriteLine(resposta);
    }

    static string Ask(string clientNow)
    {
        var prompt = $"If it is {clientNow} in Brazil and Neymar is in Emirates, did he already had dinner?";
        var content = new StringContent("{\"inputs\":\"" + prompt + "\"}", Encoding.UTF8, "application/json");

        var response = client.PostAsync("https://api-inference.huggingface.co/models/gpt2", content).Result;
        response.EnsureSuccessStatusCode();

        var jsonResponse = response.Content.ReadAsStringAsync().Result;

        // Usando Newtonsoft.Json para parsear o JSON
        var jsonArray = JArray.Parse(jsonResponse); // Parse como JArray
        string resposta = jsonArray[0]["generated_text"].ToString(); // Acessa o primeiro item e o campo generated_text

        return resposta.Trim(); // Remove espaços desnecessários
    }

}
