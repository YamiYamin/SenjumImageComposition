using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Senjum
{
    public class SoldierConverter
    {
        public SoldierConverter()
        {

        }

        // 兵の画像を生成し、文字列に変換して返す
        public string ConvertToBase64(ISoldier soldier)
        {
            using Bitmap soldierImage = ConvertToImage(soldier);
            return ImageToString(soldierImage);
        }

        // 引数の画像をData URIでbase64のテキストに変換して返す
        public static string ImageToString(Bitmap image)
        {
            ImageConverter converter = new();
            var imageArray = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return @"data:image/png;base64," + Convert.ToBase64String(imageArray);
        }

        private Graphics g;
        private ISoldier soldier;

        // 兵の画像を生成し、文字列に変換して返す
        public Bitmap ConvertToImage(ISoldier soldier)
        {
            Bitmap baseImage = GenerateBaseImage();
            this.soldier = soldier;

            // 部分ごとに画像を描画
            using (g = Graphics.FromImage(baseImage))
            {
                // 兵名
                DrawName();

                // 禄高
                DrawStipend();

                // ステ
                DrawAllStatuses();

                // 兵種
                DrawCharacter();

                // 技種
                DrawAction();

                // 向き・作戦・特殊能力
                DrawSpecialSkills();
            }
            
            // 画像を文字列に変換して返す
            return baseImage;
        }

        // 兵の基本画像の生成
        private static Bitmap GenerateBaseImage()
        {
            return GenerateImage("no_title");
        }

        // 画像の生成
        private static Bitmap GenerateImage(string name)
        {
            return (Bitmap)Properties.Resources.ResourceManager.GetObject(name);
        }

        // 画像の生成
        public static Bitmap GenerateImage(string rootPath, string path)
        {
            return new(GetAbsolutePath(rootPath, path));
        }

        // ルートパスと結合して絶対パスにする
        private static string GetAbsolutePath(string rootPath, string path)
        {
            return Path.Combine(rootPath, path);
        }

        // 兵の名前を描画
        private void DrawName()
        {
            g.DrawString(soldier.Name, new Font("MS ゴシック", 11, FontStyle.Bold), Brushes.Moccasin, new Point(133, 21));
        }

        // 禄高を下位桁から順に一桁ずつ描画
        private void DrawStipend()
        {
            int work = soldier.Stipend; // 禄高

            // expは指数
            for (int exp = 0; exp < 4; exp++)
            {
                // 描画する数字
                int num = work % 10;


                // 1～9は普通に描画
                if (num > 0)
                {
                    // 背景が微妙に違うため桁ごとに画像を用意している
                    string imageName = $"_{num * Pow10(exp)}";
                    DrawStipendNum(imageName, exp);
                }
                // 0は上位桁が存在する場合のみ描画
                else if (num == 0 && work != 0)
                {
                    string imageName = $"_{new string('0', exp + 1)}";
                    DrawStipendNum(imageName, exp);
                }

                work /= 10;
            }
        }

        // いずれかの位置に禄高用の数字を描画する
        private void DrawStipendNum(string imageName, int exp)
        {
            // 描画するx座標
            int x = 348 - (exp * 8);

            using Bitmap stipendImage = GenerateImage(imageName);
            g.DrawImage(stipendImage, new Point(x, 22));
        }

        // 10の引数乗の整数を返す
        public static int Pow10(int exp)
        {
            return (int)Math.Pow(10, exp);
        }

        // 兵種の描画
        private void DrawCharacter()
        {
            using Bitmap chImage = GenerateImage($"ch{soldier.Ch}");
            g.DrawImage(chImage, 177, 45);
        }

        // 技種の描画
        private void DrawAction()
        {
            using Bitmap acImage = GenerateImage($"ac{soldier.Ac}");

            // 炎上技種なら少し上に描画
            switch (soldier.Ac)
            {
                case 10 or 17 or 18 or 20 or 22:
                case >= 24 and <= 28:
                    g.DrawImage(acImage, 297, 38);
                    break;
                default:
                    g.DrawImage(acImage, 297, 45);
                    break;
            }
        }

        // 全てのステータスを描画
        // TODO: ステータスの描画処理を実装する。
        private void DrawAllStatuses()
        {
            DrawStatus("mp", soldier.Mp);
            DrawStatus("kp", soldier.Kp);
            DrawStatus("pw", soldier.Pw);
            DrawStatus("df", soldier.Df);
            DrawSpd();
        }

        // Spdを描画
        private void DrawSpd()
        {
            using Bitmap test = GenerateImage($"spd_{soldier.Spd}");
            g.DrawImage(test, new Point(290, 76));
        }

        // pointの位置にステータスを描画
        private void DrawStatus(string status, int value)
        {
            // ステータスの背景色と描画位置を決定する
            string color = value switch
            {
                >= 111 => "fire",
                >= 90 => "red",
                >= 80 => "orange",
                >= 70 => "yellow",
                _ => "default",
            };

            var point = status switch
            {
                "mp" => new Point(134, 76),
                "kp" => new Point(212, 76),
                "pw" => new Point(134, 101),
                "df" => new Point(212, 101),
                _ => new Point()
            };

            // 各ステータスごとの背景を描画
            using Bitmap test = GenerateImage($"{status}_{color}");
            g.DrawImage(test, point);

            // 描画した画像の幅進む
            point.X += test.Width;

            int work = value;
            int place = 100;

            while (work > 0)
            {
                // 描画する数字
                int num = work / place;

                string imagePath = "";

                // デフォルトカラーならステータスの種類を、それ以外ならその色
                if (color is "default")
                {
                    imagePath += status;
                }
                else
                {
                    imagePath += color;
                }

                if (num == 0)
                {
                    // 上位の桁が存在しない場合
                    if (value / (place * 10) == 0)
                    {
                        imagePath += $"_space_{place}";
                    }
                    else
                    {
                        // 桁数分のゼロを追加
                        imagePath += $"_{new string('0', CalcDigit(place))}";
                    }
                }
                // ステータスが70以上なら色付きのステータスを描画
                else
                {
                    imagePath += $"_{num * place}";
                }

                using Bitmap a = GenerateImage(imagePath);
                g.DrawImage(a, point);

                point.X += 8;

                work -= work / place * place;
                place /= 10;
            }
        }

        // 桁数の算出
        public static int CalcDigit(int num)
        {
            return num == 0 ? 1 : (int)Math.Log10(num) + 1;
        }

        // 作戦・特殊能力・向きを描画
        private void DrawSpecialSkills()
        {
            // デフォルト作戦を描画
            using Bitmap dfstImage = GenerateImage($"dfst{soldier.DefaultStrategy}");
            g.DrawImage(dfstImage, CalcSpecialSkillPoint(soldier.DefaultStrategy));

            // ssを切り離して配列にする
            string[] ssArray = soldier.SpecialSkills.Split("s", StringSplitOptions.RemoveEmptyEntries);

            int portraitNum = 0;

            foreach (string ss in ssArray)
            {
                int ssNum = int.Parse(ss);

                // 未割り当て
                if (ssNum is 6 or 23 or 24 or 30)
                {
                    continue;
                }

                // 向き
                // 複数所持している場合、数字(25~29)が大きい方が優先される
                if (ssNum is >= 25 and <= 29)
                {
                    portraitNum = ssNum;
                }
                // 成長
                else if (ssNum is >= 90 and <= 99)
                {
                    using Bitmap ssImage = GenerateImage($"s{ssNum}s");
                    g.DrawImage(ssImage, 289, 101);
                }
                // 作戦・特殊能力
                else if (ssNum != soldier.DefaultStrategy)
                {
                    using Bitmap ssImage = GenerateImage($"s{ssNum}s");
                    g.DrawImage(ssImage, CalcSpecialSkillPoint(ssNum));
                }
            }

            // 兵の肖像画を描画
            using Bitmap portrait = GenerateImage($"_{soldier.Ch}s{portraitNum}s");
            g.DrawImage(portrait, new Point(21, 19));
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
    }
}
