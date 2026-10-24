using System.IO;
using System.Text.RegularExpressions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace PersonalStats
{
    public class PersonalStatsModSystem : ModSystem
    {
        const string HotkeyCode = "personalstatstoggle";
        const string NetworkChannel = "personalstats";

        // Any single-tick movement beyond this is treated as a teleport/respawn, not walking.
        const double TeleportDistanceThreshold = 10.0;

        ICoreClientAPI capi;
        GuiDialogPersonalStats dialog;
        StatsData data = new StatsData();
        string dataFilePath;
        bool listenersRegistered;

        Vec3d lastPos;
        bool hasLastPos;

        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;

            api.Network.RegisterChannel(NetworkChannel)
                .RegisterMessageType<MobKilledMessage>()
                .SetMessageHandler<MobKilledMessage>(OnMobKilledMessage);

            api.Input.RegisterHotKey(HotkeyCode, "Toggle Personal Stats", GlKeys.F7, HotkeyType.GUIOrOtherControls);
            dialog = new GuiDialogPersonalStats(api, this);
            api.Input.SetHotKeyHandler(HotkeyCode, OnToggleHotkey);

            // The world (calendar, savegame id, player entity) isn't available yet at this point,
            // only once the client has finished loading into a save.
            api.Event.LevelFinalize += OnLevelFinalize;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Network.RegisterChannel(NetworkChannel)
                .RegisterMessageType<MobKilledMessage>();

            // Death is decided authoritatively on the server, so kill detection has to live here:
            // the client only learns about other entities' deaths through network sync, which
            // doesn't reliably carry a resolvable killer entity for wild animals.
            api.Event.OnEntityDespawn += (entity, despawnData) => OnServerEntityDespawn(api, entity, despawnData);
        }

        void OnServerEntityDespawn(ICoreServerAPI api, Entity entity, EntityDespawnData despawnData)
        {
            if (despawnData.Reason != EnumDespawnReason.Death) return;
            if (entity is EntityPlayer) return;

            var killer = despawnData.DamageSourceForDeath?.GetCauseEntity();
            if (killer is not EntityPlayer killerPlayer) return;

            if (api.World.PlayerByUid(killerPlayer.PlayerUID) is IServerPlayer serverPlayer)
            {
                api.Network.GetChannel(NetworkChannel).SendPacket(new MobKilledMessage(), serverPlayer);
            }
        }

        void OnMobKilledMessage(MobKilledMessage message)
        {
            data.MobsKilled++;
            if (dataFilePath != null) data.Save(dataFilePath);
        }

        void OnLevelFinalize()
        {
            dataFilePath = GetDataFilePath(capi);
            data = StatsData.Load(dataFilePath);

            hasLastPos = false;

            if (!listenersRegistered)
            {
                listenersRegistered = true;
                capi.Event.PlayerDeath += OnPlayerDeath;
                capi.Event.OnPlayerBrokenBlock += OnPlayerBrokenBlock;
                capi.Event.RegisterGameTickListener(OnTrackingTick, 200);
                capi.Event.RegisterGameTickListener(OnAutosaveTick, 10000);
            }
        }

        bool OnToggleHotkey(KeyCombination comb)
        {
            if (dialog.IsOpened()) dialog.TryClose();
            else dialog.TryOpen();
            return true;
        }

        void OnTrackingTick(float dt)
        {
            var entity = capi.World.Player?.Entity;
            if (entity == null || !entity.Alive)
            {
                hasLastPos = false;
                return;
            }

            Vec3d pos = entity.Pos.XYZ;
            if (hasLastPos)
            {
                double dist = pos.DistanceTo(lastPos);
                if (dist < TeleportDistanceThreshold)
                {
                    data.TotalDistanceWalked += dist;
                }
            }
            lastPos = pos;
            hasLastPos = true;
        }

        void OnAutosaveTick(float dt)
        {
            if (dataFilePath != null) data.Save(dataFilePath);
        }

        void OnPlayerDeath(IClientPlayer byPlayer)
        {
            data.Deaths++;
            if (dataFilePath != null) data.Save(dataFilePath);
        }

        void OnPlayerBrokenBlock(BlockDamage damage)
        {
            data.BlocksBroken++;
        }

        public double GetDistanceWalked() => data.TotalDistanceWalked;
        public int GetDeaths() => data.Deaths;
        public int GetBlocksBroken() => data.BlocksBroken;
        public int GetMobsKilled() => data.MobsKilled;

        // Days passed since this world was created, i.e. how long the world/character has existed.
        public double GetDaysSurvived() => capi?.World?.Calendar?.ElapsedDays ?? 0;

        static string GetDataFilePath(ICoreClientAPI api)
        {
            string dir = api.GetOrCreateDataPath(Path.Combine("ModData", "personalstats"));
            string safeId = SanitizeFileName(api.World.SavegameIdentifier);
            return Path.Combine(dir, safeId + ".json");
        }

        static string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "default";
            return Regex.Replace(name, "[^a-zA-Z0-9_-]", "_");
        }

        public override void Dispose()
        {
            if (dataFilePath != null)
            {
                data.Save(dataFilePath);
            }
            base.Dispose();
        }
    }
}
