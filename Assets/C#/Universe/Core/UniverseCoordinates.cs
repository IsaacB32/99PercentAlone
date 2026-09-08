using System;
using System.Globalization;
using UnityEngine;

//=!= (currently just a wrapper for Vector3Int) =!=

/// <summary>
/// Universe Coordinates for describing where things are in the universe
/// </summary>
[Serializable]
public struct UniverseCoordinates : IEquatable<UniverseCoordinates>, IFormattable
{
    //===== Readonly =====
    
    public static readonly UniverseCoordinates zero = new UniverseCoordinates(0, 0, 0);
    
    //===== Class =====
    
    public int x, y, z;
    public float magnitude => Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
    public Vector3 normalized 
    {
        get
        {
            float mag = magnitude;
            return new Vector3(x / mag, y / mag, z / mag);
        }
    }
    
    public UniverseCoordinates(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public UniverseCoordinates(Vector3 vector3)
    {
        x = Mathf.RoundToInt(vector3.x);
        y = Mathf.RoundToInt(vector3.y);
        z = Mathf.RoundToInt(vector3.z);
    }

    //===== Operators =====

    public static UniverseCoordinates operator +(UniverseCoordinates left, UniverseCoordinates right)
    {
        return new UniverseCoordinates(left.x + right.x, left.y + right.y, left.z + right.z);
    }
    public static UniverseCoordinates operator -(UniverseCoordinates left, UniverseCoordinates right) 
    {
        return new UniverseCoordinates(left.x - right.x, left.y - right.y, left.z - right.z);
    }
    public static UniverseCoordinates operator *(UniverseCoordinates left, UniverseCoordinates right)
    {
        return new UniverseCoordinates(left.x * right.x, left.y * right.y, left.z * right.z);
    }
    public static UniverseCoordinates operator *(UniverseCoordinates left, int right) 
    {
        return new UniverseCoordinates(left.x * right, left.y * right, left.z * right);
    }
    
    public static explicit operator Vector3(UniverseCoordinates item) => new Vector3((float)item.x, (float)item.y, (float)item.z);
    
    //===== Interface Implementation =====
    
    public bool Equals(UniverseCoordinates other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is UniverseCoordinates other && Equals(other);
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;
        return $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)}, {z.ToString(format, formatProvider)})";
    }
    
    //===== Static Methods =====

    public static float SqrDistance(UniverseCoordinates a, UniverseCoordinates b)
    {
        float xSqu = Mathf.Pow(a.x - b.x, 2);
        float ySqu = Mathf.Pow(a.y - b.y, 2);
        float zSqu = Mathf.Pow(a.z - b.z, 2);
        return xSqu + ySqu + zSqu;
        // return Mathf.Sqrt(xSqu + ySqu + zSqu);
    } 
    
    public static float Distance(UniverseCoordinates a, UniverseCoordinates b)
    {
        float xSqu = Mathf.Pow(a.x - b.x, 2);
        float ySqu = Mathf.Pow(a.y - b.y, 2);
        float zSqu = Mathf.Pow(a.z - b.z, 2);
        return Mathf.Sqrt(xSqu + ySqu + zSqu);
    } 
}
