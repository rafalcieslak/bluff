using System;
using System.Collections.Generic;
public class Game
{
	public static Game game{
		get{
			return singleton_game;
		}
	}
	static Game singleton_game = new Game();

	private Game (){}
	static int PlayerCount = 0;
	List<Player> players = new List<Player>();

	public Player NewPlayer(PlayerListener c){
		PlayerCount++;
		Player p = new Player(c,PlayerCount);
		players.Add(p);
		Introduce(p);
		return p;
	}

	public void PlayerSays(Player player, String message){
		foreach(Player p in players){
			if(p != player) p.Hear(player,message);
		}
	}

	public Player GetPlayerByID(int n){
		Player found = null;
		foreach(Player p in players){
			if(p.GetID() == n) {
				found = p;
			}
		}
		return found;
	}
	public void Introduce(Player p){
		foreach(Player q in players){
			if(p != q){
				q.Introduce(p);
				p.Introduce(q);
			}
		}
	}
	public void BroadcastNicknameChange(Player p, String old){
		foreach(Player q in players){
			q.NotifyNicknameChange(p, old);
		}
	}
}

