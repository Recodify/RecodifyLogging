using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public class ViewModel
	{
		public int Id { get; set; }

		public int Age { get; set; }

		public string Firstname { get; set; }

		public string LastName { get; set; }

		public string Status { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public string Postcode { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime UpdateDate { get; set; }

		public decimal Quantity { get; set; }
	}

	public class DataFactory
	{
		public IEnumerable<ViewModel> CreateViewModelCollection(int count)
		{
			return Enumerable.Range(0, count).Select(x => CreateViewModel()).ToList();
		}

		public ViewModel CreateViewModel()
		{
			return new ViewModel
			{
				Id = 1,
				Firstname = "Sam",
				LastName = "Shiles",
				Status = "Winning",
				Address1 = "Timothy's Corner",
				Address2 = "Somewhere",
				Age = 21,
				CreateDate = DateTime.Now,
				Postcode = "Bs6AL",
				Quantity = 1,
				UpdateDate = DateTime.Now
			};
		}
	}
}
