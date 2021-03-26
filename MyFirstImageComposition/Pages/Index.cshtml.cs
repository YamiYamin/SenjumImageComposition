using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstImageComposition.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public string TestImage { get; set; }

        public void OnGet()
        {
            // 兵オブジェクトが持っている情報
            int nameA = 1;
            int nameB = 1;
            string name = "京ノ介";
            int ch = 1;
            int ac = 1;
            string ss = "s1s2s3s4s5s7s8s9s10s11s12s13s14s15s16s17s18s19s20s21s22s33s32s90s";
            int mp = 0;
            int kp = 0;
            int pw = 0;
            int df = 0;
            int s = 4;


            // 元画像
            string baseImagePath = GetAbsolutePath(@"images\no_title.png");
            using Bitmap soldierImage = new(baseImagePath);

            using (Graphics g = Graphics.FromImage(soldierImage))
            {
                // 兵名
                g.DrawString(name, new Font("MS ゴシック", 11, FontStyle.Bold), Brushes.Moccasin, new Point(133, 21));

                // 兵種
                string chImagePath = GetAbsolutePath($@"images\characters\ch{ch}.png");
                using Bitmap chImage = new(chImagePath);
                g.DrawImage(chImage, 178, 45);

                // 技種
                string acImagePath = GetAbsolutePath($@"images\actions\ac{ac}.png");
                using Bitmap acImage = new(acImagePath);
                g.DrawImage(acImage, 297, 45);

                // 作戦・特殊能力
                DrawSpecialSkills(g, ss);
            }

            // 文字列に変換
            TestImage = ImageToString(soldierImage);
        }

        private string GetAbsolutePath(string path)
        {
            return Path.Combine(_hostEnvironment.WebRootPath, path);
        }

        // 作戦・特殊能力・向き・作戦を合成
        private void DrawSpecialSkills(Graphics g, string ss)
        {
            // 作戦・特殊能力
            for (int i = 1; i <= 34; i++)
            {
                // 未割り当て
                if (i is 6 or 23 or 24 or 30)
                {
                    continue;
                }
                // 向き
                if (i is >= 25 and <= 29)
                {
                    continue;
                }

                if (ss.Contains($"s{i}s"))
                {
                    string ssImagePath = GetAbsolutePath($@"images\skills\s{i}s.png");
                    using Bitmap ssImage = new(ssImagePath);
                    g.DrawImage(ssImage, CalcSpecialSkillPoint(i));
                }
            }

            // 成長
            for (int i = 90; i <= 99; i++)
            {
                if (ss.Contains($"s{i}s"))
                {
                    string ssImagePath = GetAbsolutePath($@"images\skills\s{i}s.png");
                    using Bitmap ssImage = new(ssImagePath);
                    g.DrawImage(ssImage, 289, 101);
                    break;
                }
            }
        }

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

        public Point CalcSpecialSkillPoint(int a)
        {

            if (a is 6 or 23 or 24 or 30)
            {
                return new();
            }

            int leftWidth = 28;
            int ssWidth = 50;
            int ssHeight = 156;

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

        public string ImageToString(Bitmap image)
        {
            ImageConverter converter = new();
            var imageArray = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return @"data:image/png;base64," + Convert.ToBase64String(imageArray);
        }
    }
}
