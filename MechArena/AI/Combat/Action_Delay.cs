﻿using System;

namespace MechArena.AI.Combat
{
    [Serializable()]
    class Action_Delay : AIAction
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            // Anything which takes actions must implicitly track time and be delayable, hence always true
            return true;
        }

        public override GameEvent_Command GenerateCommand(GameQuery_Command commandQuery)
        {
            return new GameEvent_Delay(commandQuery.ArenaState.CurrentTick, commandQuery.CommandEntity,
                commandQuery.ExecutorEntity, DelayDuration.NEXT_ACTION);
        }

        public override System.Collections.Generic.IEnumerable<SingleClause> EnumerateClauses()
        {
            yield return new Action_Delay();
        }
    }
}

