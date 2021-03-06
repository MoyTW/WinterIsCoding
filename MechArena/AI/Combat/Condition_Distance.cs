﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI.Combat
{
    enum DistanceOption
    {
        MELEE_RANGE,
        THIS_WEAPON_RANGE,
        // TODO: Implement the following!
        MY_LONGEST_RANGE,
        ENEMY_LONGEST_RANGE
        // MY_OPTIMAL_RANGE,
        // ENEMY_OPTIMAL_RANGE
    }

    enum ComparisonOperator
    {
        EQUAL,
        LESS_THAN,
        LESS_THAN_EQUAL,
        GREATER_THAN,
        GREATER_THAN_EQUAL
    }

    [Serializable()]
    class Condition_Distance : Condition
    {
        public DistanceOption Option { get; }
        public ComparisonOperator Operator { get; }

        public Condition_Distance() { }

        public Condition_Distance(ComparisonOperator op, DistanceOption option)
        {
            this.Operator = op;
            this.Option = option;
        }

        private int? ResolveOptionDistance(GameQuery_Command commandQuery, Entity target)
        {
            switch (this.Option)
            {
                case DistanceOption.MELEE_RANGE:
                    return 1;
                case DistanceOption.THIS_WEAPON_RANGE:
                    if (commandQuery.ExecutorEntity.HasComponentOfType<Component_Weapon>())
                    {
                        return commandQuery.ExecutorEntity
                            .TryGetAttribute(EntityAttributeType.MAX_RANGE, commandQuery.ExecutorEntity)
                            .Value;
                    }
                    else
                        return null;
                case DistanceOption.MY_LONGEST_RANGE:
                    return commandQuery.CommandEntity.TryGetSubEntities(SubEntitiesSelector.ACTIVE_TRACKS_TIME)
                        .Where(e => e.HasComponentOfType<Component_Weapon>())
                        .Select(e => e.TryGetAttribute(EntityAttributeType.MAX_RANGE, e).Value)
                        .Max();
                case DistanceOption.ENEMY_LONGEST_RANGE:
                    return target.TryGetSubEntities(SubEntitiesSelector.ACTIVE_TRACKS_TIME)
                        .Where(e => e.HasComponentOfType<Component_Weapon>())
                        .Select(e => e.TryGetAttribute(EntityAttributeType.MAX_RANGE, e).Value)
                        .Max();
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool IsMet(GameQuery_Command commandQuery)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Mech1)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Mech1;

            // TODO: Smooth out this int? business!
            // It's an int? because some conditions (asking for WEAPON_RANGE on something which has no range, like a
            // MechSkeleton, but which does get turns) are nonsensical.
            int? optionDistance = this.ResolveOptionDistance(commandQuery, target);
            if (optionDistance == null)
                return true;

            var targetPos = target.TryGetPosition();
            var selfPos = commandQuery.CommandEntity.TryGetPosition();
            int currDist = commandQuery.ArenaState.ArenaMap
                .GetCellsAlongLine(selfPos.X, selfPos.Y, targetPos.X, targetPos.Y)
                .Count() - 1;

            switch (this.Operator)
            {
                case ComparisonOperator.EQUAL:
                    return currDist == optionDistance;
                case ComparisonOperator.LESS_THAN:
                    return currDist < optionDistance;
                case ComparisonOperator.LESS_THAN_EQUAL:
                    return currDist <= optionDistance;
                case ComparisonOperator.GREATER_THAN:
                    return currDist > optionDistance;
                case ComparisonOperator.GREATER_THAN_EQUAL:
                    return currDist >= optionDistance;
                default:
                    throw new InvalidOperationException("Condition_Distance can't handle " + this.Operator);
            }
        }

        public override IEnumerable<SingleClause> EnumerateClauses()
        {
            foreach (var option in Enum.GetValues(typeof(DistanceOption)).Cast<DistanceOption>())
            {
                foreach (var op in Enum.GetValues(typeof(ComparisonOperator)).Cast<ComparisonOperator>())
                {
                    yield return new Condition_Distance(op, option);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("[Condition_Distance: Option={0}, Operator={1}]", Option, Operator);
        }
    }
}
