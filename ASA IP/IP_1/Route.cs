using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_1
{
	public class Route
	{
		public string StartCity { get; set; }
		public string EndCity { get; set; }
		public int Time { get; set; }
		public double Price { get; set; }

		public Route(string startCity, string endCity, int time, double price)
		{
			StartCity = startCity;
			EndCity = endCity;
			Time = time;
			Price = price;
		}
	}
}
