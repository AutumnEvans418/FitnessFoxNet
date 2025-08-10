// See https://aka.ms/new-console-template for more information
using FitnessFox.Components.Services;
using System.Diagnostics;
using Tesseract;

Console.WriteLine("Hello, World!");


//var testImagePath = "C:\\Users\\autumn\\source\\repos\\FitnessFox\\FitnessFox.Tests\\data\\20250809_164658.jpg";
var testImagePath = @"C:\Users\autumn\source\repos\FitnessFox\FitnessFox.Tests\data\20250420_154553.jpg";

try
{
    var options = new Dictionary<string, object>();
    //{
    //    {"tessedit_write_images", true },
    //    {"load_system_dawg", false }
    //};

    using var engine = new TesseractEngine(
        @"./tessdata", 
        "eng", 
        EngineMode.Default, 
        ["./tessdata/configs/config"], 
        options, false);

    using var img2 = Pix.LoadFromFile(testImagePath);
    using var img1 = img2.Rotate90(1);
    using var img = img1.Deskew(); 
    
    using var page = engine.Process(img);
    var text = page.GetText();
    Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

    Console.WriteLine("Text (GetText): \r\n{0}", text);
    //Console.WriteLine("Text (iterator):");
    //using var iter = page.GetIterator();
    //iter.Begin();

    //do
    //{
    //    do
    //    {
    //        do
    //        {
    //            do
    //            {
    //                if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
    //                {
    //                    Console.WriteLine("<BLOCK>");
    //                }

    //                Console.Write(iter.GetText(PageIteratorLevel.Word));
    //                Console.Write(" ");

    //                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
    //                {
    //                    Console.WriteLine();
    //                }
    //            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

    //            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
    //            {
    //                Console.WriteLine();
    //            }
    //        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
    //    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
    //} while (iter.Next(PageIteratorLevel.Block));
}
catch (Exception e)
{
    Trace.TraceError(e.ToString());
    Console.WriteLine("Unexpected Error: " + e.Message);
    Console.WriteLine("Details: ");
    Console.WriteLine(e.ToString());
}
