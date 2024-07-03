using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using SkiaSharp;

namespace IP_1
{
	public class InOut
	{
		private ExcelPackage _package;
		private ExcelWorksheet _worksheet;

		public InOut()
		{
			ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
			_package = new ExcelPackage(new System.IO.FileInfo(@"./places_data.xlsx"));
			_worksheet = _package.Workbook.Worksheets[0];
		}

		public List<City> ReadCities(int maxrow)
		{
			List<City> cities = new List<City>();
			for (int row = 6; row <= maxrow; row++) ///104
			{
				string name = _worksheet.Cells[row, 8].Value?.ToString();
				double score = Convert.ToDouble(_worksheet.Cells[row, 9].Value?.ToString());
				double x = Convert.ToDouble(_worksheet.Cells[row, 10].Value?.ToString());
				double y = Convert.ToDouble(_worksheet.Cells[row, 11].Value?.ToString());
				cities.Add(new City(name, score, x, y));
			}
			return cities;
		}

		public List<Route> ReadRoutes(List<string> cities)
		{
			List<Route> routes = new List<Route>();
			for (int row = 6; row <= 2275; row++) ///2275		///2282
			{
				string cityFrom = _worksheet.Cells[row, 13].Value?.ToString();
				string cityTo = _worksheet.Cells[row, 14].Value?.ToString();
				int time = Convert.ToInt32(_worksheet.Cells[row, 15].Value?.ToString());
				double price = Convert.ToDouble(_worksheet.Cells[row, 16].Value?.ToString());
				if(cities.Contains(cityFrom) && cities.Contains(cityTo))
				{
					routes.Add(new Route(cityFrom, cityTo, time, price));
				}
			}
			_package.Dispose();
			return routes;
		}

		public static void PaintCanvas(SKBitmap bitmap, List<SKPoint> points, List<string> cities, List<string> bestRoute)
		{
			using (var canvas = new SKCanvas(bitmap))
			{
				canvas.Clear(SKColors.White);
				using var paint = new SKPaint
				{
					Color = SKColors.Red,
					IsAntialias = true,
					StrokeWidth = 6,
					StrokeCap = SKStrokeCap.Round,
					Style = SKPaintStyle.Stroke
				};

				foreach (var point in points)
				{
					canvas.DrawCircle(point.X, point.Y, 5, paint);
				}
				using var path = new SKPath();

				path.MoveTo(points[0].X, points[0].Y);
				for (int i = 1; i < points.Count; i++)
				{
					path.LineTo(points[i].X, points[i].Y);
				}

				using var paint2 = new SKPaint
				{
					Color = SKColors.Blue,
					IsAntialias = true,
					StrokeWidth = 3,
					StrokeCap = SKStrokeCap.Round,
					Style = SKPaintStyle.Stroke
				};
				for (int i = 0; i < bestRoute.Count - 1; i++)
				{
					canvas.DrawLine(points[cities.IndexOf(bestRoute[i])], points[cities.IndexOf(bestRoute[i + 1])], paint2);
				}
			}

			using (var stream = new SKFileWStream("image.png"))
			{
				bitmap.Encode(stream, SKEncodedImageFormat.Png, 10000);
			}
		}

		public static void PrintResults(string file, int printRow, double bestScore, Stopwatch stopwatch)
		{
			using (var package = new ExcelPackage(new FileInfo(file)))
			{
				// Get the existing worksheet or create a new one
				ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
				if (worksheet == null)
					worksheet = package.Workbook.Worksheets.Add("Results");

				worksheet.Cells[printRow, 1].Value = bestScore;
				worksheet.Cells[printRow, 3].Value = stopwatch.Elapsed.TotalSeconds;

				package.Save();
			}
		}
	}
}
