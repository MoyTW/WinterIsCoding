﻿using RogueSharp.Random;
using System;
using System.Collections.Immutable;

namespace MechArena
{
    // Depends on Component_SlottedContainer! Find some way to codify this in your EC system!
    // Assumes that slotted items have Componenet_InternalStructure! Ugh, TERRIBLE!
    [Serializable()]
    public class Component_SlottedStructure : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private int GetRemainingInternalStructure(Entity e)
        {
            return e.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
        }

        private int GetRemainingSlottedStructure(Component_SlottedContainer slottedContainer)
        {
            var q = new GameQuery_EntityAttribute(EntityAttributeType.STRUCTURE, slottedContainer.Parent);
            q.RegisterBaseValue(0);
            return ((GameQuery_EntityAttribute)slottedContainer.HandleQuery(q)).Value;
        }

        private void AssignDamagePoint(Component_SlottedContainer slottedContainer, IRandom rand)
        {
            var target = rand.RandomByWeight<Entity>(slottedContainer.InspectStoredEntities(),
                (e => this.GetRemainingInternalStructure(e)));
            var damageEvent = new GameEvent_TakeDamage(1, rand);
            target.HandleEvent(damageEvent);

            if (!damageEvent.Completed)
                throw new ArgumentException("Target didn't take damage when it should have!");
        }

        private void HandleTakeDamage(GameEvent_TakeDamage ev)
        {
            var slottedContainer = this.Parent.GetComponentOfType<Component_SlottedContainer>();
            if (slottedContainer == null) { return; }

            int remainingSlottedStructure = GetRemainingSlottedStructure(slottedContainer);

            if (remainingSlottedStructure == 0)
            {
                return;
            }
            else if (ev.DamageRemaining >= remainingSlottedStructure)
            {
                ev.Notify_DamageTaken(remainingSlottedStructure);
                for (int i = 0; i < remainingSlottedStructure; i++)
                {
                    this.AssignDamagePoint(slottedContainer, ev.Rand);
                }
            }
            else
            {
                int damageToTake = ev.DamageRemaining;
                for (int i = 0; i < damageToTake; i++)
                {
                    this.AssignDamagePoint(slottedContainer, ev.Rand);
                }
                ev.Notify_DamageTaken(damageToTake);
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_TakeDamage)
                this.HandleTakeDamage((GameEvent_TakeDamage)ev);

            return ev;
        }
    }
}
