using Smod2;
using Smod2.API;
using Smod2.Commands;
using System.Linq;
using System.Collections.Generic;
using MEC;


namespace SurvivalGamemode
{
	public class Functions
	{
		private readonly Survival plugin;

		public Functions(Survival plugin) => this.plugin = plugin;

		public bool IsAllowed(ICommandSender sender)
		{
			Player player = sender as Player;

			if (player != null)
			{
				List<string> roleList = (plugin.ValidRanks != null && plugin.ValidRanks.Length > 0) ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

				if (roleList != null && roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
					return true;
				else if (roleList == null || roleList.Count == 0)
					return true;
				else
					return false;
			}
			return true;
		}

		public void Get079Rooms()
		{
			foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
				if (room.ZoneType == ZoneType.LCZ)
					plugin.BlackoutRooms.Add(room);
		}

		public IEnumerator<float> HCZBlackout()
		{
			yield return Timing.WaitForSeconds(plugin.NutDelay);
			while (plugin.RoundStarted)
			{
				Generator079.generators[0].CallRpcOvercharge();
				yield return Timing.WaitForSeconds(11f);
			}
		}

		public IEnumerator<float> LCZBlackout()
		{
			yield return Timing.WaitForSeconds(plugin.NutDelay);
			while (plugin.RoundStarted)
			{
				foreach (Room room in plugin.BlackoutRooms)
					room.FlickerLights();
				yield return Timing.WaitForSeconds(8f);
			}
		}

		public void EnableGamemode()
		{
			plugin.Enabled = true;

			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(25, "<color=#50c878>Survival of the Fittest Gamemode</color> is starting..", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			plugin.Info("Toggling Blackout off.");
		}

		public void SpawnDboi(Player player)
		{
			Vector spawn;

			if (plugin.Zone == "lcz")
				spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCIENTIST);
			else
				spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_096);

			player.ChangeRole(Role.CLASSD, false, false, false, true);

			player.Teleport(spawn);

			foreach (Smod2.API.Item item in player.GetInventory())
			{
				item.Remove();
			}

			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.CUP);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Find a hiding place and survive from the peanuts! They will spawn in 939's area when the lights go off!", false);
		}

		public void SpawnNut(Player player)
		{

			player.ChangeRole(Role.SCP_173, false, true, true, true);

			plugin.Info("Spawned " + player.Name + " as SCP-173");
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(45, "You will be teleported into the game arena when adequate time has passed for other players to hide...", false);
		}
		public Vector NutSpawn()
		{
			List<Room> rooms = new List<Room>();

			if (plugin.Zone == "lcz")
			{
				foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
				{
					if (room.ZoneType == ZoneType.LCZ && room.RoomType != RoomType.CHECKPOINT_A && room.RoomType != RoomType.CHECKPOINT_B && room.RoomType != RoomType.ENTRANCE_CHECKPOINT)
					{
						rooms.Add(room);
					}
				}
			}
			else
			{
				foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
				{
					if (room.ZoneType == ZoneType.HCZ && room.RoomType != RoomType.ENTRANCE_CHECKPOINT && room.RoomType != RoomType.CHECKPOINT_A && room.RoomType != RoomType.CHECKPOINT_B)
					{
						rooms.Add(room);
					}
				}
			}

			int randomNum = plugin.Gen.Next(rooms.Count);
			Room randomRoom = rooms[randomNum];
			Vector spawn = randomRoom.Position;

			return spawn;
		}
		public IEnumerator<float> TeleportNuts(float delay)
		{
			yield return Timing.WaitForSeconds(delay);

			plugin.Info("Timer completed!");

			foreach (Player player in plugin.Server.GetPlayers())
			{
				if (player.TeamRole.Role == Role.SCP_173)
				{
					Vector spawn = NutSpawn();
					player.Teleport(new Vector(spawn.x, spawn.y + 2, spawn.z));
					player.SetHealth(plugin.NutHealth);
					player.PersonalBroadcast(15, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the Class-D before the auto-nuke goes off!", false);
				}
			}
		}
	}
}