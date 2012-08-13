// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RmsCalculator.cs" company="The Logans Ferry Software Co.">
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
    using System;
    
    /// <summary>
    /// An error calculator that uses the Root-Mean-Square function to calculate a network's error rating:
    /// <code>
    ///                /  2    2         2   \
    ///               /  X  + X  ...  + X     \
    ///              /    1    2         n     \
    /// RMS =  SQRT |  ----------------------   |
    ///              \                         /
    ///               \           n           /
    ///                \                     /
    /// </code>
    /// </summary>
    public class RmsCalculator : IErrorCalculator
    {
        /// <summary>
        /// The name of the class (for logging purposes).
        /// </summary>
        private const string ClassName = "RmsCalculator";

        /// <summary>
        /// The size of the training set for which the error rate will be calculated.  (The 'n' value.)
        /// </summary>
        private long setSize;

        /// <summary>
        /// The running summation of the squared error values.
        /// </summary>
        private double squaredErrorSummation;

        /// <summary>
        /// Resets the calculator.
        /// </summary>
        public void Reset()
        {
            this.setSize = 0;
            this.squaredErrorSummation = 0.0d;
        }

        /// <summary>
        /// Adds to the running error calculation.
        /// </summary>
        /// <param name="errorSignals">The error signals to add.</param>
        public void AddToErrorCalc(double[] errorSignals)
        {
            const string MethodName = "AddToErrorCalc";
            Logger.TraceIn(ClassName, MethodName);

            foreach (var errorSignal in errorSignals)
            {
                this.squaredErrorSummation += errorSignal * errorSignal;
                Logger.Debug(ClassName, MethodName, "squaredErrorSummation", this.squaredErrorSummation);
            }

            this.setSize += errorSignals.Length;
            Logger.Debug(ClassName, MethodName, "setSize", this.setSize);

            Logger.TraceOut(ClassName, MethodName);
        }

        /// <summary>
        /// Returns an answer for the running error calculation.
        /// </summary>
        /// <returns>
        /// The current answer for the running error calculation.
        /// </returns>
        public double Calculate()
        {
            const string MethodName = "AddToErrorCalc";
            Logger.TraceIn(ClassName, MethodName);

            if (this.setSize == 0)
            {
                // Only Linus T. can divide by zero.
                throw new InvalidOperationException("No error values have been provided.  Use the AddToErrorCalc method before calculating an answer.");
            }

            Logger.TraceOut(ClassName, MethodName);

            return Math.Sqrt(this.squaredErrorSummation / this.setSize);
        }
    }
}
