﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI.Combat
{
    [Serializable()]
    class Condition_CanSeeEnemy : Condition
    {
        public override bool IsMet(GameQuery_Command commandQuery)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Mech1)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Mech1;

            var targetPos = target.TryGetPosition();
            var selfPos = commandQuery.CommandEntity.TryGetPosition();
            // TODO: I don't really subscribe to Demeter, but this is a pretty long chain!
            var path = commandQuery.ArenaState.ArenaMap
                .GetCellsAlongLine(selfPos.X, selfPos.Y, targetPos.X, targetPos.Y);

            return !path.Any(c => !c.IsWalkable);
        }

        public override IEnumerable<SingleClause> EnumerateClauses()
        {
            yield return new Condition_CanSeeEnemy();
        }
    }
}
