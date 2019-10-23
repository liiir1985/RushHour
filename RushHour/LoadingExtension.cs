﻿using System.Collections.Generic;
using System.Reflection;
using ICities;
using RushHour.Redirection;
using UnityEngine;
using RushHour.UI;
using RushHour.Events;
using RushHour.Experiments;
using RushHour.Logging;

namespace RushHour
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static Dictionary<MethodInfo, RedirectCallsState> redirects;
        private static bool _redirected = false; //Temporary to solve crashing for now. I think it needs to stop threads from calling it while it's reverting the redirect.
        private static bool _simulationRegistered = false;

        public static GameObject _mainUIGameObject = null;

        private GameObject _dateTimeGameObject = null;
        private DateTimeBar _dateTimeBar = null;
        private SimulationExtension _simulationManager = new SimulationExtension();

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            Debug.Log("Loading up Rush Hour main");
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || (ExperimentsToggle.EnableInScenarios && mode == (LoadMode)11)) //11 is some new mode not implemented in ICities... 
            {
                LoggingWrapper.Log("Loading mod");
                CimTools.CimToolsHandler.CimToolBase.Changelog.DownloadChangelog();
                CimTools.CimToolsHandler.CimToolBase.XMLFileOptions.Load();

                if (!ExperimentsToggle.GhostMode)
                {
                    if (_dateTimeGameObject == null)
                    {
                        _dateTimeGameObject = new GameObject("DateTimeBar");
                    }

                    if (_mainUIGameObject == null)
                    {
                        _mainUIGameObject = new GameObject("RushHourUI");
                        EventPopupManager popupManager = EventPopupManager.Instance;
                    }

                    if (_dateTimeBar == null)
                    {
                        _dateTimeBar = _dateTimeGameObject.AddComponent<DateTimeBar>();
                        _dateTimeBar.Initialise();
                    }

                    if (!_simulationRegistered)
                    {
                        SimulationManager.RegisterSimulationManager(_simulationManager);
                        _simulationRegistered = true;
                        LoggingWrapper.Log("Simulation hooked");
                    }

                    Redirect();
                }
            }
            else
            {
                Debug.Log("Rush Hour is not set to start up in this mode. " + mode.ToString());
            }
        }

        public override void OnLevelUnloading()
        {
            if (!ExperimentsToggle.GhostMode)
            {
                if (ExperimentsToggle.RevertRedirects)
                {
                    RevertRedirect();
                }

                if (_dateTimeBar != null)
                {
                    _dateTimeBar.CloseEverything();
                    _dateTimeBar = null;
                }

                _dateTimeGameObject = null;
                _simulationManager = null;
                _mainUIGameObject = null;
            }

            base.OnLevelUnloading();
        }

        public static void Redirect()
        {
            if (!_redirected || ExperimentsToggle.RevertRedirects)
            {
                _redirected = true;

                redirects = new Dictionary<MethodInfo, RedirectCallsState>();
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    redirects.AddRange(RedirectionUtil.RedirectType(type));
                }
            }
        }

        private static void RevertRedirect()
        {
            if (redirects == null)
            {
                return;
            }
            foreach (var kvp in redirects)
            {
                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
            redirects.Clear();
        }
    }
}