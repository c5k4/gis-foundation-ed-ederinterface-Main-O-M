using System;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections;

namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS
{
    /// <summary>
    /// Ambassador Address Label – Avery 5160
    /// </summary>
    public class Avery5160
    {
        //Constants for Avery Address Label 5160
        private const double PAPER_SIZE_WIDTH = 816; //8.5" x 96
        private const double PAPER_SIZE_HEIGHT = 1056; //11" x 96

        private const double LABEL_WIDTH = 252; //2.625" x 96
        private const double LABEL_HEIGHT = 96; //1" x 96

        private const double SIDE_MARGIN = 18.24; //0.19" x 96
        private const double TOP_MARGIN = 48; //0.5" x 96
        private const double HORIZONTAL_GAP = 12.48; //0.13" x 96

        private const double LABELS_PER_SHEET = 30; //3 columns of 10 labels
        
        
        private FixedPage CreatePage()
        {
            //Create new page
            FixedPage page = new FixedPage();
            //Set background
            page.Background = Brushes.White;
            //Set page size (Letter size)
            page.Width = PAPER_SIZE_WIDTH;
            page.Height = PAPER_SIZE_HEIGHT;
            return page;
        }

        public FixedDocument CreateDocument(DataTable data)
        { 
            //Create new document
            FixedDocument doc = new FixedDocument();
            //Set page size
            doc.DocumentPaginator.PageSize = new Size(PAPER_SIZE_WIDTH, PAPER_SIZE_HEIGHT);

            //Number of records
            double count = (double)data.Rows.Count;
                        
            if(count > 0)
            {
                //string line1 = "";
                //string line2 = "";
                //string line3 = "";
                //string postalCode = "";

                AveryLabel label;

                //Determine number of pages to generate
                double pageCount = Math.Ceiling(count / LABELS_PER_SHEET);

                int dataIndex = 0;
                int currentColumn = 0;
                int currentRow = 0;

                for (int i = 0; i < pageCount; i++)
                {
                    //Create page
                    PageContent page = new PageContent();
                    FixedPage fixedPage = this.CreatePage();
                    //Create labels
                    for (int j = 0; j < 30; j++)
                    {
                        if (j % 10 == 0)
                        {
                            currentRow = 0;
                        }
                        else
                        {
                            currentRow++;
                        }

                        if (j < 10)
                        {
                            currentColumn = 0;
                        }
                        else if (j > 19)
                        {
                            currentColumn = 2;
                        }
                        else
                        {
                            currentColumn = 1;
                        }

                        string[] line = new string[] { "", "", "", "", "" };
                        ArrayList list = new ArrayList();
                        if (dataIndex < count)
                        {
                            //Get data from DataTable
                            if (Convert.ToString(data.Rows[dataIndex]["Name"]).Contains("^"))
                            {
                                list.Add(Convert.ToString(data.Rows[dataIndex]["Name"]).Split('^')[0]);
                                list.Add(Convert.ToString(data.Rows[dataIndex]["Name"]).Split('^')[1]);
                            }
                            else
                            {
                                list.Add(Convert.ToString(data.Rows[dataIndex]["Name"]));
                            }

                            if (Convert.ToString(data.Rows[dataIndex]["Address"]).Contains("^"))
                            {
                                list.Add(Convert.ToString(data.Rows[dataIndex]["Address"]).Split('^')[0]);
                                list.Add(Convert.ToString(data.Rows[dataIndex]["Address"]).Split('^')[1]);
                            }
                            else
                            {
                                list.Add(Convert.ToString(data.Rows[dataIndex]["Address"]));
                            }
                            list.Add(Convert.ToString(data.Rows[dataIndex]["City, State, ZIP"]));

                            int icount_array = 0;
                            for (int icount_list = 0; icount_list < list.Count; ++icount_list)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(list[icount_list]).Trim())) continue;
                                line[icount_array] = Convert.ToString(list[icount_list]);
                                ++icount_array;
                            }
                            label = new AveryLabel(line);
                            //line1 = (string)data.Rows[dataIndex]["Name"];
                            //line2 = (string)data.Rows[dataIndex]["Address"];
                            ////postalCode = (string)data.Rows[dataIndex]["PostalCode"];
                            //line3 = (string)data.Rows[dataIndex]["City, State, ZIP"];// +" " + (string)data.Rows[dataIndex]["State"] + " " + postalCode;
                            ////Create individual label
                            //if (line1.Contains("^"))
                            //    label = new AveryLabel(line1.Split('^')[0], line1.Split('^')[1], line2, line3);
                            //else
                            //    label = new AveryLabel(line1, line2, line3);
                            ////Set label location
                            if (currentColumn == 0)
                            {
                                FixedPage.SetLeft(label, SIDE_MARGIN);
                            }
                            else if (currentColumn == 1)
                            {
                                FixedPage.SetLeft(label, SIDE_MARGIN + LABEL_WIDTH + HORIZONTAL_GAP);
                            }
                            else
                            {
                                FixedPage.SetLeft(label, SIDE_MARGIN + LABEL_WIDTH * 2 + HORIZONTAL_GAP * 2);
                            }
                            FixedPage.SetTop(label, TOP_MARGIN + currentRow * LABEL_HEIGHT); 

                            //Add label object to page
                            fixedPage.Children.Add(label);

                            dataIndex++;
                        }
                    }
                    
                    //Invoke Measure(), Arrange() and UpdateLayout() for drawing
                    fixedPage.Measure(new Size(PAPER_SIZE_WIDTH, PAPER_SIZE_HEIGHT));
                    fixedPage.Arrange(new Rect(new Point(), new Size(PAPER_SIZE_WIDTH, PAPER_SIZE_HEIGHT)));
                    fixedPage.UpdateLayout();

                    ((IAddChild)page).AddChild(fixedPage);

                    doc.Pages.Add(page);
                }
            }

            return doc;
        }
    }
}
