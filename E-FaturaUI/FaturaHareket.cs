using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_FaturaUI
{
   public class FaturaHareket
    {
        public string StokKodu { get; set; }
        public string StokAdi { get; set; }
        public string Birim { get; set; }
        public int Miktar { get; set; }
        public decimal KdvOrani { get; set; }
        public decimal Fiyat { get; set; }
        public decimal Tutar { get; set; }
        public decimal Iskonto  { get; set; }
        public decimal IskonoOrani { get; set; }
        public decimal NetTutar { get; set; }
        public decimal Kdv { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}
