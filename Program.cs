using System;
using System.Collections;
using System.Text;
using System.Net.Http;
using System.Diagnostics;

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
        char continueResponse;
        int contErrors = 0;
        bool restart = true;
        StringBuilder sb = new StringBuilder();
        HttpClient client = new HttpClient();
        HttpResponseMessage response;
        Stopwatch watch = new Stopwatch();
        TimeSpan elapsed = new TimeSpan();

        do
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
                            logFilePath = directory + "\\log-" + System.DateTime.Today.ToString("dd-MM-yy") + ".log";

                            if (!File.Exists(logFilePath))
                            {
                                FileStream fs = File.Create(logFilePath);
                                fs.Close();
                            }

                            sb.Append(responseBody);
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
                }
                else if (contErrors > 0)
                {
                    Console.WriteLine("\nProcess finished with {0} errors. Check your default folder for this" +
                        " program to access the log file.", contErrors);
                }

            }
            else
            {
                Console.WriteLine("\nNo file path was entered!");
            }

            watch.Start();
            Console.WriteLine("Do you wish to continue? [Y/N]");
            continueResponse = (char)Console.Read();

            if (continueResponse.Equals("Y"))
            {
                while (watch.ElapsedMilliseconds < 60000)
                {
                    Console.WriteLine("Please wait {0}", watch.ElapsedMilliseconds);
                }
            }
        } while (restart);
        
    }

}
