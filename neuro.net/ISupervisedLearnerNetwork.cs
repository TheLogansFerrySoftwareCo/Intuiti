// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISupervisedLearnerNetwork.cs" company="The Logans Ferry Software Co.">
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
    /// A neural network that is capable of learning through supervised learning.
    /// </summary>
    public interface ISupervisedLearnerNetwork : ISupervisedLearnerNode, INeuralNetwork
    {
        /// <summary>
        /// Trains the neural network using the specified parameters.
        /// </summary>
        /// <param name="numEpochs">The number of training epochs to execute.</param>
        /// <param name="learningRate">The learning rate that will be used.</param>
        /// <param name="momentum">The momentum that will be used.</param>
        /// <param name="trainingData">The training data set that will be used to compute outputs.</param>
        /// <param name="idealOutputs">The ideal outputs that corelate to the training data and that will be used to correct the network.</param>
        /// <returns>
        /// The error rate from the most recent training epoch.
        /// </returns>
        double Train(long numEpochs, float learningRate, float momentum, double[][] trainingData, double[][] idealOutputs);
    }
}
