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
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;

namespace KineSis.ContentManagement.Service.Helper
{
    /// <summary>
    /// Handles the excel files (xslx)
    /// Open the document, save it as html, for every sheet parse the charts and save them as images for building a 3D model
    /// </summary>
    class ExcelDocumentHelper : DocumentHelper
    {

        private static Microsoft.Office.Interop.Excel.Application excelApplication = null;

        private static String DD = "\\"; //directory delimiter
        private static String DOC_FILES = "document_files"; //document_files
        private static String _ = "_";

        /// <summary>
        /// Open the Excel application
        /// </summary>
        private void OpenOfficeApplication()
        {
            if (excelApplication == null)
            {
                excelApplication = new Microsoft.Office.Interop.Excel.Application();
            }
        }

        /// <summary>
        /// Close the Excel Application
        /// </summary>
        private void CloseOfficeApplication()
        {
            if (excelApplication != null)
            {
                excelApplication.Quit();
                excelApplication = null;
            }
        }


        /// <summary>
        /// parse an excel document and build a kinesis document model
        /// </summary>
        /// <param name="path">full path of the excel document</param>
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
            pp.CurrentOperationTotalElements = 2;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //open the given excel document
            Workbook workbook = excelApplication.Workbooks.Open(path, ReadOnly: true);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            pp.CurrentOperationName = "Saving pages";
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //create a new internal document
            Document document = new Document();
            document.Name = workbook.Name;

            //save excel document as html
            workbook.SaveAs(System.IO.Path.Combine(documentPath, "document.html"), XlFileFormat.xlHtml);

            //original document location
            document.Location = folderName;

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 2;
            pp.OverallOperationTotalElements = workbook.Sheets.Count + 1;
            pp.OverallOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //for every sheet
            for (int i = 1; i <= workbook.Sheets.Count; i++)
            {

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationName = "Page " + i;
                pp.CurrentOperationTotalElements = 1;
                pp.CurrentOperationElement = 0;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                Worksheet worksheet = workbook.Sheets[i];

                //create a new page
                KineSis.ContentManagement.Model.Page page = new KineSis.ContentManagement.Model.Page();
                page.Name = worksheet.Name;

                //standard export location of ms excel when exporting as html
                page.Location = documentPath + DD + DOC_FILES + DD + "sheet" + GetSheetNumber(i) + ".html";


                //add page to the document
                document.Pages.Add(page);

                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                pp.CurrentOperationElement = 1;
                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            }

            //close workbook without saving any possible changes (this way the "Are you sure?" or "Save changes?" dialogs will be supressed)
            workbook.Close(SaveChanges: false);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationElement++;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //every generated page contains several javascript functions which facilitate navigation between sheets and other unnecessary stuff
            //this is an impediment for kinesis web browser, because will block the programatic scrolling
            //get every generated page and remove javascripts

            int pageNumber = 1;

            foreach (KineSis.ContentManagement.Model.Page p in document.Pages)
            {
                ProcessSheet(p.Location, pp, pageNumber++);
            }

            //return the built document
            return document;
        }

        /// <summary>
        /// parse an excel document and build a kinesis document model
        /// </summary>
        /// <param name="path">full path of the excel document</param>
        /// <returns>equivalent kinesis document model</returns>
        public Document ParseNewDocumentCharts(String path, ProcessingProgress pp, Document document)
        {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.OverallOperationName = "All Document Charts";
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //directory where all the data will be saved
            String folderName = document.Location;
            String documentPath = System.IO.Path.Combine(DocumentService.TEMP_DIRECTORY, folderName);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Opening MS Office";
            pp.CurrentOperationTotalElements = 1;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //open the given excel document
            Workbook workbook = excelApplication.Workbooks.Open(path, ReadOnly: true);

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            pp.OverallOperationTotalElements = EvaluateWorkbook(workbook, pp);
            pp.OverallOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            //for every sheet
            for (int i = 1; i <= workbook.Sheets.Count; i++)
            {
                Worksheet worksheet = workbook.Sheets[i];

                //create a new page
                KineSis.ContentManagement.Model.Page page = document.Pages[i - 1];

                //check if chart generation is wanted
                if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                {
                    //get charts
                    ChartObjects chartObjects = worksheet.ChartObjects(Type.Missing);

                    //create directory for charts
                    String chartPath = System.IO.Path.Combine(documentPath, "charts");
                    System.IO.Directory.CreateDirectory(chartPath);

                    //for every chart
                    for (int j = 1; j <= chartObjects.Count; j++)
                    {

                        //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                        pp.CurrentOperationName = "Page " + i + " / Chart " + j + " of " + chartObjects.Count;
                        pp.CurrentOperationTotalElements = EvaluateChart(chartObjects.Item(j).Chart);
                        pp.CurrentOperationElement = 0;
                        //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                        KineSis.ContentManagement.Model.Chart mChart = new KineSis.ContentManagement.Model.Chart();

                        //current chart
                        Microsoft.Office.Interop.Excel.Chart chart = chartObjects.Item(j).Chart;

                        mChart.SetThumbnailUrl(GenerateThumbnail(chart, chartPath + DD + i + _ + j + "_thumb"));

                        //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                        pp.OverallOperationElement++;
                        pp.CurrentOperationElement++;
                        //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                        if (DocumentService.FORCE_CHART_SIZE)
                        {
                            chart.ChartArea.Height = ((float)DocumentService.CHART_WIDTH * chart.ChartArea.Height) / chart.ChartArea.Width;
                            chart.ChartArea.Width = DocumentService.CHART_WIDTH;
                        }

                        int chartType = GetChartType(chart);

                        //start from 0 point
                        chart.Rotation = 0;

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

                        if (chart.HasTitle)
                        {
                            mChart.Title = chart.ChartTitle.Caption;
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
                                chart.Export(chartPath + DD + i + _ + j + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FILTER, false);
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
                                chart.Elevation = 0;
                                //export face as image

                                chart.Export(chartPath + DD + i + _ + j + _ + chart.Rotation + _ + chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FILTER, false);
                                //set bitmap to view
                                hView.ImageUrl = chartPath + DD + i + _ + j + _ + chart.Rotation + _ + chart.Elevation + DocumentService.IMAGE_EXTENSION;

                                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                pp.OverallOperationElement++;
                                pp.CurrentOperationElement++;
                                //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

                                //for every vertical face
                                for (int l = 0; l < DocumentService.CHART_VERTICAL_FACES; l++)
                                {
                                    ChartVerticalView vView = new ChartVerticalView();
                                    //increse elevation
                                    chart.Elevation += verticalAngle;
                                    //export face as image
                                    chart.Export(chartPath + DD + i + _ + j + _ + chart.Rotation + _ + chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FILTER, false);

                                    //set bitmap to view
                                    vView.ImageUrl = chartPath + DD + i + _ + j + _ + chart.Rotation + _ + chart.Elevation + DocumentService.IMAGE_EXTENSION;
                                    //add vertical view to horizontal UP list
                                    hView.Up.Add(vView);

                                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                    pp.OverallOperationElement++;
                                    pp.CurrentOperationElement++;
                                    //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                }

                                //some chart types, like 3D pie, does not support elevation less than 0
                                if (SupportsNegativeElevation(chart))
                                {

                                    //reset elevation
                                    chart.Elevation = 0;

                                    //for every vertical face
                                    for (int m = 0; m < DocumentService.CHART_VERTICAL_FACES; m++)
                                    {
                                        ChartVerticalView vView = new ChartVerticalView();

                                        //decrease elevation
                                        chart.Elevation -= verticalAngle;
                                        //export face as image
                                        chart.Export(chartPath + DD + i + _ + j + _ + chart.Rotation + _ + chart.Elevation + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FILTER, false);
                                        //set bitmap to vertical view
                                        vView.ImageUrl = chartPath + DD + i + _ + j + _ + chart.Rotation + _ + chart.Elevation + DocumentService.IMAGE_EXTENSION;
                                        //add vertical view to horizontal view DOWN list
                                        hView.Down.Add(vView);

                                        //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                        pp.OverallOperationElement++;
                                        pp.CurrentOperationElement++;
                                        //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
                                    }
                                }

                                //increase horizontal angle in order to get the next horizontal view
                                chart.Rotation += horizontalAngle;
                                //add horizontal view to the chat's views list
                                mChart.Views.Add(hView);
                            }
                        }

                        //add chart to page
                        page.Charts.Add(mChart);
                    }
                }
            }

            //close workbook without saving any possible changes (this way the "Are you sure?" or "Save changes?" dialogs will be supressed)
            workbook.Close(SaveChanges: false);

            CloseOfficeApplication();

            //return the built document
            return document;
        }


        /// <summary>
        /// Evaluate a chart in order to determine the number of operations required for exporting it
        /// </summary>
        /// <param name="c">chart</param>
        /// <returns></returns>
        private int EvaluateChart(Microsoft.Office.Interop.Excel.Chart c)
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


        private int EvaluateWorkbook(Workbook workbook, ProcessingProgress pp)
        {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Evaluating document";
            pp.CurrentOperationTotalElements = workbook.Sheets.Count;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            int numberOfOperations = 0;

            //for every sheet
            for (int i = 1; i <= workbook.Sheets.Count; i++)
            {
                Worksheet worksheet = workbook.Sheets[i];

                //check if chart generation is wanted
                if (DocumentService.CHART_HORIZONTAL_FACES > 0)
                {
                    //get charts
                    ChartObjects chartObjects = worksheet.ChartObjects(Type.Missing);

                    //for every chart
                    for (int j = 1; j <= chartObjects.Count; j++)
                    {

                        numberOfOperations++;

                        //current chart
                        Microsoft.Office.Interop.Excel.Chart chart = chartObjects.Item(j).Chart;

                        int chartType = GetChartType(chart);

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
                                if (SupportsNegativeElevation(chart))
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
        private String GenerateThumbnail(Microsoft.Office.Interop.Excel.Chart shape, String path)
        {
            shape.Export(path + DocumentService.IMAGE_EXTENSION, DocumentService.IMAGE_FILTER, false);

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
        /// get the last part of the excel generated sheet name, the index
        /// it generates sheet001.html, sheet002.html, ..., sheet010.html, sheet011.html, ..., sheet100.html, ..., sheet999.html, ..., sheet123456.html, ...
        /// </summary>
        /// <param name="i">integer index of te sheet</param>
        /// <returns>generated string index of the sheet</returns>
        private String GetSheetNumber(int i)
        {
            String number = i.ToString();
            if (i < 10)
            {
                number = "00" + i;
            }
            else if (i < 100)
            {
                number = "0" + i;
            }
            return number;
        }

        /// <summary>
        /// determines the type of an excel chart
        /// </summary>
        /// <param name="chart">excel chart</param>
        /// <returns>chart type, 0 = 2D chart, 1 = 3D bar chart, 2 = 3D chart which can be fully rotated</returns>
        private int GetChartType(Microsoft.Office.Interop.Excel.Chart chart)
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
        /// check if an excel chart can be down elevated
        /// </summary>
        /// <param name="chart">excel chart</param>
        /// <returns></returns>
        private Boolean SupportsNegativeElevation(Microsoft.Office.Interop.Excel.Chart chart)
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
        /// process a generated html sheet to ensure compatibility with kinesis
        /// it removes all javascript functions
        /// 
        /// modifying an office html generated file, it will cut off the support for all languages, so an UTF-8 character encoding will be forced
        /// </summary>
        /// <param name="path">the full path of the generated html sheet</param>
        private void ProcessSheet(String path, ProcessingProgress pp, int pageNumber)
        {

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationName = "Page " + pageNumber;
            pp.CurrentOperationTotalElements = 2;
            pp.CurrentOperationElement = 0;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            System.IO.StreamReader fileI = new System.IO.StreamReader(path);
            String content = fileI.ReadToEnd();
            fileI.Close();

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 1;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//

            System.IO.StreamWriter fileO = new System.IO.StreamWriter(path, false);
            fileO.WriteLine(TrimScript(content));
            fileO.Flush();
            fileO.Close();

            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
            pp.CurrentOperationElement = 2;
            pp.OverallOperationElement++;
            //~~~~~~~~~~~~~progress~~~~~~~~~~~~~//
        }

        /// <summary>
        /// removes all javascript sections from a string, assuming that the string is the content of an html page
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>the input filtered</returns>
        private string TrimScript(String input)
        {
            String output = "";
            output = Regex.Replace(input, "<script(.|\n)*</script>", "");
            return output;
        }
    }
}
