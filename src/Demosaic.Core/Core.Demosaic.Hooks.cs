﻿using BepInEx;
using HarmonyLib;
#if AI
using AIChara;
#endif

namespace KK_Plugins
{
    public partial class Demosaic : BaseUnityPlugin
    {
        internal static class Hooks
        {
            [HarmonyPrefix, HarmonyPatch(typeof(ChaControl), "LateUpdateForce")]
            internal static void LateUpdateForce(ChaControl __instance)
            {
                if (Enabled.Value)
                    __instance.hideMoz = true;
            }
        }
    }
}