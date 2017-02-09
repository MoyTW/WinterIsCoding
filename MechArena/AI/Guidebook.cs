﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    class ActionClause
    {
        private IEnumerable<Condition> conditions;
        private Action action;

        public ActionClause(IEnumerable<Condition> conditions, Action action)
        {
            this.conditions = conditions;
            this.action = action;
        }

        public bool ShouldTakeAction(GameQuery_Command commandQuery)
        {
            return !conditions.Any(c => !c.IsMet(commandQuery));
        }

        public GameEvent_Command CommandForQuery(GameQuery_Command commandQuery)
        {
            return this.action.GenerateCommand(commandQuery);
        }
    }

    class Guidebook
    {
        private List<ActionClause> builtRules;

        public Guidebook(IEnumerable<SingleClause> rawRules)
        {
            this.builtRules = Guidebook.BuildRules(rawRules);
        }

        private static List<ActionClause> BuildRules(IEnumerable<SingleClause> rawRules)
        {
            var builtRules = new List<ActionClause>();
            var acc = new List<Condition>();
            foreach (var clause in rawRules)
            {
                if (clause is Action)
                {
                    builtRules.Add(new ActionClause(acc, (Action)clause));
                    acc = new List<Condition>();
                }
                else if (clause is Condition)
                    acc.Add((Condition)clause);
                else
                    throw new NotImplementedException();
            }
            return builtRules;
        }

        public void TryRegisterCommand(GameQuery_Command commandRequest)
        {
            GameEvent_Command command = this.builtRules.Where(r => r.ShouldTakeAction(commandRequest))
                .Select(r => r.CommandForQuery(commandRequest))
                .FirstOrDefault();
            if (command != null)
                commandRequest.RegisterCommand(command);
        }
    }
}