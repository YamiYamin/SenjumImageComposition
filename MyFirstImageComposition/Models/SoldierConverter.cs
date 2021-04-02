using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstImageComposition.Models
{
    public class SoldierConverter
    {
        private readonly string _rootPath;

        public SoldierConverter(string rootPath)
        {
            _rootPath = rootPath;
        }

        // 兵の画像を生成し、文字列に変換して返す
        public string GenerateSoldierImage(Soldier soldier)
        {
            using Bitmap baseImage = GenerateBaseImage();

            // 部分ごとに画像を描画
            using (Graphics g = Graphics.FromImage(baseImage))
            {
                // 兵名
                DrawName(g, soldier.Name);

                // 禄高
                DrawStipend(g, soldier.Stipend);

                // ステ
                DrawStatus(g, soldier);

                // 兵種
                DrawCharacter(g, soldier.Ch);

                // 技種
                DrawAction(g, soldier.Ac);

                // 向き・作戦・特殊能力
                DrawSpecialSkills(g, soldier);
            }

            // 画像を文字列に変換して返す
            return ImageToString(baseImage);
        }

        // 兵の基本画像の生成
        private Bitmap GenerateBaseImage()
        {
            return GenerateImage(@"images\no_title.png");
        }

        // 画像の生成
        private Bitmap GenerateImage(string path)
        {
            string imagePath = GetAbsolutePath(path);
            return new(imagePath);
        }

        // ルートパスと結合して絶対パスにする
        private string GetAbsolutePath(string path)
        {
            return Path.Combine(_rootPath, path);
        }

        // 兵の名前を描画
        private static void DrawName(Graphics g, string name)
        {
            g.DrawString(name, new Font("MS ゴシック", 11, FontStyle.Bold), Brushes.Moccasin, new Point(133, 21));
        }

        // 禄高を下位桁から順に一桁ずつ描画
        private void DrawStipend(Graphics g, int stipend)
        {
            int work = stipend; // 禄高

            // expは指数
            for (int exp = 0; exp < 4; exp++)
            {
                // 描画する数字
                int num = work % 10;

                // 描画するx座標
                int x = 348 - (exp * 8);

                // 1～9は普通に描画
                if (num > 0)
                {
                    DrawStipendNum(g, (num * PowOf10(exp)).ToString(), x);
                }
                // 0は上位桁が存在する場合のみ描画
                else if (num == 0 && work != 0)
                {
                    DrawStipendNum(g, new string('0', exp + 1), x);
                }

                work /= 10;
            }
        }

        //何れかの位置に禄高用の数字を描画する
        private void DrawStipendNum(Graphics g, string imageName, int x)
        {
            using Bitmap stipendImage = GenerateImage($@"images\stipend\{imageName}.png");
            g.DrawImage(stipendImage, new Point(x, 22));
        }

        // 10の引数乗の整数を返す
        public static int PowOf10(int exp)
        {
            return (int)Math.Pow(10, exp);
        }

        // 兵種の描画
        private void DrawCharacter(Graphics g, int ch)
        {
            using Bitmap chImage = GenerateImage($@"images\characters\ch{ch}.png");
            g.DrawImage(chImage, 178, 45);
        }

        // 技種の描画
        private void DrawAction(Graphics g, int ac)
        {
            using Bitmap acImage = GenerateImage($@"images\actions\ac{ac}.png");

            // 炎上技種なら少し上に描画
            if (ac is 10 or 17 or 18 or 20 or 22 or >= 24 and <= 28)
            {
                g.DrawImage(acImage, 297, 38);
            }
            // 炎上以外なら枠に合うように描画
            else
            {
                g.DrawImage(acImage, 297, 45);
            }
        }

        // ステータスを描画する
        // TODO: ステータスの描画処理を実装する。
        private void DrawStatus(Graphics g, Soldier soldier)
        {
            //g.DrawImage();
        }

        // 作戦・特殊能力・向きを描画
        // TODO: 兵の肖像画を描画する。また、複数の向きを所持していた場合の処理を調べる。
        private void DrawSpecialSkills(Graphics g, Soldier soldier)
        {

            // デフォルト作戦を描画
            using Bitmap dfstImage = GenerateImage($@"images\skills\dfst{soldier.DefaultStrategy}.png");
            g.DrawImage(dfstImage, CalcSpecialSkillPoint(soldier.DefaultStrategy));

            // ssを切り離して配列にする
            string[] ssArray = soldier.SpecialSkills.Split("s", StringSplitOptions.RemoveEmptyEntries);

            bool isPortraitDrawn = false;

            foreach (string ss in ssArray)
            {
                int ssNum = int.Parse(ss);

                // 未割り当て
                if (ssNum is 6 or 23 or 24 or 30)
                {
                    continue;
                }
                // 向き
                else if (ssNum is >= 25 and <= 29)
                {
                    using Bitmap portrait = GenerateImage($@"images\portraits\{soldier.Ch}s{ssNum}s.png");
                    g.DrawImage(portrait, new Point(21, 19));
                    isPortraitDrawn = true;
                }
                // 成長
                else if (ssNum is >= 90 and <= 99)
                {
                    using Bitmap ssImage = GenerateImage($@"images\skills\s{ssNum}s.png");
                    g.DrawImage(ssImage, 289, 101);
                }
                // 作戦・特殊能力
                else
                {
                    if (ssNum == soldier.DefaultStrategy)
                    {
                        continue;
                    }
                    using Bitmap ssImage = GenerateImage($@"images\skills\s{ssNum}s.png");
                    g.DrawImage(ssImage, CalcSpecialSkillPoint(ssNum));
                }
            }

            // 兵の肖像画が描画されていなければ正面の画像を描画
            if (isPortraitDrawn is false)
            {
                using Bitmap portrait = GenerateImage($@"images\portraits\{soldier.Ch}s0s.png");
                g.DrawImage(portrait, new Point(21, 14));
            }
        }

        // 特殊能力とその座標の辞書
        private readonly Dictionary<int, (int, int)> Coordinates = new()
        {
            { 7, (28, 215) },
            { 8, (83, 215) },
            { 10, (137, 215) },
            { 21, (192, 215) },
            { 11, (246, 215) },
            { 19, (301, 215) },

            { 9, (28, 242) },
            { 13, (83, 242) },
            { 15, (137, 242) },
            { 20, (192, 242) },
            { 14, (246, 242) },
            { 16, (301, 242) },

            { 17, (28, 269) },
            { 18, (83, 269) },
            { 12, (137, 269) },
            { 22, (192, 269) },
        };

        // 作戦行動と特殊能力の描画位置を求める
        public Point CalcSpecialSkillPoint(int a)
        {
            if (a is 6 or 23 or 24 or 30)
            {
                return new();
            }

            const int leftWidth = 28;
            const int ssWidth = 50;
            const int ssHeight = 156;

            int[] gaps = { 0, 5, 9, 14, 18, 23 };

            int width = 0;
            int height = ssHeight;

            // 作戦行動
            if (a < 6)
            {
                int gap = gaps[a - 1];

                width = leftWidth + ssWidth * (a - 1) + gap;
            }
            // 特殊能力
            else if (a is > 6 and < 23)
            {
                Coordinates.TryGetValue(a, out (int x, int y) coord);
                width = coord.x;
                height = coord.y;
            }
            // 特殊能力(赤)
            else if (a is 31 or 33 or 34)
            {
                int gap = gaps[4];

                width = leftWidth + ssWidth * 4 + gap;
                height = 269;
            }
            else if (a is 32)
            {
                width = 301;
                height = 269;
            }

            return new(width, height);
        }

        // 引数の画像をData URIでbase64のテキストに変換して返す
        public static string ImageToString(Bitmap image)
        {
            ImageConverter converter = new();
            var imageArray = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return @"data:image/png;base64," + Convert.ToBase64String(imageArray);
        }
    }
}
