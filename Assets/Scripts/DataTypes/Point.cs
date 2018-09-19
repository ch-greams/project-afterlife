using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;


[Serializable]
[TypeConverter(typeof(PointConverter))]
public struct Point : IComparable<Point>
{
    public int x, y;

    /// <summary>
    /// Create point based on position in a grid
    /// </summary>
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Create point based on Vector3 coordinates
    /// </summary>
    public Point(Vector3 worldCoord, float tileSize)
    {
        Vector3 initPos = new Vector3(0, 0, 0);

        this.x = Mathf.RoundToInt((worldCoord.x - initPos.x) / tileSize - 0.5f);
        this.y = Mathf.RoundToInt((worldCoord.z - initPos.z) / tileSize - 0.5f);
    }

    public Point(string point)
    {
        MatchCollection matches = Regex.Matches(point, @"-?\d+");

        this.x = int.Parse(matches[0].Value);
        this.y = int.Parse(matches[1].Value);
    }

    // Implementing Default Methods

    public override string ToString()
    {
        return string.Format("({0:D3}, {1:D3})", x, y);
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y);
    }

    public static Point operator *(Point a, int b)
    {
        return new Point(a.x * b, a.y * b);
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Point a, Point b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is Point && this == (Point)obj;
    }

    public override int GetHashCode()
    {
        return x ^ y;
    }

    public int CompareTo(Point other)
    {
        int sumOther = other.x + other.y;
        int sumThis = this.x + this.y;

        return (sumOther > sumThis) ? -1 : ((sumOther == sumThis) ? 0 : 1);
    }

    // TODO: Either rework or remnove
    public Vector3 CalcWorldCoord(float h, float tileSize)
    {
        Vector3 initPos = new Vector3(0, 0, 0);

        float x = initPos.x + this.x * tileSize + tileSize / 2;
        float z = initPos.z + this.y * tileSize + tileSize / 2;

        return new Vector3(x, h, z);
    }

    public float DistanceTo(Point point)
    {
        return (Math.Abs(point.x - this.x) == Math.Abs(point.y - this.y)) ? 1.5F : 1F;
    }

    public float EstimateTo(Point point)
    {
        float dx = Math.Abs(point.x - this.x);
        float dy = Math.Abs(point.y - this.y);

        return (dx >= dy)
            ? (dx - dy) + dy * 1.5F
            : (dy - dx) + dx * 1.5F;
    }
}

// check this later
public class PointConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        object result = null;
        string stringValue = value as string;

        if (!string.IsNullOrEmpty(stringValue))
        {
            result = new Point(stringValue);
        }

        return result ?? base.ConvertFrom(context, culture, value);
    }
}
