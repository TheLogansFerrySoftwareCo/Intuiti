// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IErrorCalculator.cs" company="The Logans Ferry Software Co.">
//   Copyright 2012, The Logans Ferry Software Co. 
// </copyright>
// <license>  
//   This file is part of neuro.net.
//   
//   neuro.net is free software: you can redistribute it and/or modify it under the terms
//   of the GNU General Public License as published by the Free Software Foundation, either
//   version 3 of the License, or (at your option) any later version.
//   
//   neuro.net is distributed in the hope that it will be useful, but WITHOUT ANY
//   WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
//   A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with
//   neuro.net. If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace LogansFerry.NeuroDotNet
{
    /// <summary>
    /// A utility for calculating network error rates.
    /// </summary>
    public interface IErrorCalculator
    {
        /// <summary>
        /// Resets the calculator.
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds to the running error calculation.
        /// </summary>
        /// <param name="errorSignals">The error signals to add.</param>
        void AddToErrorCalc(double[] errorSignals);

        /// <summary>
        /// Returns an answer for the running error calculation.
        /// </summary>
        /// <returns>
        /// The current answer for the running error calculation.
        /// </returns>
        double Calculate();
    }
}