using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace PAD.Planner.Heaps
{
    /// <summary>
    /// Implementation of a measured heap (for testing and evaluation purposes - not intended for practical use).
    /// </summary>
    /// <typeparam name="Value"></typeparam>
    public class MeasuredHeap<Value> : IHeap<int, Value>
    {
        /// <summary>
        /// Internal heap collection.
        /// </summary>
        private IHeap<double, HeapNode> InternalHeap { set; get; } = new RegularBinaryHeap<HeapNode>();

        /// <summary>
        /// Writer object for message logging.
        /// </summary>
        private StreamWriter Logger { set; get; } = null;

        /// <summary>
        /// Time stamp.
        /// </summary>
        private int TimeStamp { set; get; } = 0;

        /// <summary>
        /// Heap statistics.
        /// </summary>
        private HeapStatistics Stats { set; get; } = new HeapStatistics();

        /// <summary>
        /// Destructor for the object.
        /// </summary>
        ~MeasuredHeap()
        {
            if (Logger != null)
            {
                Logger.Close();
            }
        }

        /// <summary>
        /// Adds a new key-value pair into the collection.
        /// </summary>
        /// <param name="key">Key item.</param>
        /// <param name="value">Value item.</param>
        public void Add(int key, Value value)
        {
            var newElement = new HeapNode(key, value, TimeStamp++);
            InternalHeap.Add(newElement.Key, newElement);

            if (Logger != null)
            {
                Logger.WriteLine("i\t" + key);
            }
            Stats.UpdateAfterInsert(newElement);
        }

        /// <summary>
        /// Gets the value item with the minimal key and deletes the element from the collection.
        /// </summary>
        /// <returns>Value item with the minimal key.</returns>
        public Value RemoveMin()
        {
            var result = InternalHeap.RemoveMin();
            if (Logger != null)
            {
                Logger.WriteLine("r\t" + result.Key);
            }
            Stats.UpdateAfterRemove(result);
            return result.Value;
        }

        /// <summary>
        /// Gets the minimal key of the collection.
        /// </summary>
        /// <returns>Minimal key.</returns>
        public int GetMinKey()
        {
            return (int)InternalHeap.GetMinKey();
        }

        /// <summary>
        /// Gets the size of the collection, i.e. number of included elements.
        /// </summary>
        /// <returns>Collection size.</returns>
        public int GetSize()
        {
            return InternalHeap.GetSize();
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            InternalHeap.Clear();
            Stats.Clear();
            TimeStamp = 0;
        }

        /// <summary>
        /// Gets the collection identification.
        /// </summary>
        /// <returns>Collection name.</returns>
        public string GetName()
        {
            return "Measured Heap";
        }

        /// <summary>
        /// Sets the logging output file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public void SetLoggingOutputFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Logger = new StreamWriter(filePath);
        }

        /// <summary>
        /// Clears the heap statistics.
        /// </summary>
        public void ClearStats()
        {
            if (Logger != null)
            {
                Logger.Close();
            }
            Stats.Clear();
        }

        /// <summary>
        /// Prints the heap statistics.
        /// </summary>
        public void PrintStats()
        {
            Stats.PrintStats();

            long totalAgesSum = 0;
            int oldesElement = 0;
            int numberOfElementsGreaterThanExtractionLimit = 0;

            while (InternalHeap.GetSize() > 0)
            {
                var element = InternalHeap.RemoveMin();
                var age = TimeStamp - element.TimeStamp;
                if (oldesElement < age)
                {
                    oldesElement = age;
                }
                totalAgesSum += age;

                if (element.Key > Stats.HighestExtractedElement)
                {
                    numberOfElementsGreaterThanExtractionLimit++;
                }
            }

            totalAgesSum += Stats.ExtractedElementsAgeTotalSum;
            Console.WriteLine("\t\tAverage element's age (all):\t" + totalAgesSum / (Stats.ElementsAges.Count + Stats.CurrentHeapSize));
            Console.WriteLine("\t\tOldest extracted:\t\t" + Stats.OldestExtractedElement);
            Console.WriteLine("\t\tOldest not extracted:\t\t" + oldesElement);
            Console.WriteLine("\t\tElements beyond extraction limit:\t" + numberOfElementsGreaterThanExtractionLimit);
        }

        /// <summary>
        /// Heap node structure.
        /// </summary>
        private struct HeapNode : IComparable<HeapNode>
        {
            /// <summary>
            /// Key.
            /// </summary>
            public int Key;

            /// <summary>
            /// Value.
            /// </summary>
            public Value Value;

            /// <summary>
            /// Time stamp.
            /// </summary>
            public int TimeStamp;

            /// <summary>
            /// Creates the heap node.
            /// </summary>
            /// <param name="key">Key.</param>
            /// <param name="value">Value.</param>
            /// <param name="timeStamp">Time stamp.</param>
            public HeapNode(int key, Value value, int timeStamp)
            {
                Key = key;
                Value = value;
                TimeStamp = timeStamp;
            }

            /// <summary>
            /// Compares the element to the other one.
            /// </summary>
            /// <param name="other">Other element.</param>
            /// <returns>Comparison value.</returns>
            int IComparable<HeapNode>.CompareTo(HeapNode other)
            {
                return (Key - other.Key);
            }
        }

        /// <summary>
        /// Heap statistics.
        /// </summary>
        private class HeapStatistics
        {
            /// <summary>
            /// Highest added element.
            /// </summary>
            public int HighestAddedElement { set; get; } = int.MinValue;

            /// <summary>
            /// Lowest added element.
            /// </summary>
            public int LowestAddedElement { set; get; } = int.MaxValue;

            /// <summary>
            /// Highest removed element.
            /// </summary>
            public int HighestExtractedElement { set; get; } = int.MinValue;

            /// <summary>
            /// Oldest removed element.
            /// </summary>
            public int OldestExtractedElement { set; get; } = 0;

            /// <summary>
            /// Number of inserts.
            /// </summary>
            public int NumberOfInserts { set; get; } = 0;

            /// <summary>
            /// Number of removals.
            /// </summary>
            public int NumberOfExtracts { set; get; } = 0;

            /// <summary>
            /// Maximal number of elements.
            /// </summary>
            public int MaxNumberOfElements { set; get; } = 0;

            /// <summary>
            /// Currenct heap size.
            /// </summary>
            public int CurrentHeapSize { set; get; } = 0;

            /// <summary>
            /// Total sum of removed elements.
            /// </summary>
            public long ExtractedElementsAgeTotalSum { set; get; } = 0;

            /// <summary>
            /// Size of gaps.
            /// </summary>
            public Dictionary<int, int> GapsCountsBySize { set; get; } = new Dictionary<int, int>();

            /// <summary>
            /// Current time.
            /// </summary>
            public int CurrentTime = 0;

            /// <summary>
            /// Element ages.
            /// </summary>
            public List<int> ElementsAges { set; get; } = new List<int>();

            /// <summary>
            /// Used keys.
            /// </summary>
            public HashSet<int> KeysUsed { set; get; } = new HashSet<int>();

            /// <summary>
            /// Clears the stats.
            /// </summary>
            public void Clear()
            {
                HighestAddedElement = int.MinValue;
                HighestExtractedElement = int.MinValue;
                LowestAddedElement = int.MaxValue;
                MaxNumberOfElements = 0;
                NumberOfExtracts = 0;
                NumberOfInserts = 0;
                CurrentHeapSize = 0;
                ExtractedElementsAgeTotalSum = 0;
                OldestExtractedElement = 0;

                CurrentTime = 0;
                ElementsAges.Clear();
                KeysUsed.Clear();
            }

            /// <summary>
            /// Compute the gaps.
            /// </summary>
            private void ComputeGaps()
            {
                GapsCountsBySize.Clear();
                var keys = KeysUsed.ToList();
                keys.Sort();

                for (int i = 1; i < keys.Count; i++)
                {
                    int gapSize = keys[i] - keys[i - 1] - 1;
                    if (gapSize == 0)
                    {
                        continue;
                    }

                    if (!GapsCountsBySize.ContainsKey(gapSize))
                    {
                        GapsCountsBySize.Add(gapSize, 0);
                    }
                    GapsCountsBySize[gapSize]++;
                }
            }

            /// <summary>
            /// Updates the stats after element insertion.
            /// </summary>
            /// <param name="addedElement">Added element.</param>
            public void UpdateAfterInsert(HeapNode addedElement)
            {
                CurrentTime++;
                CurrentHeapSize++;
                NumberOfInserts++;
                if (HighestAddedElement < addedElement.Key)
                {
                    HighestAddedElement = addedElement.Key;
                }
                if (LowestAddedElement > addedElement.Key)
                {
                    LowestAddedElement = addedElement.Key;
                }
                if (MaxNumberOfElements < CurrentHeapSize)
                {
                    MaxNumberOfElements = CurrentHeapSize;
                }
                KeysUsed.Add(addedElement.Key);
            }

            /// <summary>
            /// Updates the stats after element removal.
            /// </summary>
            /// <param name="removedElement">Removed element.</param>
            public void UpdateAfterRemove(HeapNode removedElement)
            {
                CurrentTime++;
                CurrentHeapSize--;
                NumberOfExtracts++;
                if (HighestExtractedElement < removedElement.Key)
                {
                    HighestExtractedElement = removedElement.Key;
                }
                int elementAge = this.CurrentTime - removedElement.TimeStamp;
                if (OldestExtractedElement < elementAge)
                {
                    OldestExtractedElement = elementAge;
                }

                ElementsAges.Add(elementAge);
                ExtractedElementsAgeTotalSum += elementAge;
            }

            /// <summary>
            /// Prints the stats.
            /// </summary>
            public void PrintStats()
            {
                Console.WriteLine("\t --- Printing heap usage statistics ---");
                Console.WriteLine("\t\tCurrent heap size:\t\t" + CurrentHeapSize);
                Console.WriteLine("\t\tTotal elements added:\t\t" + NumberOfInserts);
                Console.WriteLine("\t\tTotal elements extracted:\t" + NumberOfExtracts);
                Console.WriteLine("\t\tMax heap size\t\t\t" + MaxNumberOfElements);

                Console.WriteLine("\t\tHighest added element:\t\t" + HighestAddedElement);
                Console.WriteLine("\t\tLowest added element:\t\t" + LowestAddedElement);
                Console.WriteLine("\t\tHighest extracted element:\t" + HighestExtractedElement);

                Console.WriteLine("\t\tElements range:\t\t\t" + (HighestAddedElement - LowestAddedElement));
                Console.WriteLine("\t\tTotal keys used:\t\t" + KeysUsed.Count);
                Console.WriteLine("\t\tNumber of elements / range:\t" + (MaxNumberOfElements / ((double)(HighestAddedElement - LowestAddedElement))));
                Console.WriteLine("\t\tNumber of elements / keys used:\t" + (MaxNumberOfElements / ((double)(KeysUsed.Count))));

                Console.WriteLine("\t\tGaps sizes:");
                ComputeGaps();
                foreach (var gapSize in GapsCountsBySize.Keys)
                {
                    Console.WriteLine("\t\t\tSize: " + gapSize + " Count: " + GapsCountsBySize[gapSize]);
                }

                Console.WriteLine("\t\tAdditions / extractions:\t" + (NumberOfInserts / ((double)(NumberOfExtracts))));
                Console.WriteLine("\t\tAverage element's age (extracted only):\t" + ExtractedElementsAgeTotalSum / ElementsAges.Count);
            }
        }
    }
}
