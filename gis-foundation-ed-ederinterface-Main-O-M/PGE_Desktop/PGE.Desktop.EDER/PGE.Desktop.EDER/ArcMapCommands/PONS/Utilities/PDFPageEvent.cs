using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS
{
    class PDFPageEvent : PdfPageEventHelper
    {
        //This is the contentbyte object of the writer
        //static PdfContentByte cb;
        //we will put the final number of pages in a template
        //static PdfTemplate template;
        //this is the BaseFont we are going to use for the header / footer
        //static BaseFont bf = null;

        private CustomerReport _pReport;

        public CustomerReport pReport { get; set; }

        public override void OnStartPage(PdfWriter writer, Document doc)
        {
            try
            {
                if (!String.IsNullOrEmpty(pReport.ReportID))
                {
                    Header header = new Header("H1", string.Empty);
                    header.Append("ShutDown Reference ID: " + pReport.ReportID);
                    doc.Add(header);
                }
                //FontFactory.RegisterDirectories();
                Font font_Arial11Bold = FontFactory.GetFont("arial", 11, Font.BOLD),
                font_Arial11Normal = FontFactory.GetFont("arial", 11, Font.NORMAL),
                    //fontNormal7_2 = FontFactory.GetFont("arial", 8, Font.NORMAL),
                font_Arial8Normal = FontFactory.GetFont("arial", 8, Font.BOLD);

                string sTemp = "Interruption date: ";

                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();//sTemp, fontBold11);
                Phrase phrase = new Phrase(sTemp, font_Arial11Bold);

                phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
                doc.Add(phrase);

                sTemp = string.Empty + pReport.OutageDetails[0].Outage_Date_OFF + " from " + pReport.OutageDetails[0].OFF_Time + " to " /*+ ((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? pReport.OutageDetails[0].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[0].ON_Time;
                sTemp += (pReport.OutageDetails.Count > 1) ? " and\n                                   " + pReport.OutageDetails[1].Outage_Date_OFF + " from " + pReport.OutageDetails[1].OFF_Time + " to " /*+ ((pReport.OutageDetails[1].Outage_Date_OFF != pReport.OutageDetails[1].Outage_Date_ON) ? pReport.OutageDetails[1].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[1].ON_Time : string.Empty;
                sTemp += ", weather permitting.";
                //phrase.Add(new Phrase(sTemp, fontNormal11));

                doc.Add(new Phrase(sTemp, font_Arial11Normal));
                doc.Add(paragraph);
                phrase = new Phrase("Interruption area: ", font_Arial11Bold);
                phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
                doc.Add(phrase);
                //phrase.Add(new Phrase(pReport.Description, fontNormal11));
                doc.Add(new Phrase(pReport.Description, font_Arial11Normal));
                paragraph = new iTextSharp.text.Paragraph();
                doc.Add(paragraph);

                PdfPTable ptable = new PdfPTable(7);
                //float[] widths = new float[] { 25f, 100f, 150f, 85f, 50f, 50f, 50f };
                float[] widths = new float[] { 25f, 180f, 230f, 109f, 62f, 50f, 58f };
                ptable.SetWidths(widths);
                ptable.WidthPercentage = 100;
                ptable.DefaultCell.Border = 0;
                ptable.DefaultCell.BorderColor = BaseColor.WHITE;
                font_Arial11Bold.SetStyle(Font.BOLD);
                PdfPCell pcell = new PdfPCell(new Phrase("Customer Notification List", font_Arial11Bold));
                pcell.Colspan = 6;
                pcell.BorderColor = BaseColor.WHITE;
                pcell.Border = 0;
                pcell.HorizontalAlignment = 1;
                ptable.DefaultCell.Border = 0;
                ptable.DefaultCell.BorderColor = BaseColor.WHITE;
                ptable.AddCell(pcell);
                pcell = new PdfPCell(new Phrase(DateTime.Today.ToShortDateString(), font_Arial11Normal));
                pcell.Colspan = 1;
                pcell.BorderColor = BaseColor.WHITE;
                pcell.Border = 0;
                pcell.HorizontalAlignment = 1;
                ptable.DefaultCell.Border = 0;
                ptable.DefaultCell.BorderColor = BaseColor.WHITE;
                ptable.AddCell(pcell);

                pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8Normal));
                pcell.BorderColor = BaseColor.WHITE;
                pcell.Colspan = 7;
                ptable.AddCell(pcell);

                ptable.AddCell(new Phrase("Type", font_Arial8Normal));
                ptable.AddCell(new Phrase("Customer Name", font_Arial8Normal));
                ptable.AddCell(new Phrase("Address", font_Arial8Normal));
                ptable.AddCell(new Phrase("City, State, ZIP", font_Arial8Normal));
                ptable.AddCell(new Phrase("CGC/TNum", font_Arial8Normal));
                ptable.AddCell(new Phrase("SSD", font_Arial8Normal));
                ptable.AddCell(new Phrase("Phone", font_Arial8Normal));

                pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8Normal));
                pcell.Colspan = 7;
                ptable.AddCell(pcell);
                doc.Add(ptable);
            }
            catch (Exception ex)
            {
                
               
            }
           
        }



        //public override void OnOpenDocument(PdfWriter writer, Document document)
        //{
        //    try
        //    {
        //        //bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //        //Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
        //        //bf = font_Arial10.GetCalculatedBaseFont(false);

        //        //cb = writer.DirectContent;
        //        //template = cb.CreateTemplate(50, 50);
        //    }
        //    catch (DocumentException de)
        //    {
        //    }
        //    catch (System.IO.IOException ioe)
        //    {
        //    }
        //}

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            try
            {
                PdfContentByte cb = writer.DirectContent;
                PdfTemplate template = cb.CreateTemplate(50, 50);
                Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
                BaseFont bf = font_Arial10.GetCalculatedBaseFont(false);
                base.OnEndPage(writer, document);
                int pageN = writer.PageNumber;
                String text = "Page " + pageN + "/";
                float len = bf.GetWidthPoint(text, 10);
                Rectangle pageSize = document.PageSize;
                cb.SetRGBColorFill(0, 0, 0);
                cb.BeginText();
                cb.SetFontAndSize(bf, 10);
                cb.SetTextMatrix(pageSize.GetLeft(355), pageSize.GetBottom(15));
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, pageSize.GetLeft(355) + len, pageSize.GetBottom(15));
            }
            catch (Exception ex)
            {
                
                //throw;
            }
            

           
        }
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            try
            {
                PdfContentByte cb = writer.DirectContent;
                PdfTemplate template = cb.CreateTemplate(50, 50);
                Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
                BaseFont bf = font_Arial10.GetCalculatedBaseFont(false);

                base.OnCloseDocument(writer, document);
                template.BeginText();
                template.SetFontAndSize(bf, 10);
                template.SetTextMatrix(0, 0);
                template.ShowText("" + (writer.PageNumber - 1));
                template.EndText();
            }
            catch (Exception ex)
            {

                //throw;
            }
          
        }


    }
}
