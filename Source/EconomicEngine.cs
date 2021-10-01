using System;
using System.Collections.Generic;

namespace ConsequencityNoodling
{
	public sealed class EconomicEngine
	{
		private LandSpace[,] landMap = new LandSpace[4, 4]
		{
			{new LandSpace(Globals.LandSpaceType.Agricultural), new LandSpace(Globals.LandSpaceType.Residential),   new LandSpace(Globals.LandSpaceType.Residential),   new LandSpace(Globals.LandSpaceType.Residential)},
			{new LandSpace(Globals.LandSpaceType.Agricultural), new LandSpace(Globals.LandSpaceType.Commercial),    new LandSpace(Globals.LandSpaceType.Industrial),    new LandSpace(Globals.LandSpaceType.Industrial)},
			{new LandSpace(Globals.LandSpaceType.Agricultural), new LandSpace(Globals.LandSpaceType.Agricultural),  new LandSpace(Globals.LandSpaceType.Agricultural),  new LandSpace(Globals.LandSpaceType.Agricultural)},
			{new LandSpace(Globals.LandSpaceType.Agricultural), new LandSpace(Globals.LandSpaceType.Agricultural),  new LandSpace(Globals.LandSpaceType.Agricultural),  new LandSpace(Globals.LandSpaceType.Agricultural)}
		};

		private Dictionary<Globals.LandSpaceType, float> landDemand = new Dictionary<Globals.LandSpaceType, float>();

		public EconomicEngine()
		{
			landDemand.Add(Globals.LandSpaceType.Agricultural, 1.0f);
			landDemand.Add(Globals.LandSpaceType.Commercial, 1.0f);
			landDemand.Add(Globals.LandSpaceType.Industrial, 1.0f);
			landDemand.Add(Globals.LandSpaceType.Residential, 1.1f);
		}

		public void Update(float timeStep)
		{

		}

		public void UpdateMap(float timeStep)
		{
			var g = Globals.Instance;

			// Update map values based on current demands.
			for (int height = 0; height < landMap.GetLength(1); height++)
			{
				for (int width = 0; width < landMap.GetLength(0); width++)
				{
					landMap[height, width].Value = (int)(
													   landMap[height, width].Value *
													   landDemand[landMap[height, width].Type] *
													   timeStep
												   );
				}
			}
		}

		public void UpdateDemand(float timeStep)
		{
			// Update demand values based on current map values.

		}

		public void Display()
		{
			System.Console.WriteLine("--------------------------------------------------------------------------------");
			for (int height = 0; height < landMap.GetLength(1); height++)
			{
				// Type
				for (int width = 0; width < landMap.GetLength(0); width++)
				{
					System.Console.Write($"| {landMap[height, width].Type.ToString().PadRight(12, ' ')} ");
				}
				System.Console.WriteLine("| ");

				// Population
				for (int width = 0; width < landMap.GetLength(0); width++)
				{
					System.Console.Write($"| Pop: {landMap[height, width].Population.ToString("D3").PadRight(7, ' ')} ");
				}
				System.Console.WriteLine("| ");

				// Value
				for (int width = 0; width < landMap.GetLength(0); width++)
				{
					System.Console.Write($"| Val: {landMap[height, width].Value.ToString("D3").PadRight(7, ' ')} ");
				}
				System.Console.WriteLine("| ");

				// Crime
				for (int width = 0; width < landMap.GetLength(0); width++)
				{
					System.Console.Write($"| Crm: {landMap[height, width].Crime.ToString("D3").PadRight(7, ' ')} ");
				}
				System.Console.WriteLine("| ");

				// Pollution
				for (int width = 0; width < landMap.GetLength(0); width++)
				{
					System.Console.Write($"| Pol: {landMap[height, width].Pollution.ToString("D3").PadRight(7, ' ')} ");
				}
				System.Console.WriteLine("| ");

				System.Console.WriteLine("--------------------------------------------------------------------------------");
			}
		}
	}
}
