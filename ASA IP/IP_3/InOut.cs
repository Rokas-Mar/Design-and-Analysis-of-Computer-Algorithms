using OfficeOpenXml;
using SkiaSharp;

namespace IP_3
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

		public List<City> ReadCities()
		{
			List<City> cities = new List<City>();
			for (int row = 6; row <= 104; row++) ///104 ////109
			{
				string name = _worksheet.Cells[row, 8].Value?.ToString();
				double fitness = Convert.ToDouble(_worksheet.Cells[row, 9].Value?.ToString());
				double x = Convert.ToDouble(_worksheet.Cells[row, 10].Value?.ToString());
				double y = Convert.ToDouble(_worksheet.Cells[row, 11].Value?.ToString());
				cities.Add(new City(name, fitness, x, y));
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
				if (cities.Contains(cityFrom) && cities.Contains(cityTo))
				{
					routes.Add(new Route(cityFrom, cityTo, time, price));
				}
			}
			_package.Dispose();
			return routes;
		}

		public static void PaintCanvas(SKBitmap bitmap, List<SKPoint> points, TestRoute bestRoute, List<string> cities)
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
				for (int i = 0; i < bestRoute.Cities.Count - 1; i++)
				{
					canvas.DrawLine(points[cities.IndexOf(bestRoute.Cities[i])],
						points[cities.IndexOf(bestRoute.Cities[i + 1])], paint2);
				}
			}

			using (var stream = new SKFileWStream("image.png"))
			{
				bitmap.Encode(stream, SKEncodedImageFormat.Png, 10000);
			}
		}
	}
}