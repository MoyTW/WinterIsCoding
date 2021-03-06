﻿using MechArena.Tournament;

using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.UI
{
    class Menu_CompetitorListing : IDisplay
    {
        private IDisplay parent;
        private ICompetitor player;
        private Schedule_Tournament tournament;

        private IntegerSelectionField selectionField = new IntegerSelectionField();

        public Menu_CompetitorListing(IDisplay parent, ICompetitor player, Schedule_Tournament tournament)
        {
            this.parent = parent;
            this.player = player;
            this.tournament = tournament;
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (keyPress != null)
            {
                var selection = this.selectionField.HandleKeyPress(keyPress, tournament.AllCompetitors());
                if (selection != null)
                    return new Menu_CompetitorDetails(this, this.player, this.tournament, selection);

                switch (keyPress.Key)
                {
                    case RLKey.Escape:
                        return this.parent;
                    default:
                        return this;
                }
            }
            else
            {
                return this;
            }
        }

        // http://stackoverflow.com/questions/1396048/c-sharp-elegant-way-of-partitioning-a-list
        private IEnumerable<List<T>> Partition<T>(IList<T> source, int size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }

        public void Blit(RLConsole console)
        {
            int tableWidth = 40;
            int currentX = 0;
            int lineStart = 9;
            int line = lineStart;

            console.SetBackColor(0, 0, console.Width, console.Height, RLColor.Black);
            console.Print(console.Width / 2 - 8, 1, "COMPETITOR MENU", RLColor.White);

            var byGroups = this.Partition(this.tournament.AllCompetitors(), 32);

            int g = 1;
            int i = 1;
            foreach (var group in byGroups)
            {

                console.Print(currentX+10, line++, "GROUP " + g++, RLColor.Green);
                line++;

                foreach (var c in group)
                {
                    var label = i + ") " + c.Label;
                    i++;

                    if (this.tournament.IsEliminated(c.CompetitorID))
                        console.Print(currentX+1, line, label, RLColor.Red);
                    else
                        console.Print(currentX+1, line, label, RLColor.White);

                    line++;
                }

                line += 3;
                if (line > console.Height - 40)
                {
                    line = lineStart;
                    currentX += tableWidth;
                }
            }

            console.Print(74, lineStart - 5, "############", RLColor.White);
            console.Print(74, lineStart - 4, "# Inspect  #", RLColor.White);
            console.Print(76, lineStart - 3, "# " + this.selectionField.SelectionString, RLColor.LightGreen);
            console.Print(74, lineStart - 3, "#", RLColor.White);
            console.Print(85, lineStart - 3, "#", RLColor.White);
            console.Print(74, lineStart - 2, "############", RLColor.White);
        }
    }
}
