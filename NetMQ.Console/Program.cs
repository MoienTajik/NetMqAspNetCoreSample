using NetMQ.Sockets;
using System.Collections.Generic;

namespace NetMQ.Console
{
    internal static class Program
    {
        private static void Main()
        {
            using var client = new RequestSocket("tcp://localhost:8050");

            const string messageToSend = "Hello";
            System.Console.WriteLine($"Sending message '{messageToSend}'...");

            client.SendFrame(messageToSend);
            System.Console.WriteLine("Message has been sent.");

            List<string> messages = client.ReceiveMultipartStrings(2);
            foreach (string message in messages)
            {
                System.Console.WriteLine($"Server response: {message}");
            }
        }
    }
}