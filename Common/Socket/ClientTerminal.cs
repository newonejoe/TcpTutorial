using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core;

public class ClientTerminal
{
    public event TCPTerminal_MessageRecivedDel MessageRecived;
    public event TCPTerminal_ConnectDel Connected;
    public event TCPTerminal_DisconnectDel Disconncted;

    private Socket m_socClient;
    private SocketListener m_listener;

    public void Connect(IPAddress remoteIPAddress, int alPort)
    {
        //create a new client socket ...
        m_socClient = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, alPort);

        // Connect
        m_socClient.Connect(remoteEndPoint);

        OnServerConnection();
    }

    public void SendMessage(string message)
    {
        if (m_socClient == null)
        {
            return;
        }
        byte[] byData = System.Text.Encoding.ASCII.GetBytes(message);
        m_socClient.Send(byData);

    }

    public void StartListen()
    {
        if (m_socClient == null)
        {
            return;
        }

        if (m_listener != null)
        {
            return;
        }

        m_listener = new SocketListener();
        m_listener.Disconnected += OnServerConnectionDroped;
        m_listener.MessageRecived += OnMessageRecvied;

        m_listener.StartReciving(m_socClient);
    }

    public string ReadData()
    {
        if (m_socClient == null)
        {
            return string.Empty;
        }

        byte[] buffer = new byte[1024];
        int iRx = m_socClient.Receive(buffer);
        char[] chars = new char[iRx];

        System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
        d.GetChars(buffer, 0, iRx, chars, 0);
        System.String szData = new System.String(chars);

        return szData;
    }

    public void Close()
    {
        if (m_socClient == null)
        {
            return;
        }

        if (m_listener != null)
        {
            m_listener.StopListening();
        }

        m_socClient.Close();
        m_listener = null;
        m_socClient = null;
    }

    private void OnServerConnection()
    {
        if (Connected != null)
        {
            Connected(m_socClient);
        }
    }

    private void OnMessageRecvied(string message)
    {
        if (MessageRecived != null)
        {
            MessageRecived(message);
        }
    }

    private void OnServerConnectionDroped(Socket socket)
    {
        Close();
        RaiseServerDisconnected(socket);
    }

    private void RaiseServerDisconnected(Socket socket)
    {
        if (Disconncted != null)
        {
            Disconncted(socket);
        }
    }


}
