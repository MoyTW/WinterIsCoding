﻿using MechArena.Tournament;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests.Tournament
{
    [TestClass]
    public class Schedule_TournamentTest
    {
        // TODO: Util fn
        private List<Competitor> BuildComps(int numComps)
        {
            List<Competitor> comps = new List<Competitor>();
            for (int i = 0; i < numComps; i++)
            {
                comps.Add(new Competitor(i.ToString(), i.ToString()));
            }
            return comps;
        }

        [TestMethod]
        public void TestFirstStage()
        {
            var comps = this.BuildComps(256);

            var st = new Schedule_Tournament(comps);
            Assert.AreEqual(1, st.RoundNum());
            Assert.AreEqual(896, st.ScheduledMatches().Count);

            foreach (var m in st.ScheduledMatches())
            {
                st.ReportResult(new MatchResult(m, m.Competitor1));
            }

            Assert.AreEqual(32, st.Winners(1).Count);
            Assert.AreEqual(2, st.RoundNum());
        }

        [TestMethod]
        public void TestSecondStage()
        {
            var comps = this.BuildComps(256);

            var st = new Schedule_Tournament(comps);
            foreach (var m in st.ScheduledMatches())
            {
                st.ReportResult(new MatchResult(m, m.Competitor1));
            }

            Assert.AreEqual(112, st.ScheduledMatches().Count);
            int breaker = 9999;
            while (st.RoundNum() == 2 && st.NextMatch() != null && breaker > 0)
            {
                st.ReportResult(new MatchResult(st.NextMatch(), st.NextMatch().Competitor1));
                breaker--;
            }
            Assert.AreEqual(8, st.Winners(2).Count);
            Assert.AreEqual(3, st.RoundNum());
        }
    }
}