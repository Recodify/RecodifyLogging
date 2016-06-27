using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public class Timer
	{
		private Stopwatch sw;
		private Action action;

		public long ElapsedMs
		{
			get
			{
				return sw.ElapsedMilliseconds;
			}
		}

		public Timer(Action action)
		{
			this.action = action;
			sw = new Stopwatch();
		}

		public void Perform()
		{
			sw.Start();
			action();
			sw.Stop();
		}
	}
}
