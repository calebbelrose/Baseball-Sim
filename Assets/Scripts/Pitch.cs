using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitch : IComparable <Pitch>
{
	protected int minSpeed, speedVariance;
	protected float effectiveness;

	// Calculates the effectiveness of the pitch
	public void CalculateEffectiveness (int speed, int movement)
	{
		effectiveness = 90f + (minSpeed + speedVariance) * speed * movement / 100000f;
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
	public FourSeam (int speed, int movement)
	{
		minSpeed = 85;
		speedVariance = 20;
		CalculateEffectiveness (speed, movement);
	}
}

public class TwoSeam : Pitch
{
	public TwoSeam (int speed, int movement)
	{
		minSpeed = 80;
		speedVariance = 20;
		CalculateEffectiveness (speed, movement);
	}
}

public class Cutter : Pitch
{
	public Cutter (int speed, int movement)
	{
		minSpeed = 85;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Shuuto : Pitch
{
	public Shuuto (int speed, int movement)
	{
		minSpeed = 85;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Sinker : Pitch
{
	public Sinker (int speed, int movement)
	{
		minSpeed = 80;
		speedVariance = 20;
		CalculateEffectiveness (speed, movement);
	}
}

public class Splitter : Pitch
{
	public Splitter (int speed, int movement)
	{
		minSpeed = 80;
		speedVariance = 15;
		CalculateEffectiveness (speed, movement);
	}
}

public class Gyroball : Pitch
{
	public Gyroball (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Curveball : Pitch
{
	public Curveball (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 20;
		CalculateEffectiveness (speed, movement);
	}
}

public class KnuckleCurve : Pitch
{
	public KnuckleCurve (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Screwball : Pitch
{
	public Screwball (int speed, int movement)
	{
		minSpeed = 65;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Slider : Pitch
{
	public Slider (int speed, int movement)
	{
		minSpeed = 80;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Slurve : Pitch
{
	public Slurve (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class CircleChangeup : Pitch
{
	public CircleChangeup (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Forkball : Pitch
{
	public Forkball (int speed, int movement)
	{
		minSpeed = 75;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Fosh : Pitch
{
	public Fosh (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Palmball : Pitch
{
	public Palmball (int speed, int movement)
	{
		minSpeed = 65;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class VulcanChangeup : Pitch
{
	public VulcanChangeup (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}

public class Eephus : Pitch
{
	public Eephus (int speed, int movement)
	{
		minSpeed = 45;
		speedVariance = 20;
		CalculateEffectiveness (speed, movement);
	}
}

public class Knuckleball : Pitch
{
	public Knuckleball (int speed, int movement)
	{
		minSpeed = 70;
		speedVariance = 10;
		CalculateEffectiveness (speed, movement);
	}
}