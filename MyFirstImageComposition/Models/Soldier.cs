using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstImageComposition.Models
{
    public class Soldier
    {
        public int SoldierId { get; set; }         // 兵ID 
        public string SoldierName { get; set; }
        public int ChID { get; set; }
        public int AcID { get; set; }
        public int Stipends { get; set; }
        public int Mp { get; set; }                // 体力
        public int Kp { get; set; }                // 技量
        public int Pw { get; set; }                // 戦闘
        public int Df { get; set; }                // 防御
        public int Spd { get; set; }               // 脚力
        public string SpecialSkills { get; set; }  // 特殊能力
        public int DefaultStrategy { get; set; } // デフォルトの作戦行動
    }
}
