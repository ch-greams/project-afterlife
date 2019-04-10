using System;
using System.Collections.Generic;
using System.Linq;


public static class ExtensionMethods
{
    public static Random random = new Random();


    public static T Random<T>(this T[] array)
    {
        return array[random.Next(0, array.Length - 1)];
    }

    public static T Random<T>(this IEnumerable<T> list)
    {
        return list.ToArray().Random();
    }
}
