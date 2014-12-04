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


 
