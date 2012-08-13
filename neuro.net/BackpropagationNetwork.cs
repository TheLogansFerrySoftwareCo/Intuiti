// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackpropagationNetwork.cs" company="The Logans Ferry Software Co.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A neural network that learns through back propagation.
    /// </summary>
    public class BackpropagationNetwork : NeuralNetwork, ISupervisedLearnerNetwork
    {
        /// <summary>
        /// The name of the class (for logging purposes).
        /// </summary>
        private const string ClassName = "BackpropagationNetwork";

        /// <summary>
        /// A utility for calculating the network's error rate.
        /// </summary>
        private readonly IErrorCalculator errorCalculator;

        /// <summary>
        /// A listing of the network's inbound connections.
        /// </summary>
        private readonly List<ISupervisedLearnerConnection> inboundConnections;

        /// <summary>
        /// A listing of the network's outbound connections.
        /// </summary>
        private readonly List<ISupervisedLearnerConnection> outboundConnections;

        /// <summary>
        /// A listing of the network's input nodes (Input Layer).
        /// </summary>
        private readonly List<ISupervisedLearnerNode> inputNodes;

        /// <summary>
        /// A listing of the network's output nodes (Output Layer).
        /// </summary>
        private readonly List<ISupervisedLearnerNode> outputNodes;

        /// <summary>
        /// The network's name (for logging purposes).
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackpropagationNetwork"/> class.
        /// </summary>
        /// <param name="errorCalculator">The error calculator.</param>
        public BackpropagationNetwork(IErrorCalculator errorCalculator)
        {
            // Initialize the class name that will appear in log files.
            this.name = ClassName + "_" + this.Id;
            const string MethodName = "ctor";
            Logger.TraceIn(this.name, MethodName);

            this.errorCalculator = errorCalculator;

            this.inboundConnections = new List<ISupervisedLearnerConnection>();
            this.outboundConnections = new List<ISupervisedLearnerConnection>();
            this.inputNodes = new List<ISupervisedLearnerNode>();
            this.outputNodes = new List<ISupervisedLearnerNode>();

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Gets the node's last-computed (cached) error values.
        /// </summary>
        public double[] CachedErrors { get; private set; }

        /// <summary>
        /// Adds an inbound connection from another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="inboundConnection">The inbound connection to add.</param>
        public void AddInboundConnection(ISupervisedLearnerConnection inboundConnection)
        {
            const string MethodName = "AddInboundConnection";
            Logger.TraceIn(this.name, MethodName);

            if (!this.inboundConnections.Contains(inboundConnection))
            {
                this.inboundConnections.Add(inboundConnection);
                base.AddInboundConnection(inboundConnection);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Adds an outbound connection to another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="outboundConnection">The outbound connection to add.</param>
        public void AddOutboundConnection(ISupervisedLearnerConnection outboundConnection)
        {
            const string MethodName = "AddOutboundConnection";
            Logger.TraceIn(this.name, MethodName);

            if (!this.outboundConnections.Contains(outboundConnection))
            {
                this.outboundConnections.Add(outboundConnection);
                base.AddOutboundConnection(outboundConnection);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Calculates the node's error from the provided error signal.
        /// </summary>
        /// <param name="errorSignal">The error signal.</param>
        /// <remarks>
        /// This method is invoked by each of the network's outbound connections during backpropagation training.
        /// Once all outbound connections are reporting an error signal, the network will aggregate the signals
        /// and use its own backprogagation to train itself.  It will then take the results of its internal
        /// backpropagation training and report the resulting error signals back to the outer network for the
        /// larger backpropagation to continue.
        /// </remarks>
        public void CalculateError(double errorSignal)
        {
            const string MethodName = "CalculateError";
            Logger.TraceIn(this.name, MethodName);
            
            // If all outbound connections are reporting an error signal, then this network
            // can perform internal backpropagation training.
            if (this.outboundConnections.All(connection => connection.IsReportingError))
            {
                // Retrieve the reported eror signals and allocate them to match the number of cached output values.
                // These allocated error signals will be used as the ideal outputs for correcting the cached outputs.
                var reportedErrorSignals = this.GetReportedErrorSignals();
                Logger.Debug(this.name, MethodName, "reportedErrorSignals", reportedErrorSignals);
                var digitizedErrorSignals = this.DigitizeValues(reportedErrorSignals);
                var allocatedErrorSignals = this.AllocateValues(digitizedErrorSignals, this.CachedOutputs.Length);
                Logger.Debug(this.name, MethodName, "aggregatedErrorSignals", allocatedErrorSignals);

                // Calculate this network's overall error by comparing the cached outputs to the "ideal" outputs.
                // Then backpropagate the error signals through this network and back to the outer network.
                var overallErrorSignals = this.GetOverallErrorSignals(this.CachedOutputs, allocatedErrorSignals);
                Logger.Debug(this.name, MethodName, "overallErrorSignals", overallErrorSignals);
                this.BackpropagateErrorSignals(overallErrorSignals);
            }
            else
            {
                Logger.Debug(this.name, MethodName, "Not all outbound connections are reporting yet.");
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Apply weight adjustments throughout all connections.
        /// </summary>
        /// <param name="learningRate">The degree to which the adjustments will affect the current values.</param>
        /// <param name="momentum">The degree to which the previous adjustments will carry-over to the current adjustments.</param>
        public void ApplyWeightAdjustments(float learningRate, float momentum)
        {
            const string MethodName = "ApplyWeightAdjustments";
            Logger.TraceIn(this.name, MethodName);
            
            // First, apply weight adjustments through this network by signaling to the input nodes.
            foreach (var inputNode in this.inputNodes)
            {
                inputNode.ApplyWeightAdjustments(learningRate, momentum);
            }

            // Next, continue the command to apply weight adjustments in the larger network by signaling the outbound connections.
            foreach (var outboundConnection in this.outboundConnections)
            {
                outboundConnection.ApplyWeightAdjustments(learningRate, momentum);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Clears the cached error values.
        /// </summary>
        public void ClearCachedErrors()
        {
            const string MethodName = "ClearCachedErrors";
            Logger.TraceIn(this.name, MethodName);

            // First, clear the temp values through this network by signaling to the input nodes.
            foreach (var inputNode in this.inputNodes)
            {
                inputNode.ClearCachedErrors();
            }

            // Next, continue the command to clear the temps in the larger network by signaling the outbound connections.
            foreach (var connection in this.outboundConnections)
            {
                connection.ClearCachedErrors();
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Adds a new input node to the network.
        /// </summary>
        /// <param name="node">The input node.</param>
        public void AddInputNode(ISupervisedLearnerNode node)
        {
            const string MethodName = "AddInputNode";
            Logger.TraceIn(this.name, MethodName);

            // Add the node to this class and the base class.
            if (!this.inputNodes.Contains(node))
            {
                this.inputNodes.Add(node);
                base.AddInputNode(node);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Adds a new output node to the network.
        /// </summary>
        /// <param name="node">The output node.</param>
        public void AddOutputNode(ISupervisedLearnerNode node)
        {
            const string MethodName = "AddOutputNode";
            Logger.TraceIn(this.name, MethodName);

            // Add the node to this class and the base class.
            if (!this.outputNodes.Contains(node))
            {
                this.outputNodes.Add(node);
                base.AddOutputNode(node); 
            }

            Logger.TraceOut(this.name, MethodName);
        }

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
        public double Train(long numEpochs, float learningRate, float momentum, double[][] trainingData, double[][] idealOutputs)
        {
            const string MethodName = "Train";
            Logger.TraceIn(this.name, MethodName);

            // Train for the specified number of epochs...
            for (var epochIndex = 0; epochIndex < numEpochs; epochIndex++)
            {
                Logger.Debug(this.name, MethodName, "epochIndex", epochIndex);

                this.errorCalculator.Reset();

                // Use each training set once per epoch...
                for (var trainingSetIndex = 0; trainingSetIndex < trainingData.Length; trainingSetIndex++)
                {
                    Logger.Debug(this.name, MethodName, "trainingSetIndex", trainingSetIndex);

                    // Compute actual outputs from the provided inputs.  Then calculate error signals by comparing
                    // the actual outputs to the provided ideal outputs.
                    Logger.Debug(this.name, MethodName, "inputs", trainingData[trainingSetIndex]);
                    var actualOutputs = this.ComputeOutputs(trainingData[trainingSetIndex]);
                    Logger.Debug(this.name, MethodName, "actualOutputs", actualOutputs);
                    Logger.Debug(this.name, MethodName, "idealOutputs", idealOutputs[trainingSetIndex]);
                    var errorSignals = this.GetOverallErrorSignals(actualOutputs, idealOutputs[trainingSetIndex]);
                    this.errorCalculator.AddToErrorCalc(errorSignals);
                    Logger.Debug(this.name, MethodName, "errorSignals", errorSignals);

                    // Backpropagate the calculated error signals through the network.
                    this.BackpropagateErrorSignals(errorSignals);

                    // Clear the cached errors before the next iteration.
                    this.ClearCachedErrors();
                }

                // Apply all pending weight adjustments at the end of each epoch.
                this.ApplyWeightAdjustments(learningRate, momentum);
            }

            Logger.TraceOut(this.name, MethodName);

            return this.errorCalculator.Calculate();
        }

        /// <summary>
        /// Gets the error signals that are being reported from the network's outbound connections.
        /// </summary>
        /// <returns>
        /// The error signals that are being reported from the network's outbound connections.
        /// </returns>
        private double[] GetReportedErrorSignals()
        {
            const string MethodName = "GetReportedErrorSignals";
            Logger.TraceIn(this.name, MethodName);

            var reportedErrorSignals = new double[this.outboundConnections.Count];  // one signal per connection

            for (var outboundIndex = 0; outboundIndex < this.outboundConnections.Count; outboundIndex++)
            {
                // Read each error signal.
                reportedErrorSignals[outboundIndex] = this.outboundConnections[outboundIndex].ErrorSignal;
                Logger.Debug(this.name, MethodName, "reportedErrorSignals", reportedErrorSignals);

                // Clear the signal.
                this.outboundConnections[outboundIndex].ClearReportingFlag();
            }

            Logger.TraceOut(this.name, MethodName);

            return reportedErrorSignals;
        }

        /// <summary>
        /// Gets the error signal for each calculated output.
        /// </summary>
        /// <param name="actualOutputs">The actual outputs.</param>
        /// <param name="idealOutputs">The ideal outputs.</param>
        /// <returns>
        /// The error signals for all outputs.
        /// </returns>
        /// <remarks>
        /// This method calculates the error signal for each output node as follows:
        /// <code>
        /// errorSignal = idealOutput - actualOutput
        /// </code>
        /// </remarks>
        private double[] GetOverallErrorSignals(double[] actualOutputs, double[] idealOutputs)
        {
            const string MethodName = "GetOverallErrorSignals";
            Logger.TraceIn(this.name, MethodName);

            var errorSignals = new double[actualOutputs.Length];
            for (var index = 0; index < actualOutputs.Length; index++)
            {
                errorSignals[index] = idealOutputs[index] - actualOutputs[index];
            }

            Logger.TraceOut(this.name, MethodName);
            
            return errorSignals;
        }

        /// <summary>
        /// Backpropagates the error signals through the network and back upward to the higher-level network.
        /// </summary>
        /// <param name="errorSignals">The error signals to backpropagate.</param>
        /// <remarks>
        /// This method will backpropagate the overall network error signals by applying each error signal to
        /// its corresponding output node.  Then once all errors have backpropagated through the network, the final
        /// signals can be read from the input nodes and reported to the inbound connections.
        /// </remarks>
        private void BackpropagateErrorSignals(IList<double> errorSignals)
        {
            const string MethodName = "BackpropagateErrorSignals";
            Logger.TraceIn(this.name, MethodName);

            // Apply each signal to its corresponding output node.
            for (var outputNodeIndex = 0; outputNodeIndex < this.outputNodes.Count; outputNodeIndex++)
            {
                this.outputNodes[outputNodeIndex].CalculateError(errorSignals[outputNodeIndex]);
            }

            //// Now read the resulting errors from the input nodes.

            this.CachedErrors = new double[this.InputSize];  // reset the cached errors.
            var errorArrayCursor = 0;
            var inputNodeIndex = 0;

            while (errorArrayCursor < this.CachedErrors.Length)
            {
                // Read each node's cached errors and copy them to this network's cached error array.  (A node may have multiple cached errors to read if it is another sub-network.)
                Array.Copy(this.inputNodes[inputNodeIndex].CachedErrors, 0, this.CachedErrors, errorArrayCursor, this.inputNodes[inputNodeIndex].CachedErrors.Length);

                // Loop control.
                errorArrayCursor += this.inputNodes[inputNodeIndex].CachedErrors.Length;
                inputNodeIndex++;
            }

            // Complete backpropagation by reporting the error signals to the higher level network.
            Logger.Debug(this.name, MethodName, "CalculatedErrors", this.CachedErrors);
            this.ReportErrorsToInboundConnections();

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Reports the network's cached errors to its inbound connections.  In this way, this network
        /// will continue the higher-level backpropagation of its parent network.
        /// </summary>
        private void ReportErrorsToInboundConnections()
        {
            const string MethodName = "ReportErrorsToInboundConnections";
            Logger.TraceIn(this.name, MethodName);

            var inputRatio = this.inboundConnections.Count / this.CachedErrors.Length;
            var inputExcess = this.inboundConnections.Count % this.CachedErrors.Length;

            var connectionIndex = 0;
            for (var errorIndex = 0; errorIndex < this.CachedErrors.Length; errorIndex++)
            {
                for (var counter = 0; counter < inputRatio; counter++)
                {
                    this.inboundConnections[connectionIndex].ReportError(this.CachedErrors[errorIndex]);
                    connectionIndex++;
                }

                if (errorIndex < inputExcess)
                {
                    this.inboundConnections[connectionIndex].ReportError(this.CachedErrors[errorIndex]);
                    connectionIndex++;
                }
            }

            Logger.TraceOut(this.name, MethodName);
        }
    }
}
