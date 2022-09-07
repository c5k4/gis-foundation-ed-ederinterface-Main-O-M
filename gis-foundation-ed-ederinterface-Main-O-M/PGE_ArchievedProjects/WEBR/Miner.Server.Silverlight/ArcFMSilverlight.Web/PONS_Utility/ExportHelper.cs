using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using System.Collections;
using ClosedXML.Excel;
//using System.Windows;


namespace ArcFMSilverlight.Web
{
    public class Exporthelper
    {
        protected Font footer
        {
            get
            {
                // create a basecolor to use for the footer font, if needed.
                BaseColor grey = new BaseColor(128, 128, 128);
                Font font = FontFactory.GetFont("Arial", 9, Font.ITALIC);
                return font;
            }
        }

        //PDf file create
        //public string createSilverlightPDF(string outagetime, string Header, string msg, DataTable dt, string strUserName)
        //{
        //    string strCompletePath = "";

        //    Document pdfDoc = new Document();
        //    pdfDoc = new Document(PageSize.A4.Rotate(), 25f, 25f, 10f, 25f);
        //    try
        //    {
        //        string strFileURL = System.Configuration.ConfigurationManager.AppSettings["ExpportPDFURLNew"];
        //        string strFilePath = System.Configuration.ConfigurationManager.AppSettings["ExpportFileLocationNew"];
        //        string userName = strUserName;

        //        //file delete from folder
        //        string[] files = Directory.GetFiles(strFilePath);

        //        foreach (string file in files)
        //        {
        //            FileInfo fi = new FileInfo(file);
        //            string _filename = fi.Name;


        //            if (_filename.IndexOf(userName) != -1)
        //                fi.Delete();
        //        }

        //        //New file creation
        //        DateTime t1 = System.DateTime.Now;

        //        string strFileName = t1.Day.ToString() + t1.Month.ToString() + t1.Year.ToString() + t1.Hour.ToString() + t1.Minute.ToString() + t1.Second.ToString() + "_" + userName;
        //        strCompletePath = strFilePath + strFileName + ".pdf";
        //        PdfWriter Writer = PdfWriter.GetInstance(pdfDoc, new FileStream(strCompletePath, FileMode.Create));

        //        // calling PDFFooter class to Include in document
        //        Writer.PageEvent = new PDFFooter();
        //        pdfDoc.Open();

        //        if (dt != null)
        //        {
        //            //Craete instance of the pdf table and set the number of column in that table
        //            PdfPTable PdfTable = new PdfPTable(dt.Columns.Count);
        //            PdfTable.TotalWidth = 520f;
        //            // PdfTable.TotalHeight = 715f;
        //            PdfTable.LockedWidth = true;
        //            float[] widths = new float[] { 25f, 100f, 160f, 85f, 50f, 45f, 50f };
        //            PdfTable.SetWidths(widths);
        //            PdfPCell PdfPCell = null;
        //            PdfTable.HorizontalAlignment = 0;
        //            // PdfTable.FooterHeight = 10;
        //            iTextSharp.text.Font hrrowFontStyle = FontFactory.GetFont("verdana", 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        //            iTextSharp.text.Font rowFontStyle = FontFactory.GetFont("verdana", 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        //            iTextSharp.text.Font Para_rowFontStyle = FontFactory.GetFont("verdana", 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        //            iTextSharp.text.Font Paracus_rowFontStyle = FontFactory.GetFont("verdana", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        //            string sTemp = "Interruption date: ";
        //            //iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
        //            ////sTemp = "" + outagetime + " , weather permitting.";
        //            //iTextSharp.text.Phrase phrase = new iTextSharp.text.Phrase(sTemp, hrrowFontStyle);

        //            //phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
        //            //pdfDoc.Add(phrase);
        //            //pdfDoc.Add(new iTextSharp.text.Phrase(sTemp, Para_rowFontStyle));
        //            //pdfDoc.Add(paragraph);
        //            iTextSharp.text.Paragraph paragraph_1 = new iTextSharp.text.Paragraph(new Chunk(sTemp + "" + outagetime + " , weather permitting.", Para_rowFontStyle)); //new Paragraph(p1chunk + "" + outagetime + " weather permitting.");
        //            iTextSharp.text.Paragraph paragraph_2 = new iTextSharp.text.Paragraph(new Chunk("Interruption Area :" + " " + Header, Para_rowFontStyle)); // new Paragraph(p2 + " "+ Header);

        //            //iTextSharp.text.Paragraph pl = new iTextSharp.text.Paragraph(new Chunk("____________________________________________________________________________________________________________________________________", hrrowFontStyle));

        //            iTextSharp.text.Paragraph pl = new iTextSharp.text.Paragraph(new Chunk("                                                                                                                                                                                                                                                                                           " + t1.ToShortDateString(), Para_rowFontStyle));
        //            iTextSharp.text.Paragraph paragraph_3 = new iTextSharp.text.Paragraph(new Chunk(msg + "                                           ", Paracus_rowFontStyle));
        //            paragraph_3.IndentationLeft = 210;

        //            //  paragraph_3.Alignment = "Middle";

        //            //Add Header of the pdf table
        //            for (int i = 0; i < dt.Columns.Count; i++)
        //            {
        //                string strColumName = dt.Columns[i].ColumnName;
        //                if (strColumName == "City")
        //                {
        //                    strColumName = "City State ZIP";
        //                }
        //                PdfPCell = new PdfPCell(new Phrase(new Chunk(strColumName, hrrowFontStyle)));
        //                PdfPCell.Border = Rectangle.BOTTOM_BORDER;
        //                PdfPCell.BorderWidthBottom = .7f;
        //                PdfPCell.PaddingBottom = 7f;
        //                PdfPCell.HorizontalAlignment = 0;
        //                PdfTable.AddCell(PdfPCell);
        //            }
        //            //How add the data from datatable to pdf table
        //            for (int rows = 0; rows < dt.Rows.Count; rows++)
        //            {
        //                for (int column = 0; column < dt.Columns.Count; column++)
        //                {
        //                    PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), rowFontStyle)));
        //                    PdfPCell.Border = Rectangle.NO_BORDER;
        //                    PdfPCell.HorizontalAlignment = 0;

        //                    PdfTable.AddCell(PdfPCell);
        //                }
        //            }

        //            PdfTable.DefaultCell.Border = Rectangle.NO_BORDER;
        //            PdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table

        //            pdfDoc.Add(paragraph_1);// add paragraph to the document
        //            pdfDoc.Add(paragraph_2);
        //            pdfDoc.Add(pl);
        //            pdfDoc.Add(paragraph_3);

        //            pdfDoc.Add(PdfTable); // add pdf table to the document

        //        }
        //        pdfDoc.Close();
        //        strCompletePath = strFileURL + strFileName + ".pdf";
        //        return strFileURL + strFileName + ".pdf";

        //    }
        //    catch (Exception ex)
        //    {
        //        strCompletePath = ex.Message;
        //        return strCompletePath;
        //    }
        //}

        //public class PDFPageEvent : PdfPageEventHelper
        //{
        //    CustomerReport pReport = new CustomerReport();

        //    public PDFPageEvent(CustomerReport _Report)
        //    {
        //        pReport = _Report;
        //    }
        //    public override void OnStartPage(PdfWriter writer, Document doc)
        //    {
        //        if (!String.IsNullOrEmpty(pReport.ReportID))
        //        {
        //            Header header = new Header("H1", string.Empty);
        //            header.Append("ShutDown Reference ID: " + pReport.ReportID);
        //            doc.Add(header);
        //        }
        //        //FontFactory.RegisterDirectories();
        //        Font font_Arial11Bold = FontFactory.GetFont("arial", 11, Font.BOLD),
        //        font_Arial11Normal = FontFactory.GetFont("arial", 11, Font.NORMAL),
        //            //fontNormal7_2 = FontFactory.GetFont("arial", 8, Font.NORMAL),
        //        font_Arial8Normal = FontFactory.GetFont("arial", 8, Font.NORMAL);

        //        string sTemp = "Interruption date: ";

        //        iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();//sTemp, fontBold11);
        //        iTextSharp.text.Phrase phrase = new iTextSharp.text.Phrase(sTemp, font_Arial11Bold);

        //        phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
        //        doc.Add(phrase);

        //        sTemp = string.Empty + pReport.OutageDetails[0].Outage_Date_OFF + " from " + pReport.OutageDetails[0].OFF_Time + " to " /*+ ((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? pReport.OutageDetails[0].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[0].ON_Time;
        //        sTemp += (pReport.OutageDetails.Count > 1) ? " and\n                                   " + pReport.OutageDetails[1].Outage_Date_OFF + " from " + pReport.OutageDetails[1].OFF_Time + " to " /*+ ((pReport.OutageDetails[1].Outage_Date_OFF != pReport.OutageDetails[1].Outage_Date_ON) ? pReport.OutageDetails[1].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[1].ON_Time : string.Empty;
        //        sTemp += ", weather permitting.";
        //        //phrase.Add(new iTextSharp.text.Phrase(sTemp, fontNormal11));

        //        doc.Add(new iTextSharp.text.Phrase(sTemp, font_Arial11Normal));
        //        doc.Add(paragraph);
        //        phrase = new iTextSharp.text.Phrase("Interruption area: ", font_Arial11Bold);
        //        phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
        //        doc.Add(phrase);
        //        //phrase.Add(new iTextSharp.text.Phrase(pReport.Description, fontNormal11));
        //        doc.Add(new iTextSharp.text.Phrase(pReport.Description, font_Arial11Normal));
        //        paragraph = new iTextSharp.text.Paragraph();
        //        doc.Add(paragraph);

        //        PdfPTable ptable = new PdfPTable(7);
        //        float[] widths = new float[] { 25f, 100f, 150f, 85f, 50f, 50f, 50f };
        //        ptable.SetWidths(widths);
        //        ptable.WidthPercentage = 100;
        //        ptable.DefaultCell.Border = 0;
        //        ptable.DefaultCell.BorderColor = BaseColor.WHITE;
        //        font_Arial11Bold.SetStyle(Font.BOLD);
        //        PdfPCell pcell = new PdfPCell(new Phrase("Customer Notification List", font_Arial11Bold));
        //        pcell.Colspan = 6;
        //        pcell.BorderColor = BaseColor.WHITE;
        //        pcell.Border = 0;
        //        pcell.HorizontalAlignment = 1;
        //        ptable.DefaultCell.Border = 0;
        //        ptable.DefaultCell.BorderColor = BaseColor.WHITE;
        //        ptable.AddCell(pcell);
        //        pcell = new PdfPCell(new Phrase(DateTime.Today.ToShortDateString(), font_Arial11Normal));
        //        pcell.Colspan = 1;
        //        pcell.BorderColor = BaseColor.WHITE;
        //        pcell.Border = 0;
        //        pcell.HorizontalAlignment = 1;
        //        ptable.DefaultCell.Border = 0;
        //        ptable.DefaultCell.BorderColor = BaseColor.WHITE;
        //        ptable.AddCell(pcell);

        //        pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8Normal));
        //        pcell.BorderColor = BaseColor.WHITE;
        //        pcell.Colspan = 7;
        //        ptable.AddCell(pcell);

        //        ptable.AddCell(new Phrase("Type", font_Arial8Normal));
        //        ptable.AddCell(new Phrase("Customer Name", font_Arial8Normal));
        //        ptable.AddCell(new Phrase("Address", font_Arial8Normal));
        //        ptable.AddCell(new Phrase("City, State, ZIP", font_Arial8Normal));
        //        ptable.AddCell(new Phrase("CGC/TNum", font_Arial8Normal));
        //        ptable.AddCell(new Phrase("SSD", font_Arial8Normal));
        //        ptable.AddCell(new Phrase("Phone", font_Arial8Normal));

        //        pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8Normal));
        //        pcell.Colspan = 7;
        //        ptable.AddCell(pcell);
        //        doc.Add(ptable);
        //    }

        //    // This is the contentbyte object of the writer
        //    PdfContentByte cb;
        //    // we will put the final number of pages in a template
        //    PdfTemplate template;
        //    // this is the BaseFont we are going to use for the header / footer
        //    BaseFont bf = null;
        //    // This keeps track of the creation time
        //    DateTime PrintTime = DateTime.Now;
        //    #region Properties
        //    private string _Title;
        //    public string Title
        //    {
        //        get { return _Title; }
        //        set { _Title = value; }
        //    }

        //    private string _HeaderLeft;
        //    public string HeaderLeft
        //    {
        //        get { return _HeaderLeft; }
        //        set { _HeaderLeft = value; }
        //    }
        //    private string _HeaderRight;
        //    public string HeaderRight
        //    {
        //        get { return _HeaderRight; }
        //        set { _HeaderRight = value; }
        //    }
        //    private Font _HeaderFont;
        //    public Font HeaderFont
        //    {
        //        get { return _HeaderFont; }
        //        set { _HeaderFont = value; }
        //    }
        //    private Font _FooterFont;
        //    public Font FooterFont
        //    {
        //        get { return _FooterFont; }
        //        set { _FooterFont = value; }
        //    }
        //    #endregion

        //    public override void OnOpenDocument(PdfWriter writer, Document document)
        //    {
        //        try
        //        {
        //            PrintTime = DateTime.Now;

        //            //bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //            Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
        //            bf = font_Arial10.GetCalculatedBaseFont(false);
        //            cb = writer.DirectContent;
        //            template = cb.CreateTemplate(50, 50);
        //        }
        //        catch (DocumentException de)
        //        {
        //        }
        //        catch (System.IO.IOException ioe)
        //        {
        //        }
        //    }

        //    public override void OnEndPage(PdfWriter writer, Document document)
        //    {
        //        Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
        //        bf = font_Arial10.GetCalculatedBaseFont(false);
        //        base.OnEndPage(writer, document);
        //        int pageN = writer.PageNumber;
        //        String text = "Page " + pageN + " of ";
        //        float len = bf.GetWidthPoint(text, 10);
        //        Rectangle pageSize = document.PageSize;
        //        cb.SetRGBColorFill(0, 0, 0);
        //        cb.BeginText();
        //        cb.SetFontAndSize(bf, 10);
        //        cb.SetTextMatrix(pageSize.GetLeft(355), pageSize.GetBottom(15));
        //        cb.ShowText(text);
        //        cb.EndText();
        //        cb.AddTemplate(template, pageSize.GetLeft(355) + len, pageSize.GetBottom(15));

        //        //cb.BeginText();
        //        //cb.SetFontAndSize(bf, 8);
        //        //cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
        //        //    "Printed On " + PrintTime.ToString(),
        //        //    pageSize.GetRight(40),
        //        //    pageSize.GetBottom(30), 0);
        //        //cb.EndText();
        //    }
        //    public override void OnCloseDocument(PdfWriter writer, Document document)
        //    {
        //        base.OnCloseDocument(writer, document);
        //        template.BeginText();
        //        template.SetFontAndSize(bf, 8);
        //        template.SetTextMatrix(0, 0);
        //        template.ShowText("" + (writer.PageNumber - 1));
        //        template.EndText();
        //    }
        //}

        public string createSilverlightPDF(string outagetime, string Header, string msg, DataTable dt, string strUserName)
        {
            string strCompletePath = "";
            Document doc = new Document(PageSize.A4.Rotate(), 25f, 25f, 10f, 25f);
            try
            {
                string strFileURL = System.Configuration.ConfigurationManager.AppSettings["ExpportPDFURLNew"];
                string strFilePath = System.Configuration.ConfigurationManager.AppSettings["ExpportFileLocationNew"];
                string userName = strUserName;

                //file delete from folder
                string[] files = Directory.GetFiles(strFilePath);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    string _filename = fi.Name;


                    if (_filename.IndexOf(userName) != -1)
                        fi.Delete();
                }

                //New file creation
                DateTime t1 = System.DateTime.Now;

                string strFileName = t1.Day.ToString() + t1.Month.ToString() + t1.Year.ToString() + t1.Hour.ToString() + t1.Minute.ToString() + t1.Second.ToString() + "_" + userName;
                strCompletePath = strFilePath + strFileName + ".pdf";
                PdfWriter Writer = PdfWriter.GetInstance(doc, new FileStream(strCompletePath, FileMode.Create));

                // calling PDFFooter class to Include in document
                Writer.PageEvent = new PDFPageEvent(outagetime, Header);
                doc.Open();
                Font font_Arial8_25 = FontFactory.GetFont("arial", 8.25f, Font.NORMAL);
                if (dt != null)
                {
                    //Craete instance of the pdf table and set the number of column in that table
                    PdfPTable PdfTable = new PdfPTable(dt.Columns.Count);
                    float[] widths = new float[] { 30f, 189f, 220f, 120f, 60f, 45f, 55f };
                    //float[] widths = new float[] { 30f, 180f, 230f, 115f, 60f, 50f, 50f };
                    PdfTable.WidthPercentage = 100;
                    PdfTable.SetWidths(widths);
                    PdfTable.DefaultCell.NoWrap = false;
                    PdfTable.DefaultCell.FixedHeight = 12.5f;
                    PdfTable.DefaultCell.Border = 0;
                    PdfTable.DefaultCell.BorderColor = BaseColor.WHITE;
                    PdfPCell PdfPCell = null;
                    PdfTable.HorizontalAlignment = 0;
                    for (int rows = 0; rows < dt.Rows.Count; rows++)
                    {
                        for (int column = 0; column < dt.Columns.Count; column++)
                        {
                            PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font_Arial8_25)));
                            PdfPCell.Border = 0;
                            PdfPCell.BorderColor = BaseColor.WHITE;
                           
                            PdfPCell.HorizontalAlignment = (column > 3) ? 2 : 0;
                            PdfPCell.NoWrap = (column > 3); // changed to get full string of NO NUMBER
                            PdfPCell.FixedHeight = 12.5f;
                            PdfTable.AddCell(PdfPCell);
                        }
                    }

                    doc.Add(new iTextSharp.text.Paragraph());// add paragraph to the document
                    doc.Add(new iTextSharp.text.Paragraph());

                    doc.Add(PdfTable);

                }
                doc.Close();
                strCompletePath = strFileURL + strFileName + ".pdf";
               return strFileURL + strFileName + ".pdf";
                

            }
            catch (Exception ex)
            {
                strCompletePath = ex.Message;
                return strCompletePath;
            }
        }

        //Text wrap
        
        public string createTEXTFile(string strDateTime, string strDecLoc, string msg, DataTable DataTabTEXT, string struserName)
        {
            string strCompletePath = "";
            string strFilePath = System.Configuration.ConfigurationManager.AppSettings["ExpportFileLocationNew"];
            string strFileURL = System.Configuration.ConfigurationManager.AppSettings["ExpportPDFURLNew"];

            string userName = struserName;

            //file delete from folder
            string[] files = Directory.GetFiles(strFilePath);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                string _filename = fi.Name;


                if (_filename.IndexOf(userName) != -1)
                    fi.Delete();
            }

            //New file creation
            DateTime t1 = System.DateTime.Now;

            string strFileName = t1.Day.ToString() + t1.Month.ToString() + t1.Year.ToString() + t1.Hour.ToString() + t1.Minute.ToString() + t1.Second.ToString() + "_" + userName;


            strCompletePath = strFilePath + strFileName + ".txt";


            try
            {
                int[] padf = new int[7];
               
                StringBuilder strBuilder = new StringBuilder();
                if (DataTabTEXT == null)
                    return "";

                List<int> maximumLengthForColumns =
   Enumerable.Range(0, DataTabTEXT.Columns.Count)
             .Select(col => DataTabTEXT.AsEnumerable()
                                     .Select(row => row[col]).OfType<string>()
                                     .Max(val => val.Length)).ToList();

                padf[0] = maximumLengthForColumns[0] + 3;
                padf[1] = maximumLengthForColumns[1] + 3;
                padf[2] = maximumLengthForColumns[2] + 3;

                padf[3] = maximumLengthForColumns[3] + 3;
                padf[4] = maximumLengthForColumns[4] + 3;
                padf[5] = maximumLengthForColumns[5] + 3;
                padf[6] = maximumLengthForColumns[6] + 3;
                int padsum = padf[0] + padf[1] + padf[2] + padf[3] + padf[4] + padf[5] + padf[6];

                List<string> lstFields = new List<string>();
                strBuilder.AppendLine("Interruption Date: " + strDateTime + " weather permitting");

                string decs = StringTool.WrapString(strDecLoc, 100);
                strBuilder.AppendLine("Interruption Area:" + decs);
                strBuilder.AppendLine();
                strBuilder.AppendLine();
                strBuilder.AppendLine(" Customer Notification List ".PadLeft(padsum / 2) + "                                                                 " + DateTime.Today.Date.ToShortDateString().PadRight(5));
                strBuilder.AppendLine();
                lstFields.Add("S.No");
                if (DataTabTEXT != null)
                {
                    for (int iHeader = 0; iHeader < DataTabTEXT.Columns.Count; iHeader++)
                    {
                        lstFields.Add(DataTabTEXT.Columns[iHeader].ColumnName.ToString());
                    }
                }
                string strAddress = "";
                strAddress = lstFields[3].ToString();
                if (strAddress.Length >= 30)
                {
                    strAddress = strAddress.Replace(" ", "on");
                }
                string strCustNameAdd = "";
                strCustNameAdd = lstFields[4].ToString();
                if (strCustNameAdd.Length >= 70)
                {
                    strCustNameAdd = strCustNameAdd.Replace(" ", "/n");
                }
                strBuilder.AppendLine(lstFields[1].ToString().PadRight(padf[0]) + lstFields[2].ToString().PadRight(padf[1]) + strAddress.PadRight(padf[2]) + strCustNameAdd.PadRight(padf[3]) + lstFields[5].ToString().PadRight(padf[4]) + lstFields[6].ToString().PadRight(padf[5]) + lstFields[7].ToString().PadRight(padf[6]));
                for (int i = 0; i < padsum; i++)
                {
                    strBuilder.Append("_");
                }
                strBuilder.AppendLine();
                lstFields.Clear();
                for (int rows = 0; rows < DataTabTEXT.Rows.Count; rows++)
                {
                    lstFields.Clear();
                    strBuilder.AppendLine();
                    string strValue = "";
                    strValue = (rows + 1).ToString();
                    lstFields.Add(strValue);
                    int[] lineamounts = new int[7];
                    int king = 0;
                    for (int column = 0; column < DataTabTEXT.Columns.Count; column++)
                    {
                        lineamounts[column] = (int)Math.Ceiling((double)DataTabTEXT.Rows[rows][column].ToString().Length / (double)padf[column]);
                        if (lineamounts[column] > king)
                        {
                            king = lineamounts[column];
                        }
                    }
                    int[] charcount = new int[7];
                    for (int linecount = 1; linecount <= king; linecount++)
                    {
                        for (int column = 0; column < DataTabTEXT.Columns.Count; column++)
                        {
                            if (lineamounts[column] > linecount)
                            {
                                strValue = DataTabTEXT.Rows[rows][column].ToString().Substring((linecount - 1) * padf[column], padf[column] - 1);
                                charcount[column] += strValue.Length;
                                lstFields.Insert(column + 1, strValue);
                            }
                            else if (lineamounts[column] == linecount)
                            {
                                strValue = DataTabTEXT.Rows[rows][column].ToString().Substring(charcount[column]);
                                lstFields.Insert(column + 1, strValue);
                            }
                            else
                            {
                                strValue = " ";
                                lstFields.Insert(column + 1, strValue);
                            }
                        }
                        strBuilder.Append(lstFields[1].ToString().PadRight(padf[0]) + lstFields[2].ToString().PadRight(padf[1]) + lstFields[3].ToString().PadRight(padf[2]) + lstFields[4].ToString().PadRight(padf[3]) + lstFields[5].ToString().PadRight(padf[4]) + lstFields[6].ToString().PadRight(padf[5]) + lstFields[7].ToString().PadRight(padf[6]));
                        //strBuilder.Append("\n");
                    }
                }

                StreamWriter sw = new StreamWriter(new FileStream(strCompletePath, FileMode.Create));
                sw.Write(strBuilder.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                return strFileURL + strFileName + ".txt";
            }
            return strFileURL + strFileName + ".txt";
        }

        public string createExcelFile(string strDateTime, string strDecLoc, string msg, DataTable dtable, string strUserName)
        {
            string strCompletePath = "";
            string strFilePath = System.Configuration.ConfigurationManager.AppSettings["ExpportFileLocationNew"];
            string strFileURL = System.Configuration.ConfigurationManager.AppSettings["ExpportPDFURLNew"];
            string userName = strUserName;

            //file delete from folder
            string[] files = Directory.GetFiles(strFilePath);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                string _filename = fi.Name;


                if (_filename.IndexOf(userName) != -1)
                    fi.Delete();
            }
            //New file creation
            DateTime t1 = System.DateTime.Now;
            string strFileName = t1.Day.ToString() + t1.Month.ToString() + t1.Year.ToString() + t1.Hour.ToString() + t1.Minute.ToString() + t1.Second.ToString() + "_" + userName;
            strCompletePath = strFilePath + strFileName + ".xlsx";
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                if (dtable == null)
                    return "";

                #region New Approach
                XLWorkbook xlw = new XLWorkbook();

                IXLWorksheet xls = xlw.AddWorksheet("CustomerList");
                IXLCell xlc = xls.Cell(1, 1);
                xls.Range("A1:B1").Merge();
                xlc.Value = "Interruption date:";
                xlc.RichText.Underline = XLFontUnderlineValues.Single;
                xlc.RichText.Bold = true;
                xlc = xls.Cell(1, 3);
                //xls.Range("C1:" + /*((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? "E1" : "I1")*/"I1").Merge();
                xls.Range("C1:" + (strDateTime.Contains("\n") ? "I1" : "I1")).Merge();
                string sTemp = (strDateTime.Contains("\n") ? strDateTime.Split('\n')[0].Trim() : strDateTime.Trim());
                sTemp += (!strDateTime.Contains("\n") ? "" : Environment.NewLine + strDateTime.Split('\n')[1].Trim());
                //sTemp += ", weather permitting.";
                xlc.Value = sTemp;
                xlc.WorksheetRow().Height *= (strDateTime.Contains("\n") ? 2 : 1);
                xls.Range("A2:B2").Merge();
                xlc = xls.Cell(2, 1);
                xlc.Value = "Interruption area:";
                xlc.RichText.Underline = XLFontUnderlineValues.Single;
                xlc.RichText.Bold = true;
                xlc = xls.Cell(2, 3);
                xls.Range("C2:E2").Merge();
                xlc.Value = strDecLoc;
                xls.Range("A4:F4").Merge();
                xlc = xls.Cell(4, 1);
                xlc.Value = "Customer Notification List";
                xls.Range("A4:F4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                xlc = xls.Cell(4, 7);
                xlc.Value = DateTime.Today.ToShortDateString();

                DataTable pDTable = new DataTable("CUSTOMERLIST");
                pDTable.Columns.Add("Type");
                pDTable.Columns.Add("Customer Name");
                pDTable.Columns.Add("Address");
                pDTable.Columns.Add("City, State, ZIP");
                pDTable.Columns.Add("CGC/TNUM");
                pDTable.Columns.Add("SSD");
                pDTable.Columns.Add("Phone");
                foreach (DataRow row in dtable.Rows)
                //foreach (Customer customer in pReport.AffectedCustomers.ToList())
                {

                    DataRow pRow = pDTable.NewRow();
                    pRow[0] = row[0];
                    pRow[1] = row[1];
                    pRow[2] = row[2];
                    pRow[3] = row[3];
                    pRow[4] = row[4];
                    pRow[5] = row[5];
                    pRow[6] = row[6];
                    pDTable.Rows.Add(pRow);
                }
                xls.SheetView.Freeze(5, 2);
                xlc = xls.Cell(5, 1);
                xlc.InsertTable(pDTable);

                xlw.SaveAs(strCompletePath);

                #endregion

                #region Old Approach
                //List<string> lstFields = new List<string>();
                //lstFields.Add(FormatHeaderField("S.No"));
                //if (dtable != null)
                //{
                //    for (int iHeader = 0; iHeader < dtable.Columns.Count; iHeader++)
                //    {
                //        lstFields.Add(FormatHeaderField(dtable.Columns[iHeader].ColumnName.ToString()));
                //    }
                //    BuildStringOfRow(strBuilder, lstFields);
                //}
                //lstFields.Clear();
                //for (int rows = 0; rows < dtable.Rows.Count; rows++)
                //{
                //    lstFields.Clear();
                //    string strValue = "";
                //    strValue = (rows + 1).ToString();
                //    lstFields.Add(FormatField(strValue));
                //    for (int column = 0; column < dtable.Columns.Count; column++)
                //    {
                //        strValue = dtable.Rows[rows][column].ToString();
                //        lstFields.Add(FormatField(strValue));
                //    }

                //    BuildStringOfRow(strBuilder, lstFields);
                //}
                //string strScnd = "";
                //if (strDateTime.Contains("\n"))
                //{
                //    string[] arrDate = strDateTime.Split('\n');
                //    strDateTime = arrDate[0].ToString();
                //    strScnd = arrDate[1].ToString();
                //}
                //else
                //{
                //    strDateTime = " " + strDateTime;
                //    strScnd = "";
                //}
                //StreamWriter sw = new StreamWriter(new FileStream(strCompletePath, FileMode.Create));
                //sw.WriteLine("<?xml version=\"1.0\" " + "encoding=\"utf-8\"?>");
                //sw.WriteLine("<?mso-application progid" + "=\"Excel.Sheet\"?>");
                //sw.WriteLine("<Workbook xmlns=\"urn:" + "schemas-microsoft-com:office:spreadsheet\">");
                //sw.WriteLine("<DocumentProperties " + "xmlns=\"urn:schemas-microsoft-com:" + "office:office\">");
                //sw.WriteLine("<Created>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</Created>");
                //sw.WriteLine("</DocumentProperties>");
                //sw.WriteLine("<Styles>");
                //sw.WriteLine("<Style ss:ID=\"head\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Bottom\"/><Borders/><Font ss:Underline=\"Single\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
                //sw.WriteLine("<Style ss:ID=\"title\"><Alignment ss:Horizontal=\"Center\"/><Borders/><Font ss:Size=\"13\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
                //sw.WriteLine("<Style ss:ID=\"date\"><Alignment ss:Horizontal=\"Right\"/><Borders/><Font/><Interior/><NumberFormat/><Protection/></Style>");
                //sw.WriteLine("<Style ss:ID=\"rowhead\"><Alignment ss:Horizontal=\"Left\"/><Borders/><Font ss:Color=\"#ffffff\" ss:Bold=\"1\"/><Interior ss:Color=\"#4f81BD\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
                //sw.WriteLine("<Style ss:ID=\"data\"><Borders><Border ss:Color=\"#4f81BD\" ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                //         "<Border ss:Color=\"#4f81BD\" ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                //          "<Border ss:Color=\"#4f81BD\" ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                //         "<Border ss:Color=\"#4f81BD\" ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" /></Borders></Style>");
                //sw.WriteLine("</Styles>");
                //sw.WriteLine("<Worksheet ss:Name=\"Customer Information\" " + "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                //sw.WriteLine("<Table>");
                //sw.WriteLine("<Column ss:Index=\"1\" ss:AutoFitWidth=\"0\" ss:Width=\"30\"/>");
                //sw.WriteLine("<Column ss:Index=\"2\" ss:AutoFitWidth=\"0\" ss:Width=\"44\"/>");
                //sw.WriteLine("<Column ss:Index=\"3\" ss:AutoFitWidth=\"0\" ss:Width=\"72\"/>");
                //sw.WriteLine("<Column ss:Index=\"4\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                //sw.WriteLine("<Column ss:Index=\"5\" ss:AutoFitWidth=\"0\" ss:Width=\"359\"/>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Date:") + String.Format("<Cell ss:MergeAcross=\"1\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDateTime) + "</Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "") + String.Format("<Cell ss:MergeAcross=\"1\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strScnd) + "</Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Area:") + String.Format("<Cell><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDecLoc) + "</Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell ss:StyleID=\"date\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", DateTime.Now.ToLocalTime().ToLongDateString()) + "</Row>");
                //sw.WriteLine("<Row></Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"title\" ss:MergeAcross=\"7\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Customer Notification Wizard") + "</Row>");
                //sw.WriteLine("<Row></Row>");
                //sw.Write(strBuilder.ToString());
                //sw.WriteLine("</Table>");
                //sw.WriteLine("</Worksheet>");
                //sw.WriteLine("</Workbook>");
                //sw.Close();
                #endregion

            }
            catch (Exception ex)
            {
                string steError = ex.Message;
                return strFileURL + strFileName + ".xlsx";
            }
            return strFileURL + strFileName + ".xlsx";
            //return strCompletePath;
        }
        //Premise Report
        public string createPremiseExcelFile(string strDateTime, string strDecLoc, string msg, DataTable dtable, string strUserName)
        {
            string strCompletePath = "";
            string strFilePath = System.Configuration.ConfigurationManager.AppSettings["ExpportFileLocationNew"];
            string strFileURL = System.Configuration.ConfigurationManager.AppSettings["ExpportPDFURLNew"];
            string userName = strUserName;

            //file delete from folder
            string[] files = Directory.GetFiles(strFilePath);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                string _filename = fi.Name;


                if (_filename.IndexOf(userName) != -1)
                    fi.Delete();
            }
            //New file creation
            DateTime t1 = System.DateTime.Now;
            string strFileName = t1.Day.ToString() + t1.Month.ToString() + t1.Year.ToString() + t1.Hour.ToString() + t1.Minute.ToString() + t1.Second.ToString() + "_" + userName;
            strCompletePath = strFilePath + strFileName + ".xlsx";
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                if (dtable == null)
                    return "";

                #region New Approach
                XLWorkbook xlw = new XLWorkbook();

                IXLWorksheet xls = xlw.AddWorksheet("CustomerList");
                IXLCell xlc = xls.Cell(1, 1);
                xls.Range("A1:B1").Merge();
                xlc.Value = "Interruption date:";
                xlc.RichText.Underline = XLFontUnderlineValues.Single;
                xlc.RichText.Bold = true;
                xlc = xls.Cell(1, 3);
                //xls.Range("C1:" + /*((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? "E1" : "I1")*/"I1").Merge();
                xls.Range("C1:" + (strDateTime.Contains("\n") ? "I1" : "I1")).Merge();
                string sTemp = (strDateTime.Contains("\n") ? strDateTime.Split('\n')[0].Trim() : strDateTime.Trim());
                sTemp += (!strDateTime.Contains("\n") ? "" : Environment.NewLine + strDateTime.Split('\n')[1].Trim());
                //sTemp += ", weather permitting.";
                xlc.Value = sTemp;
                xlc.WorksheetRow().Height *= (strDateTime.Contains("\n") ? 2 : 1);
                xls.Range("A2:B2").Merge();
                xlc = xls.Cell(2, 1);
                xlc.Value = "Interruption area:";
                xlc.RichText.Underline = XLFontUnderlineValues.Single;
                xlc.RichText.Bold = true;
                xlc = xls.Cell(2, 3);
                xls.Range("C2:E2").Merge();
                xlc.Value = strDecLoc;
                xls.Range("A4:F4").Merge();
                xlc = xls.Cell(4, 1);
                xlc.Value = "Customer Notification List";
                xls.Range("A4:F4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                xlc = xls.Cell(4, 7);
                xlc.Value = DateTime.Today.ToShortDateString();

                DataTable pDTable = new DataTable("CUSTOMERLIST");
                //pDTable.Columns.Add("Type");
                //pDTable.Columns.Add("Customer Name");
                //pDTable.Columns.Add("Address");
                //pDTable.Columns.Add("City, State, ZIP");
              
                //pDTable.Columns.Add("CGC/TNUM");
                //pDTable.Columns.Add("SSD");
                //pDTable.Columns.Add("Phone");
                pDTable.Columns.Add("Premise ID");
               // pDTable.Columns.Add("Premise Type");
                foreach (DataRow row in dtable.Rows)
                //foreach (Customer customer in pReport.AffectedCustomers.ToList())
                {

                    DataRow pRow = pDTable.NewRow();
                    //pRow[0] = row[0];
                    //pRow[1] = row[1];
                    //pRow[2] = row[2];
                    //pRow[3] = row[3];
                    //pRow[4] = row[4];
                    //pRow[5] = row[5];
                    //pRow[6] = row[6];
                    pRow[0] = row[7];
                  //  pRow[8] = row[8];
                    pDTable.Rows.Add(pRow);
                }
                xls.SheetView.Freeze(5, 2);
                xlc = xls.Cell(5, 1);
                xlc.InsertTable(pDTable);

                xlw.SaveAs(strCompletePath);

                #endregion

              

            }
            catch (Exception ex)
            {
                string steError = ex.Message;
                return strFileURL + strFileName + ".xlsx";
            }
           return strFileURL + strFileName + ".xlsx";
            //return strCompletePath;
        }
        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields)
        {
            strBuilder.AppendLine("<Row>");
            strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
            strBuilder.AppendLine("</Row>");
        }

        private static void BuildStringOfRowHeader(StringBuilder strBuilder, string strHeading)
        {
            strBuilder.AppendLine("<Row>");
            strBuilder.AppendLine("<Cell ss:MergeAcross=\"8\"><Data ss:Type=\"String" + "\">" + strHeading + "</Data></Cell>");
            strBuilder.AppendLine("</Row>");
        }

        private static string FormatField(string data)
        {
            return String.Format("<Cell ss:StyleID=\"data\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", data);
        }

        private static string FormatHeaderField(string data)
        {
            return String.Format("<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", data);
        }

    }

    class PDFPageEvent : PdfPageEventHelper
    {
        public PDFPageEvent(string outagetime, string Header)
        {
            // TODO: Complete member initialization
            this.outagetime = outagetime;
            this.Header = Header;
        }
        public override void OnStartPage(PdfWriter writer, Document doc)
        {
            Font font_Arial11Bold = FontFactory.GetFont("arial", 11, Font.BOLD),
            font_Arial11Normal = FontFactory.GetFont("arial", 11, Font.NORMAL),
            font_Arial8_25Normal = FontFactory.GetFont("arial", 8.25f, Font.BOLD);

            string sTemp = "Interruption date:";

            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();//sTemp, fontBold11);
            doc.Add(paragraph);
            doc.Add(paragraph);
            Phrase phrase = new Phrase(sTemp, font_Arial11Bold);

            phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
            doc.Add(phrase);

            sTemp = this.outagetime;
            sTemp += ", weather permitting.";

            doc.Add(new Phrase(sTemp, font_Arial11Normal));
            doc.Add(paragraph);
            phrase = new Phrase("Interruption area:", font_Arial11Bold);
            phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
            doc.Add(phrase);

            doc.Add(new Phrase(this.Header, font_Arial11Normal));
            paragraph = new iTextSharp.text.Paragraph();
            doc.Add(paragraph);

            PdfPTable ptable = new PdfPTable(7);
            //float[] widths = new float[] { 25f, 100f, 150f, 85f, 50f, 50f, 50f };
            float[] widths = new float[] { 30f, 189f, 220f, 120f, 60f, 45f, 55f };
            ptable.SetWidths(widths);
            ptable.WidthPercentage = 100;
            ptable.DefaultCell.Border = 0;
            ptable.DefaultCell.BorderColor = BaseColor.WHITE;
            font_Arial11Bold.SetStyle(Font.BOLD);
            PdfPCell pcell = new PdfPCell(new Phrase("           Customer Notification List", font_Arial11Bold));
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

            pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8_25Normal));
            pcell.BorderColor = BaseColor.WHITE;
            pcell.Colspan = 7;
            ptable.AddCell(pcell);

            ptable.AddCell(new PdfPCell(new Phrase("Type", font_Arial8_25Normal)) { HorizontalAlignment = 0, BorderColor = BaseColor.WHITE });
            ptable.AddCell(new PdfPCell(new Phrase("Customer Name", font_Arial8_25Normal)) { HorizontalAlignment = 0, BorderColor = BaseColor.WHITE });
            ptable.AddCell(new PdfPCell(new Phrase("Address", font_Arial8_25Normal)) { HorizontalAlignment = 0, BorderColor = BaseColor.WHITE });
            ptable.AddCell(new PdfPCell(new Phrase("City, State, ZIP", font_Arial8_25Normal)) { HorizontalAlignment = 0, BorderColor = BaseColor.WHITE });
            ptable.AddCell(new PdfPCell(new Phrase("CGC/TNum", font_Arial8_25Normal)) { HorizontalAlignment = 2, BorderColor = BaseColor.WHITE });
            ptable.AddCell(new PdfPCell(new Phrase("SSD", font_Arial8_25Normal)) { HorizontalAlignment = 2, BorderColor = BaseColor.WHITE });
            ptable.AddCell(new PdfPCell(new Phrase("Phone", font_Arial8_25Normal)) { HorizontalAlignment = 2, BorderColor = BaseColor.WHITE });
            
            pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8_25Normal));
            pcell.Colspan = 7;
            ptable.AddCell(pcell);
            doc.Add(ptable);
        }

        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;
        private string outagetime;
        private string Header;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                //base.OnOpenDocument(writer, document);
                //document.Add(new iTextSharp.text.Phrase());
                //document.Add(new iTextSharp.text.Phrase());
                //bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
                bf = font_Arial10.GetCalculatedBaseFont(false);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
                //base.OnCloseDocument(writer, document);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
            bf = font_Arial10.GetCalculatedBaseFont(false);
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Page " + pageN + "/";
            float len = bf.GetWidthPoint(text, 10);
            Rectangle pageSize = document.PageSize;
            cb.SetRGBColorFill(0, 0, 0);
            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(pageSize.GetLeft(400), pageSize.GetBottom(15));
            cb.ShowText(text);
            //cb.ShowTextAligned(1, text, 0, pageSize.GetBottom(15), 0);
            cb.EndText();
            cb.AddTemplate(template, pageSize.GetLeft(400) + len, pageSize.GetBottom(15));
        }
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 10);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }
    }

    public class Avery5160
    {
        //Constants for Avery Address Label 5160
        private const float PAPER_SIZE_WIDTH = 816; //8.5" x 96
        private const float PAPER_SIZE_HEIGHT = 1056; //11" x 96

        private const float LABEL_WIDTH = 252; //2.625" x 96
        private const float LABEL_HEIGHT = 96; //1" x 96

        private const float LEFT_MARGIN = 18.24f; //0.19" x 96
        private const float TOP_MARGIN = 48; //0.5" x 96
        private const float BOTTOM_MARGIN = 48;
        private const float RIGHT_MARGIN = 18.24f;
        private const float HORIZONTAL_GAP = 15; //0.13" x 96

        private const int LABELS_PER_SHEET = 30; //3 columns of 10 labels
        private const int PAGECOLS = 3;
        private const int PAGEROWS = 10;

        public string CreateDocument(DataTable dt, int skips, string _strUserName)
        {
            //Create new document
            string strCompletePath = "";
            string strFileName = "";
            try
            {
                double count = (double)dt.Rows.Count + skips;
                Document pdfDoc = new Document();
                DateTime t1 = System.DateTime.Now;
                Rectangle rect = new Rectangle((float)PAPER_SIZE_WIDTH, (float)PAPER_SIZE_HEIGHT);
                pdfDoc = new Document(rect, LEFT_MARGIN, RIGHT_MARGIN, TOP_MARGIN, BOTTOM_MARGIN);
                string strFilePath = System.Configuration.ConfigurationManager.AppSettings["ExpportFileLocationNew"];
                string strFileURL = System.Configuration.ConfigurationManager.AppSettings["ExpportPDFURLNew"];
                strFileName = t1.Day.ToString() + t1.Month.ToString() + t1.Year.ToString() + t1.Hour.ToString() + t1.Minute.ToString() + t1.Second.ToString() + "_" + _strUserName;
                strCompletePath = strFilePath + strFileName + ".pdf";
                PdfWriter Writer = PdfWriter.GetInstance(pdfDoc, new FileStream(strCompletePath, FileMode.Create));
                pdfDoc.Open();
                
                if (dt != null)
                {
                    string line1 = "";
                    string line2 = "";
                    string line3 = "";

                    //Number of columns to create
                    var columnCount = 3;

                    //Distance be columns
                    var gutterWidth = HORIZONTAL_GAP;

                    //Setup and calculate some helper variables

                    //The left-most edge
                    var tableLeft = pdfDoc.LeftMargin;

                    //The bottom-most edge
                    var tableBottom = pdfDoc.BottomMargin;

                    //The available width and height of the table taking into account the pdfDocument's margins
                    var tableWidth = pdfDoc.PageSize.Width - (pdfDoc.LeftMargin + pdfDoc.RightMargin);
                    var tableHeight = pdfDoc.PageSize.Height - (pdfDoc.TopMargin + pdfDoc.BottomMargin);

                    //The width of a column taking into account the gutters (three columns have two gutters total)
                    var columnWidth = (tableWidth - (gutterWidth * (columnCount - 1))) / columnCount;

                    //Create an array of columns
                    var columns = new List<iTextSharp.text.Rectangle>();

                    //Create one rectangle per column
                    for (var i = 0; i < columnCount; i++)
                    {
                        columns.Add(new iTextSharp.text.Rectangle(
                                tableLeft + (columnWidth * i) + (gutterWidth * i),               //Left
                                tableBottom,                                                     //Bottom
                                tableLeft + (columnWidth * i) + (gutterWidth * i) + columnWidth, //Right
                                tableBottom + tableHeight                                        //Top
                                )
                           );
                    }

                    for (int i = 0; i < skips; i++)
                    {
                        DataRow blankRow = dt.NewRow();
                        blankRow["CustomerName"] = "";
                        blankRow["Address"] = "";
                        blankRow["City State Zip"] = "";
                        dt.Rows.InsertAt(blankRow, 0);
                    }
                    //Create our column text object
                    var ct = new ColumnText(Writer.DirectContent);

                    //Create and set some placeholder copy
                    foreach (DataRow row in dt.Rows)
                    {
                        line1 = row["CustomerName"].ToString();
                        //if (line1.Length == 10)
                        //{
                        //    line1 = StringTool.Truncate(line1, 10);
                        //}
                        line2 = row["Address"].ToString().ToUpper();
                        //Check Address cannot more than 20 char
                        //if (line2.Length >25)
                        //{
                        //    line2 = StringTool.Truncate(line2, 25);
                        //}
                        //else
                        {
                            line2 = row["Address"].ToString().ToUpper();
                        }

                        line3 = row["City State Zip"].ToString().ToUpper();
                        PdfPCell cell = new PdfPCell();
                        cell.FixedHeight = LABEL_HEIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        var contents = new iTextSharp.text.Paragraph();
                        contents.Alignment = Element.ALIGN_LEFT;
                        Font font = FontFactory.GetFont("Arial",11 , Font.NORMAL);
                        
                        Chunk chunker = new Chunk(line1 + "\n" + line2 + "\n" + line3);
                        PdfPTable table = new PdfPTable(1);
                        chunker.Font = font;
                        //table.DefaultCell.NoWrap = true;
                        //table.DefaultCell.FixedHeight = 12f;
                        //table.DefaultCell.Border = 0;
                        //table.DefaultCell.BorderColor = BaseColor.WHITE;
                        //table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //cell.FixedHeight = 12f;
                        //cell.NoWrap = false;
                        cell.AddElement(chunker);
                        cell.Border = 0;
                        table.AddCell(cell);
                        //cell.AddElement(new Phrase(line1));
                        //table.AddCell(new Phrase(line1));
                        ////cell.AddElement(new Phrase(line2));
                        //table.AddCell(new Phrase(line2));
                        ////cell.AddElement(new Phrase(line3));
                        //table.AddCell(new Phrase(line3));
                        ct.AddElement(table);

                    }

                    //As we draw content below we'll loop through each column defined above
                    //This holds the current column index that we'll change in the loop
                    var currentColumnIndex = 0;

                    //Current status as returned from ct.go()
                    int status = 0; //START_COLUMN is defined in Java but is currently missing in .Net

                    //Loop until we've drawn everything
                    while (ColumnText.HasMoreText(status))
                    {

                        //Setup our column
                        ct.SetSimpleColumn(columns[currentColumnIndex]);

                        //To be honest, not quite sure how this is used but I think it is related to leading
                        ct.YLine = columns[currentColumnIndex].Top;

                        //This actually "draws" the text and will return either 0, NO_MORE_TEXT (1) or NO_MORE_COLUMN(2)
                        status = ct.Go();

                        //Increment our current column index
                        currentColumnIndex += 1;

                        //If we're out of columns
                        if (currentColumnIndex > (columns.Count - 1))
                        {

                            //Create a new page and reset to the first column
                            pdfDoc.NewPage();
                            currentColumnIndex = 0;
                        }
                    }
                }
                pdfDoc.Close();
                strCompletePath = strFileURL + strFileName + ".pdf";
                return strCompletePath;
            }
            catch (Exception ex)
            {
                return strCompletePath;
            }
        }

    }

    public static class StringTool
    {
        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        public static string Truncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }

        public static List<String> Wrap(string text, int maxLength)
        {

            // Return empty list of strings if the text was empty
            if (text.Length == 0) return new List<string>();

            var words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = "";

            foreach (var currentWord in words)
            {

                if ((currentLine.Length > maxLength) ||
                    ((currentLine.Length + currentWord.Length) > maxLength))
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                if (currentLine.Length > 0)
                    currentLine += " " + currentWord;
                else
                    currentLine += currentWord;

            }

            if (currentLine.Length > 0)
                lines.Add(currentLine);


            return lines;
        }
        public static string WrapString(string text, int maxLength)
        {
            string returnString = "";

            foreach (string inputString in Wrap(text, maxLength))
            {
                returnString += inputString + "\n";
            }

            return returnString;
        }
    }
}

