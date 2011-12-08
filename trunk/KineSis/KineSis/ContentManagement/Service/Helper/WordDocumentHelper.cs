/*
   Copyright 2011 Alexandru Albu - sandu.albu@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KineSis.ContentManagement.Utils;
using KineSis.ContentManagement.Progress;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.PowerPoint;
using System.Windows;
using KineSis.ContentManagement.Model;
using System.IO;
using Microsoft.Office.Core;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.IO.Packaging;
using System.Threading;
using System.Drawing;

namespace KineSis.ContentManagement.Service.Helper
{

    /// <summary>
    /// Handles the word files (docx)
    /// Open the document, save it as xps, then save as html page, import shapes in a power point application and parse the charts and save them as images for building a 3D model
    /// </summary>
    class WordDocumentHelper : DocumentHelper
    {

        private static Microsoft.Office.Interop.Word._Application wordApplication;
        private static Microsoft.Office.Interop.PowerPoint.Application powerPointApplication;

        private static String DD = "\\"; //directory delimiter
        private static String DOC_FILES = "document_files"; //document_files
        private static String _ = "_";


        /// <summary>
        /// Open the PowerPoint application
        /// </summary>
        private void OpenOfficeApplication()
        {
            if (wordApplication == null)
            {
                wordApplication = new Microsoft.Office.Interop.Word.Application();
            }
            if (powerPointApplication == null)
            {
                powerPointApplication = new Microsoft.Office.Interop.PowerPoint.Application();
            }
        }

        /// <summary>
        /// Close the Powerpoint Application
        /// </summary>
        private void CloseOfficeApplication()
        {
            if (wordApplication != null)
            {
                wordApplication.Quit(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                wordApplication = null;
            }
            if (powerPointApplication != null)
            {
                powerPointApplication.Quit();
                powerPointApplication = null;
            }
        }

        /// <summary>
        /// parse a word document and build a kinesis document model
        /// </summary>
        /// <param name="path">full path of the word document</param>
        /// <returns>equivalent kinesis document model</returns>
        KineSis.ContentManagement.Model.Document DocumentHelper.ParseNewDocument(String path, ProcessingProgress pp)
        {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationName = "All Document Pages";
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            OpenOfficeApplication();


            //directory where all the data will be saved
            String folderName = DocumentUtil.GenerateDirectoryName();
            String documentPath = System.IO.Path.Combine(DocumentService.TEMP_DIRECTORY, folderName);
            System.IO.Directory.CreateDirectory(documentPath);

            // Make this instance of word invisible (Can still see it in the taskmgr).
            wordApplication.Visible = false;

            // Interop requires objects.
            object oMissing = System.Reflection.Missing.Value;
            object isVisible = false;
            object readOnly = false;
            object oInput = path;
            object oOutput = documentPath + DD + DOC_FILES + DD + "document.xps";
            object oFormat = WdSaveFormat.wdFormatXPS;

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Opening MS Office";
            pp.CurrentOperationTotalElements = 1;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            // Load a document into our instance of word.exe
            Microsoft.Office.Interop.Word._Document wdoc = wordApplication.Documents.Open(ref oInput, ref oMissing, ref readOnly, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref isVisible, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            // Make this document the active document.
            wdoc.Activate();

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            KineSis.ContentManagement.Model.Document document = new KineSis.ContentManagement.Model.Document();
            document.Name = wdoc.Name;
            document.Location = folderName;

            //write directory for pages
            String pagesPath = documentPath + DD + DOC_FILES;
            System.IO.Directory.CreateDirectory(pagesPath);

            //create a new page
            KineSis.ContentManagement.Model.Page page = new KineSis.ContentManagement.Model.Page();
            page.Name = wdoc.Name;
            page.Location = documentPath + DD + DOC_FILES + DD + "document.html";

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Saving document";
            pp.CurrentOperationTotalElements = 2;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            // Save this document in XPS format.
            wdoc.SaveAs(ref oOutput, ref oFormat, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            wdoc.Close(SaveChanges: false);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 2;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            try
            {
                wdoc.Close(SaveChanges: false);
            }
            catch (Exception)
            {
            }
            //CloseOfficeApplication();
            //wordApplication.Quit(ref oMissing, ref oMissing, ref oMissing);

            BuildDocumentHTMLArgs args = new BuildDocumentHTMLArgs(documentPath + DD + DOC_FILES, pp);

            Thread thread = new Thread(new ParameterizedThreadStart(BuildDocumentHTML));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(args);
            thread.Join();

            //build a html page from saved xps
            //BuildDocumentHTML(documentPath + DD + DOC_FILES, pp);

            //add page to document model
            document.Pages.Add(page);

            //delete the generated xps file
            FileInfo fi = new FileInfo(documentPath + DD + DOC_FILES + DD + "document.xps");
            fi.Delete();

            //return built document
            return document;
        }

        class BuildDocumentHTMLArgs
        {
            public String path;
            public ProcessingProgress pp;

            public BuildDocumentHTMLArgs(String path, ProcessingProgress pp)
            {
                this.path = path;
                this.pp = pp;
            }
        }

        public void ClearClipboard()
        {
            Clipboard.Clear();
        }

        /// <summary>
        /// parse a word document and build a kinesis document model
        /// </summary>
        /// <param name="path">full path of the word document</param>
        /// <returns>equivalent kinesis document model</returns>
        public KineSis.ContentManagement.Model.Document ParseNewDocumentCharts(String path, ProcessingProgress pp, KineSis.ContentManagement.Model.Document document)
        {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationName = "All Document Charts";
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //directory where all the data will be saved
            String folderName = document.Location;
            String documentPath = System.IO.Path.Combine(DocumentService.TEMP_DIRECTORY, folderName);

            // Make this instance of word invisible (Can still see it in the taskmgr).
            wordApplication.Visible = false;

            // Interop requires objects.
            object oMissing = System.Reflection.Missing.Value;
            object isVisible = false;
            object readOnly = false;
            object oInput = path;
            object oOutput = documentPath + DD + DOC_FILES + DD + "document.xps";
            object oFormat = WdSaveFormat.wdFormatXPS;

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Opening MS Office";
            pp.CurrentOperationTotalElements = 1;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            // Load a document into our instance of word.exe
            Microsoft.Office.Interop.Word._Document wdoc = wordApplication.Documents.Open(ref oInput, ref oMissing, ref readOnly, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref isVisible, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            // Make this document the active document.
            wdoc.Activate();

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //KineSis.ContentManagement.Model.Document document = new KineSis.ContentManagement.Model.Document();
            document.Name = wdoc.Name;
            document.Location = folderName;

            //create a new page
            KineSis.ContentManagement.Model.Page page = document.Pages[0];


            //check if chart generation is wanted
            if (DocumentService.CHART_HORIZONTAL_FACES > 0)
            {

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationName = "Transforming";
                pp.CurrentOperationTotalElements = 4;
                pp.CurrentOperationElement = 0;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                Thread thread = new Thread(new ThreadStart(ClearClipboard));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

                //handle the charts
                wdoc.Shapes.SelectAll();

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 1;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                wdoc.ActiveWindow.Selection.Copy(); //copy all shapes

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 2;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                //open a new powerpoint application
                Presentation presentation = powerPointApplication.Presentations.Add();

                //paste all copied shapes
                presentation.SlideMaster.Shapes.Paste();

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 3;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                //clear the clipboard
                //Clipboard.Clear();
                Thread thread1 = new Thread(new ThreadStart(ClearClipboard));
                thread1.SetApartmentState(ApartmentState.STA);
                thread1.Start();
                thread1.Join();

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 4;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                List<Microsoft.Office.Interop.PowerPoint.Shape> charts = new List<Microsoft.Office.Interop.PowerPoint.Shape>();

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.OverallOperationTotalElements = EvaluatePresentation(presentation, pp);
                pp.OverallOperationElement = 0;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                //get all charts
                for (int j = 1; j <= presentation.SlideMaster.Shapes.Count; j++)
                {
                    Microsoft.Office.Interop.PowerPoint.Shape shape = presentation.SlideMaster.Shapes[j];
                    if (shape.HasChart == Microsoft.Office.Core.MsoTriState.msoTrue)
                    {
                        charts.Add(shape);
                    }
                }

                //create directory for charts
                String chartPath = System.IO.Path.Combine(documentPath, "charts");
                System.IO.Directory.CreateDirectory(chartPath);

                //for every chart
                for (int j = 0; j < charts.Count; j++)
                {

                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                    pp.CurrentOperationName = "Chart " + (j + 1) + " of " + charts.Count;
                    pp.CurrentOperationTotalElements = EvaluateChart(charts.ElementAt(j).Chart);
                    pp.CurrentOperationElement = 0;
                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                    KineSis.ContentManagement.Model.Chart mChart = new KineSis.ContentManagement.Model.Chart();
                    Microsoft.Office.Interop.PowerPoint.Shape chart = charts.ElementAt(j);

                    mChart.SetThumbnailUrl(GenerateThumbnail(chart, chartPath + DD + j + "_thumb"));

                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                    pp.OverallOperationElement++;
                    pp.CurrentOperationElement++;
                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                    //set preferred width and height
                    chart.Height = ((float)DocumentService.CHART_WIDTH * chart.Height) / chart.Width;
                    chart.Width = DocumentService.CHART_WIDTH;

                    //get chart type
                    int chartType = GetChartType(chart.Chart);

                    //reset rotation
                    chart.Chart.Rotation = 0;

                    int horizontalAngle = 0;

                    //depending on how many horizontal faces are required, calculate the angle between them
                    if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                    {
                        horizontalAngle = 360 / DocumentService.CHART_HORIZONTAL_FACES;
                    }

                    int verticalAngle = 0;

                    //depending on how many vertical faces are required for a horizontal face, celaculate the angle between them, excluding the vertical face at 90 degrees
                    if (DocumentService.CHART_VERTICAL_FACES > 0)
                    {
                        verticalAngle = 90 / (DocumentService.CHART_VERTICAL_FACES + 1);
                    }

                    if (chart.Chart.HasTitle)
                    {
                        mChart.Title = chart.Chart.ChartTitle.Caption;
                    }
                    else
                    {
                        mChart.Title = chart.Name;
                    }

                    //does not support rotation (it's plain)
                    if (chartType == 0)
                    {

                        //if horizontal faces number is 0, then no chart will be outputed
                        if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                        {

                            ChartHorizontalView hView = new ChartHorizontalView();
                            //draw chart face as image
                            chart.Export(chartPath + DD + j + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                            //add to hView
                            hView.ImageUrl = chartPath + DD + j + DocumentService.IMAGE_EXTENSION;
                            //add to views
                            mChart.Views.Add(hView);

                            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                            pp.OverallOperationElement++;
                            pp.CurrentOperationElement++;
                            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                        }
                    }
                    else
                    {

                        //for every horizontal face
                        for (int k = 0; k < DocumentService.CHART_HORIZONTAL_FACES; k++)
                        {

                            ChartHorizontalView hView = new ChartHorizontalView();
                            //reset elevation
                            chart.Chart.Elevation = 0;
                            //export face as image
                            chart.Export(chartPath + DD + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                            //set bitmap to view
                            hView.ImageUrl = chartPath + DD + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION;

                            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                            pp.OverallOperationElement++;
                            pp.CurrentOperationElement++;
                            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                            //for every vertical face
                            for (int l = 0; l < DocumentService.CHART_VERTICAL_FACES; l++)
                            {
                                ChartVerticalView vView = new ChartVerticalView();

                                //increse elevation
                                chart.Chart.Elevation += verticalAngle;
                                //export face as image
                                chart.Export(chartPath + DD + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                                //set bitmap to view
                                vView.ImageUrl = chartPath + DD + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION;
                                //add vertical view to horizontal UP list
                                hView.Up.Add(vView);

                                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                pp.OverallOperationElement++;
                                pp.CurrentOperationElement++;
                                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                            }

                            //some chart types, like 3D pie, does not support elevation less than 0
                            if (SupportsNegativeElevation(chart.Chart))
                            {

                                //reset elevation
                                chart.Chart.Elevation = 0;

                                //for every vertical face
                                for (int m = 0; m < DocumentService.CHART_VERTICAL_FACES; m++)
                                {
                                    ChartVerticalView vView = new ChartVerticalView();

                                    //decrease elevation
                                    chart.Chart.Elevation -= verticalAngle;
                                    //export face as image
                                    chart.Export(chartPath + DD + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                                    //set bitmap to vertical view
                                    vView.ImageUrl = chartPath + DD + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION;
                                    //add vertical view to horizontal view DOWN list
                                    hView.Down.Add(vView);

                                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                    pp.OverallOperationElement++;
                                    pp.CurrentOperationElement++;
                                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                }
                            }
                            //increase horizontal angle in order to get the next horizontal view
                            chart.Chart.Rotation += horizontalAngle;
                            //add horizontal view to the chat's views list
                            mChart.Views.Add(hView);
                        }
                    }
                    //add chart to page
                    page.Charts.Add(mChart);
                }

                //close presentation
                presentation.Close();
            }

            wdoc.Close(SaveChanges: false);

            CloseOfficeApplication();

            //return built document
            return document;
        }


        /// <summary>
        /// Evaluate a chart in order to determine the number of operations required for exporting it
        /// </summary>
        /// <param name="c">chart</param>
        /// <returns></returns>
        private int EvaluateChart(Microsoft.Office.Interop.PowerPoint.Chart c)
        {
            int numberOfOperations = 1;

            //get chart type
            int chartType = GetChartType(c);

            //does not support rotation (it's plain)
            if (chartType == 0)
            {

                //if horizontal faces number is 0, then no chart will be outputed
                if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                {
                    numberOfOperations++;
                }
            }
            else
            {

                //for every horizontal face
                for (int k = 0; k < DocumentService.CHART_HORIZONTAL_FACES; k++)
                {

                    ChartHorizontalView hView = new ChartHorizontalView();

                    numberOfOperations++;

                    //for every vertical face
                    for (int l = 0; l < DocumentService.CHART_VERTICAL_FACES; l++)
                    {
                        numberOfOperations++;
                    }

                    //some chart types, like 3D pie, does not support elevation less than 0
                    if (SupportsNegativeElevation(c))
                    {

                        //for every vertical face
                        for (int m = 0; m < DocumentService.CHART_VERTICAL_FACES; m++)
                        {
                            numberOfOperations++;
                        }
                    }
                }
            }

            return numberOfOperations;
        }

        /// <summary>
        /// Evaluate the presentation in order to determine the number of operations required for exporting it
        /// </summary>
        /// <param name="presentation">presentation</param>
        /// <param name="pp">processing progress</param>
        /// <returns></returns>
        private int EvaluatePresentation(Presentation presentation, ProcessingProgress pp)
        {

            int numberOfOperations = 0;


            List<Microsoft.Office.Interop.PowerPoint.Shape> charts = new List<Microsoft.Office.Interop.PowerPoint.Shape>();

            //get all shapes and charts
            for (int j = 1; j <= presentation.SlideMaster.Shapes.Count; j++)
            {
                Microsoft.Office.Interop.PowerPoint.Shape shape = presentation.SlideMaster.Shapes[j];
                if (shape.HasChart == Microsoft.Office.Core.MsoTriState.msoTrue)
                {
                    charts.Add(shape);
                }
            }

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Evaluating document";
            pp.CurrentOperationTotalElements = charts.Count;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //for every chart
            for (int j = 0; j < charts.Count; j++)
            {

                Microsoft.Office.Interop.PowerPoint.Shape chart = charts.ElementAt(j);

                numberOfOperations++;

                //get chart type
                int chartType = GetChartType(chart.Chart);

                //does not support rotation (it's plain)
                if (chartType == 0)
                {

                    //if horizontal faces number is 0, then no chart will be outputed
                    if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                    {
                        numberOfOperations++;
                    }
                }
                else
                {

                    //for every horizontal face
                    for (int k = 0; k < DocumentService.CHART_HORIZONTAL_FACES; k++)
                    {
                        numberOfOperations++;

                        //for every vertical face
                        for (int l = 0; l < DocumentService.CHART_VERTICAL_FACES; l++)
                        {
                            numberOfOperations++;
                        }

                        //some chart types, like 3D pie, does not support elevation less than 0
                        if (SupportsNegativeElevation(chart.Chart))
                        {

                            //for every vertical face
                            for (int m = 0; m < DocumentService.CHART_VERTICAL_FACES; m++)
                            {
                                numberOfOperations++;
                            }
                        }
                    }
                }
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = j + 1;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            }
            return numberOfOperations;
        }

        /// <summary>
        /// geterate thumbnail for a shape or a chart
        /// </summary>
        /// <param name="shape"> desired shape</param>
        /// <param name="path">path where the thumbnail will be created, excluding the .extension</param>
        /// <returns>full path of the created thumbnail</returns>
        private String GenerateThumbnail(Microsoft.Office.Interop.PowerPoint.Shape shape, String path)
        {
            shape.Export(path + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);

            Bitmap bmp = new Bitmap(path + DocumentService.IMAGE_EXTENSION);

            int T_H, T_W;

            if (bmp.Width <= bmp.Height)
            {
                T_W = (int)(DocumentService.THUMB_WIDTH * bmp.Width / bmp.Height);
                T_H = DocumentService.THUMB_WIDTH;
            }
            else
            {
                T_H = (int)(DocumentService.THUMB_WIDTH * bmp.Height / bmp.Width);
                T_W = DocumentService.THUMB_WIDTH;
            }

            System.Drawing.Size s = new System.Drawing.Size(T_W, T_H);

            Bitmap bmp1 = new Bitmap(bmp, s);
            bmp.Dispose();

            FileInfo fi = new FileInfo(path + DocumentService.IMAGE_EXTENSION);
            fi.Delete();

            bmp1.Save(path + DocumentService.IMAGE_EXTENSION);

            return path + DocumentService.IMAGE_EXTENSION;
        }

        /// <summary>
        /// check if a powerpoint chart can be down elevated
        /// </summary>
        /// <param name="chart">powerpoint chart</param>
        /// <returns></returns>
        private Boolean SupportsNegativeElevation(Microsoft.Office.Interop.PowerPoint.Chart chart)
        {
            Boolean result = true;
            int originalElevation = chart.Elevation;
            try
            {
                chart.Elevation = -45;
                chart.Elevation = originalElevation;
            }
            catch (Exception)
            {
                chart.Elevation = originalElevation;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// determines the type of a powerpoint chart
        /// </summary>
        /// <param name="chart">powerpoint chart</param>
        /// <returns>chart type, 0 = 2D chart, 1 = 3D bar chart, 2 = 3D chart which can be fully rotated</returns>
        private int GetChartType(Microsoft.Office.Interop.PowerPoint.Chart chart)
        {
            int result = 0;
            if ((chart.ChartType == XlChartType.xl3DArea) ||
                (chart.ChartType == XlChartType.xl3DAreaStacked) ||
                (chart.ChartType == XlChartType.xl3DAreaStacked100) ||
                (chart.ChartType == XlChartType.xl3DColumn) ||
                (chart.ChartType == XlChartType.xl3DColumnClustered) ||
                (chart.ChartType == XlChartType.xl3DColumnStacked) ||
                (chart.ChartType == XlChartType.xl3DColumnStacked100) ||
                (chart.ChartType == XlChartType.xl3DLine) ||
                (chart.ChartType == XlChartType.xl3DPie) ||
                (chart.ChartType == XlChartType.xl3DPieExploded) ||
                (chart.ChartType == XlChartType.xlConeCol) ||
                (chart.ChartType == XlChartType.xlConeColClustered) ||
                (chart.ChartType == XlChartType.xlConeColStacked) ||
                (chart.ChartType == XlChartType.xlConeColStacked100) ||
                (chart.ChartType == XlChartType.xlConeBarClustered) ||
                (chart.ChartType == XlChartType.xlConeBarStacked) ||
                (chart.ChartType == XlChartType.xlConeBarStacked100) ||
                (chart.ChartType == XlChartType.xlCylinderCol) ||
                (chart.ChartType == XlChartType.xlCylinderColClustered) ||
                (chart.ChartType == XlChartType.xlCylinderColStacked) ||
                (chart.ChartType == XlChartType.xlCylinderColStacked100) ||
                (chart.ChartType == XlChartType.xlCylinderBarClustered) ||
                (chart.ChartType == XlChartType.xlCylinderBarStacked) ||
                (chart.ChartType == XlChartType.xlCylinderBarStacked100) ||
                (chart.ChartType == XlChartType.xlPyramidCol) ||
                (chart.ChartType == XlChartType.xlPyramidColClustered) ||
                (chart.ChartType == XlChartType.xlPyramidColStacked) ||
                (chart.ChartType == XlChartType.xlPyramidColStacked100) ||
                (chart.ChartType == XlChartType.xlPyramidBarClustered) ||
                (chart.ChartType == XlChartType.xlPyramidBarStacked) ||
                (chart.ChartType == XlChartType.xlPyramidBarStacked100) ||
                (chart.ChartType == XlChartType.xlSurface) ||
                (chart.ChartType == XlChartType.xlSurfaceTopView) ||
                (chart.ChartType == XlChartType.xlSurfaceTopViewWireframe) ||
                (chart.ChartType == XlChartType.xlSurfaceWireframe))
            {
                result = 2;
            }
            else if ((chart.ChartType == XlChartType.xl3DBarClustered) ||
              (chart.ChartType == XlChartType.xl3DBarStacked) ||
              (chart.ChartType == XlChartType.xl3DBarStacked100))
            {
                result = 1;
            }

            return result;
        }

        /// <summary>
        /// build a html document based on an xps file
        /// </summary>
        /// <param name="path">path where the xps will be found and the html will be generated</param>
        private void BuildDocumentHTML(object args)
        {

            BuildDocumentHTMLArgs bdha = args as BuildDocumentHTMLArgs;
            String path = bdha.path;
            ProcessingProgress pp = bdha.pp;

            XpsDocument xpsDoc = new XpsDocument(path + DD + "document.xps", System.IO.FileAccess.Read, CompressionOption.Normal);

            FixedDocumentSequence docSeq = xpsDoc.GetFixedDocumentSequence();
            System.IO.StreamWriter fileO = new System.IO.StreamWriter(path + DD + "document.html", false);
            fileO.WriteLine("<html><body>");

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationTotalElements = docSeq.DocumentPaginator.PageCount;
            pp.OverallOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            // You can get the total page count from docSeq.PageCount
            for (int pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
            {

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationName = "Page " + (pageNum + 1);
                pp.CurrentOperationTotalElements = 3;
                pp.CurrentOperationElement = 0;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                DocumentPage docPage = docSeq.DocumentPaginator.GetPage(pageNum);
                BitmapImage bitmap = new BitmapImage();
                RenderTargetBitmap renderTarget =
                    new RenderTargetBitmap((int)docPage.Size.Width,
                                            (int)docPage.Size.Height,
                                            96, // WPF (Avalon) units are 96dpi based
                                            96,
                                            System.Windows.Media.PixelFormats.Default);

                renderTarget.Render(docPage.Visual);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 1;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                FileStream pageOutStream = new FileStream(path + DD + "page" + (pageNum + 1) + ".png", FileMode.Create, FileAccess.Write);
                encoder.Save(pageOutStream);
                pageOutStream.Close();

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 2;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                fileO.WriteLine("<div align=\"center\"><img src=\"" + path + DD + "page" + (pageNum + 1) + ".png\"/></div>");

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 3;
                pp.OverallOperationElement = pageNum + 1;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            }
            fileO.WriteLine("</body></html>");
            fileO.Flush();
            fileO.Close();
            docSeq = null;
            xpsDoc.Close();
            xpsDoc = null;
        }
    }
}
