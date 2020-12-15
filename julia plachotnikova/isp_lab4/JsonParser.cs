using System;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using Newtonsoft;

namespace ConfigParser
{
	public class JsonParcer : IParce
	{
		public T Parce<T>(string path)
		{
			T options = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

			return options;
		}
	}
}
