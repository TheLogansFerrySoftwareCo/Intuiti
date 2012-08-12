// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISupervisedLearnerConnection.cs" company="The Logans Ferry Software Co.">
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
    /// A connection in a network that is capable of supervised learning.
    /// </summary>
    public interface ISupervisedLearnerConnection : INeuralConnection
    {
        /// <summary>
        /// Gets the current error signal.
        /// </summary>
        double ErrorSignal { get; }

        /// <summary>
        /// Gets a value indicating whether this connection is currently reporting an error signal.
        /// </summary>
        /// <value>
        /// <c>true</c> if this connection is reporting an error signal; otherwise, <c>false</c>.
        /// </value>
        bool IsReportingError { get; }

        /// <summary>
        /// Reports the error signal to the source node.
        /// </summary>
        /// <param name="errorSignal">The error signal.</param>
        void ReportError(double errorSignal);

        /// <summary>
        /// Applies the pending weight adjustments to the connection.
        /// </summary>
        /// <param name="learningRate">The learning rate.</param>
        /// <param name="momentum">The momentum.</param>
        void ApplyWeightAdjustments(float learningRate, float momentum);

        /// <summary>
        /// Clears the IsReporting flag.
        /// </summary>
        void ClearReportingFlag();

        /// <summary>
        /// Clears the cached error values.
        /// </summary>
        void ClearCachedErrors();
    }
}
