using System;
using System.Linq;

namespace MultiConfig
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var gdcc = new GreenDataCenterConfig ();
			gdcc.LoadConfigs ("../../data/");

			Console.WriteLine ("TimeSlots = {0}", gdcc.TimeSlots);
			Console.WriteLine ("ServerCount = {0}", gdcc.ServerCount);
			Console.WriteLine ("ServerNumes = ");
			gdcc.ServerNames.ForEach (Console.WriteLine);
			Console.WriteLine ("BrownEnegyPrice = ");
			gdcc.BrownPrices.ForEach (Console.WriteLine);
			Console.WriteLine ("GreenEnegy = ");
			gdcc.GreenEnegies.ForEach (Console.WriteLine);
		}
	}
}
