namespace IP_3
{
	public class City
	{
		public string Name { get; set; }
		public double Score { get; set; }
		public double X { get; set; }
		public double Y { get; set; }

		public City(string name, double score, double x, double y)
		{
			Name = name;
			Score = score;
			X = x;
			Y = y;
		}
	}
}
