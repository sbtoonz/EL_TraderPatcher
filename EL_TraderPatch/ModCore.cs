using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using EpicLoot.Adventure;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EL_TraderPatch
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("randyknapp.mods.epicloot", BepInDependency.DependencyFlags.HardDependency)]
    public class NewMod : BaseUnityPlugin
    {
        private const string ModName = "TraderPatch";
        private const string ModVersion = "1.0";
        private const string ModGUID = "com.zarboz.traderpatch";
        public static Doodad? Doodadthing = null;
        private static Harmony harmony = null!;

        ServerSync.ConfigSync configSyncs = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        internal static ConfigEntry<bool> ServerConfigLocked = null!;

        ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSyncs.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true) =>
            config(group, name, value, new ConfigDescription(description), synchronizedSetting);

        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            ServerConfigLocked = config("1 - General", "Lock Configuration", true,
                "If on, the configuration is locked and can be changed by server admins only.");
            configSyncs.AddLockingConfigEntry(ServerConfigLocked);
            

        }

        [HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Awake))]
        public static class AwakeStorePatch
        {
            public static void Postfix(StoreGui __instance)
            {
                if (!Doodadthing) Doodadthing = __instance.gameObject.AddComponent<Doodad>();
            }
        }

        [HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Show))]
        [HarmonyAfter(nameof(EpicLoot.Adventure.StoreGui_Patch.Show_Postfix))]
        public static class storeguishowpatch
        {
            public static void Postfix(StoreGui __instance)
            {
                Doodadthing.SetPanel();
            }
        }
        
        [HarmonyPatch(typeof(Trader), nameof(Trader.Interact))]
        public static class TraderTestPatch
        {
            public static void Postfix(Trader __instance)
            {
                if (__instance.gameObject.name.StartsWith("Haldor"))
                {
                    Doodadthing.mIsSetFromHaldor = true;
                    Doodadthing.SetPanel();
                }
            }
        }
    }
}
