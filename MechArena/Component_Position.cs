﻿using System;
using System.Collections.Immutable;

namespace MechArena
{
    [Serializable()]
    class Component_Position : Component
    {
        private int x, y;
        private bool blocksMovement;

        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }
        public bool BlocksMovement { get { return this.blocksMovement; } }

        public Component_Position(int x, int y, bool blocksMovement)
        {
            this.x = x;
            this.y = y;
            this.blocksMovement = blocksMovement;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleMove(GameEvent_MoveSingle ev)
        {
            if (ev.GameArena.IsWalkableAndOpen(this.x + (int)ev.X, this.y + (int)ev.Y))
            {
                this.x += (int)ev.X;
                this.y += (int)ev.Y;
            }
            ev.Completed = true;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_MoveSingle)
                this.HandleMove((GameEvent_MoveSingle)ev);

            return ev;
        }

        private void HandleQueryPosition(GameQuery_Position q)
        {
            q.RegisterPosition(this.x, this.y, this.blocksMovement);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Position)
                this.HandleQueryPosition((GameQuery_Position)q);

            return q;
        }
    }
}
