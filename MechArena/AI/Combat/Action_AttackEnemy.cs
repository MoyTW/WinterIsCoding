﻿using System;

namespace MechArena.AI.Combat
{
    [Serializable()]
    class Action_AttackEnemy : AIAction
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            return commandQuery.ExecutorEntity.HasComponentOfType<Component_Weapon>();
        }

        public override GameEvent_Command GenerateCommand(GameQuery_Command commandQuery)
        {
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Mech1)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Mech1;

            return new GameEvent_Attack(commandQuery.ArenaState.CurrentTick, commandQuery.CommandEntity, target,
                commandQuery.ExecutorEntity, commandQuery.ArenaState.ArenaMap, commandQuery.Rand);
        }

        public override System.Collections.Generic.IEnumerable<SingleClause> EnumerateClauses()
        {
            yield return new Action_AttackEnemy();
        }
    }
}

