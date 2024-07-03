namespace IP_3;

public class TestRoute
{
	public List<string> Cities { get; set; }
	public double Score { get; set; }

	public int TotalTravelTime { get; set; }
	public TestRoute(List<string> cities, double score, int totalTravelTime)
	{
		Cities = cities;
		Score = score;
		TotalTravelTime = totalTravelTime;
	}

	public TestRoute Copy()
	{
		return new TestRoute(this.Cities, this.Score, this.TotalTravelTime);
	}
}