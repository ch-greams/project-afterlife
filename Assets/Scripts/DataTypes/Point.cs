using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable, InlineProperty(LabelWidth = 13), TypeConverter(typeof(PointConverter))]
public struct Point : IComparable<Point>
{
    [HorizontalGroup]
    public int x, y;

    public static Point zero { get { return new Point(0, 0); } }

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
    public Point(Vector3 pointCoordinates)
    {
        this.x = Mathf.FloorToInt(pointCoordinates.x);
        this.y = Mathf.FloorToInt(pointCoordinates.z);
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

    public Vector3 CalcWorldCoord(Vector3 offset)
    {
        return new Vector3(( this.x + offset.x ), offset.y, ( this.y + offset.z ));
    }

    public Vector3 CalcWorldCoord(Vector3 offset, float height)
    {
        return this.CalcWorldCoord(new Vector3(offset.x, height, offset.z));
    }

    public Vector3 CalcWorldCoord(Vector3 offset, float height, float shift)
    {
        return this.CalcWorldCoord(new Vector3(offset.x + shift, height, offset.z + shift));
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

// TODO: check this later
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
