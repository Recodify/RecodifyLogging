using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public class PerformanceTest
	{
		private Action action;
		private int numberOfIterations;
		private bool warmup;

		private IEnumerable<Timer> timers;

		public long AveragePerIterationTime
		{
			get
			{
				return timers.Sum(x => x.ElapsedMs) / numberOfIterations;
			}
		}

		public long TotalTime
		{
			get
			{
				return timers.Sum(x => x.ElapsedMs);
			}
		}

		public PerformanceTest(Action action, int numberOfIterations, bool warmup)
		{
			this.action = action;
			this.numberOfIterations = numberOfIterations;
			this.warmup = warmup;	
		}

		public void Perform()
		{
			if (warmup)
			{
				action();
			}

			this.timers = Enumerable.Range(0, numberOfIterations).Select(x => new Timer(action)).ToList();

			foreach(var t in timers)
			{
				t.Perform();
			}

			OutputResultsToConsole();
		}

		private void OutputResultsToConsole()
		{
			Console.WriteLine($"Number of iterations {numberOfIterations}");
			Console.WriteLine($"Total Time {TotalTime}ms");
			Console.WriteLine($"Average time per action {AveragePerIterationTime}ms");
			Console.WriteLine($"Warm up {warmup}");
		}		
	}

}