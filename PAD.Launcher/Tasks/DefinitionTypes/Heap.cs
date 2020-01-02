
namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Heap used in the planning task.
    /// </summary>
    public enum Heap
    {
        BinomialHeap,
        FibonacciHeap,
        FibonacciHeap2,
        LeftistHeap,
        RedBlackTreeHeap,
        RegularBinaryHeap,
        RegularTernaryHeap
    }

    /// <summary>
    /// Auxilliary class for converting heap parameters.
    /// </summary>
    public static class HeapConverter
    {
        /// <summary>
        /// Converts the parameter name to the type.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>Corresponding type.</returns>
        public static Heap Convert(string paramName)
        {
            switch (paramName.ToUpper())
            {
                case "BINOMIALHEAP":
                    return Heap.BinomialHeap;
                case "FIBONACCIHEAP":
                    return Heap.FibonacciHeap;
                case "FIBONACCIHEAP2":
                    return Heap.FibonacciHeap2;
                case "LEFTISTHEAP":
                    return Heap.LeftistHeap;
                case "REDBLACKTREEHEAP":
                    return Heap.RedBlackTreeHeap;
                case "REGULARBINARYHEAP":
                    return Heap.RegularBinaryHeap;
                case "REGULARTERNARYHEAP":
                    return Heap.RegularTernaryHeap;
                default:
                    throw new TasksLoaderException($"Unknown heap name '{paramName}'!");
            }
        }
    }
}
