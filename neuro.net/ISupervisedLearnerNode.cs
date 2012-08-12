// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISupervisedLearnerNode.cs" company="The Logans Ferry Software Co.">
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
    /// A node within a neural network that can learn from supervised learning.
    /// </summary>
    public interface ISupervisedLearnerNode : INeuralNode
    {
        /// <summary>
        /// Gets the node's last-computed (cached) error signals.
        /// </summary>
        double[] CachedErrors { get; }

        /// <summary>
        /// Adds an inbound connection from another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="inboundConnection">The inbound connection to add.</param>
        void AddInboundConnection(ISupervisedLearnerConnection inboundConnection);

        /// <summary>
        /// Adds an outbound connection to another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="outboundConnection">The outbound connection to add.</param>
        void AddOutboundConnection(ISupervisedLearnerConnection outboundConnection);

        /// <summary>
        /// Calculates the node's error from the provided error signal.
        /// </summary>
        /// <param name="errorSignal">The error signal.</param>
        void CalculateError(double errorSignal);

        /// <summary>
        /// Apply weight adjustments throughout all connections.
        /// </summary>
        /// <param name="learningRate">The degree to which the adjustments will affect the current values.</param>
        /// <param name="momentum">The degree to which the previous adjustments will carry-over to the current adjustments.</param>
        void ApplyWeightAdjustments(float learningRate, float momentum);

        /// <summary>
        /// Clears the cached error values.
        /// </summary>
        void ClearCachedErrors();
    }
}
