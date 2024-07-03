using SkiaSharp;
using System.Diagnostics;

namespace IP_3;

public class Program
{
	private static List<City> cities; // List of available cities
	private static List<Route> routes; // List of available routes
	private static Random random; // Random number generator
	private static Stopwatch stopwatch; // Stopwatch to track execution time
	private static int populationSize = 300; // Size of the population
	private static int maxGenerations = 300; // Maximum number of generations
	private static double mutationRate = 0.1; // Rate of mutation
	private const int maxTime = 48 * 3600; // Maximum travel time allowed
	private const int maxExecutionTime = 60; // Maximum execution time in seconds

	public static void Main(string[] args)
	{
		var bitmap = new SKBitmap(4434086 / 1000, 4405311 / 1000);
		InOut inOut = new InOut();
		cities = inOut.ReadCities();
		var points = new List<SKPoint>();
		List<string> citiesNames = new List<string>();
		foreach (var city in cities)
		{
			points.Add(new SKPoint((float)(city.X + 1017080) / 1000, (float)(city.Y - 4031810) / 1000));
			citiesNames.Add(city.Name);
		}

		routes = inOut.ReadRoutes(citiesNames);
		string startingCity = "Alicante";
		random = new Random();
		stopwatch = new Stopwatch();

		stopwatch.Start();
		TestRoute bestRoute = GeneticAlgorithm(startingCity);
		stopwatch.Stop();

		Console.WriteLine("Best Route:");
		foreach (var city in bestRoute.Cities)
		{
			Console.Write(city + "->");
		}

		Console.WriteLine();
		Console.WriteLine("City Score: " + bestRoute.Score);
		Console.WriteLine("Execution Time: " + stopwatch.Elapsed.TotalSeconds + " seconds");

		InOut.PaintCanvas(bitmap, points, bestRoute, citiesNames);
	}

	private static TestRoute TournamentSelection(List<TestRoute> population)
	{
		List<TestRoute> tournamentParticapants = new List<TestRoute>();
		int tournamentSize = populationSize / 10;
		for (int i = 0; i < tournamentSize; i++)
		{
			int randomIndex = random.Next(population.Count);
			tournamentParticapants.Add(population[randomIndex]);
		}
		TestRoute bestIndividual = tournamentParticapants.OrderBy(r => r.Score).ThenByDescending(r => r.TotalTravelTime).ThenByDescending(r => r.Cities.Count).Last();
		return bestIndividual;
	}

	static TestRoute GetShuffledCityNames(string startingCity)
	{
		TestRoute route = new TestRoute(new List<string>(), 0, 0);
		route.Cities.Add(startingCity);
		route.Score = GetCityByName(startingCity).Score;
		string currentCity = startingCity;
		while (true)
		{
			List<Route> availableRoutes = routes.FindAll(r => r.CityFrom == currentCity);
			List<Route> validRoutes = availableRoutes.FindAll(r => r.Time + route.TotalTravelTime < maxTime);
			if (validRoutes.Count == 0)
				break;
			Route randomRoute = validRoutes[random.Next(validRoutes.Count)];
			currentCity = randomRoute.CityTo;
			route.TotalTravelTime += randomRoute.Time;
			if (!route.Cities.Contains(currentCity))
			{
				route.Score += GetCityByName(currentCity).Score;
			}
			route.Cities.Add(currentCity);
			if (currentCity == startingCity)
				break;
		}
		if (route.Cities[0] == route.Cities[route.Cities.Count - 1])
		{
			return route;
		}
		return null;
	}

	static City GetCityByName(string name)
	{
		return cities.FirstOrDefault(c => c.Name == name);
	}

	private static TestRoute GeneticAlgorithm(string startingCity)
	{
		List<TestRoute> population = InitializePopulation(startingCity);

		TestRoute bestRoute = new TestRoute(new List<string>(), 0, 0);

		int generation = 0;
		while (generation < maxGenerations && stopwatch.Elapsed.TotalSeconds < maxExecutionTime)
		{
			TestRoute elite = GetBestIndividual(population);
			if (elite.Score > bestRoute.Score || (elite.Score == bestRoute.Score && elite.TotalTravelTime < bestRoute.TotalTravelTime) || (elite.Score == bestRoute.Score && elite.TotalTravelTime == bestRoute.TotalTravelTime && elite.Cities.Count < bestRoute.Cities.Count))
			{
				bestRoute = elite.Copy();
				Console.WriteLine("New best route found with score: {0}", bestRoute.Score);
			}
			Console.WriteLine("Generation: {0}", generation + 1);
			List<TestRoute> nextGeneration = new List<TestRoute>();
			nextGeneration.Add(elite);
			while (nextGeneration.Count < populationSize)
			{
				TestRoute parent1 = TournamentSelection(population);
				TestRoute offspring = null;
				int tries = 0;
				while (offspring == null && tries < 5)
				{
					TestRoute parent2 = TournamentSelection(population);
					if (parent1 == parent2)
					{
						offspring = null;
						tries++;
						continue;
					}
					offspring = Crossover(parent1, parent2);
					tries++;
				}
				nextGeneration.Add(offspring == null ? parent1 : offspring);
			}
			foreach (var individual in nextGeneration)
			{
				Mutate(individual);
			}
			population = nextGeneration;
			generation++;
		}
		return bestRoute;
	}

	static List<TestRoute> InitializePopulation(string startingCity)
	{
		List<TestRoute> population = new List<TestRoute>();
		while (population.Count < populationSize)
		{
			TestRoute route = GetShuffledCityNames(startingCity);
			if (route != null)
			{
				population.Add(route);
			}
		}
		return population;
	}

	private static double CalculateScore(List<string> cities2)
	{
		double totalScore = 0;
		int time = 0;
		List<string> visitedCities = new List<string>();
		string prevCity = null;
		foreach (string city in cities2)
		{
			if (!visitedCities.Contains(city))
			{
				City currentCity = GetCityByName(city);
				totalScore += currentCity.Score;
				visitedCities.Add(city);
			}
			if (prevCity != null)
			{
				var route = routes.FirstOrDefault(r => (r.CityFrom == prevCity && r.CityTo == city));
				if (route != default)
				{
					time += route.Time;
				}
				else
				{
					return 0;
				}
			}
			prevCity = city;
		}
		if (time > 48 * 3600)
		{
			return 0;
		}
		return totalScore;
	}
	private static TestRoute Crossover(TestRoute parent1, TestRoute parent2)
	{
		bool flag = false;
		List<string> childCities = new List<string>();
		int length = Math.Min(parent1.Cities.Count, parent2.Cities.Count);
		childCities.Add(parent1.Cities[0]);
		for (int i = 1; i < length - 1; i++)
		{
			if (!flag && parent1.Cities[i] != parent2.Cities[i])
			{
				flag = true;
			}

			childCities.Add(flag ? parent2.Cities[i] : parent1.Cities[i]);
		}
		for (int i = length - 1; i < parent2.Cities.Count; i++)
		{
			childCities.Add(parent2.Cities[i]);
		}


		double score = CalculateScore(childCities);
		int totalTravelTime = CalculateTotalTravelTime(childCities);
		if (flag && totalTravelTime <= maxTime)
		{
			return new TestRoute(childCities, score, totalTravelTime);
		}
		else
		{
			return null;
		}
	}
	static int CalculateTotalTravelTime(List<string> cities2)
	{
		double score = 0;
		HashSet<string> visitedCities = new HashSet<string>();
		int totalTravelTime = 0;

		for (int i = 0; i < cities2.Count - 1; i++)
		{
			string cityA = cities2[i];
			string cityB = cities2[i + 1];
			visitedCities.Add(cityA);
			var route = routes.FirstOrDefault(r => (r.CityFrom == cityA && r.CityTo == cityB) || (r.CityFrom == cityB && r.CityTo == cityA));
			if (route != default)
			{
				City cityAObject = GetCityByName(cityA);
				if (cityAObject != default)
				{
					score += cityAObject.Score;
					totalTravelTime += route.Time;
				}
				else
				{
					Console.WriteLine($"City object not found: {cityA}");
				}
			}
		}
		string lastCity = cities2.Last();
		visitedCities.Add(lastCity);
		var returnTrip = routes.FirstOrDefault(r => (r.CityFrom == lastCity && r.CityTo == cities2[0]) || (r.CityFrom == cities2[0] && r.CityTo == lastCity));
		if (returnTrip != default)
		{
			totalTravelTime += returnTrip.Time;
		}
		if (totalTravelTime > maxTime)
		{
			score = 0;
		}

		return totalTravelTime;
	}

	private static void Mutate(TestRoute route)
	{
		for (int i = 1; i < route.Cities.Count - 1; i++)
		{
			if (random.NextDouble() < mutationRate)
			{
				if (random.NextDouble() >= 0.005)
				{
					List<Route> validRoutesFrom = routes.Where(r => r.CityFrom == route.Cities[i - 1]).ToList();
					List<Route> validRoutesTo = routes.Where(r => r.CityTo == route.Cities[i + 1]).ToList();
					List<string> validCities = (from fromRoute in validRoutesFrom
												join toRoute in validRoutesTo on fromRoute.CityTo equals toRoute.CityFrom
												select fromRoute.CityTo).Distinct().ToList();
					if (validCities.Count == 0)
					{
						continue;
					}

					string city = ShuffleList(validCities).First();
					route.Cities[i] = city;
				}
				else
				{
					List<Route> validRoutesFrom = routes.Where(r => r.CityFrom == route.Cities[i]).ToList();
					List<Route> validRoutesTo = routes.Where(r => r.CityTo == route.Cities[i + 1]).ToList();
					List<string> validCities = (from fromRoute in validRoutesFrom
												join toRoute in validRoutesTo on fromRoute.CityTo equals toRoute.CityFrom
												select fromRoute.CityTo).Distinct().ToList();
					if (validCities.Count == 0)
					{
						continue;
					}

					string city = ShuffleList(validCities).First();
					route.Cities.Insert(i, city);
					continue;
				}
			}
		}
		route.Score = CalculateScore(route.Cities);
	}

	private static TestRoute GetBestIndividual(List<TestRoute> population)
	{
		TestRoute bestIndividual = population.OrderBy(r => r.Score).ThenByDescending(r => r.TotalTravelTime).ThenByDescending(r => r.Cities.Count).Last();

		return bestIndividual;
	}
	private static List<T> ShuffleList<T>(List<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = random.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}

		return list;
	}

}