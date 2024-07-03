using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SkiaSharp;
using OfficeOpenXml;
using Microsoft.Office.Interop.Excel;

namespace IP_1;
class Program
{
	private static List<City> cities; // List of available cities
	private static List<Route> routes; // List of available routes
	private static List<string> visitedCities; // List of visited cities in the current route
	private static double bestScore; // Best score found so far
	private static Stopwatch stopwatch; // Stopwatch to track execution time
	private static List<string> bestRoute; // Best route found so far
	private const int maxTime = 172800; // Maximum travel time allowed

	private static int bestTimep;

	static void Main(string[] args)
	{
		int printRow = 2;
		//for (int row = 25; row <= 25; row++)
		var bitmap = new SKBitmap(4434086 / 1000, 4405311 / 1000);
		InOut inOut = new InOut();
		cities = inOut.ReadCities(104);
		var points = new List<SKPoint>();
		List<string> citiesNames = new List<string>();

		foreach (var city in cities)
		{
			points.Add(new SKPoint((float)(city.X + 1017080) / 1000, (float)(city.Y - 4031810) / 1000));
			citiesNames.Add(city.Name);
		}

		routes = inOut.ReadRoutes(citiesNames);
		string startingCity = "Alicante";
		// Start the recursive method to find the best route
		visitedCities = new List<string>();
		visitedCities.Add(startingCity);
		bestScore = 0;
		bestRoute = new List<string>();
		bestTimep = 0;

		stopwatch = new Stopwatch();

		// Start the algorithm and track the execution time
		stopwatch.Start();
		FindBestRouteLocal(startingCity, 0, GetCityScore(startingCity));
		//FindBestRouteBranchAndCut(startingCity, 0, GetCityScore(startingCity));
		stopwatch.Stop();

		// Print the best route and its score score
		Console.WriteLine("Best Route:");
		foreach (var city in bestRoute)
		{
			Console.Write(city + "->");
		}

		Console.WriteLine();
		Console.WriteLine("Total city Score: " + bestScore);
		Console.WriteLine("Execution Time: " + stopwatch.Elapsed.TotalSeconds + " seconds");

		InOut.PaintCanvas(bitmap, points, citiesNames, bestRoute);

		// Printing to result file
		string filePath = "Results.xlsx";
		InOut.PrintResults(filePath, printRow, bestScore, stopwatch);
		printRow++;
	}
	private static void FindBestRouteBranchAndCut(string currentCity, int currentTime, double currentScore)
	{
		if (currentTime > maxTime && stopwatch.Elapsed.TotalSeconds > 10)
		{
			return;
		}

		double maxPossibleAdditionalScore = GetMaxPossibleScoreForRemainingTime(maxTime - currentTime);
		if (currentScore + maxPossibleAdditionalScore <= bestScore)
		{
			return;
		}

		if (currentCity != "" && currentCity == visitedCities[0] && currentTime != 0)
		{
			if (currentScore > bestScore)
			{
				bestScore = currentScore;
				bestRoute = new List<string>(visitedCities);
				bestTimep = currentTime;
			}
			return;
		}

		foreach (var route in routes)
		{
			if (route.StartCity == currentCity && (!visitedCities.Contains(route.EndCity) || visitedCities[0] == route.EndCity))
			{
				visitedCities.Add(route.EndCity);
				FindBestRouteBranchAndCut(route.EndCity, currentTime + route.Time, currentScore + GetCityScore(route.EndCity));
				visitedCities.RemoveAt(visitedCities.Count - 1);
			}
		}
	}

	private static double GetMaxPossibleScoreForRemainingTime(int remainingTime)
	{
		double maxScore = 0;
		foreach (var route in routes)
		{
			if (route.Time <= remainingTime)
			{
				maxScore += GetCityScore(route.EndCity);
				remainingTime -= route.Time;
			}
		}
		return maxScore;
	}


	private static void FindBestRouteLocal(string currentCity, int currentTime, double currentScore)
	{
		if (currentTime > maxTime)
		{
			return;
		}
		if (currentCity != "" && currentCity == visitedCities[0] && currentTime != 0)
		{
			if (currentScore > bestScore)
			{
				bestScore = currentScore;
				bestRoute = new List<string>(visitedCities);
				bestTimep = currentTime;
			}
			return;
		}
		string bestCityTo = "";
		int bestTime = 1;
		double bestScorel = 0;
		string backCityTo = "";
		int backTime = 1;
		foreach (var route in routes)
		{
			if (route.StartCity == currentCity && (!visitedCities.Contains(route.EndCity) || visitedCities[0] == route.EndCity))
			{
				if (GetCityScore(route.EndCity) / route.Time > bestScorel / bestTime)
				{
					bestCityTo = route.EndCity;
					bestTime = route.Time;
					bestScorel = GetCityScore(route.EndCity);
				}
				if (route.EndCity == visitedCities[0])
				{
					backCityTo = route.EndCity;
					backTime = route.Time;
				}
			}
		}
		if (bestCityTo != "")
		{
			visitedCities.Add(bestCityTo);
			FindBestRouteLocal(bestCityTo, currentTime + bestTime, currentScore + GetCityScore(bestCityTo));
			visitedCities.RemoveAt(visitedCities.Count - 1);
		}
		if (backCityTo != "")
		{
			visitedCities.Add(backCityTo);
			FindBestRouteLocal(backCityTo, currentTime + backTime, currentScore + GetCityScore(backCityTo));
			visitedCities.RemoveAt(visitedCities.Count - 1);
		}
	}

	private static double GetCityScore(string cityName)
	{
		foreach (var city in cities)
		{
			if (city.Name == cityName)
			{
				return city.Score;
			}
		}
		return 0;
	}
}