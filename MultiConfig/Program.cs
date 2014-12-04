using System;
using System.Linq;

namespace MultiConfig
{
	class MainClass
	{
		public static void Main (string[] args)
		{

			var prop = typeof(MultiConfig).GetProperties ().Where (p => {
				var attrs = (ConfigAttribute[])p.GetCustomAttributes (typeof(ConfigAttribute), true);
				return attrs != null && attrs.Length > 0;
			}).ToList ();

			prop.ForEach (p => Console.WriteLine (p.PropertyType));

			var mc = new MultiConfig ();
			mc.LoadConfigs ("../../data/");

			Console.WriteLine ("TimeSlots = {0}", mc.TimeSlots);
			Console.WriteLine ("ServerCount = {0}", mc.ServerCount);
			Console.WriteLine ("ServerNumes = ");
			mc.ServerNames.ForEach (Console.WriteLine);
			Console.WriteLine ("BrownEnegyPrice = ");
			mc.BrownPrices.ForEach (Console.WriteLine);
			Console.WriteLine ("GreenEnegy = ");
			mc.GreenEnegies.ForEach (Console.WriteLine);
		}
	}
}
