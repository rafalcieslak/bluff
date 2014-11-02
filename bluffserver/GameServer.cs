using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public sealed class GameServer
{
	public static GameServer server
	{
		get
		{
			return singleton_server;
		}
	}
	private static GameServer singleton_server = new GameServer();

	Thread server_thread;
	bool running = false;

	List<ClientConnection> clients = new List<ClientConnection>();

	private GameServer(){}
	~GameServer(){
		clients.Clear();
		Stop();
	}

	public void Start()
	{
		if(running) return; 
		System.Console.WriteLine("Server started.");
		server_thread = new Thread(new ThreadStart(this.ServerThreadMain));
		server_thread.Start();
		running = true;
	}
	public void Stop()
	{
		if (!running) return;
		foreach(ClientConnection c in clients){
			c.Kill();
		}
		server_thread.Abort();
		server_thread.Join();
		running = false;
	}

	private void ServerThreadMain()
	{
		TcpListener tcplistener = new TcpListener(IPAddress.Parse("127.0.0.1"),11732);
		tcplistener.Start();
		while(true){
			TcpClient new_client = tcplistener.AcceptTcpClient();
			ClientConnection client_connection = new ClientConnection(this,new_client);
			clients.Add(client_connection);
		}
	}
	public void Say(String s, params Object[] o)
	{
		String msg = String.Format("SERVER: {0}",s);
		msg = String.Format(msg,o);
		System.Console.WriteLine(msg);
	}
	public void Shout(String s, params Object[] o)
	{
		String msg = String.Format(s,o);
		foreach(ClientConnection c in clients){
			c.ServerMessage(msg);
		}
	}
}

