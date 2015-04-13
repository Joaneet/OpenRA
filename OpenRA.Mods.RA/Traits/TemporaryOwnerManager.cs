#region Copyright & License Information
/*
 * Copyright 2007-2015 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.Drawing;
using OpenRA.Traits;
using System;
using System.Collections.Generic;

namespace OpenRA.Mods.RA.Traits
{
	[Desc("Interacts with the ChangeOwner warhead.",
		"Displays a bar how long this actor is affected and reverts back to the old owner on temporary changes.")]
	public class TemporaryOwnerManagerInfo : ITraitInfo
	{        
		public readonly Color BarColor = Color.Orange;

		public object Create(ActorInitializer init) { return new TemporaryOwnerManager(init.Self, this); }
	}

	public class TemporaryOwnerManager : INotifyOwnerChanged, INotifyKilled
	{
		readonly TemporaryOwnerManagerInfo info;

        Actor self;
        Actor fireBy;
		Player originalOwner;
		Player changingOwner;

		public TemporaryOwnerManager(Actor self, TemporaryOwnerManagerInfo info)
		{
			this.info = info;
			originalOwner = self.Owner;
		}

        public bool Possesed()
        {
            return fireBy != null;
        }

        public void ChangeOwner(Actor self, Actor fireBy)
		{            
            this.self = self;
            if (fireBy.IsInWorld)
            {
                var Yuri = fireBy.TraitOrDefault<YuriUnit>();
                if (Yuri != null && Yuri.getOwner()==null)
                {
                    var tYuri = self.TraitOrDefault<YuriUnit>();
                    if (tYuri != null)
                        tYuri.CancelPossesion();

                    Yuri.setOwner(self);
                    changingOwner = fireBy.Owner;                    
                    self.ChangeOwner(fireBy.Owner);
                    this.fireBy = fireBy;
                }                    
            }
		}

        public void CancelOwner()
        {
            changingOwner = originalOwner;
            self.ChangeOwner(originalOwner);
            self.CancelActivity(); // Stop shooting, you have got new enemies
            fireBy = null;
        }

		public void OnOwnerChanged(Actor self, Player oldOwner, Player newOwner)
		{
			if (changingOwner == null || changingOwner != newOwner)
				originalOwner = newOwner; // It wasn't a temporary change, so we need to update here
			else
				changingOwner = null; // It was triggered by this trait: reset
		}

        public void Killed(Actor self, AttackInfo e)
        {
            if (fireBy != null && fireBy.IsInWorld)
            {
                var Yuri = fireBy.TraitOrDefault<YuriUnit>();
                if (Yuri != null)
                    Yuri.CancelPossesion();
            }
        }
	}
}
