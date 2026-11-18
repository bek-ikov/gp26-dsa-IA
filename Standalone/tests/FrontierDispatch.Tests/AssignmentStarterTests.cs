using FrontierDispatch.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FrontierDispatch.Tests
{
    /// <summary>
    /// Small contract examples for later assignment phases.
    ///
    /// These are not an exhaustive assignment test suite.
    /// Students are expected to enable the relevant tests as they reach each
    /// phase and add meaningful tests of their own, including edge cases.
    /// </summary>
    public sealed class AssignmentStarterTests
    {
        // These tests document later assignment contracts without making the
        // untouched baseline red. Remove Ignore when you reach the named phase.

        [Test] // Phase 3
        public void PriorityQueue_EnqueueAndDequeue_ReturnsLowestPriorityFirst()
        {
            var queue = new MinPriorityQueue<string>();

            queue.Enqueue("expensive", 9);
            queue.Enqueue("cheap", 2);
            queue.Enqueue("medium", 5);
            Assert.That(queue.Dequeue(), Is.EqualTo("cheap"));
        }

        [Test] // Phase 3
        public void PriorityQueue_MultipleDequeues_MaintainsPriorityOrder()
        {
            var queue = new MinPriorityQueue<string>();

            queue.Enqueue("highest", 10);
            queue.Enqueue("lowest", 1);
            queue.Enqueue("middle", 5);
            queue.Enqueue("second", 3);
            queue.Enqueue("fourth", 7);
            Assert.That(queue.Dequeue(), Is.EqualTo("lowest"));
            Assert.That(queue.Dequeue(), Is.EqualTo("second"));
            Assert.That(queue.Dequeue(), Is.EqualTo("middle"));
            Assert.That(queue.Dequeue(), Is.EqualTo("fourth"));
            Assert.That(queue.Dequeue(), Is.EqualTo("highest"));

            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test] // Phase 3
        public void PriorityQueue_DuplicatePriorities_ReturnsAllItems()
        {
            var queue = new MinPriorityQueue<string>();

            queue.Enqueue("first", 5);
            queue.Enqueue("second", 2);
            queue.Enqueue("third", 5);
            queue.Enqueue("fourth", 2);

            var results = new List<string>
            {
                queue.Dequeue(),
                queue.Dequeue(),
                queue.Dequeue(),
                queue.Dequeue()
            };

            Assert.That(results, Does.Contain("first"));
            Assert.That(results, Does.Contain("second"));
            Assert.That(results, Does.Contain("third"));
            Assert.That(results, Does.Contain("fourth"));
            Assert.That(results[0], Is.AnyOf("second", "fourth"));
            Assert.That(results[1], Is.AnyOf("second", "fourth"));
            Assert.That(results[2], Is.AnyOf("first", "third"));
            Assert.That(results[3], Is.AnyOf("first", "third"));
        }

        [Test] // Phase 4
        public void Dijkstra_StartEqualsGoal_ReturnsZeroCostRoute()
        {
            var finder = new DijkstraRouteFinder(WorldDatasetFactory.CreateSmall());
            RouteResult result = finder.FindRoute(0, 0);
            Assert.That(result.Found, Is.True);
            Assert.That(result.TotalCost, Is.EqualTo(0));
            Assert.That(result.LocationIds, Is.EqualTo(new[] { 0 }));
        }

        [Test] // Phase 4
        public void Dijkstra_FindsLeastCostRoute_ReturnsCorrectPathAndCost()
        {
            var finder = new DijkstraRouteFinder(SampleWorldFactory.CreateSmall());
            RouteResult result = finder.FindRoute(205, 734);
            Assert.That(result.Found, Is.True);
            Assert.That(result.TotalCost, Is.EqualTo(10));
            Assert.That(result.LocationIds, Is.EqualTo(new[] { 205, 310, 512, 734 }));
        }

        [Test] // Phase 4
        public void Dijkstra_UnreachableGoal_ReturnsNoRoute()
        {
            var finder = new DijkstraRouteFinder(SampleWorldFactory.CreateSmall());
            RouteResult result = finder.FindRoute(101, 999);
            Assert.That(result.Found, Is.False);
            Assert.That(result.TotalCost, Is.EqualTo(0));
            Assert.That(result.LocationIds, Is.Empty);
        }

        // TODO: Optimize phase 4 tests. Currently, a separate world gets created for each test. Opportunity for optimization: Simply make them all use the same world. Too lazy rn.

        [Test] // Phase 5
        public void Reachability_ZeroBudget_IncludesStartAtZeroCost()
        {
            var query = new ReachabilityQuery(WorldDatasetFactory.CreateSmall());
            var result = query.FindReachable(0, 0);
            Assert.That(result.ContainsKey(0), Is.True);
            Assert.That(result[0], Is.EqualTo(0));
        }

        [Test] // Phase 5
        public void Reachability_TravelBudget_ReturnsOnlyAffordableLocations()
        {
            var query = new ReachabilityQuery(SampleWorldFactory.CreateSmall());
            var result = query.FindReachable(205, 7);
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result[205], Is.EqualTo(0));
            Assert.That(result[101], Is.EqualTo(4));
            Assert.That(result[310], Is.EqualTo(3));
            Assert.That(result[901], Is.EqualTo(5));
            Assert.That(result[512], Is.EqualTo(7));
            Assert.That(result.ContainsKey(734), Is.False);
            Assert.That(result.ContainsKey(950), Is.False);
            Assert.That(result.ContainsKey(999), Is.False);
        }
    }
}
