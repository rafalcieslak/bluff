using System;

public interface PlayerListener
{
	void Hear(String nickname, String text);
	void Introduce(int i, String s);
	void NotifyNicknameChange(int id, String oldname, String newname);
}

