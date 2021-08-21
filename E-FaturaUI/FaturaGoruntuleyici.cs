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

namespace E_FaturaUI
{
    public partial class FaturaGoruntuleyici : DevExpress.XtraEditors.XtraForm
    {
        public FaturaGoruntuleyici(string documentText)
        {
            InitializeComponent();
            webBrowser1.DocumentText = documentText;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //yazdır
            webBrowser1.ShowPrintDialog();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //baskıönizleme
            webBrowser1.ShowPrintPreviewDialog();
            
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }
    }
}