﻿using MechArena.Tournament;
using NUnit.Framework;
using RogueSharp.Random;
using System.Collections.Generic;

namespace MechArenaTests.Tournament
{
    [TestFixture]
    public class Schedule_TournamentTest
    {
        // TODO: Util fn
        private List<ICompetitor> BuildComps(int numComps)
        {
            List<ICompetitor> comps = new List<ICompetitor>();
            for (int i = 0; i < numComps; i++)
            {
                comps.Add(new CompetitorPlaceholder(i.ToString(), i.ToString()));
            }
            return comps;
        }

        [Test]
        public void TestFirstStage()
        {
            var comps = this.BuildComps(256);

            var st = new Schedule_Tournament(comps, new DotNetRandom(), new MapPickerPlaceholder());
            Assert.AreEqual(1, st.RoundNum());
            Assert.AreEqual(896, st.ScheduledMatches().Count);

            foreach (var m in st.ScheduledMatches())
            {
                st.ReportResult(new MatchResult(m, m.Competitor1, "", 0));
            }

            Assert.AreEqual(32, st.Winners(1).Count);
            Assert.AreEqual(2, st.RoundNum());
        }

        [Test]
        public void TestSecondStage()
        {
            var comps = this.BuildComps(256);

            var st = new Schedule_Tournament(comps, new DotNetRandom(), new MapPickerPlaceholder());
            foreach (var m in st.ScheduledMatches())
            {
                st.ReportResult(new MatchResult(m, m.Competitor1, "", 0));
            }

            Assert.AreEqual(112, st.ScheduledMatches().Count);
            int breaker = 9999;
            while (st.RoundNum() == 2 && st.NextMatch() != null && breaker > 0)
            {
                st.ReportResult(new MatchResult(st.NextMatch(), st.NextMatch().Competitor1, "", 0));
                breaker--;
            }
            Assert.AreEqual(8, st.Winners(2).Count);
            Assert.AreEqual(3, st.RoundNum());
        }
    }
}
