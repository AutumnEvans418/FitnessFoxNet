using IronOcr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.Services
{
    public class OcrService
    {
        public async Task<string> ParseText(string path)
        {
            var ocr = new IronTesseract();

            using var ocrInput = new OcrInput();
            ocrInput.LoadImage(path);
            // Optionally Apply Filters if needed:
            // ocrInput.Deskew();  // use only if image not straight
            // ocrInput.DeNoise(); // use only if image contains digital noise


            var ocrResult = await ocr.ReadAsync(ocrInput);

            return ocrResult.Text;
        }
    }
}
