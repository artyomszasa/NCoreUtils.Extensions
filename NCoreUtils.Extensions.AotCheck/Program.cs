// See https://aka.ms/new-console-template for more information
using NCoreUtils;

Console.WriteLine("Hello, World!");

var pool = new FixSizePool2<MyObject>(16);

pool.Return(new MyObject { Value = 1 });
pool.Return(new MyObject { Value = 2 });
Console.WriteLine(pool.AvailableCount);
pool.TryRent(out var v);
Console.WriteLine(pool.AvailableCount);
Console.WriteLine(v?.Value);


class MyObject { public int Value { get; set; } }
