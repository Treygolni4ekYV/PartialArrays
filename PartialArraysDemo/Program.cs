using PartialArrays.Arrays.imp;

namespace PartialArraysDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PartialIntArray pa = new PartialIntArray(3.5);

            pa[0] = 4;
            pa[1] = 2;
            pa[2] = 3;
            pa[3] = 01020304;

            Console.WriteLine(pa[0]);
            Console.WriteLine(pa[1]);
            Console.WriteLine(pa[2]);
            Console.WriteLine(pa[3]);
        }
    }
}
