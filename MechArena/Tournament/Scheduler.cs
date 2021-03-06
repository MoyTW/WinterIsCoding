﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Tournament
{
    public static class Scheduler
    {
        public static List<Match> ScheduleRoundRobin(List<ICompetitor> originalCompetitors, IMapPicker mapPicker,
            bool tieBreaker=false)
        {
            var competitors = new List<ICompetitor>(originalCompetitors);

            if (competitors.Count < 2)
                throw new ArgumentException("Can't schedule if only 1 competitor!");

            var bye = new CompetitorPlaceholder("TOURNAMENT_BYE", "TOURNAMENT_BYE");
            if (competitors.Count % 2 != 0)
            {
                competitors.Add(bye);
            }

            List<Match> matches = new List<Match>();

            List<ICompetitor> teams = new List<ICompetitor>(competitors);
            teams.RemoveAt(0);

            int teamsSize = teams.Count;

            for (int day = 0; day < competitors.Count - 1; day++)
            {
                int teamIdx = day % teamsSize;

                matches.Add(new Match(competitors[0], teams[teamIdx], mapPicker.PickMapID(), tieBreaker));

                for (int idx = 1; idx < competitors.Count / 2; idx++)
                {
                    var match = new Match(teams[(day + idx) % teamsSize], teams[(day + teamsSize - idx) % teamsSize],
                        mapPicker.PickMapID(), tieBreaker);
                    matches.Add(match);
                }
            }

            return matches.Where(m => !m.HasCompetitor(bye.CompetitorID)).ToList();
        }
    }
}
