namespace ConsequencityNoodling
{
	public sealed class LandSpace
	{
		private int _population = 0;
		public int Population
		{
			get { return _population; }
			set { _population = value; }
		}

		private int _value = 10;
		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}

		private int _crime = 0;
		public int Crime
		{
			get { return _crime; }
			set { _crime = value; }
		}

		private int _pollution = 0;
		public int Pollution
		{
			get { return _pollution; }
			set { _pollution = value; }
		}

		public Globals.LandSpaceType Type = Globals.LandSpaceType.Residential;

		public LandSpace()
		{
		}

		public LandSpace(Globals.LandSpaceType _type)
		{
			Type = _type;
		}

		public void PrintLandSpace()
		{
			System.Console.Write($"V: {Value}");
		}
	}
}
