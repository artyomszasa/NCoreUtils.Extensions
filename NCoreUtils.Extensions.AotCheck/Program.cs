// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using NCoreUtils;



DoWrite(new YearAndMonth(2040, 10));


[MethodImpl(MethodImplOptions.NoInlining)]
static void DoWrite(YearAndMonth s)
{
    Console.WriteLine(s.ToString());
}