// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="The Logans Ferry Software Co.">
//   Copyright 2012, The Logans Ferry Software Co. 
// </copyright>
// <license>  
//   This file is part of Intuiti.
//   
//   Intuiti is free software: you can redistribute it and/or modify it under the terms
//   of the GNU General Public License as published by the Free Software Foundation, either
//   version 3 of the License, or (at your option) any later version.
//   
//   Intuiti is distributed in the hope that it will be useful, but WITHOUT ANY
//   WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
//   A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with
//   Intuiti. If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace LogansFerry.Intuiti
{
    using System.Collections.Generic;
    using System.Text;

    using log4net;

    /// <summary>
    /// A static logging utility that will write statements to the log4net logger.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(Logger));

        /// <summary>
        /// Add a trace statement to the debug log that records an entrance to the class/method.
        /// </summary>
        /// <param name="className">The class name to trace.</param>
        /// <param name="methodName">The method name to trace.</param>
        public static void TraceIn(string className, string methodName)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Class=" + className + "; Method=" + methodName + "; TRACE-IN");
            }
        }

        /// <summary>
        /// Add a trace statement to the debug log that records an exit from the class/method.
        /// </summary>
        /// <param name="className">The class name to trace.</param>
        /// <param name="methodName">The method name to trace.</param>
        public static void TraceOut(string className, string methodName)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Class=" + className + "; Method=" + methodName + "; TRACE-OUT");
            }
        }

        /// <summary>
        /// Writes the provided debug statement to the debug log.
        /// </summary>
        /// <param name="className">Name of the class from which the statement was published.</param>
        /// <param name="methodName">Name of the method from which the statement was published.</param>
        /// <param name="debugStatement">The debug statement to write.</param>
        public static void Debug(string className, string methodName, string debugStatement)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Class=" + className + "; Method=" + methodName + "; " + debugStatement);
            }
        }

        /// <summary>
        /// Writes the provided value to the debug log.
        /// </summary>
        /// <param name="className">Name of the class from which the value was published.</param>
        /// <param name="methodName">Name of the method from which the value was published.</param>
        /// <param name="valueName">Name of the value being published.</param>
        /// <param name="value">The value.</param>
        public static void Debug(string className, string methodName, string valueName, double value)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Class=" + className + "; Method=" + methodName + "; " + valueName + "=" + value);
            }
        }

        /// <summary>
        /// Writes the provided value array to the debug log.
        /// </summary>
        /// <param name="className">Name of the class from which the array was published.</param>
        /// <param name="methodName">Name of the method from which the array was published.</param>
        /// <param name="arrayName">Name of the array being published.</param>
        /// <param name="values">The values in the array.</param>
        public static void Debug(string className, string methodName, string arrayName, IList<double> values)
        {
            if (Log.IsDebugEnabled)
            {
                var entryBuilder =
                    new StringBuilder("Class=" + className + "; Method=" + methodName + "; " + arrayName + "={");

                if (values.Count > 0)
                {
                    entryBuilder.Append(values[0]);

                    for (var index = 1; index < values.Count; index++)
                    {
                        entryBuilder.Append(", " + values[index]);
                    }
                }

                entryBuilder.Append("}");

                Log.Debug(entryBuilder.ToString());
            }
        }
    }
}
