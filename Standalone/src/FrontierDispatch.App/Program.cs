using System;
using System.Diagnostics;
using FrontierDispatch.Core;

if (args.Length > 0 && args[0] == "--baseline")
{
    RunBaseline();
    return;
}

if (args.Length > 0 && args[0] == "--loading")
{
    RunLoadingBenchmark();
    return;
}

if (args.Length > 0 && args[0] == "--allocations")
{
    RunAllocationBenchmark();
    return;
}

// Small sanity-check application.
// This is separate from the larger performance workload below.
WorldData world = SampleWorldFactory.CreateSmall();
var queries = new WorldQueries(world);

Console.WriteLine("Frontier Dispatch - assignment starter");
Console.WriteLine($"Small demo locations: {world.Locations.Count}");
Console.WriteLine($"Find 734: {queries.FindLocationById(734)?.Name ?? "missing"}");
Console.WriteLine();
Console.WriteLine("Run with --baseline to record the supplied baseline workload.");

static void RunBaseline()
{
    // Build the dataset before timing the repeated runtime queries.
    // This baseline therefore measures query work, not world construction.
    WorldData world = WorldDatasetFactory.CreateMedium();
    var queries = new WorldQueries(world);

    // The checksum provides a stable sanity check that the workload still
    // produces equivalent results after implementation changes.
    long checksum = 0;

    const int iterations = 5000;
    const int repetitions = 100;

    for (int i = 0; i < iterations; i++)
    {
        int id = (i * 997) % world.Locations.Count;
        Location? location = queries.FindLocationById(id);
        checksum += location?.Id ?? 0;
        checksum += queries.FindOutgoingConnections(id).Count;
    }

    checksum = 0;

    var sw = Stopwatch.StartNew();

    for (int repetition = 0; repetition < repetitions; repetition++)
    {
        for (int i = 0; i < iterations; i++)
        {
            int id = (i * 997) % world.Locations.Count;
            Location? location = queries.FindLocationById(id);
            checksum += location?.Id ?? 0;
            checksum += queries.FindOutgoingConnections(id).Count;
        }
    }

    sw.Stop();

    Console.WriteLine("Baseline world-query workload");
    Console.WriteLine($"Locations: {world.Locations.Count}");
    Console.WriteLine($"Connections: {world.Connections.Count}");
    Console.WriteLine($"Iterations: {iterations}");
    Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds:F3} ms");
    Console.WriteLine($"Average per workload: {sw.Elapsed.TotalMilliseconds / repetitions:F3} ms");
    Console.WriteLine($"Checksum: {checksum}");
    Console.WriteLine("Absolute time varies by machine. Keep this result for before/after comparison.");
}

static void RunLoadingBenchmark()
{
    const int repetitions = 10;
    WorldData warmup = WorldDatasetFactory.CreateMedium();

    long totalLocations = 0;
    long totalConnections = 0;

    var sw = Stopwatch.StartNew();

    for (int i = 0; i < repetitions; i++)
    {
        WorldData world = WorldDatasetFactory.CreateMedium();

        totalLocations += world.Locations.Count;
        totalConnections += world.Connections.Count;
    }

    sw.Stop();

    Console.WriteLine("World loading and preparation workload");
    Console.WriteLine($"Repetitions: {repetitions}");
    Console.WriteLine($"Average elapsed: {sw.Elapsed.TotalMilliseconds / repetitions:F3} ms");
    Console.WriteLine($"Total locations: {totalLocations}");
    Console.WriteLine($"Total connections: {totalConnections}");
}

static void RunAllocationBenchmark()
{
    WorldData world = WorldDatasetFactory.CreateMedium();
    var queries = new WorldQueries(world);

    const int iterations = 5000;

    for (int i = 0; i < iterations; i++)
    {
        int id = (i * 997) % world.Locations.Count;
        queries.FindLocationById(id);
        queries.FindOutgoingConnections(id);
    }

    long checksum = 0;

    long before = GC.GetAllocatedBytesForCurrentThread();

    for (int i = 0; i < iterations; i++)
    {
        int id = (i * 997) % world.Locations.Count;

        Location? location = queries.FindLocationById(id);
        checksum += location?.Id ?? 0;
        checksum += queries.FindOutgoingConnections(id).Count;
    }

    long after = GC.GetAllocatedBytesForCurrentThread();

    Console.WriteLine("World-query allocation workload");
    Console.WriteLine($"Iterations: {iterations}");
    Console.WriteLine($"Allocated bytes: {after - before:N0}");
    Console.WriteLine($"Checksum: {checksum}");
}