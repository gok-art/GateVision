using DevExpress.Data.ExpressionEditor;
using DevExpress.XtraEditors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GateVision.Forms
{
    public partial class FrmAnasayfa : DevExpress.XtraEditors.XtraForm
    {
        private PlateRecognitionEngine _engine;
        private PlateDatabase _db;
        private RelayController _relay;
        private SettingsDatabase _settingsDb = new SettingsDatabase();
        public FrmAnasayfa()
        {
            InitializeComponent();
        }
        private void LoadSavedCamera()
        {
            string camIndexStr = _settingsDb.GetSetting("CameraIndex");
            if (int.TryParse(camIndexStr, out int camIndex))
            {
                if (camIndex >= 0 && camIndex < comboBoxEditCameras.Properties.Items.Count)
                {
                    comboBoxEditCameras.SelectedIndex = camIndex;
                }
            }
        }
        private void LoadCameraDevices()
        {
            comboBoxEditCameras.Properties.Items.Clear();

            for (int i = 0; i < 10; i++)  // Maksimum 10 kamera deniyoruz
            {
                using (var capture = new VideoCapture(i))
                {
                    if (capture.IsOpened())
                    {
                        comboBoxEditCameras.Properties.Items.Add($"Kamera {i}");
                        capture.Release();
                    }
                }
            }

            if (comboBoxEditCameras.Properties.Items.Count > 0)
                comboBoxEditCameras.SelectedIndex = 0;
            else
                MessageBox.Show("Hiç kamera bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void FrmAnasayfa_Load(object sender, EventArgs e)
        {
            TessDataHelper.EnsureTessdataExists();
            LoadCameraDevices();
            LoadSavedCamera(); ;
            _db = new PlateDatabase();
            _relay = new RelayController("COM3");

            if (!_relay.Connect())
            {
                MessageBox.Show("Röle portuna bağlanılamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _engine = new PlateRecognitionEngine(0, _db, _relay, LogMessage, plate =>
            {
                Invoke(new Action(() =>
                {
                    memoEdit2.Text = plate; // formdaki bir Label kontrolü
                }));
            });
            _engine.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _engine.Stop();
            _relay.Dispose();
        }

        public void LogMessage(string message)
        {
            if (memoEdit1.InvokeRequired)
            {
                memoEdit1.Invoke(new Action(() =>
                {
                    memoEdit1.Text += $"{DateTime.Now:HH:mm:ss} - {message}\r\n";
                    memoEdit1.SelectionStart = memoEdit1.Text.Length;
                    memoEdit1.ScrollToCaret();
                }));
            }
            else
            {
                memoEdit1.Text += $"{DateTime.Now:HH:mm:ss} - {message}\r\n";
                memoEdit1.SelectionStart = memoEdit1.Text.Length;
                memoEdit1.ScrollToCaret();
            }

        }

        private void comboBoxEditCameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settingsDb.SaveSetting("CameraIndex", comboBoxEditCameras.SelectedIndex.ToString());
        }
    }
}