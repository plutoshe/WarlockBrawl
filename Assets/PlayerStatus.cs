using System;


public class PlayerStatus
{
	public string name;
	public int score;
	public int color;

	public PlayerStatus (string name_, int color_)
	{
		name = name_;
		color = color_;
		score = 0;
	}

	public string ToString() {
		return name + " " + score + " " + color.ToString();
	}
		
}

