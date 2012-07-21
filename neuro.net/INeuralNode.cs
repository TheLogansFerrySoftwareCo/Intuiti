// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INeuralNode.cs" company="The Logans Ferry Software Co.">
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
    /// A node within a neural network.  A node may be a neuron or a subnetwork.
    /// </summary>
    public interface INeuralNode
    {
        /// <summary>
        /// Gets the number of inputs expected by the node.
        /// </summary>
        /// <value>
        /// The number of expected inputs.
        /// </value>
        int InputSize { get; }

        /// <summary>
        /// Gets the number of ouputs published by the node.
        /// </summary>
        /// <value>
        /// The number of published outputs.
        /// </value>
        int OutputSize { get; }

        /// <summary>
        /// Gets the node's last-computed (cached) outputs.
        /// </summary>
        double[] CachedOutputs { get; }

        /// <summary>
        /// Gets the node's name (for logging purposes).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Adds an inbound connection from another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="inboundConnection">The inbound connection to add.</param>
        void AddInboundConnection(INeuralConnection inboundConnection);

        /// <summary>
        /// Adds an outbound connection to another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="outboundConnection">The outbound connection to add.</param>
        void AddOutboundConnection(INeuralConnection outboundConnection);

        /// <summary>
        /// Fires the node with the provided input.
        /// </summary>
        /// <param name="input">The input.</param>
        void Fire(double input);

        /// <summary>
        /// Fires the node with the provided inputs.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        void Fire(double[] inputs);
    }
}
