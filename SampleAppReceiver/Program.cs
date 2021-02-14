using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using SampleShared.Models;

namespace SampleAppReceiver
{
    class Program
    {
        const string connString = "Endpoint=sb://samplewebservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=XXz8pHsKRciAcSQkE64o7kI18TGtCN6NVx/JjbH6tzo=";
        static IQueueClient qClient;

        static async Task Main(string[] args)
        {
            qClient = new QueueClient(connString, "personqueue");

            var msgOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {   
                // How many messages we can process at time
                MaxConcurrentCalls = 1,
                // need to wait until a message is fully processed
                AutoComplete = false,
            };

            qClient.RegisterMessageHandler(ProcessMessageAsync, msgOptions);

            Console.ReadLine();
            await qClient.CloseAsync();
        }

        private static async Task ProcessMessageAsync(Message msg, CancellationToken token)
        {
            // Deserialisr the msg body
            var jsonBody = Encoding.UTF8.GetString(msg.Body);
            var personObj = JsonSerializer.Deserialize<Person>(jsonBody);

            Console.WriteLine($"First Name: {personObj.FirstName}");
            Console.WriteLine($"Last Name: {personObj.LastName}");
            Console.WriteLine($"Email: {personObj.Email}");

            // Updating the queue that the message has been processed successfully
            await qClient.CompleteAsync(msg.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine($"Something went wrong, {args.Exception}");
            return Task.CompletedTask;
        }
    }
}
