using Photon.MmoDemo.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public enum CharStatus
{
    Idle,
    Moving,
    Atk
}

public enum MapType
{
    map = 0,
    collider = 1,
    trap = 2
}

public static class Global
{
    public static readonly List<Vector3Int> DirValue = new List<Vector3Int>()
    {
        new Vector3Int (1, 1,0),//N
        new Vector3Int (0, 1,0),
        new Vector3Int (-1, 1,0),
        new Vector3Int (-1, 0,0),

        new Vector3Int (-1, -1,0),//S
        new Vector3Int (0, -1,0),
        new Vector3Int (1, -1,0),
        new Vector3Int (1, 0,0),
    };

    public static readonly float SliceAttack = 0.2f;

    public static string ToMD5(string str)
    {
        string hashString;
        MD5 md5Hash = MD5.Create();
        hashString = GetMD5Hash(md5Hash, str);
        hashString = hashString.ToUpper();

        return hashString;
    }

    private static string GetMD5Hash(MD5 md5Hash, string input)
    {
        //Convert the input string to a byte array and compute the hash.
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        //Create a new StringBuilder to collect the bytes and create a string.
        StringBuilder builder = new StringBuilder();

        //Loop through each byte of the hashed data and format each one as a hexadecimal strings.
        for (int cnt = 0; cnt < data.Length; cnt++)
        {
            builder.Append(data[cnt].ToString("x2"));
        }

        //Return the hexadecimal string
        return builder.ToString();
    }

    public static Dictionary<int, string> MapName = new Dictionary<int, string>()
    {
        { 400, "隆特尼亞" },
        { 1600, "競技場" },
    };

    public static Vector3Int VtV3i(Vector vector)
    {
        return new Vector3Int((int)vector.X, (int)vector.Y, 0);
    }

    public static Vector2 VtV2(Vector vector)
    {
        return new Vector2(vector.X, vector.Y);
    }
}
