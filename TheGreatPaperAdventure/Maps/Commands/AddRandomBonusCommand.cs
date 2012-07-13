using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TGPA.Utils;

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Add a random bonuses on the screen
    /// </summary>
    public class AddRandomBonusCommand : Command
    {
        private Bonus bonus;

         public AddRandomBonusCommand(String commandLine)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid addrandombonus command : " + commandLine);

        }

        public AddRandomBonusCommand()
        {
             RandomWeapon();
            this.IsEnded = false;
        }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            bonus.Location = RandomMachine.GetRandomVector2(TGPAContext.Instance.ScreenWidth - 30, TGPAContext.Instance.ScreenWidth, 0, TGPAContext.Instance.ScreenWidth);
            TGPAContext.Instance.AddBonus(bonus);
            IsEnded = true;
        }

        public override string ToString()
        {
            return "addrandombonus " + bonus.Type.ToString();
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens2 = commandLine.Split(' ');

            if (tokens2.Length != 2) return false;

            if(tokens2[1].Equals("Weapon")) {
                RandomWeapon();
            }
            else {
                throw new NotImplementedException();
            }

            IsEnded = false;

            return true;
        }

        private void RandomWeapon() {
            bonus = new Bonus(Weapon.GetRandomWeapon().GetType().Name.ToString());
        }
    }
}

