using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace queuedotnetcore
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("\nHello from Dotnet core! This application adds a message to a Storage Account queue using Dotnet Core!");

            if (args.Length < 3)
            {
                Console.WriteLine("Invalid Argument Count! Arguments should be sent in the following order: <StorageAccountName> <StorageAccountKey> <JsonFileURL>");
                return;
            }
            for (int i=0;i<args.Length;i++)
            {
                Console.WriteLine("\narg #" + i.ToString() +":" + args[i]);
            }
            string inputAccountName, inputAccountKey, inputFileURI;

                inputAccountName = args[0];
                inputAccountKey = args[1];
                inputFileURI = args[2];


            string jsonFile = ReadJson(inputFileURI).Result;

            Console.WriteLine("\n Json File that will be sent to the queue:\n" + jsonFile);
    

            CloudQueue queue = CreateQueueAsync(inputAccountName,inputAccountKey).Result;

            InsertMessage(queue,jsonFile).Wait();

        }


        private static async Task<CloudQueue> CreateQueueAsync(string accountName, string accountKey)
        {

            string myAccountName, myAccountKey;

            myAccountName = accountName;
            myAccountKey = accountKey;
            //myAccountName = "anvkujznhobmgg";
            //myAccountKey = "7khIWqus/cXiHpQl7OXhqarman43zr030AuIscViT+grTuWGrIqUblcaKtKX1SlNDDkYB11e3jAkDxTvE9tstA==";

            StorageCredentials storageCredentials = new StorageCredentials(myAccountName, myAccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            Console.WriteLine("\nCreating queue...");
            CloudQueue queue = queueClient.GetQueueReference("samplequeue1");
            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine("Failed to create queue!" + "Error: " + ex.ToString());
                Console.ReadLine();
                throw;
            }

            return queue;
        }

        private static async Task InsertMessage(CloudQueue queue, string message)
        {
            Console.WriteLine("\nAdding Message...");
            try
            {
                await queue.AddMessageAsync(new CloudQueueMessage(message));
            }
            catch(StorageException ex)
            {
                Console.WriteLine("Failed to add Message:" + ex.ToString());
                throw;
            }

            Console.WriteLine("\nSuccessfully added Queue Message!");

        }

        private static async Task<string> ReadJson(string url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    string json = await httpClient.GetStringAsync(url);

                    // Now parse with JSON.Net

                    return json;
                }
                catch(HttpRequestException ex)
                {
                    Console.WriteLine("Could not read JSON file, Error:" + ex.ToString());
                    throw;
                }
            }

            
        }


    }
}
