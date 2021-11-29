using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstImageComposition.Models
{
    public interface ISoldier
    {
        public string SoldierName { get; }           // 兵名
        public int Stipends { get; set; }           // 禄高
        public int ChID { get; set; }                // 兵種
        public int AcID { get; set; }                // 技種
        public int Mp { get; set; }                // 体力
        public int Kp { get; set; }                // 技量
        public int Pw { get; set; }                // 戦闘
        public int Df { get; set; }                // 防御
        public int Spd { get; set; }               // 脚力
        public string SpecialSkills { get; set; }  // 特殊能力
        public int DefaultStrategy { get; set; } // デフォルトの作戦行動
    }
}
