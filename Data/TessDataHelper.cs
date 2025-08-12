using System;
using System.IO;
using System.Windows.Forms;

public static class TessDataHelper
{
    public static void EnsureTessdataExists()
    {
        var tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
        if (!Directory.Exists(tessdataPath))
        {
            Directory.CreateDirectory(tessdataPath);
            MessageBox.Show($"\"tessdata\" klasörü oluşturuldu. İçine dil dosyalarını (eng.traineddata vb.) koymayı unutmayın!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
