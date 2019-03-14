﻿using Ow.Game.Objects;
using Ow.Managers;
using Ow.Net.netty.commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ow.Game.Objects.Players.Managers
{
    class CpuManager
    {
        public const String CLK_XL = "equipment_extra_cpu_cl04k-xl";
        public const String AUTO_ROCKET_CPU = "equipment_extra_cpu_arol-x";
        public const String AUTO_HELLSTROM_CPU = "equipment_extra_cpu_rllb-x";
        public const String ROCKET_LAUNCHER = "equipment_weapon_rocketlauncher_hst-2";
        public const String GALAXY_JUMP_CPU = "equipment_extra_cpu_jp-02";

        private const int PREMIUM_CLOAK_COOLDOWN = 10000;
        private const int CLOAK_COOLDOWN = 20000;
        private const int CLOAK_PRICE = -256;

        public Player Player { get; set; }

        public CpuManager(Player player) { Player = player; }

        public DateTime cloakCooldown = new DateTime();
        public void Cloak()
        {
            if (cloakCooldown.AddMilliseconds(Player.Premium ? PREMIUM_CLOAK_COOLDOWN : CLOAK_COOLDOWN) < DateTime.Now || Player.Storage.GodMode)
            {
                if (Player.Spacemap.Id != 42 && Player.Spacemap.Id != 121)
                    if (!Player.Invisible)
                        EnableCloak();

                cloakCooldown = DateTime.Now;
            }
            else Player.SendPacket("0|A|STD|Cloak cooldown:" + ((Player.Premium ? PREMIUM_CLOAK_COOLDOWN : CLOAK_COOLDOWN) / 1000 - ((DateTime.Now - cloakCooldown).TotalSeconds)));
        }

        public void ArolX()
        {
            if(!Player.Storage.AutoRocket)
                EnableArolX();
            else
                DisableArolX();
        }

        public void RllbX()
        {
            if (!Player.Storage.AutoRocketLauncher)
                EnableRllbX();
            else
                DisableRllbX();
        }

        public void EnableCloak()
        {
            Player.Invisible = true;
            string cloakPacket = "0|n|INV|" + Player.Id + "|1";
            Player.SendPacket(cloakPacket);
            Player.SendPacketToInRangePlayers(cloakPacket);

            var pet = Player.Pet;
            if (pet.Activated)
            {
                pet.Invisible = true;
                string petCloakPacket = "0|n|INV|" + pet.Id + "|1";
                pet.SendPacketToInRangePlayers(petCloakPacket);
            }

            AddSelectedCpu(CLK_XL);

            Player.SettingsManager.SendNewItemStatus(CLK_XL);
        }

        public void DisableCloak()
        {
            Player.Invisible = false;
            string cloakPacket = "0|n|INV|" + Player.Id + "|0";
            Player.SendPacket("0|A|STM|msg_uncloaked");
            Player.SendPacket(cloakPacket);
            Player.SendPacketToInRangePlayers(cloakPacket);
            RemoveSelectedCpu(CLK_XL);
            Player.SettingsManager.SendNewItemStatus(CLK_XL);
        }

        public void EnableArolX()
        {
            Player.Storage.AutoRocket = true;

            AddSelectedCpu(AUTO_ROCKET_CPU);

            Player.SettingsManager.SendNewItemStatus(AUTO_ROCKET_CPU);
        }

        public void DisableArolX()
        {
            Player.Storage.AutoRocket = false;
            RemoveSelectedCpu(AUTO_ROCKET_CPU);
            Player.SettingsManager.SendNewItemStatus(AUTO_ROCKET_CPU);
        }

        public void EnableRllbX()
        {
            Player.Storage.AutoRocketLauncher = true;

            AddSelectedCpu(AUTO_HELLSTROM_CPU);

            Player.SettingsManager.SendNewItemStatus(AUTO_HELLSTROM_CPU);
        }

        public void DisableRllbX()
        {
            Player.Storage.AutoRocketLauncher = false;
            RemoveSelectedCpu(AUTO_HELLSTROM_CPU);
            Player.SettingsManager.SendNewItemStatus(AUTO_HELLSTROM_CPU);
        }

        public void AddSelectedCpu(string cpu)
        {
            if (!Player.Settings.InGameSettings.selectedCpus.Contains(cpu))
                Player.Settings.InGameSettings.selectedCpus.Add(cpu);
        }

        public void RemoveSelectedCpu(string cpu)
        {
            if (Player.Settings.InGameSettings.selectedCpus.Contains(cpu))
                Player.Settings.InGameSettings.selectedCpus.Remove(cpu);
        }
    }
}
