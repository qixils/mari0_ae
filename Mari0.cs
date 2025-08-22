using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ConnectorLib;
using ConnectorLib.JSON;
using ConnectorLib.SimpleTCP;
using CrowdControl.Common;
using Newtonsoft.Json;
using ConnectorType = CrowdControl.Common.ConnectorType;
using EffectResponse = ConnectorLib.JSON.EffectResponse;
using EffectStatus = CrowdControl.Common.EffectStatus;
using Log = CrowdControl.Common.Log;
using LogLevel = CrowdControl.Common.LogLevel;
using static System.Linq.Enumerable;

namespace CrowdControl.Games.Packs.Mari0;

public class Mari0 : SimpleTCPPack<SimpleTCPServerConnector>
{
	public override string Host => "127.0.0.1";
	public override ushort Port => 28379;

	public Mari0(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

	public override Game Game => new("Mari0", "Mari0", "PC", ConnectorType.SimpleTCPServerConnector);

	public override EffectList Effects => new Effect[]
	{
		new("Invert D-Pad", "invert_wasd") { Price = 150, Duration = 15, Category = "Movement", Description = "Swaps left, right, up and down" },
		new("Swap Buttons", "invert_buttons") { Price = 150, Duration = 15, Category = "Movement", Description = "Swaps the run, jump, and portal buttons" },
		new("Randomize Outfit", "randomize_outfit") { Price = 15, Category = "Player", Description = "Randomizes Mario's color palette and accessories" },
		new("Add a Hat", "hat_give") { Price = 10, Category = "Player", Description = "Places a random silly hat on Mario's head" },
		new("Take a Hat", "hat_take") { Price = 10, Category = "Player", Description = "Takes a hat right off the top of Mario's head" },
		new("Randomize Powerup", "randomize_powerup") { Price = 25, Category = "Player", Description = "Randomize Mario's current powerup" },
		new("Remove Powerup", "remove_powerup") { Price = 25, Category = "Player", Description = "Takes Mario's current powerup" },
		new("Supersize Enemies", "supersize_enemies") { Price = 50, Category = "Enemies", Description = "Supersizes all supported on-screen enemies" },
		new("Kill Enemies", "kill_enemies") { Price = 75, Category = "Enemies", Description = "Kills all supported on-screen enemies" },
		new("Restart Level", "restart_level") { Price = 150, Category = new("Player", "Level"), Description = "Restarts the current level" },
		new("Lights Out", "enable_lightsout") { Price = 50, Duration = 10, Category = "Level", Description = "Enables the lights out effect" },
		new("Lights On", "disable_lightsout") { Price = 75, Duration = 10, Category = "Level", Description = "Disables the lights out effect" },
		new("Give Extra Life", "add_life") { Price = 50, Quantity = 100, Category = "Player", Description = "Gives Mario an extra life" },
		new("Take Extra Life", "take_life") { Price = 100, Quantity = 100, Category = "Player", Description = "Steals an extra life from Mario" },
		new("Kill Player", "kill_player") { Price = 250, Category = "Player", Description = "Murder Mario." },
		new("Can't Move", "stun_player") { Price = 150, Duration = 5, Category = "Movement", Description = "Stuns Mario, freezing him in place for a few seconds" },
		new("Auto Walk", "auto_walk") { Price = 175, Duration = 10, Category = "Movement", Description = "Force Mario to auto-walk through the level" },
		new("Auto Run", "auto_run") { Price = 200, Duration = 10, Category = "Movement", Description = "Force Mario to auto-run through the level" },
		new("Flip Gravity", "flip_gravity") { Price = 200, Duration = 10, Category = "Level", Description = "Inverts the levels gravity" },
		new("Speedup Game", "speedup_game") { Price = 150, Duration = 10, Category = "Level", Description = "Speeds up the game to x1.5" },
		new("Slowdown Game", "slowdown_game") { Price = 150, Duration = 10, Category = "Level", Description = "Speeds down the game to x0.67" },
		new("Shader Shuffle", "shader_1") { Note = "Slot 1", Price = 50, Category = "Visual", Description = "Selects a random shader in the first slot" },
		new("Shader Shuffle", "shader_2") { Note = "Slot 2", Price = 50, Category = "Visual", Description = "Selects a random shader in the second slot" },
		new("Shader Reset", "shader_clear") { Price = 40, Category = "Visual", Description = "Clears all active shaders" },
		new("Goomba Attack", "goomba_attack") { Price = 125, Duration = 20, Category = "Level", Description = "Enables the Goomba Attack cheat, spawning random goombas as the player walks through the level" },
		new("Gamemode: Minecraft", "minecraft") { Price = 100, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with the pickaxe from Minecraft, which can be used to break and place tiles" },
		new("Gamemode: Gelcannon", "gelcannon") { Price = 100, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with the Gel Cannon, which fires out various gels for bouncing and increasing speed" },
		// new("Gamemode: Cappy", "cappy") { Price = 100, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with Cappy from Super Mario Odyssey, which can be used to gain height and capture enemies" },
		new("Gamemode: Classic", "classic") { Price = 150, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with nothing! All the player can do is run and jump!" },
		new("Become 3D", "3d") { Price = 75, Duration = 30, Category = "Visual", Description = "Turns the game 3-dimensional" },
		new("Time Add", "time_add") { Price = 2, Quantity = 60, Category = "Level", Description = "Adds real-time to the in-game timer" },
		new("Time Remove", "time_sub") { Price = 5, Quantity = 60, Category = "Level", Description = "Removes real-time from the in-game timer" },
		new("Bonk", "bonk") { Price = 25, Category = "Player", Description = "Halts Mario's vertical momentum as if he hit an invisible block" },
		new("Starman", "star") { Price = 50, Category = "Player", Description = "Grants Mario a Super Star! Yahoo!" },
		new("Disable Screen Scroll", "disable_scroll") { Price = 50, Duration = 10, Category = "Level", Description = "Prevents the screen from scrolling for a short time" },
		new("Deadly Coins", "deadly_coin") { Price = 100, Duration = 15, Category = "Level", Description = "Makes collected coins kill the player for a short time" },
		new("Coin Give", "coin_add") { Price = 2, Quantity = 99, Category = "Level", Description = "Gives some coins which might contribute to a 1-Up!" },
		new("Coin Take", "coin_sub") { Price = 2, Quantity = 99, Category = "Level", Description = "Take some coins away from contributing to a 1-Up!" },
		new("Underwater", "underwater") { Price = 100, Duration = 15, Category = new("Player", "Level"), Description = "Sets the player to swim through the sky!" },
		new("Flying Fish", "flyingfish") { Price = 50, Duration = 15, Category = "Level", Description = "Spawns fish that fly from the bottom of the screen for a short time" },
		new("Meteor Shower", "meteorshower") { Price = 50, Duration = 15, Category = "Level", Description = "Spawns meteors that destroy tiles from the top of the screen for a short time" },
		new("Bullet Bill Storm", "bulletbills") { Price = 50, Duration = 15, Category = "Level", Description = "Spawns bullet bills from the edge of the screen for a short time" },
		new("Windy", "wind") { Price = 50, Duration = 15, Category = "Level", Description = "Spawns a gust of wind that pushes Mario for a short time" },
		new("Bowser Fire", "bowserfire") { Price = 50, Duration = 15, Category = "Level", Description = "Spawns Bowser's fire from the edge of the screen for a short time" },
		new("Huge Mario", "bigmario") { Price = 100, Duration = 15, Category = "Player", Description = "Turns Mario huge for a short time" },
		new("Frames Per Minute", "lowfps") { Price = 200, Duration = 15, Category = "Level", Description = "Lowers the FPS (and physics accuracy!) drastically for a short time" },

		// ENEMY SPAWNING EFFECTS
		new("Spawn Goomba", "spawn_goomba") { Price = 25, Category = "Enemies", Description = "Summon a classic goomba to stomp around!" },
		new("Spawn Koopa", "spawn_koopa") { Price = 25, Category = "Enemies", Description = "Drop a koopa troopa into the action!" },
		new("Spawn Magikoopa", "spawn_magikoopa") { Price = 75, Category = "Enemies", Description = "Conjure up a magical magikoopa!" },
		new("Spawn Thwomp", "spawn_thwomp") { Price = 50, Category = "Enemies", Description = "Crash down a thwomp from above!" },
		new("Spawn Hammer Bro", "spawn_hammerbro") { Price = 100, Category = "Enemies", Description = "Unleash a hammer-throwing bro!" },
		new("Spawn Boo", "spawn_boo") { Price = 60, Category = "Enemies", Description = "Materialize a spooky boo ghost!" },
		new("Spawn Chain Chomp", "spawn_chainchomp") { Price = 80, Category = "Enemies", Description = "Release a ferocious chain chomp!" },
		new("Spawn Cheep Cheep", "spawn_cheepcheep") { Price = 30, Category = "Enemies", Description = "Splash in a cheep cheep fish!" },
		new("Spawn Dry Bones", "spawn_drybones") { Price = 45, Category = "Enemies", Description = "Rise up a skeletal dry bones!" },
		new("Spawn Fire", "spawn_fire") { Price = 40, Category = "Enemies", Description = "Ignite a blazing fire enemy!" },
		new("Spawn Bomb", "spawn_bomb") { Price = 35, Category = "Enemies", Description = "Drop an explosive bomb!" },
		new("Spawn Bullet Bill", "spawn_bulletbill") { Price = 45, Category = "Enemies", Description = "Launch a speeding bullet bill!" },
		new("Spawn Cannonball", "spawn_cannonball") { Price = 40, Category = "Enemies", Description = "Fire a heavy cannonball!" },
		new("Spawn Castle Fire", "spawn_castlefire") { Price = 55, Category = "Enemies", Description = "Summon castle fire to burn!" },
		new("Spawn Fishbone", "spawn_fishbone") { Price = 35, Category = "Enemies", Description = "Bone up with a fishbone!" },
		new("Spawn Flying Fish", "spawn_flyingfish") { Price = 40, Category = "Enemies", Description = "Soar in a flying fish!" },
		new("Spawn GLaDOS", "spawn_glados") { Price = 150, Category = "Enemies", Description = "Activate the AI overlord GLaDOS!" },
		new("Spawn Grinder", "spawn_grinder") { Price = 50, Category = "Enemies", Description = "Grind up a deadly grinder!" },
		new("Spawn Icicle", "spawn_icicle") { Price = 35, Category = "Enemies", Description = "Freeze with a sharp icicle!" },
		new("Spawn King Bill", "spawn_kingbill") { Price = 70, Category = "Enemies", Description = "Crown the mighty King Bill!" },
		new("Spawn Lakitu", "spawn_lakito") { Price = 65, Category = "Enemies", Description = "Float down a cloud-riding lakitu!" },
		new("Spawn Meteor", "spawn_meteor") { Price = 60, Category = "Enemies", Description = "Crash land a destructive meteor!" },
		new("Spawn Mole", "spawn_mole") { Price = 40, Category = "Enemies", Description = "Burrow up a tunneling mole!" },
		new("Spawn Muncher", "spawn_muncher") { Price = 45, Category = "Enemies", Description = "Chomp down with a hungry muncher!" },
		new("Spawn Ninji", "spawn_ninji") { Price = 50, Category = "Enemies", Description = "Stealth in a ninja ninji!" },
		new("Spawn Para Beetle", "spawn_parabeetle") { Price = 40, Category = "Enemies", Description = "Wing in a para beetle!" },
		new("Spawn Plant", "spawn_plant") { Price = 35, Category = "Enemies", Description = "Grow a vicious piranha plant!" },
		new("Spawn Plant Creeper", "spawn_plantcreeper") { Price = 55, Category = "Enemies", Description = "Creep in a plant creeper!" },
		new("Spawn Plant Fire", "spawn_plantfire") { Price = 45, Category = "Enemies", Description = "Blaze with a fire-breathing plant!" },
		new("Spawn Poison Mush", "spawn_poisonmush") { Price = 30, Category = "Enemies", Description = "Poison with a deadly mushroom!" },
		new("Spawn Pokey", "spawn_pokey") { Price = 40, Category = "Enemies", Description = "Stack up a spiky pokey!" },
		new("Spawn Rocket Turret", "spawn_rocketturret") { Price = 80, Category = "Enemies", Description = "Deploy a rocket-firing turret!" },
		new("Spawn Rocky Wrench", "spawn_rockywrench") { Price = 45, Category = "Enemies", Description = "Rock out with a rocky wrench!" },
		new("Spawn Sidestepper", "spawn_sidestepper") { Price = 35, Category = "Enemies", Description = "Side-step with a crabby sidestepper!" },
		new("Spawn Skewer", "spawn_skewer") { Price = 60, Category = "Enemies", Description = "Skewer through with a deadly skewer!" },
		new("Spawn Spike", "spawn_spike") { Price = 40, Category = "Enemies", Description = "Spike up a dangerous spike!" },
		new("Spawn Splunkin", "spawn_splunkin") { Price = 35, Category = "Enemies", Description = "Pumpkin up a spooky splunkin!" },
		new("Spawn Squid", "spawn_squid") { Price = 40, Category = "Enemies", Description = "Ink up an underwater squid!" },
		new("Spawn Torpedo Ted", "spawn_torpedoted") { Price = 50, Category = "Enemies", Description = "Launch a torpedo ted!" },
		new("Spawn Turret", "spawn_turret") { Price = 70, Category = "Enemies", Description = "Set up a firing turret!" },
		new("Spawn Barrel", "spawn_barrel") { Price = 30, Category = "Enemies", Description = "Roll in a rolling barrel!" },
		new("Spawn Big Bill", "spawn_bigbill") { Price = 60, Category = "Enemies", Description = "Go big with a massive Big Bill!" },
		new("Spawn Big Mole", "spawn_bigmole") { Price = 70, Category = "Enemies", Description = "Dig deep with a huge Big Mole!" },
		new("Spawn Boom Boom", "spawn_boomboom") { Price = 120, Category = "Enemies", Description = "Explode with the boss Boom Boom!" },
		new("Spawn Core", "spawn_core") { Price = 90, Category = "Enemies", Description = "Unleash the powerful Core!" },
		new("Spawn Box", "spawn_box") { Price = 5, Category = "Enemies", Description = "Drop a mysterious box!" },
		new("Spawn Lava Bubble", "spawn_upfire") { Price = 35, Category = "Enemies", Description = "Bubble up some scorching lava!" },
		new("Spawn Amp", "spawn_amp") { Price = 20, Category = "Enemies", Description = "Electrify with a shocking amp!" },
		new("Spawn Fuzzy", "spawn_fuzzy") { Price = 20, Category = "Enemies", Description = "Fuzz up with a fuzzy enemy!" },
		new("Spawn Angry Sun", "spawn_angrysun") { Price = 50, Category = "Enemies", Description = "Blaze down with an angry sun!" },
		new("Spawn Bowser", "spawn_bowser") { Price = 100, Category = "Enemies", Description = "Unleash the mighty King Bowser!" },

		// PROJECTILE SPAWNING EFFECTS
		new("Spawn Fireball", "spawn_fireball") { Price = 25, Category = "Projectiles", Description = "Launch a blazing fireball!" },
		new("Spawn Super Ball", "spawn_superball") { Price = 35, Category = "Projectiles", Description = "Bounce with a super ball!" },


		// ITEM SPAWNING EFFECTS
		new("Spawn Mushroom", "spawn_mushroom") { Price = 15, Category = "Items", Description = "Power up with a magic mushroom!" },
		new("Spawn Star", "spawn_star") { Price = 50, Category = "Items", Description = "Shine bright with a super star!" },
		new("Spawn Gel", "spawn_gel") { Price = 25, Category = "Items", Description = "Splash with portal gel!" },
		new("Spawn Gel Dispenser", "spawn_geldispenser") { Price = 45, Category = "Items", Description = "Install a gel dispensing machine!" },

		// MECHANIC SPAWNING EFFECTS

		new("Spawn Spring", "spawn_spring") { Price = 25, Category = "Mechanics", Description = "Bounce high with a spring!" },
		new("Spawn Small Spring", "spawn_smallspring") { Price = 20, Category = "Mechanics", Description = "Hop with a small spring!" },

		new("Spawn Donut", "spawn_donut") { Price = 25, Category = "Mechanics", Description = "Drop a delicious donut block!" },
		new("Spawn Flip Block", "spawn_flipblock") { Price = 30, Category = "Mechanics", Description = "Flip with a flippable block!" },
		new("Spawn Mini Block", "spawn_miniblock") { Price = 20, Category = "Mechanics", Description = "Place a tiny mini block!" },
		new("Spawn POW Block", "spawn_powblock") { Price = 40, Category = "Mechanics", Description = "Power up with a POW block!" },

	};
}
