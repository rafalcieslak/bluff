using System;

public class Player : Playable
{
	String nickname = "Unnamed";
	PlayerListener listener;
	int id;
	public int GetID() {return id;}
	public void SetNickname(String s){
		String oldname = nickname;
		nickname = s;
		Game.game.BroadcastNicknameChange(this,oldname);
	}
	public String GetNickname()      { return nickname;}

	public void Say(String s){
		Game.game.PlayerSays(this,s);
	}
	public void Hear(Player p, String s){
		listener.Hear(p.GetNickname(),s);
	}
	public Player (PlayerListener c, int _id)
	{
		listener = c;
		id = _id;
	}
	public void Introduce(Player p){
		listener.Introduce(p.GetID(),p.GetNickname());
	}
	public void NotifyNicknameChange(Player p, String old){
		listener.NotifyNicknameChange(p.GetID(),old,p.GetNickname());
	}
}

