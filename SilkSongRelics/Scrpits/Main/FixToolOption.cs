using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SilkSong.Scrpits.Relics;
using SilkSongRelics.Scrpits.Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkSongRelics.Scrpits.Main
{
    public class FixToolOption : RestSiteOption
    {
        public override string OptionId => "FIX_TOOL";

        public FixToolOption(Player owner)
            : base(owner)
        {
        }

        public override async Task<bool> OnSelect()
        {
            foreach(RelicModel rm in Owner.Creature.Player.Relics) 
            {
                if(rm is ToolRelic tr) 
                {
                    tr.Reset();
                    tr.Refresh();
                    tr.Flash();
                }
            }
            return true;
        }

        public override Task DoLocalPostSelectVfx(CancellationToken ct = default(CancellationToken))
        {
            SfxCmd.Play("event:/sfx/byrdpip/byrdpip_attack");
            return Task.CompletedTask;
        }

        public override Task DoRemotePostSelectVfx()
        {
            SfxCmd.Play("event:/sfx/byrdpip/byrdpip_attack");
            NRestSiteCharacter nRestSiteCharacter = NRestSiteRoom.Instance?.Characters.First((NRestSiteCharacter c) => c.Player == base.Owner);
            NRelicFlashVfx nRelicFlashVfx = NRelicFlashVfx.Create(ModelDb.Relic<WispfireLantern>());
            if (nRelicFlashVfx == null)
            {
                return Task.CompletedTask;
            }

            nRestSiteCharacter?.AddChildSafely(nRelicFlashVfx);
            nRelicFlashVfx.Position = Vector2.Zero;
            return Task.CompletedTask;
        }
    }
}
