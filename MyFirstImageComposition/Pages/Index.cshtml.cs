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
                Mp = 0,
                Kp = 0,
                Pw = 0,
                Df = 0,
                Spd = 4,
                DefaultStrategy = 1,
                SpecialSkills = "s1s2s3s4s5s7s8s9s10s11s12s13s14s15s16s17s18s19s20s21s22s33s32s90s",
            };

            SoldierConverter soldierConverter = new(_hostEnvironment.WebRootPath);

            SoldierImage = soldierConverter.GenerateSoldierImage(soldier);
        }
    }
}
