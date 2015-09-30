using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyChanger
{
	public class Key
	{
		public string Name { get; set; }
		public KeyTypes Type { get; set; }
		public TShockAPI.DB.Region Region { get; set; }
		public bool Enabled { get; set; }
		public List<Terraria.Item> Items { get; set; }

		public Key(string name, KeyTypes type, bool enabled)
		{
			this.Name = name;
			this.Type = type;
			this.Enabled = enabled;

			this.Region = null;
			this.Items = new List<Terraria.Item>();
		}

		public static Key Golden = new Key("golden", KeyTypes.Golden, Config.contents.EnableGoldenKey);
	}
}
