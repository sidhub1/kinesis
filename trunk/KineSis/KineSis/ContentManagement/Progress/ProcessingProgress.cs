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
using System.Windows.Forms;
using System.Windows.Controls;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;

namespace KineSis.ContentManagement.Progress
{

    /// <summary>
    /// Processing progress management, used for displaying loading bars
    /// </summary>
    class ProcessingProgress
    {

        private String currentOperationName = "";
        private Double currentOperationElement = 0;
        private Double currentOperationTotalElements = 1;

        private String overallOperationName = "";
        private Double overallOperationElement = 0;
        private Double overallOperationTotalElements = 1;

        //the background worker
        BackgroundWorker worker;

        public String CurrentOperationName
        {
            get
            {
                return currentOperationName;
            }

            set
            {
                currentOperationName = value;
                WriteProgress();
            }
        }

        public String OverallOperationName
        {
            get
            {
                return overallOperationName;
            }

            set
            {
                overallOperationName = value;
                WriteProgress();
            }
        }

        public Double CurrentOperationElement
        {
            get
            {
                return currentOperationElement;
            }

            set
            {
                currentOperationElement = value;
                WriteProgress();
            }
        }

        public Double OverallOperationElement
        {
            get
            {
                return overallOperationElement;
            }

            set
            {
                overallOperationElement = value;
                WriteProgress();
            }
        }

        public Double CurrentOperationTotalElements
        {
            get
            {
                return currentOperationTotalElements;
            }

            set
            {
                currentOperationTotalElements = value;
                WriteProgress();
            }
        }

        public Double OverallOperationTotalElements
        {
            get
            {
                return overallOperationTotalElements;
            }

            set
            {
                overallOperationTotalElements = value;
                WriteProgress();
            }
        }

        /// <summary>
        /// Creates an ProcessingProgress for a given background worker
        /// </summary>
        /// <param name="worker"></param>
        public ProcessingProgress(BackgroundWorker worker)
        {
            this.worker = worker;
        }

        /// <summary>
        /// Report progress
        /// </summary>
        private void WriteProgress()
        {
            Double overallOperation = ((Double)overallOperationElement * 100 / (Double)overallOperationTotalElements);
            worker.ReportProgress((int)overallOperation, this);
        }
    }
}
