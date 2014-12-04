## MultiConfig

###Used Techniques:
* Custom Attributes
* Reflection
* Generic Types

### MultiConfig Prototype

Suppose we have a MultiConfig class whose prototype looks like below. Unlike triditional config class, each property of MultiConfig class has its own config file in raw format (single column vector). The task is to load the config files for all the properties.

```CSharp
public class MultiConfig
{
	public int TimeSlots { get; private set; }
	public int ServerCount { get; private set; }
	public List<string> ServerNames { get; private set; }
	public List<double> BrownPrices { get; private set; }
	public List<int> GreenEnegies { get; private set; }
}
```

----

### Previous Method

In previous method, we have to set the values for each property like below and have to write different Util functions in order to return different types.

```CSharp
var mc = new MultiConfig();
mc.TimeSlots = FileUtil.LoadInt("timeslots.txt");
mc.ServerNames = FileUtil.LoadStringList("servernames.txt");
mc.BrownPrices = FileUtil.LoadDoubleList("brown_enegy_price_list.txt");
...
```

### Now

Now, this task can be done in only two lines, no matter how many properties that class has.
Don't believe me? check out [Program.cs](Program.cs) yourself.

```CSharp
var mc = new MultiConfig();
mc.LoadConfigs("../../data/");
```
### Implementation

Let us now talk about how can we achieve this. First we need to create a **Custom Attribute** class to tell which proporty has the config file and the filename of that config file. Here we create a *ConfigAttribute* class which has only property: *Filename*, the filename of the config file.

```CSharp
// only apply on property
[AttributeUsage (AttributeTargets.Property)]
public class ConfigAttribute : Attribute
{
	/// <summary>
	/// Config filename
	/// </summary>
	/// <value>The filename.</value>
	public string Filename { get; set; }

	public ConfigAttribute (string filename)
	{
		this.Filename = filename;
	}
}
```
Now, we can decorate the properties with their config filenames. Note, some property such as *Path* doesn't have the *Config* attribute which means it is not configurable.

```CSharp
public class MultiConfig
{
	[Config ("timeslots.txt")]
	public int TimeSlots { get; private set; }

	[Config ("servercount.txt")]
	public int ServerCount { get; private set; }

	[Config ("servernames.txt")]
	public List<string> ServerNames { get; private set; }

	[Config ("brown_enegy_price_list.txt")]
	public List<double> BrownPrices { get; private set; }

	[Config ("green-enegy.txt")]
	public List<int> GreenEnegies { get; private set; }
	
	public string Path { get; private set; }

	// ...
}
```
