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
    
    public double x, y, z;
    public double magnitude => Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
    public UniverseCoordinates normalized 
    {
        get
        {
            double mag = magnitude;
            return new UniverseCoordinates(x / mag, y / mag, z / mag);
        }
    }

    public UniverseCoordinates(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public UniverseCoordinates(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
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

    public static double Distance(UniverseCoordinates a, UniverseCoordinates b)
    {
        double xSqu = Math.Pow(a.x - b.x, 2);
        double ySqu = Math.Pow(a.y - b.y, 2);
        double zSqu = Math.Pow(a.z - b.z, 2);
        return Math.Sqrt(xSqu + ySqu + zSqu);
    } 
}
