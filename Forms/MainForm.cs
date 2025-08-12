using DevExpress.Data.ExpressionEditor;
using DevExpress.XtraEditors;
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
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public void OpenChildForm<T>() where T : Form, new()
        {
            // Aktif MDI alt formu al
            Form activeForm = this.ActiveMdiChild;

            // Eğer aktif form istenen form ise, yeni açmaya gerek yok
            if (activeForm != null && activeForm.GetType() == typeof(T))
                return;

            // MDI içinde açık formları kontrol et
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == typeof(T))
                {
                    f.Activate(); // Açık olan formu aktif yap
                    return;
                }
            }

            // Form açık değilse yeni oluştur ve aç
            T form = new T();
            form.MdiParent = this;
            form.Show();
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAnasayfa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenChildForm<FrmAnasayfa>();
        }
    }
}