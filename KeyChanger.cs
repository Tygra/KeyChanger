using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace KeyChanger
{
	[ApiVersion(1, 21)]
	public class KeyChanger : TerrariaPlugin
	{
		#region Plugin Info
		public override Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}
		public override string Name
		{
			get { return "KeyChanger"; }
		}


		public override string Author
		{
			get { return "Enerdy"; }
		}


		public override string Description
		{
			get { return "SBPlanet KeyChanger System"; }
		}


		public KeyChanger(Main game)
			: base(game)
		{
		}
		#endregion

		#region Initialize & Dispose
		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(new List<string>() { "key.change", "key.reload", "key.mode" }, KeyChange, "key")
			{
				HelpDesc = new[]
				{
					"{0}key - Shows plugin info".SFormat(Commands.Specifier),
					"{0}key change <type> - Exchanges golden keys".SFormat(Commands.Specifier),
					"{0}key list - Shows a list of available items".SFormat(Commands.Specifier),
					"If an exchange fails, make sure your inventory has free slots"
				}
			});
			if (!Config.ReadConfig())
			{
				TShock.Log.ConsoleError("Failed to read KeyChangerConfig.json. Consider deleting the file so that it may be recreated.");
			}
			Utils.InitKeys();
		}

		protected override void Dispose(bool disposing)
		{
			
		}
		#endregion

		#region KeyChange
		private void KeyChange(CommandArgs args)
		{
			TSPlayer ply = args.Player;

			if (!Main.ServerSideCharacter)
			{
				ply.SendWarningMessage("[Warning] This plugin will not work properly with ServerSideCharacters disabled.");
			}

			if (args.Parameters.Count < 1)
			{
				var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				ply.SendMessage(string.Format("KeyChanger (v{0}) by Enerdy", version), Color.SkyBlue);
				ply.SendMessage("Syntax: {0}key <list/change> [type]".SFormat(Commands.Specifier), Color.SkyBlue);
				ply.SendMessage("Type {0}help key for more info".SFormat(Commands.Specifier), Color.SkyBlue);
			}
			else if (args.Parameters[0].ToLower() == "change" && args.Parameters.Count == 1)
			{
				ply.SendErrorMessage("Invalid syntax! Proper syntax: {0}key change <type>", Commands.Specifier);
			}
			else if (args.Parameters.Count > 0)
			{
				string cmd = args.Parameters[0].ToLower();
				switch (cmd)
				{
					case "change":
						if (!ply.Group.HasPermission("key.change"))
						{
							ply.SendErrorMessage("You do not have access to this command.");
							break;
						}

						if (ply == TSPlayer.Server)
						{
							ply.SendErrorMessage("You must use this command in-game.");
							return;
						}

						Key key;
						string str = args.Parameters[1].ToLower();

						if (str == Key.Golden.Name)
							key = Key.Golden;						
						else
						{
							ply.SendErrorMessage("Invalid key type! Available types: " + string.Join(", ",
								Key.Golden.Enabled ? Key.Golden.Name : null));
							return;
						}

						if (!key.Enabled)
						{
							ply.SendInfoMessage("The selected key is disabled.");
							return;
						}

						var lookup = ply.TPlayer.inventory.FirstOrDefault(i => i.netID == (int)key.Type);
						if (lookup == null)
						{
							ply.SendErrorMessage("Make sure you carry the selected key in your inventory.");
							return;
						}

						if (Config.contents.EnableRegionExchanges)
						{
							Region region;
							if (Config.contents.MarketMode)
								region = TShock.Regions.GetRegionByName(Config.contents.MarketRegion);
							else
								region = key.Region;

							if (region == null)
							{
								ply.SendInfoMessage("No valid region was set for this key.");
								return;
							}

							if (args.Player.CurrentRegion != region)
							{
								ply.SendErrorMessage("You can only exchange your Golden Keys at Spawn!");
								return;
							}
						}

						Item item;
						for (int i = 0; i < 50; i++)
						{
							item = ply.TPlayer.inventory[i];

							if (item.netID == (int)key.Type)
							{
								if (item.stack == 1 || ply.InventorySlotAvailable)
								{
									ply.TPlayer.inventory[i].stack--;
									NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, string.Empty, ply.Index, i);
									Random rand = new Random();
									Item give = key.Items[rand.Next(0, key.Items.Count)];
									ply.GiveItem(give.netID, give.name, give.width, give.height, 1);
									Item take = TShock.Utils.GetItemById((int)key.Type);
									ply.SendSuccessMessage("Exchanged a {0} for 1 {1}!", take.name, give.name);
									return;
								}

								ply.SendErrorMessage("Make sure you have at least one available inventory slot.");
								return;
							}
						}
						break;

					case "reload":
						{
							if (!ply.Group.HasPermission("key.reload"))
							{
								ply.SendErrorMessage("You do not have access to this command.");
								break;
							}

							if (Config.ReadConfig())
							{
								Utils.InitKeys();
								ply.SendMessage("KeyChangerConfig.json reloaded successfully.", Color.Green);
								break;
							}
							else
							{
								ply.SendErrorMessage("Failed to read KeyChangerConfig.json. Consider deleting the file so that it may be recreated.");
								break;
							}
						}

					case "list":
						{
							ply.SendMessage("Golden Key - " + string.Join(", ", Key.Golden.Items.Select(i => i.name)), Color.Goldenrod);
							break;
						}

					case "mode":
						{
							if (!ply.Group.HasPermission("key.mode"))
							{
								ply.SendErrorMessage("You do not have access to this command.");
								break;
							}

							if (args.Parameters.Count < 2)
							{
								ply.SendErrorMessage("Invalid syntax! Proper syntax: {0}key mode <normal/region/market>", Commands.Specifier);
								break;
							}

							string query = args.Parameters[1].ToLower();

							if (query == "normal")
							{
								Config.contents.EnableRegionExchanges = false;
								ply.SendSuccessMessage("Exchange mode set to normal (exchange everywhere).");
							}
							else if (query == "region")
							{
								Config.contents.EnableRegionExchanges = true;
								Config.contents.MarketMode = false;
								ply.SendSuccessMessage("Exchange mode set to region (a region for each type).");
							}
							else if (query == "market")
							{
								Config.contents.EnableRegionExchanges = true;
								Config.contents.MarketMode = true;
								ply.SendSuccessMessage("Exchange mode set to market (one region for every type).");
							}
							else
							{
								ply.SendErrorMessage("Invalid syntax! Proper syntax: {0}key mode <normal/region/market>", Commands.Specifier);
								return;
							}
							Config.UpdateConfig();
							break;
						}
					default:
						{
							ply.SendErrorMessage(Utils.ErrorMessage(ply));
							break;
						}
				}
			}
			else
			{
				ply.SendErrorMessage(Utils.ErrorMessage(ply));
			}
		}
		#endregion
	}
}
