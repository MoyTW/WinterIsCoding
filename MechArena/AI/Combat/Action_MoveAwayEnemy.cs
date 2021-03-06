﻿using System;
using System.Collections.Generic;

namespace MechArena.AI.Combat
{
    [Serializable()]
    class Action_MoveAwayEnemy : AIAction
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            return commandQuery.ExecutorEntity.HasComponentOfType<Component_MechSkeleton>();
        }

        public override GameEvent_Command GenerateCommand(GameQuery_Command commandQuery)
        {
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Mech1)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Mech1;

            // TODO: Wow this is awkward!?
            var commandPos = commandQuery.CommandEntity.TryGetPosition();
            var commandCell = commandQuery.ArenaState.ArenaMap.GetCell(commandPos.X, commandPos.Y);
            var targetPos = target.TryGetPosition();
            var targetCell = commandQuery.ArenaState.ArenaMap.GetCell(targetPos.X, targetPos.Y);

            var myDist = commandQuery.ArenaState.ArenaPathFinder.ShortestPath(commandCell, targetCell).Length;
            var currDist = myDist;
            int awayX = 0, awayY = 0;
            // TODO: This is incredibly, absurdly inefficient! However it's sure to be optimal in a one-move timeframe.
            for (int x = commandPos.X - 1; x < commandPos.X + 2; x++)
            {
                for (int y = commandPos.Y - 1; y < commandPos.Y + 2; y++)
                {
                    if (commandQuery.ArenaState.ArenaMap.IsWalkable(x, y) && !(x == targetPos.X && y == targetPos.Y))
                    {
                        var candidateCell = commandQuery.ArenaState.ArenaMap.GetCell(x, y);
                        var path = commandQuery.ArenaState.ArenaPathFinder.ShortestPath(candidateCell, targetCell);
                        if (path.Length > currDist)
                        {
                            awayX = x;
                            awayY = y;
                            currDist = path.Length;
                        }
                    }
                }
            }

            if (currDist > myDist)
            {
                return new GameEvent_MoveSingle(commandQuery.CommandEntity, commandQuery.ArenaState.CurrentTick,
                    awayX - commandPos.X, awayY - commandPos.Y, commandQuery.ArenaState);
            }
            else
                return null;
        }

        public override System.Collections.Generic.IEnumerable<SingleClause> EnumerateClauses()
        {
            yield return new Action_MoveAwayEnemy();
        }
    }
}

