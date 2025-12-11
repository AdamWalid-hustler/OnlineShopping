using System.Diagnostics;

namespace OnlineShopping.Utilities
{
    public class PerformanceMeasurement
    {
        // Measure how long an operation takes
        public static void MeasureOperation(string operationName, Action operation)
        {
            var stopwatch = Stopwatch.StartNew();
            
            operation();
            
            stopwatch.Stop();
            Console.WriteLine($"[PERFORMANCE] {operationName}: {stopwatch.ElapsedMilliseconds}ms");
        }

        // Compare two operations
        public static void CompareOperations(string operation1Name, Action operation1, 
                                            string operation2Name, Action operation2)
        {
            Console.WriteLine("\n=== Performance Comparison ===");
            
            var stopwatch1 = Stopwatch.StartNew();
            operation1();
            stopwatch1.Stop();
            
            var stopwatch2 = Stopwatch.StartNew();
            operation2();
            stopwatch2.Stop();
            
            Console.WriteLine($"[BEFORE] {operation1Name}: {stopwatch1.ElapsedMilliseconds}ms");
            Console.WriteLine($"[AFTER]  {operation2Name}: {stopwatch2.ElapsedMilliseconds}ms");
            
            var improvement = stopwatch1.ElapsedMilliseconds - stopwatch2.ElapsedMilliseconds;
            var percentImprovement = stopwatch1.ElapsedMilliseconds > 0 
                ? (improvement * 100.0 / stopwatch1.ElapsedMilliseconds) 
                : 0;
            
            Console.WriteLine($"[RESULT] Improvement: {improvement}ms ({percentImprovement:F1}% faster)");
            Console.WriteLine("================================\n");
        }
    }
}
