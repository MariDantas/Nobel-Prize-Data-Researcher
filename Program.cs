using System.Collections;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string filePath;
        ArrayList file = new ArrayList();
        string content;
        string baseUri = "https://api.nobelprize.org/2.1/nobelPrize/";
        string category;
        string year;
        string[] splitContent;
        string responseBody;
        string directory;
        string logFilePath;
        string fullUri;
        string currentDate = System.DateTime.Today.ToString("dd-MM-yy");
        string lastLine;
        string lastResearch;
        int seconds = 0;
        TimeSpan elapsed = new TimeSpan();

        int contErrors = 0;
        StringBuilder sb = new StringBuilder();
        HttpClient client = new HttpClient();
        HttpResponseMessage response;
        TimeSpan time = new TimeSpan();

        directory = Directory.GetCurrentDirectory();
        logFilePath = directory + "\\log-" + currentDate + ".log";

        if (File.Exists(logFilePath))
        {
            lastLine = File.ReadLines(logFilePath).Last();
            lastResearch = lastLine.Substring(15);
            time = TimeSpan.Parse(lastResearch);
            elapsed = DateTime.Now.TimeOfDay.Subtract(time);

            if (elapsed.Seconds < 60 && elapsed.Minutes < 1)
            {
                seconds = 60 - elapsed.Seconds;
            }
        }

        if (seconds > 0)
        {
            Console.WriteLine("Please wait " + seconds + " more seconds, close this application and then open again");
        } else
        {
            Console.Write("Enter the path of your file: ");
            filePath = Console.ReadLine();

            if (filePath != "")
            {
                try
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);

                    foreach (var line in lines)
                    {
                        file.Add(line);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error has occurred!");
                    Console.WriteLine("Message: {0}", e.ToString());
                }

                for (int i = 0; i < file.Count; i++)
                {
                    content = (string?)file[i];

                    if (content.Contains(";"))
                    {
                        splitContent = content.Split(";");
                        category = splitContent[0];
                        year = splitContent[1];
                        fullUri = baseUri + category + "/" + year;

                        Console.WriteLine("\nCategory: {0}", category);
                        Console.WriteLine("Year: {0}", year);
                        Console.WriteLine("URL used: {0}", fullUri);
                        Console.WriteLine("Response: \n");
                        Thread.Sleep(15000);

                        try
                        {
                            response = client.GetAsync(baseUri + category + "/" + year).Result;
                            response.EnsureSuccessStatusCode();
                            responseBody = response.Content.ReadAsStringAsync().Result;

                            Console.WriteLine(responseBody);

                            directory = Directory.GetCurrentDirectory();
                            logFilePath = directory + "\\log-" + currentDate + ".log";

                            if (!File.Exists(logFilePath))
                            {
                                FileStream fs = File.Create(logFilePath);
                                fs.Close();
                            }

                            time = DateTime.Now.TimeOfDay;
                            sb.Append(responseBody + "\n\n" + "Last searched: " + time);
                            File.AppendAllText(logFilePath, sb.ToString());
                            sb.Clear();

                            if (!response.IsSuccessStatusCode)
                            {
                                contErrors++;
                            }

                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("\nAn error has ocurred!");
                            Console.WriteLine("Message: {0}", e.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid line read! Going to the next line");
                    }
                }

                if (contErrors == 0)
                {
                    Console.WriteLine("\nProcess finished successfully. Check your default folder for this program" +
                        " to access the log file.");
                    Thread.Sleep(5000);
                    Environment.Exit(0);
                }
                else if (contErrors > 0)
                {
                    Console.WriteLine("\nProcess finished with {0} errors. Check your default folder for this" +
                        " program to access the log file.", contErrors);
                    Thread.Sleep(5000);
                    Environment.Exit(0);
                }

            }
            else
            {
                Console.WriteLine("\nNo file path was entered!");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
        }

    }

}
