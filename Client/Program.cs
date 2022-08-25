// See https://aka.ms/new-console-template for more information
using Common.Core;
using System.Net.Sockets;

ClientTerminal? clientTerminal = null;

var _datamanClient_cts = new CancellationTokenSource();

// Start the DatamanClient
Task.Factory.StartNew(() => StartClient(_datamanClient_cts.Token), _datamanClient_cts.Token);


if (Environment.UserInteractive)
{
    string inputKey = string.Empty;
    //Read Command from
    while (inputKey != "Q")
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey().Key;

            switch (key)
            {

                case ConsoleKey.T:
                    clientTerminal.SendMessage("+");
                    break;
                case ConsoleKey.Add:
                    clientTerminal.SendMessage("+");
                    break;
                case ConsoleKey.Q:
                    inputKey = "Q";
                    break;
                default:
                    break;
            }
        }
    }
}

// Read Key, Press any key to exit
Console.WriteLine("Press any key to exit");
Console.ReadKey();


async void StartClient(CancellationToken token)
{
    // Initial the terminal
    clientTerminal = new ClientTerminal();

    // Subscribe events
    clientTerminal.Connected += ClientTerminal_Connected;
    clientTerminal.Disconncted += ClientTerminal_Disconncted;
    clientTerminal.MessageRecived += ClientTerminal_MessageRecived;


    // Start connection
    // clientTerminal.Connect(System.Net.IPAddress.Parse(m_IpAddress), m_Port);
    // clientTerminal.Connect(System.Net.IPAddress.Loopback, 9800);
    clientTerminal.Connect(System.Net.IPAddress.Loopback, 56677);

    // Start listen
    clientTerminal.StartListen();
    await Task.Delay(100);
}

void ClientTerminal_MessageRecived(string message)
{
    Console.WriteLine($"Received: {message}");
}

void ClientTerminal_Disconncted(Socket socket)
{
    Console.WriteLine("Dataman Device Disconnected");
}

void ClientTerminal_Connected(Socket socket)
{
    Console.WriteLine("Server Connected");
    Console.WriteLine("Press T to send the trigger, Press Q to exit the console");
}
