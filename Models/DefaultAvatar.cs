using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace Dajeej.Models
{
    public class DefaultAvatar
    {
        private readonly IWebHostEnvironment _host;
        private List<string> _BackgroundColours = new List<string> { "339966", "3366CC", "CC33FF", "FF5050" };

        public DefaultAvatar(IWebHostEnvironment host)
        {
            _host = host;
        }

        // generate with change text color and fixed bg color
        private string GenerateAvtarImage(String text, Font font, Color textColor, Color backColor, string filename)
        {
            var avatarString = text;

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);



            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.Clear(Color.White);


            graphics.DrawString(avatarString, font, new SolidBrush(backColor), 95, 100, sf);
            graphics.Flush();



            filename = Guid.NewGuid() + "_" + "defaultPicture535" + filename + ".gif";

            string pathOfAllPictures = Path.Combine(_host.WebRootPath, "images", filename);

            var ms = new MemoryStream();
            bmp.Save(pathOfAllPictures);

            return filename;

        }

        // generate with change bg color and fixed text color

        private string GenerateAvtarImageWithChangeBackground(String text, Font font, Color textColor, Color backColor, string filename)
        {
            var avatarString = text;

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.Clear(backColor);


            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), 95, 100, sf);
            graphics.Flush();



            filename = Guid.NewGuid() + "_" + "defaultPicture535" + filename + ".gif";

            string pathOfAllPictures = Path.Combine(_host.WebRootPath, "images", filename);

            var ms = new MemoryStream();
            bmp.Save(pathOfAllPictures);

            return filename;

        }

        // generate circle 
        private string GenerateCircleAvtarImage(String text, Font font, Color textColor, Color backColor, string filename)
        {
            var avatarString = text;

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using (Brush b = new SolidBrush(ColorTranslator.FromHtml("#" + bgColour)))
            {
                graphics.FillEllipse(b, new Rectangle(0, 0, 192, 192));
            }
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), 95, 100, sf);
            graphics.Flush();



            filename = Guid.NewGuid() + "_" + "defaultPicture535" + filename + ".gif";

            string pathOfAllPictures = Path.Combine(_host.WebRootPath, "images", filename);

            var ms = new MemoryStream();
            bmp.Save(pathOfAllPictures);

            return filename;

        }


        public MemoryStream GenerateCircle(string firstName, string lastName)
        {
            var avatarString = string.Format("{0}{1}", firstName[0], lastName[0]).ToUpper();

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using (Brush b = new SolidBrush(ColorTranslator.FromHtml("#" + bgColour)))
            {
                graphics.FillEllipse(b, new Rectangle(0, 0, 192, 192));
            }
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), 95, 100, sf);
            graphics.Flush();

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms;
        }

        public string CreateProfilePicture(string avatarName)
        {
            var avatarNewName = "";

            avatarName = avatarName.Trim();

            var avatarNameChainArray = avatarName.Split(" ");

            var countOfWordsInAvatarName = avatarNameChainArray.Length;

            // If The Name Contains From One Word
            if (countOfWordsInAvatarName == 1)
            {
                if (avatarNameChainArray[0].Length == 1) //If Contains From One Character
                {
                    avatarNewName = avatarName.Substring(0, 1);
                }
                else // If Contains From More Than One Character Will Take The First Two Character
                {
                    avatarNewName = avatarName.Substring(0, 1) + avatarName.Substring(1, 1);
                }
            }
            else if (countOfWordsInAvatarName > 1)
            {
                avatarNewName = avatarNameChainArray[0].Substring(0, 1) + avatarNameChainArray[1].Substring(0, 1);
            }

            Font font = new Font(FontFamily.GenericSansSerif, 45, FontStyle.Bold);


            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            Color fontcolor = ColorTranslator.FromHtml("#FFF");
            Color bgcolor = ColorTranslator.FromHtml("#" + bgColour);

            return GenerateAvtarImage(avatarNewName.ToUpper(), font, fontcolor, bgcolor, avatarName);
        }
    }
}
