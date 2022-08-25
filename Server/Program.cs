using System.Net.Sockets;
using System.Net;
using System.Text;
using Common.Core;

ServerTerminal? server = null;
try
{
    server = new ServerTerminal();

    server.MessageRecived += Server_MessageRecived;
    server.ClientConnect += Server_ClientConnect;
    server.ClientDisconnect += Server_ClientDisconnect;
    server.StartListen(9800);


    Console.WriteLine("Server Started");
    Console.WriteLine("Press any key to send test message to client");
    Console.ReadLine();
    server.SendMessage("Test");

    Console.ReadLine();


}
catch (Exception e)
{
    Console.WriteLine("Error..... " + e.StackTrace);
}

void Server_ClientDisconnect(Socket socket)
{
    Console.WriteLine($"Socket disconnect: ${socket.ProtocolType}");
}

void Server_ClientConnect(Socket socket)
{
    Console.WriteLine($"Socket connect: ${socket.ProtocolType}");
}

void Server_MessageRecived(string message)
{
    Console.WriteLine(message);
    server.SendMessage(message);
}