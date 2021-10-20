
public sealed class LandSpace
{
	private int _population = 0;
	public int Population
	{
		get => _population;
		set => _population = value;
	}

	private int _value = 10;
	public int Value
	{
		get => _value;
		set => _value = value;
	}

	private int _crime = 0;
	public int Crime
	{
		get => _crime;
		set => _crime = value;
	}

	private int _pollution = 0;
	public int Pollution
	{
		get => _pollution;
		set => _pollution = value;
	}

	private int _density = 1;
	public int Density
	{
		get => _density;
		set => _density = value;
	}

	public Globals.LandSpaceType Type = Globals.LandSpaceType.None;

	public LandSpace()
	{
	}

	public LandSpace(Globals.LandSpaceType _type)
	{
		Type = _type;
	}
}
