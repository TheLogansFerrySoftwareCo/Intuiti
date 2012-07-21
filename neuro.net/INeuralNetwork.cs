// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INeuralNetwork.cs" company="The Logans Ferry Software Co.">
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
    /// A neural network containing connected neural nodes.
    /// </summary>
    public interface INeuralNetwork : INeuralNode
    {
        /// <summary>
        /// Computes output values from the provided input values.
        /// </summary>
        /// <param name="inputs">The input values.</param>
        /// <returns>
        /// The computed output values.
        /// </returns>
        double[] ComputeOutputs(double[] inputs);
    }
}
