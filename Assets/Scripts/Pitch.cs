using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitch : IComparable <Pitch>
{
	protected int minSpeed;			// Minimum speed of the pitch
	protected int speedVariance;	// Maximum variance in speeds of the pitch
	protected float effectiveness;	// Effectiveness of the pitch based off the pitcher's skills

	public Pitch ()
	{
	}

	// Calculates the effectiveness of the pitch
	public void CalculateEffectiveness (int speed, int movement)
	{
		if((effectiveness = (90f + (minSpeed + speedVariance) * speed * movement / 100000f) * (float)(Manager.Instance.RandomGen.NextDouble () / 5 + 0.9)) > 100.0)
			effectiveness = 100.0f;
	}

	public int CompareTo (Pitch other)
	{
		if (other == null)
			return 1;
		else
			return effectiveness.CompareTo (other.Effectiveness);
	}

	public float Effectiveness
	{
		get
		{
			return effectiveness;
		}
	}
}

public class FourSeam : Pitch
{
	public FourSeam (int speed, int movement, float _effectiveness)
	{
		minSpeed = 85;
		speedVariance = 20;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}

	public override string ToString ()
	{
		return "Four-Seam Fastball";
	}
}

public class TwoSeam : Pitch
{
	public TwoSeam (int speed, int movement, float _effectiveness)
	{
		minSpeed = 80;
		speedVariance = 20;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}

	public override string ToString ()
	{
		return "Two-Seam Fastball";
	}
}

public class Cutter : Pitch
{
	public Cutter (int speed, int movement, float _effectiveness)
	{
		minSpeed = 85;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Shuuto : Pitch
{
	public Shuuto (int speed, int movement, float _effectiveness)
	{
		minSpeed = 85;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Sinker : Pitch
{
	public Sinker (int speed, int movement, float _effectiveness)
	{
		minSpeed = 80;
		speedVariance = 20;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Splitter : Pitch
{
	public Splitter (int speed, int movement, float _effectiveness)
	{
		minSpeed = 80;
		speedVariance = 15;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Gyroball : Pitch
{
	public Gyroball (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Curveball : Pitch
{
	public Curveball (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 20;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class KnuckleCurve : Pitch
{
	public KnuckleCurve (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}

	public override string ToString ()
	{
		return "Knuckle Curve";
	}
}

public class Screwball : Pitch
{
	public Screwball (int speed, int movement, float _effectiveness)
	{
		minSpeed = 65;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Slider : Pitch
{
	public Slider (int speed, int movement, float _effectiveness)
	{
		minSpeed = 80;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Slurve : Pitch
{
	public Slurve (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class CircleChangeup : Pitch
{
	public CircleChangeup (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}

	public override string ToString ()
	{
		return "Circle Changeup";
	}
}

public class Forkball : Pitch
{
	public Forkball (int speed, int movement, float _effectiveness)
	{
		minSpeed = 75;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Fosh : Pitch
{
	public Fosh (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Palmball : Pitch
{
	public Palmball (int speed, int movement, float _effectiveness)
	{
		minSpeed = 65;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class VulcanChangeup : Pitch
{
	public VulcanChangeup (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}

	public override string ToString ()
	{
		return "Vulcan Changeup";
	}
}

public class Eephus : Pitch
{
	public Eephus (int speed, int movement, float _effectiveness)
	{
		minSpeed = 45;
		speedVariance = 20;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}

public class Knuckleball : Pitch
{
	public Knuckleball (int speed, int movement, float _effectiveness)
	{
		minSpeed = 70;
		speedVariance = 10;

		if (_effectiveness == -1.0f)
			CalculateEffectiveness (speed, movement);
		else
			effectiveness = _effectiveness;
	}
}