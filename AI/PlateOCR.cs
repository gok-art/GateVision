using System;
using OpenCvSharp;
using Tesseract;
using System.Drawing;
using OpenCvSharp.Extensions;

public class PlateOCR
{
    private readonly TesseractEngine _engine;

    public PlateOCR()
    {
        _engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        _engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
    }

    public string Recognize(Mat mat)
    {
        try
        {
            using (Bitmap bitmap = BitmapConverter.ToBitmap(mat))
            using (Pix pix = PixConverter.ToPix(bitmap))
            using (Page page = _engine.Process(pix))
            {
                return page.GetText().ToUpper().Replace(" ", "").Trim();
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}
