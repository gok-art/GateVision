using System;
using System.IO.Ports;
using System.Windows.Forms;  // MessageBox için

public class RelayController : IDisposable
{
    private SerialPort _port;
    private readonly string _portName;
    private readonly int _baudRate;

    public RelayController(string portName, int baudRate = 9600)
    {
        _portName = portName;
        _baudRate = baudRate;
    }

    public bool Connect()
    {
        try
        {
            if (_port == null)
            {
                _port = new SerialPort(_portName, _baudRate);
            }

            if (!_port.IsOpen)
            {
                _port.Open();
            }

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Röle bağlantı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public void TriggerRelay()
    {
        try
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Write("A"); // Röle komutu
            }
            else
            {
                MessageBox.Show("Röle portu açık değil.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Röle tetikleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void Close()
    {
        if (_port != null && _port.IsOpen)
        {
            _port.Close();
        }
    }

    public void Dispose()
    {
        Close();
        if (_port != null)
        {
            _port.Dispose();
            _port = null;
        }
    }
}
