using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using System.Collections.Generic;
using MEC;

namespace PresidentialEscortGamemode
{
	[PluginDetails(
		author = "mkrzy",
		name = "Presidential Escort Gamemode",
		description = "Scientist (VIP) has to escape from SCPs with help of NTF",
		id = "presidential.gamemode",
		version = "2.0.0",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 2
	)]
	public class PresidentialEscort : Plugin
	{
		public Functions Functions { get; private set; }

		public Player VIP { get; internal set; } = null;

		public string[] ValidRanks { get; private set; }

		public bool VIPEscaped { get; internal set; } = false;
		public bool Enabled { get; internal set; } = false;
		public bool RoundStarted { get; internal set; } = false;

		public int VIPHealth { get; private set; }
		public int GuardHealth { get; private set; }

		public override void OnDisable()
		{
			Info(Details.name + " v." + Details.version + " has been disabled.");
		}

		public override void OnEnable()
		{
			Info(Details.name + " v." + Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			AddConfig(new ConfigSetting("vip_vip_health", 2500, true, "The amount of health VIP's start with."));
			AddConfig(new ConfigSetting("vip_guard_health", 200, true, "The amount of health guards have."));
			AddConfig(new ConfigSetting("vip_gamemode_ranks", new string[] { }, true, "The ranks able to use gamemode commands."));

			AddEventHandlers(new EventsHandler(this), Priority.Normal);

			AddCommands(new string[] { "presidentialescort", "presidential", "escort", "pe" }, new PresidentialEscortCommand(this));

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			VIPHealth = GetConfigInt("vip_vip_health");
			GuardHealth = GetConfigInt("vip_guard_health");
			ValidRanks = GetConfigList("vip_gamemode_ranks");
		}
	}
}