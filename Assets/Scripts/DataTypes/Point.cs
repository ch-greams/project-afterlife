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
    public Point(Vector3 pointCoordinates, Vector3 zeroPointCoordinates)
    {
        this.x = Mathf.FloorToInt(pointCoordinates.x - zeroPointCoordinates.x);
        this.y = Mathf.FloorToInt(pointCoordinates.z - zeroPointCoordinates.z);
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
        return (this.x == other.x) ? (this.y - other.y) : (this.x - other.x);
    }

    public Vector3 CalcWorldCoord(float height)
    {
        return new Vector3(( this.x + 0.5F ), height, ( this.y + 0.5F ));
    }

    public Vector3 CalcWorldCoord(float height, Vector3 zeroPointCoordinates)
    {
        return new Vector3(
            (zeroPointCoordinates.x + this.x + 0.5F),
            height,
            (zeroPointCoordinates.z + this.y + 0.5F)
        );
    }

    public float DistanceTo(Point point)
    {
        return (Math.Abs(point.x - this.x) == Math.Abs(point.y - this.y)) ? 1.5F : 1F;
    }

    public float EstimateTo(Point point)
    {
        float dx = Math.Abs(point.x - this.x);
        float dy = Math.Abs(point.y - this.y);

        return (dx >= dy) ? ((dx - dy) + dy * 1.5F) : ((dy - dx) + dx * 1.5F);
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
