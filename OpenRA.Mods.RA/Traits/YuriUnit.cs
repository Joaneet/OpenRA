#region Copyright & License Information
/*
 * Copyright 2007-2015 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Drawing;
using OpenRA.Activities;
using OpenRA.Mods.Common;
using OpenRA.Mods.Common.Activities;
using OpenRA.Mods.Common.Orders;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;
using OpenRA.Mods.Common.Graphics;
using OpenRA.Graphics;

namespace OpenRA.Mods.RA.Traits
{
    public class YuriUnitInfo : ITraitInfo{
        public readonly Color PosessionBar = Color.Red;

        public object Create(ActorInitializer init) { return new YuriUnit(init.Self, this); }
    }
    class YuriUnit : IResolveOrder, INotifyKilled, IPostRender
	{
        private YuriUnitInfo info;
        private Actor self;
        private Actor owner;

        public YuriUnit(Actor self, YuriUnitInfo info)
		{
			this.info = info;
			this.self = self;
		}

        public Actor getOwner()
        {
            if (owner != null && !owner.IsInWorld) owner = null;
            return this.owner;
        }

        public void setOwner(Actor owner)
        {
            this.owner = owner;
            ChangeStance(UnitStance.HoldFire);
        }   
        private void ChangeStance(UnitStance us)
        {
            AutoTarget at = self.TraitOrDefault<AutoTarget>();
            if (at != null)
            {
                at.Stance = us;
            }
        }
		public void ResolveOrder(Actor self, Order order)
		{
            CancelPossesion();
		}
        public void Killed(Actor self, AttackInfo e){
            CancelPossesion();
        }

        public void CancelPossesion()
        {
            ChangeStance(UnitStance.AttackAnything);
            if (owner != null && owner.IsInWorld)
            {
                TemporaryOwnerManager ownerManager = this.owner.TraitOrDefault<TemporaryOwnerManager>();
                if (ownerManager != null)
                {
                    ownerManager.CancelOwner();
                    owner = null;                    
                }
            }
        }

        public void RenderAfterWorld(WorldRenderer wr, Actor self)
        {            
            if (getOwner() != null && getOwner().Owner == self.World.LocalPlayer) {
                Game.Renderer.WorldLineRenderer.DrawLine(wr.ScreenPxPosition(getOwner().CenterPosition), wr.ScreenPxPosition(self.CenterPosition), info.PosessionBar, info.PosessionBar);
                wr.DrawTargetMarker(info.PosessionBar, wr.ScreenPxPosition(getOwner().CenterPosition));
                wr.DrawTargetMarker(info.PosessionBar, wr.ScreenPxPosition(self.CenterPosition));                
            }
        }
	}
}
