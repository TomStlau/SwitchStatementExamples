using System.Text.Json;



PlayGame();
Console.ReadKey();


static void PlayGame()
{
    string url = "https://opentdb.com/api.php?amount=10&category=9&difficulty=easy&type=multiple";
    Dictionary<string, string> data = FetchDataFromApi(url).GetAwaiter().GetResult();
    int score = 0;
    for (int i = 0; i < 3; i++)
    {
        string question = GetRandomQuestion(data);
        string answer = GetAnswer(data, question);
        Console.WriteLine(answer);
        score = AskQuestion(question, answer, score);
    }
    Console.Clear();
    Console.WriteLine($"Your final score is {score}/3");
}
static int AskQuestion(string question, string answer, int score)
{
    Console.WriteLine(question);
    string userAnswer = Console.ReadLine();
    if (answer.Equals(userAnswer, StringComparison.InvariantCultureIgnoreCase))
    {
        Console.WriteLine("Correct!");
        score++;
    }
    else
    {
        Console.WriteLine("Incorrect!");
    }
    Console.WriteLine($"{score}/3");
    Thread.Sleep(2000);
    return score;
}
static string GetAnswer(Dictionary<string, string> data, string question)
{
    return data[question];
}
static string GetRandomQuestion(Dictionary<string, string> data)
{
    Random random = new Random();
    List<string> questions = GetQuestions(data);
    int index = random.Next(questions.Count);
    return questions[index];
}
static List<string> GetQuestions(Dictionary<string, string> data)
{
    return data.Keys.ToList();
}
static async Task<Dictionary<string, string>> FetchDataFromApi(string url)
{
    using (HttpClient client = new HttpClient())
    {
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            JsonDocument json = JsonDocument.Parse(data);
            JsonElement root = json.RootElement;
            JsonElement doc = root.GetProperty("results");

            Dictionary<string, string> questionAnswerPairs = new Dictionary<string, string>();
            foreach (JsonElement item in doc.EnumerateArray())
            {
                string question = item.GetProperty("question").GetString();
                string answer = item.GetProperty("correct_answer").GetString(); // replace "correct_answer" with the actual property name in your JSON
                questionAnswerPairs.Add(question, answer);
            }

            return questionAnswerPairs;
        }
        else
        {
            throw new Exception($"Failed to fetch data: {response.StatusCode}");
        }
    }
}

