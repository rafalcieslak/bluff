using System;
using System.Threading;
using System.Net.Sockets;

public class ClientConnection : PlayerListener
{
	TcpClient client;
	GameServer server;
	NetworkStream data_stream;
	Thread the_thread;

	Playable player;

	public ClientConnection (GameServer parent, TcpClient _client)
	{
		client = _client;
		server = parent;
		data_stream = client.GetStream();
		player = Game.game.NewPlayer(this);
		the_thread = new Thread(new ThreadStart(this.ThreadMain));
		the_thread.Start();
	}
	~ClientConnection()
	{
		System.Console.WriteLine("Client {0} destroyed.", GetName());
		Kill();
	}
	public void Kill()
	{
		the_thread.Abort();
		client.Close();
		the_thread.Join();
	}
	private void ThreadMain()
	{
		System.Console.WriteLine("Connection estabilished: {0}.", GetName());
		HandleIncomingData();
	}
	private String GetName(){
		return "[" + client.Client.RemoteEndPoint.ToString() + "]";
	}
	private void HandleIncomingData(){
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
		String[] data = s.Split('|');
		System.Console.WriteLine(data[0].Trim().ToLower());
		switch(data[0].Trim().ToLower()){
		case "setnick":
		case "setnickname":
			server.Say("Client {0} changes nickname to '{1}'.", GetName(),data[1]);
			player.SetNickname(data[1]);
			break;
		case "say":
			player.Say(data[1]);
			break;
		default:
			server.Say("Client {0} send an unrecognized command '{1}'.", GetName(),data[0]);
			break;
		}
	}
	public void SendData(String s){
		byte[] buffer = System.Text.Encoding.ASCII.GetBytes(s);
		data_stream.Write(buffer,0,buffer.Length);
	}
	public void ServerMessage(String s){
		SendData(String.Format ("servermsg|{0}",s));
	}
	public void Hear(String q, String s){
		SendData(String.Format ("say|{0}|{1};",q, s));
	}
	public void Introduce(int i, String s){
		SendData(String.Format ("player|{0}|{1};",i,s));
	}
	public void NotifyNicknameChange(int id, String oldname, String newname){
		SendData(String.Format("setnick|{0}|{1}|{2};",id,oldname,newname));
	}
}


