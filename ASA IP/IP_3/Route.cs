namespace IP_3
{
	public class Route
	{
		public string CityFrom { get; set; }
		public string CityTo { get; set; }
		public int Time { get; set; }
		public double Price { get; set; }

		public Route(string cityFrom, string cityTo, int time, double price)
		{
			CityFrom = cityFrom;
			CityTo = cityTo;
			Time = time;
			Price = price;
		}
	}
}
