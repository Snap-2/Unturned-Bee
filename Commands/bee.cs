using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeeMovie.Commands
{
    public static class Globals
    {
        public static bool is_running { get; set; } = false;
        public static int time_delay { get; set; }
    }

    [Command("bee")]
    public class Bee : UnturnedCommand
    {
        private readonly IConfiguration B_configuration;
        private readonly ILogger<Bee> B_logger;
        public Bee(ILogger<Bee> logger, IConfiguration configuration, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            B_configuration = configuration;
            B_logger = logger;
        }

        protected override async UniTask OnExecuteAsync()
        {
            await UniTask.SwitchToMainThread();

            Globals.is_running = true;
            string? cfg_path = @$"{B_configuration.GetValue<string>("scriptPath")}";
            string cfg_preset = @"BLANK\BeeMovie.script.txt";

            if (cfg_path == cfg_preset)
            {
                B_logger.LogWarning("BeeMovie script path not found, command not executed!");
                B_logger.LogWarning("Look in the plugins folder for the BeeMovie folder, copy path as text and see instructions in the config.yaml!");
                return;
            }

            try
            {
                IEnumerable<string> lines = File.ReadLines(cfg_path);

                int i = 0;
                foreach (var line in lines)
                {
                    if (Globals.is_running == true)
                    {
                        Globals.time_delay = line.Length * 75;
                        int j = i % 2;

                        Color[] colors = new Color[2] { Color.black, Color.yellow };

                        SDG.Unturned.ChatManager.serverSendMessage($"{line}", colors[j], null, null, SDG.Unturned.EChatMode.GLOBAL);

                        await UniTask.Delay(Globals.time_delay);

                        i++;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            catch (Exception ex)
            {
                B_logger.LogError("Something went wrong with loading the bee movie script:");
                B_logger.LogError(ex.ToString());
            }
        }
    }

    [Command("unbee")]
    public class UnBee : UnturnedCommand
    {
        public UnBee(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            await UniTask.SwitchToMainThread();

            Globals.is_running = false;

            SDG.Unturned.ChatManager.serverSendMessage("Unbee-ing...", Color.magenta, null, null, SDG.Unturned.EChatMode.GLOBAL);

            await UniTask.Delay(Globals.time_delay);

            SDG.Unturned.ChatManager.serverSendMessage("You've exited the bee movie's curse.", Color.magenta, null, null, SDG.Unturned.EChatMode.GLOBAL);
        }
    }
}
