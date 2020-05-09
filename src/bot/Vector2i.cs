using System;

public struct Vector2i
{
	public int x;
	public int y;

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static Vector2i operator +(Vector2i c1, Vector2i c2)
	{
		return new Vector2i(c1.x + c2.x, c1.y + c2.y);
	}

	public static Vector2i operator -(Vector2i c1, Vector2i c2)
	{
		return new Vector2i(c1.x - c2.x, c1.y - c2.y);
	}

	public static Vector2i operator *(Vector2i c1, int c2)
	{
		return new Vector2i(c1.x * c2, c1.y * c2);
	}

	public static Vector2i operator *(int c1, Vector2i c2)
	{
		return new Vector2i(c1 * c2.x, c1 * c2.y);
	}

	// allow callers to initialize
	public int this[int idx]
	{
		get { return idx == 0 ? x : y; }
		set
		{
			switch (idx)
			{
				case 0:
					x = value;
					break;
				default:
					y = value;
					break;
			}
		}
	}

	public double magnitude
	{
		get { return Math.Sqrt(x * x + y * y); }
	}

    public int size
    {
		get { return Math.Abs(x) + Math.Abs(y); }
    }

	public bool Equals(Vector2i p)
	{
		// If parameter is null return false:
		if ((object)p == null)
		{
			return false;
		}

		// Return true if the fields match:
		return (x == p.x) && (y == p.y);
	}

	public static bool operator ==(Vector2i a, Vector2i b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}

		// Return true if the fields match:
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(Vector2i a, Vector2i b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public override string ToString()
	{
		return "{" + x + ", " + y + "}";
	}
}