using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MultiConfig
{
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

	public class FileUtil
	{
		public static List<string> LoadFile (string filename)
		{
			var values = new List<string> ();


			using (var sr = new StreamReader (filename)) {
				while (!sr.EndOfStream) {
					var line = sr.ReadLine ();
					if (string.IsNullOrWhiteSpace (line))
						continue;
					values.Add (line);
				}
			}

			return values;
		}
	}

	public abstract class MultiConfigBase
	{
		public string Path { get; private set; }

		public MultiConfigBase ()
		{
		}

		/// <summary>
		/// Load the configs under the given path
		/// </summary>
		/// <param name="path">Path.</param>
		public void LoadConfigs (string path)
		{

			this.Path = path;

			Console.WriteLine ("start loading configs from {0}", path);

			// get all properties
			var props = this.GetType ().GetProperties ().ToList ();
			
			props.ForEach (this.LoadConfig);
		}

		/// <summary>
		/// Loads the config for a given property.
		/// </summary>
		/// <param name="propInfo">Property info.</param>
		private void LoadConfig (PropertyInfo propInfo)
		{
			var attr = (ConfigAttribute[])propInfo.GetCustomAttributes (typeof(ConfigAttribute), true);
			if (attr == null || attr.Length == 0)
				return;

			var configFilename = attr [0].Filename;
			var ppt = propInfo.PropertyType;


			Console.WriteLine ("propName = {0} type = {1} config filename = {2}", propInfo.Name, ppt, configFilename);

			var values = FileUtil.LoadFile (this.Path + configFilename);

			// is a container
			if (ppt.GetInterfaces ().Any (x =>
				x.IsGenericType &&
			    x.GetGenericTypeDefinition () == typeof(IList<>))) {

				var valuetype = ppt.GetGenericArguments () [0];

				var listInstance = (IList)typeof(List<>)
					.MakeGenericType (valuetype)
					.GetConstructor (Type.EmptyTypes)
					.Invoke (null);

				values.ForEach (v => {
					var vv = Convert.ChangeType (v, valuetype);
					listInstance.Add (vv);
				});

				propInfo.SetValue (this, listInstance, null);
			} else {
				// single value
				var valueInPropType = Convert.ChangeType (values [0], ppt);

				propInfo.SetValue (this, valueInPropType, null);
			}

		}
	}

	public class GreenDataCenterConfig : MultiConfigBase
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

		public GreenDataCenterConfig ()
		{

		}
	}
}

