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
using OpenRA.GameRules;
using OpenRA.Mods.RA.Traits;
using OpenRA.Traits;
using System;

namespace OpenRA.Mods.RA.Warheads
{
	public class ChangeOwnerWarhead : Warhead
	{

		public readonly WRange Range = WRange.FromCells(1);

		public override void DoImpact(Target target, Actor firedBy, IEnumerable<int> damageModifiers)
		{

			var actors = target.Type == TargetType.Actor ? new[] { target.Actor } :
				firedBy.World.FindActorsInCircle(target.CenterPosition, Range);

			foreach (var a in actors)
			{
				if (a.Owner == firedBy.Owner) // don't do anything on friendly fire
					continue;

                var tempOwnerManager = a.TraitOrDefault<TemporaryOwnerManager>();
                if (tempOwnerManager == null || tempOwnerManager.Possesed()) //Inform to player tempOwnerManager.Possesed()
                    continue;
                tempOwnerManager.ChangeOwner(a, firedBy);
                
				a.CancelActivity(); // stop shooting, you have got new enemies
                break; //Yuri only can owned one unit
			}
		}
	}
}
