using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TShockAPI;
using Newtonsoft.Json;

namespace KeyChanger
{
	public class Config
	{
		private static string savepath = TShock.SavePath;
		public static Contents contents;

		#region Contents
		public class Contents
		{
			public bool EnableRegionExchanges = true;
			public bool MarketMode = false;
			public bool EnableGoldenKey = true;			

			public int[] GoldenKeyItem = new int[] { 43, 48, 70, 73, 73, 73, 73, 73, 73, 73, 73, 73, 73, 73, 73, 136, 148, 149, 149, 149, 165, 544, 556, 557, 560, 625, 626, 627, 789, 790, 791, 914, 1007, 1007, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1050, 1133, 1138, 1293, 1331, 1372, 1373, 1374, 1375, 1376, 1377, 1384, 1385, 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 1412, 1413, 1415, 1416, 1418, 1419, 1420, 1421, 1422, 1423, 1424, 1425, 1426, 1433, 1434, 1435, 1436, 1437, 1438, 1439, 1441, 1451, 1452, 1453, 1454, 1455, 1456, 1470, 1471, 1472, 1500, 1502, 1509, 1510, 1511, 1512, 1573, 1992, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2004, 2006, 2007, 2156, 2157, 2195, 2230, 2249, 2250, 2376, 2377, 2378, 2386, 2387, 2388, 2402, 2403, 2404, 2436, 2437, 2438, 2526, 2544, 2559, 2574, 2612, 2613, 2614, 2615, 2616, 2617, 2618, 2619, 2620, 2645, 2646, 2647, 2652, 2653, 2654, 2658, 2659, 2660, 2664, 2665, 2666, 2673, 2674, 2675, 2676, 2740, 2741, 2748, 2814, 2837, 2838, 2839, 2891, 2895, 2995, 2999, 3000, 3125, 3180, 3181, 3191, 3192, 3193, 3194 };

			public string MarketRegion = "spawn";
			public string GoldenRegion = "spawn";
		}
		#endregion

		#region Create Config
		public static void CreateConfig()
		{
			string filepath = Path.Combine(savepath, "KeyChangerConfig.json");
			try
			{
				using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
				{
					using (var sr = new StreamWriter(stream))
					{
						contents = new Contents();
						var configString = JsonConvert.SerializeObject(contents, Formatting.Indented);
						sr.Write(configString);
					}
					stream.Close();
				}
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.Message);
				contents = new Contents();
			}
		}
		#endregion

		#region Read Config
		public static bool ReadConfig()
		{
			string filepath = Path.Combine(savepath, "KeyChangerConfig.json");
			try
			{
				if (File.Exists(filepath))
				{
					using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						using (var sr = new StreamReader(stream))
						{
							var configString = sr.ReadToEnd();
							contents = JsonConvert.DeserializeObject<Contents>(configString);
						}
						stream.Close();
					}
					return true;
				}
				else
				{
					CreateConfig();
					TShock.Log.ConsoleInfo("Created KeyChangerConfig.json.");
					return true;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.Message);
			}
			return false;
		}
		#endregion

		#region Update Config
		public static bool UpdateConfig()
		{
			string filepath = Path.Combine(savepath, "KeyChangerConfig.json");
			try
			{
				if (!File.Exists(filepath))
					return false;

				string query = JsonConvert.SerializeObject(contents, Formatting.Indented);
				using (var stream = new StreamWriter(filepath, false))
				{
					stream.Write(query);
				}
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.Message);
				return false;
			}
		}
		#endregion
	}
}
