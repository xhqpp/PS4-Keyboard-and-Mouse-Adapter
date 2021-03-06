﻿using System;
using System.Linq;
using HarmonyLib;
using PS4RemotePlayInterceptor;

namespace PS4RemotePlayInjection
{
    [Serializable]
    class Patcher
    {
        public static InjectionInterface server;
        private static bool isToolbarShown = true;

        public static void DoPatching()
        {
            var harmony = new Harmony("com.example.patch");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var dll = assemblies?.FirstOrDefault(x => x.FullName.Contains("RemotePlay"));
            var types = dll?.GetTypes();

            // Show toolbar patch
            var type = types?.FirstOrDefault(x => x.Name.Equals("StreamingToolBar"));

            var showToolBarMethod = AccessTools.Method(type, "ShowToolBar");
            var showToolBarPrefix = SymbolExtensions.GetMethodInfo(() => ShowToolBarPrefix());
            var showToolBarPostfix = SymbolExtensions.GetMethodInfo(() => ShowToolBarPostfix());
            // in general, add null checks here (new HarmonyMethod() does it for you too)

            harmony.Patch(showToolBarMethod, new HarmonyMethod(showToolBarPrefix), new HarmonyMethod(showToolBarPostfix));

            // Hide toolbar patch
            var hideToolBarMethod = AccessTools.Method(type, "HideToolBar");
            var hideToolBarPrefix = SymbolExtensions.GetMethodInfo(() => HideToolBarPrefix());
            var hideToolBarPostfix = SymbolExtensions.GetMethodInfo(() => HideToolBarPostfix());

            harmony.Patch(hideToolBarMethod, new HarmonyMethod(hideToolBarPrefix), new HarmonyMethod(hideToolBarPostfix));
        }

        public static bool ShowToolBarPrefix()
        {
            var showToolbar = !server.ShouldHideToolbar();

            if (showToolbar)
                isToolbarShown = true;

            return showToolbar;
        }

        public static void ShowToolBarPostfix()
        {
            // ...
        }

        public static bool HideToolBarPrefix()
        {
            var dontHideHud = !server.ShouldHideToolbar();

            if (dontHideHud || (!dontHideHud && isToolbarShown))
            {
                isToolbarShown = false;
                return true;
            }
            return false;
        }

        public static void HideToolBarPostfix()
        {
            // ...
        }
    }
}
