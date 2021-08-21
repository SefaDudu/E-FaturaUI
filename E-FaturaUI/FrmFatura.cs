using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using E_FaturaUI.tr.com.edmbilisim.test;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace E_FaturaUI
{
    public partial class FrmFatura : DevExpress.XtraEditors.XtraForm
    {
        public FrmFatura()
        {
            InitializeComponent();
            DataSource();
            Toplamlar();
        }
        private void DataSource()
        {
            texFaturaNo.Text = "ABC2009123456789";
            txtCariKodu.Text = "120 10 01 002";
            textCariAdi.Text = "Hasan Yılmaz";
            dateTarih.EditValue=DateTime.Now;
            textSube.Text = "001 Merkez Şube";
            faturaTablo.DataSource = new BindingList<FaturaHareket>
            {
                new FaturaHareket
                {
                    StokKodu="STK 0001",
                    StokAdi = "Masa" ,
                    Birim = "Adet",
                    Miktar=12,
                    KdvOrani = 18,
                    Fiyat = 55 ,
                    Tutar = 256,
                    Iskonto = 66 ,
                    NetTutar = 596,
                    Kdv=(decimal)106.92,
                    ToplamTutar = (decimal)700.92
                },new FaturaHareket
                {
                    StokKodu="STK 0002",
                    StokAdi = "sandalye" ,
                    Birim = "Adet",
                    Miktar=12,
                    KdvOrani = 18,
                    Fiyat = 55 ,
                    Tutar = 256,
                    Iskonto = 66 ,
                    NetTutar = 596,
                    Kdv=(decimal)106.92,
                    ToplamTutar = (decimal)700.92
                },new FaturaHareket
                {
                    StokKodu="STK 0003",
                    StokAdi = "Telefon" ,
                    Birim = "Adet",
                    Miktar=12,
                    KdvOrani = 18,
                    Fiyat = 55 ,
                    Tutar = 256,
                    Iskonto = 66 ,
                    NetTutar = 596,
                    Kdv=(decimal)106.92,
                    ToplamTutar = (decimal)700.92
                }
            };
        }
        private void Toplamlar()
        {
            txtTutar.Text = gridView1.DataController.ListSource.Cast<FaturaHareket>().Sum(x => x.Tutar).ToString();
            txtIskonto.Text = gridView1.DataController.ListSource.Cast<FaturaHareket>().Sum(x => x.Iskonto).ToString();
            txtNetTutar.Text = gridView1.DataController.ListSource.Cast<FaturaHareket>().Sum(x => x.NetTutar).ToString();
            TxtKDV.Text = gridView1.DataController.ListSource.Cast<FaturaHareket>().Sum(x => x.Kdv).ToString();
            txtToplamTutar.Text = txtNetTutar.Text + TxtKDV.Text;
            

        }
        private void FaturaOlustur(Fatura faturaBilgi )
        {

            InvoiceLineType[] FaturaHareketleri()
            {
                var lines = new List<InvoiceLineType>();
                var source = gridView1.DataController.ListSource.Cast<FaturaHareket>();
                var lineNumber = 1;
                source.ForEach(action: x =>
                {
                    var line = new InvoiceLineType
                    {
                        ID = new IDType { Value = lineNumber.ToString() },
                        InvoicedQuantity = new InvoicedQuantityType { unitCode = "C62", Value = x.Miktar },
                        LineExtensionAmount = new LineExtensionAmountType { currencyID = "TRY", Value = x.NetTutar },
                        //AllowanceCharge = new[] {new AllowanceChargeType
                        //{
                        //    ChargeIndicator = new ChargeIndicatorType{Value=false},
                        //    MultiplierFactorNumeric = new MultiplierFactorNumericType{Value =x.IskonoOrani},
                        //    Amount= new AmountType2{Value = x.Iskonto},
                        //    BaseAmount = new BaseAmountType{ currencyID = " TRY",Value = x.Tutar}
                        //}

                        //}, //,
                        TaxTotal = new TaxTotalType
                        {
                            TaxAmount = new TaxAmountType { currencyID = "TRY", Value = x.Kdv },
                            TaxSubtotal = new[] { new TaxSubtotalType {
                                TaxableAmount = new TaxableAmountType{currencyID = "TRY" , Value=x.NetTutar},
                                TaxAmount = new TaxAmountType {currencyID = "TRY", Value=x.Kdv},
                                CalculationSequenceNumeric = new CalculationSequenceNumericType {Value=1 },
                                Percent = new PercentType1 {Value = x.KdvOrani},
                                TaxCategory = new TaxCategoryType
                                {
                                    TaxExemptionReason = new TaxExemptionReasonType
                                    {
                                        Value="undefined",
                                    },
                                    TaxScheme = new TaxSchemeType{TaxTypeCode =new TaxTypeCodeType{ Value = "0015"} , Name =new NameType1{ Value="KDV"} }
                                }
                            } }//
                        },//
                        Item = new ItemType
                        {
                            Name = new NameType1 { Value = x.StokAdi + " " + x.StokKodu },

                        },//
                        Price = new PriceType
                        {
                            PriceAmount = new PriceAmountType { currencyID = "TRY", Value = x.Fiyat }
                        }//
                    };
                    lineNumber++;
                    lines.Add(line);
                });
                return lines.ToArray();
            }

            var invoice = new InvoiceType
            {
                UBLVersionID = new UBLVersionIDType { Value = "2.1" }
                ,
                CustomizationID = new CustomizationIDType { Value = "TR1.2" }
                ,
                ProfileID = new ProfileIDType { Value = "EFATURA" }
                ,
                ID = new IDType { Value = faturaBilgi.FaturaNo },
                CopyIndicator = new CopyIndicatorType { Value = false },
                UUID = new UUIDType { Value = Guid.NewGuid().ToString() },
                IssueDate = new IssueDateType { Value = faturaBilgi.Tarih },
                IssueTime = new IssueTimeType { Value = faturaBilgi.Tarih },
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = "SATIS" },
                Note = new[]
                {
                    new NoteType
                    { Value = "Yazı ile: # Yüz Kırk Yedi  TL Elli KRŞ #" }


                },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = "TRY" },
                LineCountNumeric = new LineCountNumericType { Value = 1 },
                OrderReference = new OrderReferenceType
                {
                    ID = new IDType { Value = faturaBilgi.FaturaNo.ToString() },
                    IssueDate = new IssueDateType { Value = faturaBilgi.Tarih }
                },
                DespatchDocumentReference = new[]
                 {
                    new DocumentReferenceType
                    {
                        ID = new IDType { Value = faturaBilgi.FaturaNo.ToString() },
                        IssueDate = new IssueDateType { Value = faturaBilgi.Tarih }
                    }
                },


                AccountingSupplierParty = new SupplierPartyType
                {
                    Party = new PartyType
                    {
                        PartyIdentification = new[] { new PartyIdentificationType { ID = new IDType { schemeID = "VKN", Value = "3230512384" } },
                            new PartyIdentificationType { ID = new IDType { schemeID = "MERSISNO" }
                            },
                            new PartyIdentificationType { ID = new IDType { schemeID = "HIZMETNO" } },
                            new PartyIdentificationType { ID = new IDType { schemeID = "TICARETSICILNO" } } }
                ,
                        PartyName = new PartyNameType { Name = new NameType1 { Value = "EDM Bilişim Hizmetleri A.Ş." } },
                        PostalAddress = new AddressType
                        {
                            BuildingName = new BuildingNameType { Value = "Adres" },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = "Maltepe" },
                            CityName = new CityNameType { Value = "İstanbul" },
                            PostalZone = new PostalZoneType { Value = "06508" },
                            Country = new CountryType {
                                IdentificationCode = new IdentificationCodeType {
                                    Value="TR"

                                },
                                Name = new NameType1
                                {

                                    Value = "Türkiye"
                                },
                            }
                        },
                        PartyTaxScheme = new PartyTaxSchemeType
                        {
                            TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = "Kadıköy VD" } }
                        }
                    }

                }
                ,
                AccountingCustomerParty = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        PartyIdentification = new[]
                        {
                            new PartyIdentificationType { ID = new IDType { schemeID = "VKN", Value = "3230512384" } }
                        },
                        PartyName = new PartyNameType { Name = new NameType1 { Value = "Sefa Dudu Market" } },
                        PostalAddress = new AddressType
                        {
                            BuildingName = new BuildingNameType { Value = "Sefa Dudu A.Ş" },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = "Maltepe" },
                            CityName = new CityNameType { Value = "Istanbul" },
                            Country = new CountryType {
                                IdentificationCode = new IdentificationCodeType
                                {
                                    Value = "TR"
                                },
                                Name = new NameType1 { Value = "Türkiye" } }
                        },
                        PartyTaxScheme = new PartyTaxSchemeType
                        {
                            TaxScheme = new TaxSchemeType
                            {
                                Name = new NameType1
                                {
                                    Value = "Kurum Adı"
                                }
                            }
                        }

                    }
                },
                PaymentTerms = new PaymentTermsType
                {
                                     PaymentDueDate = new PaymentDueDateType
                    {
                        Value = DateTime.Now
                    }
                },
                PricingExchangeRate = new ExchangeRateType
                {
                    SourceCurrencyCode = new SourceCurrencyCodeType { Value="TRY"},
                    TargetCurrencyCode= new TargetCurrencyCodeType { Value="TRY"},
                    CalculationRate=new CalculationRateType { Value=1}
                },
                TaxTotal = new[]
                {
                   new TaxTotalType{TaxAmount= new TaxAmountType{currencyID = "TRY",  Value=faturaBilgi.KdvTutari},

                       TaxSubtotal = new[]
                   {
                        new TaxSubtotalType
                        {
                            TaxableAmount = new TaxableAmountType
                            {
                                currencyID="TRY",
                                Value=faturaBilgi.NetTutar


                            },
                            TaxAmount = new TaxAmountType
                            {
                                currencyID="TRY",
                                Value=faturaBilgi.KdvTutari
                            },
                            CalculationSequenceNumeric = new CalculationSequenceNumericType
                            {
                                Value=1
                            },
                            Percent = new PercentType1
                            {
                                Value=8
                            },
                            TaxCategory = new TaxCategoryType
                            {
                                TaxScheme = new TaxSchemeType
                                {
                                         Name = new NameType1
                                         {
                                             Value="KDV"

                                         },
                                         TaxTypeCode = new TaxTypeCodeType
                                         {
                                             Value="0015"
                                         }

                               }
                            },
                            PerUnitAmount = new PerUnitAmountType
                            {
                                currencyID="TRY"
                            }




                        }
                   }

                   }

                },

                LegalMonetaryTotal = new MonetaryTotalType
                {
                    LineExtensionAmount = new LineExtensionAmountType
                    {
                        currencyID = "TRY",
                        Value = faturaBilgi.Tutar
                    },
                    TaxExclusiveAmount = new TaxExclusiveAmountType
                    {
                        currencyID = "TRY",
                        Value = faturaBilgi.NetTutar
                    },
                    TaxInclusiveAmount = new TaxInclusiveAmountType
                    {
                        currencyID = "TRY",
                        Value = faturaBilgi.ToplamTutar
                    },
                    AllowanceTotalAmount = new AllowanceTotalAmountType
                    {
                        currencyID = "TRY",
                        Value = faturaBilgi.Iskonto
                    },
                    PayableAmount = new PayableAmountType
                    {
                        currencyID = "TRY",
                        Value = faturaBilgi.ToplamTutar
                    },
                    ChargeTotalAmount = new ChargeTotalAmountType
                    {
                        currencyID="TRY"
                    },
                    PayableRoundingAmount = new PayableRoundingAmountType
                    {
                        currencyID="TRY"
                    }
                    
                },
                InvoiceLine = FaturaHareketleri()



            };

            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
            var ms = new MemoryStream();
            var writer = XmlWriter.Create(ms, settings);
            var srl = new XmlSerializer(invoice.GetType());
            srl.Serialize(writer, invoice, XmlNameSpace());
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            var srRead = new StreamReader(ms);
            var readXml = srRead.ReadToEnd();
            var path = Path.Combine($@"{Application.StartupPath}\EArsivFaturalar\{ invoice.ID.Value }.xml");


            void EFaturaOlustur()
            {
                using (var sWriter = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sWriter.Write(readXml);
                    sWriter.Close();
                }
            }

            if (!Directory.Exists($@"{Application.StartupPath}\EArsivFaturalar"))
                Directory.CreateDirectory($@"{Application.StartupPath}\EArsivFaturalar");

            if (!File.Exists($@"{Application.StartupPath}\EArsivFaturalar\{ invoice.ID.Value }.xml"))
                EFaturaOlustur();

            else
                if (MessageBox.Show($@"{ invoice.ID.Value }.xml dosyasıdaha önce oluşturulmuş. Yeniden oluşturulsun mu?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                EFaturaOlustur();

            if (MessageBox.Show($@"Fatura oluşturma işlemi başarılı bir şekilde tamamlandı. Faturalar '{Application.StartupPath}\EArsivFaturalar' konumunda oluşturuldu. Konumu açmak istiyor musununz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes) return;
            Process.Start(Application.StartupPath + "\\EArsivFaturalar");
            XmlSerializerNamespaces XmlNameSpace()
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                ns.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                ns.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");
                ns.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                ns.Add("ccts", "urn:un:unece:uncefact:documentation:2");
                ns.Add("ubltr", "urn:oasis:names:specification:ubl:schema:xsd:TurkishCustomizationExtensionComponents");
                ns.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                ns.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
                ns.Add("ef", "http://www.efatura.gov.tr/package-namespace");
                ns.Add("sh", "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader");
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                ns.Add("sch", "http://purl.oclc.org/dsdl/schematron");
                ns.Add("urn", "urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2");
                ns.Add("urn1", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                ns.Add("urn2", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

                return ns;
            }

        }
        private void FrmFatura_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {





            try
            {
                string xml = $@"{Application.StartupPath }\EArsivFaturalar\{texFaturaNo.Text}.xml";
                string xslt = $@"{Application.StartupPath }\general.xslt";
                var frm = new FaturaGoruntuleyici(GetDocumentText(xml, xslt));
                string GetDocumentText(string xmlFilePath, string xsltFilePath)
                {
                    var xsltransform = new XslCompiledTransform();
                    var stringwriter = new StringWriter();
                    var reader = XmlReader.Create(xsltFilePath, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });

                    xsltransform.Load(reader );
                    xsltransform.Transform(xmlFilePath, null, stringwriter);
                    return stringwriter.ToString();
                }
              
                frm.ShowDialog();
            }
            catch (Exception)
            {

                throw;
            }
            //yazdır
        

        }

        private void labelControl1_Click(object sender, EventArgs e)
        {

        }

        private void labelControl5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelControl4_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateEdit3_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {

            var faturaBilgileri = new Fatura
            {
                CariKodu = txtCariKodu.Text,
                CariAdi = textCariAdi.Text,
                Tarih = dateTarih.DateTime,
                FaturaNo = texFaturaNo.Text,
                Tutar=Convert.ToDecimal(txtTutar.Text),
                Iskonto= Convert.ToDecimal(txtIskonto.Text),
                NetTutar= Convert.ToDecimal(txtNetTutar.Text),
                KdvTutari= Convert.ToDecimal(txtNetTutar.Text),
                ToplamTutar= Convert.ToDecimal(txtToplamTutar.Text)



            };
            //var FaturaHareketleri = gridView1.DataController.ListSource.Cast<FaturaHareket>();
            FaturaOlustur(faturaBilgileri);
        }
    }
   
}
