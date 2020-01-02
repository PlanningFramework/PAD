using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PAD.Planner;
using PAD.Planner.Heaps;
using System;

namespace PAD.Tests
{
    /// <summary>
    /// Testing suite for the planner. Testing components of the planning problem and the searching engine.
    /// </summary>
    [TestClass]
    public class PlannerHeapsUnitTests
    {
        /// <summary>
        /// Gets full filepath to the specified test case.
        /// </summary>
        /// <param name="fileName">Test case file name.</param>
        /// <returns>Filepath to the test case.</returns>
        private string GetFilePath(string fileName)
        {
            return $@"..\..\Planner\TestCases\{fileName}";
        }

        [TestMethod]
        public void TC_BinomialHeap()
        {
            var heap = new BinomialHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Binomial Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_FibonacciHeap()
        {
            var heap = new FibonacciHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Fibonacci Heap (1)", heap.GetName());
        }

        [TestMethod]
        public void TC_FibonacciHeap2()
        {
            var heap = new FibonacciHeap2<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Fibonacci Heap (2)", heap.GetName());
        }

        [TestMethod]
        public void TC_LeftistHeap()
        {
            var heap = new LeftistHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Leftist Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_MeasuredHeap()
        {
            var heap = new MeasuredHeap<int>();
            HeapTest(heap, GetIntTestList());
            Assert.AreEqual("Measured Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_RadixHeap()
        {
            var heap = new RadixHeap<int>(30);
            HeapTest(heap, GetIntTestList());
            Assert.AreEqual("Radix Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_RedBlackTreeHeap()
        {
            var heap = new RedBlackTreeHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Red-Black Tree Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_RegularBinaryHeap()
        {
            var heap = new RegularBinaryHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Regular Binary Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_RegularTernaryHeap()
        {
            var heap = new RegularTernaryHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Regular Ternary Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_SimpleQueue()
        {
            var queue = new SimpleQueue<double>();

            var input = GetDoubleTestList();
            foreach (var item in input)
            {
                queue.Add(item, item);
            }
            Assert.AreEqual(input.Count, queue.GetSize());

            List<double> poppedItems = new List<double>();
            while (queue.GetSize() > 0)
            {
                poppedItems.Add(queue.RemoveMin());
            }
            Assert.IsTrue(CollectionsEquality.Equals(input, poppedItems));

            queue.Clear();
            Assert.AreEqual(0, queue.GetSize());
            Assert.AreEqual("Simple Queue", queue.GetName());
        }

        [TestMethod]
        public void TC_SimpleStack()
        {
            var stack = new SimpleStack<double>();

            var input = GetDoubleTestList();
            foreach (var item in input)
            {
                stack.Add(item, item);
            }
            Assert.AreEqual(input.Count, stack.GetSize());

            List<double> poppedItems = new List<double>();
            while (stack.GetSize() > 0)
            {
                poppedItems.Add(stack.RemoveMin());
            }
            input.Reverse();
            Assert.IsTrue(CollectionsEquality.Equals(input, poppedItems));

            stack.Clear();
            Assert.AreEqual(0, stack.GetSize());
            Assert.AreEqual("Simple Stack", stack.GetName());
        }

        [TestMethod]
        public void TC_SingleBucketHeap()
        {
            var heap = new SingleBucketHeap<int>(10);
            HeapTest(heap, GetIntTestList());
            Assert.AreEqual("Single Bucket Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_SortedDictionaryHeap()
        {
            var heap = new SortedDictionaryHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Sorted Dictionary Heap", heap.GetName());
        }

        [TestMethod]
        public void TC_SortedSetHeap()
        {
            var heap = new SortedSetHeap<double>();
            HeapTest(heap, GetDoubleTestList());
            Assert.AreEqual("Sorted Set Heap", heap.GetName());
        }

        /// <summary>
        /// Returns a test input for int heaps.
        /// </summary>
        /// <returns>Test input.</returns>
        private List<int> GetIntTestList()
        {
            return new List<int> { 5, 1, 9, 11, 2, 3, 5, 1, 4, 6, 12, 7, 0, 10, 8, 1, 20, 3, 9 };
        }

        /// <summary>
        /// Returns a test input for double heaps.
        /// </summary>
        /// <returns>Test input.</returns>
        private List<double> GetDoubleTestList()
        {
            return new List<double> { 5.2, 1, 1.5, 1.2, 9, 11.1, 2, 3.0, 0, 5.2, 1.7, 4, 6.1, 12.8, 7.4, 0.2, 10.0, 8.9, 1.0, 20.8, 2.0, 9.1 };
        }

        /// <summary>
        /// Does a generic heap test.
        /// </summary>
        /// <param name="heap">Heap to be checked.</param>
        private void HeapTest<Value>(IHeap<Value, Value> heap, List<Value> input) where Value : IComparable
        {
            List<Value> sortedInput = new List<Value>(input);
            sortedInput.Sort();

            input.ForEach(number => heap.Add((number), number));
            Assert.AreEqual(input.Count, heap.GetSize());

            Assert.AreEqual(sortedInput[0], heap.GetMinKey());

            heap.Clear();
            Assert.AreEqual(0, heap.GetSize());

            input.ForEach(number => heap.Add(number, number));
            List<Value> result = new List<Value>();
            while (heap.GetSize() != 0)
            {
                result.Add(heap.RemoveMin());
            }

            Assert.IsTrue(CollectionsEquality.Equals(sortedInput, result));
        }
    }
}
