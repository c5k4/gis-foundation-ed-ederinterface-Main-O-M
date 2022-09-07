using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Handles the Step Progress Bar of the Application
    /// </summary>
    internal class StepProgressBar
    {
        #region Private Variables

        /// <summary>
        /// Progress bar of application
        /// </summary>
        private IStepProgressor _stepProgressBar = null;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initilizes new instance of <see cref="StepProgressBar"/>
        /// </summary>
        /// <param name="baseApplication">Instance of the currently running ArcMap Application</param>
        internal StepProgressBar(IApplication baseApplication)
        {
            _stepProgressBar = baseApplication.StatusBar.ProgressBar as IStepProgressor;
        }

        #endregion Constructor

        #region Internal Methods

        /// <summary>
        /// Resets the Step Progress Bar
        /// </summary>
        /// <param name="minRange">Min Range of the Progress Bar</param>
        /// <param name="maxRange">Max Range of the Progress Bar</param>
        /// <param name="stepValue">Step Value of the Progress Bar</param>
        /// <param name="message">Status Message to be displayed</param>
        internal void Reset(int minRange, int maxRange, int stepValue, string message)
        {
            _stepProgressBar.MinRange = minRange;
            _stepProgressBar.MaxRange = maxRange;
            _stepProgressBar.StepValue = stepValue;
            _stepProgressBar.Message = message;
            //Set the initial pointer to MinRange value
            _stepProgressBar.Position = minRange;
        }

        /// <summary>
        /// Increments the Step Position
        /// </summary>
        internal void Step()
        {
            _stepProgressBar.Step();
        }

        /// <summary>
        /// Display the Progress Bar
        /// </summary>
        internal void Show() { _stepProgressBar.Show(); }

        /// <summary>
        /// Display the Progress Bar
        /// </summary>
        internal void Hide() { _stepProgressBar.Hide(); }

        /// <summary>
        /// Set the message
        /// </summary>
        /// <param name="message">Status message to be displayed</param>
        internal void SetMessage(string message)
        {
            _stepProgressBar.Message = message;
        }

        #endregion Internal Methods
    }
}
