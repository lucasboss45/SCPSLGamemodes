using Smod2;
using Smod2.API;
using Smod2.Commands;
using System.Linq;
using System.Collections.Generic;
using MEC;

namespace MuskateersGamemode
{
	public class Functions
	{
		private readonly Muskateers plugin;

		public Functions(Muskateers plugin) => this.plugin = plugin;

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
		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(25, "<color=#308ADA> Three Muskateers</color> gamemode starting..", false);
			}
		}
		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}
		public IEnumerator<float> SpawnNTF(Player player)
		{
			player.ChangeRole(Role.NTF_COMMANDER, true, true, false, true);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are a <color=#308ADA>Muskateer</color>. Enter the facility and eliminate all Class-D.", false);

			yield return Timing.WaitForSeconds(10);

			player.SetHealth(plugin.NTFHealth);
		}
		public IEnumerator<float> SpawnClassD(Player player)
		{
			player.ChangeRole(Role.CLASSD, true, true, false, true);
			yield return Timing.WaitForSeconds(2);

			player.SetHealth(plugin.ClassDHealth);

			player.GiveItem(ItemType.USP);
			player.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are a <color=#DAA130>Class-D personnel</color>. Escape the facility before the auto-nuke, but evade the NTF sent to kill you!", false);
		}
		public void EndGamemodeRound()
		{
			plugin.Info("The Gamemode round has ended!");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}
	}
}