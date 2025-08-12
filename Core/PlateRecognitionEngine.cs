using OpenCvSharp;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

public class PlateRecognitionEngine
{
    private VideoCapture _camera;
    private PlateOCR _ocr;
    private PlateDatabase _db;
    private RelayController _relay;
    private CancellationTokenSource _cts;
    private Action<string> _logCallback;
    private Action<string> _plateCallback = null;

    public PlateRecognitionEngine(int cameraIndex, PlateDatabase db, RelayController relay, Action<string> logCallback, Action<string> plateCallback)
    {
        _camera = new VideoCapture(cameraIndex);
        _ocr = new PlateOCR();
        _db = db;
        _relay = relay;
        _logCallback = logCallback;
    }
    bool IsTurkishPlate(string plate)
    {
        return Regex.IsMatch(plate.ToUpperInvariant(), @"^[0-9]{2}[A-Z]{1,3}[0-9]{2,4}$");
    }
    public void Start()
    {
        _cts = new CancellationTokenSource();

        Task.Run(() =>
        {
            using (var frame = new Mat())
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (!_camera.Read(frame) || frame.Empty())
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    var plate = _ocr.Recognize(frame);
                    if (!string.IsNullOrEmpty(plate))
                    {
                        plate = plate.ToUpperInvariant();
                        _plateCallback?.Invoke(plate); // UI'a plaka göster

                        _logCallback?.Invoke($"Tespit Edilen Plaka: {plate}");

                        if (IsTurkishPlate(plate))
                        {
                            if (_db.IsPlateAuthorized(plate))
                            {
                                _logCallback?.Invoke($"Plaka beyaz listede: {plate}. Röle tetikleniyor.");
                                _relay.TriggerRelay();
                            }
                            else
                            {
                                _logCallback?.Invoke($"Plaka beyaz listede değil: {plate}");
                            }
                        }
                        else
                        {
                            _logCallback?.Invoke($"Geçersiz plaka formatı: {plate}");
                        }
                    }

                    Thread.Sleep(500);
                }
            }
        });
    }

    public void Stop()
    {
        _cts?.Cancel();
        _camera?.Release();
    }
}
