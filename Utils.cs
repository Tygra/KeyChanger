using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using TShockAPI.DB;
using Terraria;

namespace KeyChanger
{
	class Utils
	{
		#region InitKeys
		/// <summary>
		/// Loads all keys.
		/// </summary>
		public static void InitKeys()
		{
			Key.Golden = LoadKey(KeyTypes.Golden, TShock.Regions.GetRegionByName(Config.contents.GoldenRegion));
		}
		#endregion

		#region LoadKey
		/// <summary>
		/// Loads a key from KeyChangerConfig.json.
		/// </summary>
		/// <param name="type">The type of key to load.</param>
		/// <returns>The key with all the required data.</returns>
		public static Key LoadKey(KeyTypes type, Region region = null)
		{
			Key key;
			switch (type)
			{
				case KeyTypes.Golden:
					key = new Key("golden", KeyTypes.Golden, Config.contents.EnableGoldenKey);
					key.Items = GetItems(Config.contents.GoldenKeyItem);
					key.Region = TShock.Regions.GetRegionByName(Config.contents.GoldenRegion);
					break;
				default:
					return null;
			}
			return key;
		}
		#endregion

		#region GetItems
		/// <summary>
		/// Returns a list of Terraria.Item from a list of Item ids.
		/// </summary>
		/// <param name="id">The int[] containing the Item ids.</param>
		/// <returns>List[Item]</returns>
		public static List<Item> GetItems(int[] id)
		{
			List<Item> list = new List<Item>();
			foreach (int item in id)
			{
				list.Add(TShock.Utils.GetItemById(item));
			}
			return list;
		}
		#endregion

		#region ErrorMessageHandler
		/// <summary>
		/// Handles error messages thrown by erroneous / lack of parameters by checking a player's group permissions.
		/// </summary>
		/// <param name="ply">The player executing the command.</param>
		/// <returns>A string matching the error message.</returns>
		public static string ErrorMessage(TSPlayer ply)
		{
			string error;
			var list = new List<string>()
			{
				ply.Group.HasPermission("geldar.level5") ? "change" : null,
				ply.Group.HasPermission("geldar.admin") ? "reload" : null,
				ply.Group.HasPermission("geldar.admin") ? "mode" : null,
				"list"
			};

			string valid = string.Join("/", list.FindAll(i => i != null));
			error = string.Format("Invalid syntax! Proper syntax: {0}key <{1}> [type]", Commands.Specifier, valid);
			return error;
		}
		#endregion
	}
}
