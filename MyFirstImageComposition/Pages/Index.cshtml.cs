﻿using Microsoft.AspNetCore.Hosting;
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
using MyFirstImageComposition.Models;

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

        public string SoldierImage { get; set; }

        public void OnGet()
        {
            Soldier soldier = new()
            {
                Name = "京之介",
                Stipend = 3136,
                Ch = 3,
                Ac = 28,
                Mp = 77,
                Kp = 106,
                Pw = 87,
                Df = 68,
                Spd = 4,
                DefaultStrategy = 2,
                SpecialSkills = "s1s2s3s4s5s7s8s9s10s11s12s13s14s15s16s17s18s19s20s21s22s91s",
            };

            // 
            SoldierConverter converter = new(_hostEnvironment.WebRootPath);
            SoldierImage = converter.ConvertToBase64Image(soldier);


            using Bitmap mp98 = converter.GenerateImage($@"images\test\mp98.png");
            using Bitmap pw98 = converter.GenerateImage($@"images\test\pw98.png");

            string mp98str = SoldierConverter.ImageToString(mp98);
            string pw98str = SoldierConverter.ImageToString(pw98);

            Debug.WriteLine("mp98str" + mp98str);
            Debug.WriteLine("pw98str" + pw98str);

            bool test = mp98str.Equals(pw98str, StringComparison.Ordinal);

            Debug.WriteLine("比較結果: " + test);
            string path = _hostEnvironment.WebRootPath + @"\images\test\";
            string file1 = path + "mp98.png";
            string file2 = path + "pw98.png";

            string outPath = path + "result.png";
            Debug.WriteLine(file1);

            bool test2 = Compare(file1, file2, outPath);
            Debug.WriteLine("比較結果: " + test2);
        }

        public static bool Compare(string bmp1Path, string bmp2Path, string path)
        {
            bool isSame = true;

            // 画像を比較する際に「大きい方の画像」のサイズに合わせて比較する。
            Bitmap bmp1 = new(bmp1Path);
            Bitmap bmp2 = new(bmp2Path);
            int width = Math.Max(bmp1.Width, bmp2.Width);
            int height = Math.Max(bmp1.Height, bmp2.Height);

            Bitmap diffBmp = new(width, height);         // 返却する差分の画像。
            Color diffColor = Color.Red;                        // 画像の差分に付ける色。

            // 全ピクセルを総当りで比較し、違う部分があればfalseを返す。
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    try
                    {
                        Color color1 = bmp1.GetPixel(i, j);
                        if (color1 == bmp2.GetPixel(i, j))
                        {
                            diffBmp.SetPixel(i, j, color1);
                        }
                        else
                        {
                            diffBmp.SetPixel(i, j, diffColor);
                            isSame = false;
                        }
                    }
                    catch
                    {
                        // 画像のサイズが違う時は、ピクセルを取得できずにエラーとなるが、ここでは「差分」として扱う。
                        diffBmp.SetPixel(i, j, diffColor);
                        isSame = false;
                    }
                }
            }
            diffBmp.Save(path, ImageFormat.Png);
            return isSame;
        }
    }
}
