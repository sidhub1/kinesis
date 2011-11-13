/*
   Copyright 2011 Alexandru Albu - http://code.google.com/p/kinesis/

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
using KineSis.ContentManagement.Model;
using KineSis.ContentManagement.Progress;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Drawing;
using System.IO;

namespace KineSis.ContentManagement.Service.Helper
{

    /// <summary>
    /// Handles the powerpoint files (pptx)
    /// Open the document, save slides as images, then build a html page for each one, and for every slide parse the charts and shapes and save them as images for building a 3D model
    /// </summary>
    class PowerPointDocumentHelper : DocumentHelper
    {

        /// <summary>
        /// The application is static in order to be used across the helper's methods.
        /// It will be opened at the begining of parsing and closed only after all charts and shapes are successfully exported.
        /// Exporting process will be made in two steps in order to be able to start presenting right after only slides were exported, 
        /// and the charts will be exported in background in parallel with presentation, so the user can use them as soon they are ready without blocking the rest of presentation
        /// </summary>
        private static Microsoft.Office.Interop.PowerPoint.Application powerPointApplication = null;

        private static String DD = "\\"; //directory delimiter
        private static String DOC_FILES = "document_files"; //document_files
        private static String _ = "_";
        private static String SLIDE = "slide";

        /// <summary>
        /// Open the PowerPoint application
        /// </summary>
        private void OpenOfficeApplication()
        {
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
            if (powerPointApplication != null)
            {
                powerPointApplication.Quit();
                powerPointApplication = null;
            }
        }

        /// <summary>
        /// parse a power point document and build a kinesis document model
        /// </summary>
        /// <param name="path">full path of the power point document</param>
        /// <param name="pp">processing progress reporter</param>
        /// <returns>equivalent kinesis document model</returns>
        Document DocumentHelper.ParseNewDocument(String path, ProcessingProgress pp)
        {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationName = "All Document Pages";
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            OpenOfficeApplication();

            //directory where all the data will be saved
            String folderName = DocumentUtil.GenerateDirectoryName();
            String documentPath = System.IO.Path.Combine(DocumentService.TEMP_DIRECTORY, folderName);
            System.IO.Directory.CreateDirectory(documentPath);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Opening MS Office";
            pp.CurrentOperationTotalElements = 1;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //open the presentation
            Presentation presentation = powerPointApplication.Presentations.Open(path, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);

            Document document = new Document();
            document.Name = presentation.Name;
            document.Location = folderName;

            //write directory for pages
            String pagesPath = System.IO.Path.Combine(documentPath + DD + DOC_FILES);
            System.IO.Directory.CreateDirectory(pagesPath);

            int H = (int)(DocumentService.SLIDE_WIDTH * presentation.SlideMaster.Height / presentation.SlideMaster.Width);
            int W = DocumentService.SLIDE_WIDTH;

            int T_H = (int)(DocumentService.THUMB_WIDTH * H / W);
            int T_W = DocumentService.THUMB_WIDTH;

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            pp.OverallOperationTotalElements = presentation.Slides.Count;
            pp.OverallOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //for every slide
            for (int i = 1; i <= presentation.Slides.Count; i++)
            {

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationName = "Page " + i;
                pp.CurrentOperationTotalElements = 4;
                pp.CurrentOperationElement = 0;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                //get slide
                Slide slide = presentation.Slides[i];

                //create a new page
                KineSis.ContentManagement.Model.Page page = new KineSis.ContentManagement.Model.Page();
                page.Name = slide.Name;

                //export the slide as image
                slide.Export(documentPath + DD + DOC_FILES + DD + SLIDE + i + ImageUtil.PNG_EXTENSION, ImageUtil.PNG_FILTER, W, H);

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 1;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                //export the slide as image
                slide.Export(documentPath + DD + DOC_FILES + DD + SLIDE + i + "_thumb" + ImageUtil.PNG_EXTENSION, ImageUtil.PNG_FILTER, T_W, T_H);

                page.SetThumbnailUrl(documentPath + DD + DOC_FILES + DD + SLIDE + i + "_thumb" + ImageUtil.PNG_EXTENSION);

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 2;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                //html page location, including the no-zoom version
                page.Location = GenerateHtmlPage(documentPath + DD + DOC_FILES + DD + SLIDE + i);

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 3;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                page.LocationNoZoom = GenerateHtmlPageNoZoom(documentPath + DD + DOC_FILES + DD + SLIDE + i);

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 4;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                //add page to document model
                document.Pages.Add(page);

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.OverallOperationElement = i;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            }

            //close presentation
            presentation.Close();

            //return built document
            return document;
        }

        /// <summary>
        /// parse a power point document and export all charts and shapes
        /// </summary>
        /// <param name="path">full path of the power point document</param>
        /// <returns>equivalent kinesis document model</returns>
        public Document ParseNewDocumentCharts(String path, ProcessingProgress pp, Document document)
        {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationName = "All Document Charts and Shapes";
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //directory where all the data will be saved
            String folderName = document.Location;
            String documentPath = System.IO.Path.Combine(DocumentService.TEMP_DIRECTORY, folderName);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Opening MS Office";
            pp.CurrentOperationTotalElements = 1;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //open the presentation
            Presentation presentation = powerPointApplication.Presentations.Open(path, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            pp.OverallOperationTotalElements = EvaluatePresentation(presentation, pp);
            pp.OverallOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //for every slide
            for (int i = 1; i <= presentation.Slides.Count; i++)
            {

                //get slide
                Slide slide = presentation.Slides[i];

                //create a new page
                KineSis.ContentManagement.Model.Page page = document.Pages[i - 1];

                //check if chart generation is wanted
                if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                {

                    List<Microsoft.Office.Interop.PowerPoint.Shape> charts = new List<Microsoft.Office.Interop.PowerPoint.Shape>();

                    //get all shapes and charts
                    for (int j = 1; j <= slide.Shapes.Count; j++)
                    {
                        Microsoft.Office.Interop.PowerPoint.Shape shape = slide.Shapes[j];
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
                        pp.CurrentOperationName = "Page " + i + " / Chart " + (j + 1) + " of " + charts.Count;
                        pp.CurrentOperationTotalElements = EvaluateChart(charts.ElementAt(j).Chart);
                        pp.CurrentOperationElement = 0;
                        //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                        KineSis.ContentManagement.Model.Chart mChart = new KineSis.ContentManagement.Model.Chart();
                        Microsoft.Office.Interop.PowerPoint.Shape chart = charts.ElementAt(j);

                        mChart.SetThumbnailUrl(GenerateThumbnail(chart, chartPath + DD + i + _ + j + "_thumb"));

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
                                chart.Export(chartPath + DD + i + _ + j + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                                //add to hView
                                hView.ImageUrl = chartPath + DD + i + _ + j + DocumentService.IMAGE_EXTENSION;
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
                                chart.Export(chartPath + DD + i + _ + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                                //set bitmap to view
                                hView.ImageUrl = chartPath + DD + i + _ + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION;

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
                                    chart.Export(chartPath + DD + i + _ + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                                    //set bitmap to view
                                    vView.ImageUrl = chartPath + DD + i + _ + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION;
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
                                        chart.Export(chartPath + DD + i + _ + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FORMAT);
                                        //set bitmap to vertical view
                                        vView.ImageUrl = chartPath + DD + i + _ + j + _ + chart.Chart.Rotation + _ + chart.Chart.Elevation + DocumentService.IMAGE_EXTENSION;
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
                }
            }

            //close presentation
            presentation.Close();

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

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Evaluating document";
            pp.CurrentOperationTotalElements = presentation.Slides.Count;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            int numberOfOperations = 0;

            //for every slide
            for (int i = 1; i <= presentation.Slides.Count; i++)
            {

                //get slide
                Slide slide = presentation.Slides[i];

                //check if chart generation is wanted
                if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                {

                    List<Microsoft.Office.Interop.PowerPoint.Shape> charts = new List<Microsoft.Office.Interop.PowerPoint.Shape>();

                    //get all shapes and charts
                    for (int j = 1; j <= slide.Shapes.Count; j++)
                    {
                        Microsoft.Office.Interop.PowerPoint.Shape shape = slide.Shapes[j];
                        if (shape.HasChart == Microsoft.Office.Core.MsoTriState.msoTrue)
                        {
                            charts.Add(shape);
                        }
                    }

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
                    }
                }

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = i;
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

            Size s = new Size(T_W, T_H);

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
        /// create a new html document having generated slide picture as content
        /// </summary>
        /// <param name="partialName">full name of the picture without extension</param>
        /// <returns>full path to the generated html page</returns>
        private String GenerateHtmlPage(String partialName)
        {
            System.IO.StreamWriter fileO = new System.IO.StreamWriter(partialName + ".html", false);
            fileO.WriteLine("<html><body><div align=\"center\"><img src=\"" + partialName + ".png\"/></div></body></html>");
            fileO.Flush();
            fileO.Close();

            return partialName + ".html";
        }

        /// <summary>
        /// create a new html document having generated slide picture as content
        /// include some javascript functions and css styling which always scale the picture to browser's width, which makes zoom possibility unavailable
        /// </summary>
        /// <param name="partialName">full name of the picture without extension</param>
        /// <returns>full path to the generated html page</returns>
        private String GenerateHtmlPageNoZoom(String partialName)
        {
            System.IO.StreamWriter fileO = new System.IO.StreamWriter(partialName + "NoZoom.html", false);
            fileO.Write("<html><head><style type=\"text/css\">*{margin:0;padding:0;}#imgId{width:100%;height:auto;}</style></head><body><center><img id=\"imgId\" src=\"");
            fileO.Write(partialName + ".png");
            fileO.Write("\"/></center><script type=\"text/javascript\"> var img = document.getElementById('imgId'); window.onresize = function(){adjustRatio(img);} document.onload = function(){adjustRatio(img);} var imageratio = img.height/img.width;function adjustRatio(img) {var winheight = (document.body.clientHeight === undefined)?window.innerHeight:document.body.clientHeight;var winwidth = (document.body.clientWidth === undefined)?window.innerWidth:document.body.clientWidth;winratio = winheight / winwidth;if(winratio < imageratio){img.style.height = '100%';img.style.width = 'auto';}else{img.style.width = '100%';img.style.height = 'auto';}}setTimeout(\"adjustRatio(img)\",1);</script></body></html>");
            fileO.Flush();
            fileO.Close();

            return partialName + "NoZoom.html";
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
    }
}
