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
// ...
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

The next step is to figure out those configuable properties on run time. We need reflection technique. Reflection allows us to obtain all the infomation about a type includes its name, property infos, interfaces implemented etc. First, we want to extract all the properties of a MultiConfig object, then we can filter out those non-configurable ones.

```CSharp
var props = typeof(MultiConfig).GetProperties ().Where (p => {
	var attrs = (ConfigAttribute[])p.GetCustomAttributes (typeof(ConfigAttribute), true);
	return attrs != null && attrs.Length > 0;
}).ToList ();
```

Finally, we can define a priave method in MultiConfig class to load the config for a given property.

```CSharp
private void LoadConfig (PropertyInfo propInfo)
{
	var attr = (ConfigAttribute[])propInfo.GetCustomAttributes (typeof(ConfigAttribute), true);
	if (attr == null || attr.Length == 0) return;

	var configFilename = attr [0].Filename;
	var ppt = propInfo.PropertyType;
	
	var values = FileUtil.LoadFile (this.Path + configFilename);
	
	// a list
	if (ppt.GetInterfaces ().Any (x =>
		x.IsGenericType &&
	    x.GetGenericTypeDefinition () == typeof(IList<>))) {

		var valueType = ppt.GetGenericArguments () [0];

		var listInstance = (IList)typeof(List<>)
			.MakeGenericType (valueType)
			.GetConstructor (Type.EmptyTypes)
			.Invoke (null);

		values.ForEach (v => {
			var vv = Convert.ChangeType (v, valueType);
			listInstance.Add (vv);
		});

		propInfo.SetValue (this, listInstance, null);
	} else {
		// single value
		var valueInPropType = Convert.ChangeType (values [0], ppt);
		propInfo.SetValue (this, valueInPropType, null);
	}

}
```
