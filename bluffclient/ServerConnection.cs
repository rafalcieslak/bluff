using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;

public class ServerConnection
{
	TcpClient tcpclient;
	NetworkStream data_stream;
	Thread the_thread;
	public ServerConnection ()
	{
		tcpclient = new TcpClient();
		tcpclient.Connect(IPAddress.Parse("127.0.0.1"),11732);
		data_stream = tcpclient.GetStream();
		the_thread = new Thread(new ThreadStart(this.ThreadMain));
		the_thread.Start();
	}
	~ServerConnection(){
		tcpclient.Close ();
		the_thread.Abort();
		the_thread.Join	();
	}
	private void ThreadMain(){
		byte[] buffer = new byte[1024];
		String message = "";
		while(true){
			while(!data_stream.DataAvailable)
				Thread.Sleep(20);
			int read = data_stream.Read(buffer,0,buffer.Length);
			message = String.Concat(message,System.Text.Encoding.ASCII.GetString(buffer,0,read));
			if(message.Contains(";")){
				String[] messages = message.Split(';');
				int n = messages.Length;
				for(int i = 0; i < n-1; i++){
					InterpretMessage(messages[i]);
				}
				message = messages[n-1];
			}
		}
	}
	private void InterpretMessage(String s){
		System.Console.WriteLine("Got message {0} from server.",s);
		String[] data = s.Split('|');
		System.Console.WriteLine(data[0].Trim().ToLower());
		int playerid;
		Player p ;
		switch(data[0].Trim().ToLower()){
		case "say":
			playerid = Convert.ToInt32(data[1]);
			p = Game.game.GetPlayerByID(playerid);
			System.Console.WriteLine("Player {0} [{1}] says {2}",playerid,p.GetNickname(),data[2]);
			break;
		case "setnick":
		case "setnickname":
			playerid = Convert.ToInt32(data[1]);
			p = Game.game.GetPlayerByID(playerid);
			System.Console.WriteLine("Player {0} [{1}] changes nickname to {2}",playerid,p.GetNickname(),data[2]);
			p.SetNickname(data[2]);
			break;
		case "player":
			playerid = Convert.ToInt32(data[1]);
			p = Game.game.GetPlayerByID(playerid);
			if(p==null){
				p = new LocalPlayer(playerid);
			}
			p.SetNickname(data[2]);
			break;
		default:
			System.Console.WriteLine("Unknown message from server: {0}.",s);
			break;
		}
	}

}

