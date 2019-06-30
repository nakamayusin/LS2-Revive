using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public static class Adrn
{
    public static SortedDictionary<int, int> Images = new SortedDictionary<int, int>();
    public static Dictionary<int, Real> Reals = new Dictionary<int, Real>();

    public static void Init(Stream stream)
    {
        Images.Clear();
        Reals.Clear();

        byte[] buffer = new byte[76];
        int capacity = ((int)stream.Length) / buffer.Length;
        Reals = new Dictionary<int, Real>(capacity);
        Images = new SortedDictionary<int, int>(new Dictionary<int, int>(capacity));

        int imageCount = (int)stream.Length / buffer.Length;

        for (int i = 0; i < imageCount; i++)
        {
            stream.Read(buffer, 0, buffer.Length);
            Real real = new Real();

            real.index = BitConverter.ToInt32(buffer, 0 * 4);
            real.position = BitConverter.ToInt32(buffer, 1 * 4);
            if (real.position == 0x66C49B8)
                real.position = BitConverter.ToInt32(buffer, 1 * 4);

            real.offsetX = BitConverter.ToInt32(buffer, 2 * 4);
            real.offsetY = BitConverter.ToInt32(buffer, 3 * 4);
            real.height = BitConverter.ToInt32(buffer, 4 * 4);
            real.width = BitConverter.ToInt32(buffer, 5 * 4);
            real.imageno = BitConverter.ToInt32(buffer, 72);
            if (real.imageno == 0) //|| real.index < CharAnimateMaker.invo || real.index > CharAnimateMaker.invo + 1280 * 4
                continue;

            Reals[real.index] = real;
            Images[real.imageno] = real.index;            
        }
    }
}

public class Real
{
    public int index;
    public int position;
    public int length;
    public int offsetX;
    public int offsetY;
    public int height;
    public int width;
    public int imageno;
}