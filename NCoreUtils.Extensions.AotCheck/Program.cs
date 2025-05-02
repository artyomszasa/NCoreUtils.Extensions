// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using NCoreUtils;



// DoWrite(new YearAndMonth(2040, 10));
// [MethodImpl(MethodImplOptions.NoInlining)]
// static void DoWrite(YearAndMonth s)
// {
//     Console.WriteLine(s.ToString());
// }
//
// Console.WriteLine("Hello, World!");


DoWork();

[MethodImpl(MethodImplOptions.NoInlining)]
static void DoWork()
{

    var pool = new FixSizePool<MyObject>(16);
    pool.Return(new MyObject { Value = 1 });
    pool.Return(new MyObject { Value = 2 });
    Console.WriteLine(pool.AvailableCount);
    pool.TryRent(out var v);
    Console.WriteLine(pool.AvailableCount);
    Console.WriteLine(v?.Value);
}


class MyObject { public int Value { get; set; } }
