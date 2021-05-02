using GemBox.Spreadsheet;
using HeavyClient.objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceReference2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeavyClient
{
    class Program
    {
        private static RoutingServiceClient RSclient;
        private static JObject jsonResult;

        static void Main(string[] args)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            RSclient = new RoutingServiceClient(RoutingServiceClient.EndpointConfiguration.SoapRouting);

            consoleAdmin();
        }

        /**
         * Creer un fichier excel et stocke les statistiques dedans 
         * stocker ds bin/Debug
        **/ 
        static void createExcelFileSummaryStat(bool verif)
        {
            string result = RSclient.getStationsStatAsync().Result;

            jsonResult = JObject.Parse(result);

            if (verif)
            {
                Console.WriteLine("\n*****\n");
                Console.WriteLine(jsonResult.ToString());
                Console.WriteLine("\n*****\n");
            }
            else
            {
                // Create empty excel file with a sheet
                ExcelFile workbook = new ExcelFile();
                ExcelWorksheet worksheet = workbook.Worksheets.Add("Stations_Statistiques");

                // Define header values
                worksheet.Cells[0, 0].Value = "Station Name";
                worksheet.Cells[0, 1].Value = "Station Location";
                worksheet.Cells[0, 2].Value = "Number of uses";

                // Write deserialized values to a sheet

                int cpt = 1;
                int row = 0;
                foreach (var value in jsonResult)
                {
                    string[] subs = value.Key.Split("_");
                    string location = subs[0];
                    string name = subs[1];
                    string numberOfUse = value.Value.ToString();

                    if (Int16.Parse(numberOfUse) > 0)
                    {
                        worksheet.Cells[++row, 0].Value = name;
                        worksheet.Cells[row, 1].Value = location;
                        worksheet.Cells[row, 2].Value = numberOfUse;
                        Console.Write("Station Added \n");
                        cpt++;
                    }

                    /**
                     * Limite excel gratuit
                    */
                    if (cpt == 149)
                    {
                        break;
                    }

                }

                // Save excel file
                workbook.Save("AppStats.xlsx");

                Console.Write("Excel File Created -- Consult bin/Debug/");
            }

        }


        /**
         * Display statistiques
        */
        static void displayStat()
        {
            createExcelFileSummaryStat(true);
        }

        static void iteneraryInDetails(string addressStart, string addressEnd, string option)
        {
            var data = RSclient.searchRouteAsync(addressStart, addressEnd).Result;

            Root responseWalking1 = JsonConvert.DeserializeObject<Root>(data[0]);
            Root responseBiking = JsonConvert.DeserializeObject<Root>(data[0]);
            Root responseWalking2 = JsonConvert.DeserializeObject<Root>(data[0]);

            if(option == "time")
            {
                double duration = responseWalking1.features[0].properties.segments[0].duration +
                responseBiking.features[0].properties.segments[0].duration +
                responseWalking2.features[0].properties.segments[0].duration;

                Console.WriteLine("La durée du trajet est estimée à " + duration + " minute(s) à vélo");
            }
            else if(option == "distance")
            {
                double distance = responseWalking1.features[0].properties.segments[0].distance +
                responseBiking.features[0].properties.segments[0].distance +
                responseWalking2.features[0].properties.segments[0].distance;
                Console.WriteLine("La distance du trajet est de " + distance + " mètre(s)");
            }
            else
            {
                Console.WriteLine("Instructions gps pour vélo --> \n");
                List<Step> instructions_gps = responseBiking.features[0].properties.segments[0].steps;

                foreach(Step step in instructions_gps)
                {
                    Console.WriteLine("--" + step.instruction + "\n");
                }
            }
            

            
        }

        /**
         * Liste des commandes possibles (description dans le readme)
        **/
        static void commands()
        {
            Console.WriteLine("\nCommand 1 : Statistiques");
            Console.WriteLine("Command 2 : Excel");
            Console.WriteLine("Command 3 : Itinerarytime");
            Console.WriteLine("Command 3 : Itinerarydistance");
            Console.WriteLine("Command 3 : Itinerarygps");
            Console.WriteLine("Command 4 : Exit\n");
        }


        /**
         * Console admin pour effectuer différentes commandes
         **/
        static void consoleAdmin()
        {

            commands();

            while (true)
            {
                Console.Write("Console Administrateur > ");
                string command = Console.ReadLine();
                string[] tokens = command.Split(' ');
                switch (tokens[0])
                {
                    case "Statistiques":
                        displayStat();
                        commands();
                        break;

                    case "Excel":
                        createExcelFileSummaryStat(false);
                        commands();
                        break;

                    case "Itinerarytime":
                        Console.Write("Enter address start: ");
                        string addressStart = Console.ReadLine();
                        Console.Write("You have selected start : " + addressStart + "\n");

                        Console.Write("Enter address end: ");
                        string addressEnd = Console.ReadLine();
                        Console.Write("You have selected address end : " + addressStart + "\n\n");

                        iteneraryInDetails(addressStart, addressEnd, "time");
                        commands();
                        break;

                    case "Itinerarydistance":
                        Console.Write("Enter address start: ");
                        string addressS = Console.ReadLine();
                        Console.Write("You have selected start : " + addressS + "\n");

                        Console.Write("Enter address end: ");
                        string addressE = Console.ReadLine();
                        Console.Write("You have selected address end : " + addressE + "\n\n");

                        iteneraryInDetails(addressS, addressE, "distance");
                        commands();
                        break;

                    case "Itinerarygps":
                        Console.Write("Enter address start: ");
                        string address1 = Console.ReadLine();
                        Console.Write("You have selected start : " + address1 + "\n");

                        Console.Write("Enter address end: ");
                        string address2 = Console.ReadLine();
                        Console.Write("You have selected address end : " + address2 + "\n\n");

                        iteneraryInDetails(address1, address2, "gps");
                        commands();
                        break;

                    case "Exit":
                        System.Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Erreur commande n'existe pas !");
                        commands();
                        break;
                }

            }
        }

    }
}
