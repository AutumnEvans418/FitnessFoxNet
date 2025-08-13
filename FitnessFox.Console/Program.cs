// See https://aka.ms/new-console-template for more information
using FitnessFox.Components.Services;
using OpenCvSharp;
using System.Diagnostics;
using Tesseract;

Console.WriteLine("Hello, World!");


//var testImagePath = "C:\\Users\\autumn\\source\\repos\\FitnessFox\\FitnessFox.Tests\\data\\20250809_164658.jpg";
var testImagePath = @"C:\Users\autumn\source\repos\FitnessFox\FitnessFox.Tests\data\20250420_154553-1.jpg";

static byte[] Preprocess(string testImagePath)
{
    using var t = new ResourcesTracker();

    using var img = t.T(new Mat(testImagePath))
        .CvtColor(ColorConversionCodes.BGR2GRAY)
        .Threshold(128, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

    using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(25, 1));

    var dest = t.NewMat();
    Cv2.MorphologyEx(img, dest, MorphTypes.Open, kernel, iterations: 2);

    //Cv2.FindContours(dest, out var cnts, null, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

    //Mat cnts1;

    //if(cnts.Length == 2)
    //{
    //    cnts1 = cnts[0];
    //}
    //else
    //{
    //    cnts1 = cnts[1];
    //}

    //foreach (var item in cnts1.)
    //{
        
    //}

    //var dest = t.NewMat();

    //Cv2.Resize(img, dest, new Size() { Height = 100, Width = 100 });

    //Cv2.ConvertScaleAbs(img, dest, alpha:1.5);

    t.T(new Window("resize", dest));

    Cv2.WaitKey();

    return img.ToBytes();
}

var img = Preprocess(testImagePath);


try
{
    var options = new Dictionary<string, object>();

    using var engine = new TesseractEngine(
        @"./tessdata",
        "eng",
        EngineMode.Default,
        ["./tessdata/configs/config"],
        options, false);

    
    using var img1 = Pix.LoadFromMemory(img).Rotate90(1).Deskew();

    using var page = engine.Process(img1);
    var text = page.GetText();
    Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

    Console.WriteLine("Text (GetText): \r\n{0}", text.Replace("\n\n", "\n"));
}
catch (Exception e)
{
    Trace.TraceError(e.ToString());
    Console.WriteLine("Unexpected Error: " + e.Message);
    Console.WriteLine("Details: ");
    Console.WriteLine(e.ToString());
}
Console.ReadLine();