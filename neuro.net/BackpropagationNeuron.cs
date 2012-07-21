// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackpropagationNeuron.cs" company="The Logans Ferry Software Co.">
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A neuron within a backpropagation network.
    /// </summary>
    public class BackpropagationNeuron : Neuron, ISupervisedLearnerNode
    {
        /// <summary>
        /// The name of the class (for logging purposes).
        /// </summary>
        private const string ClassName = "BackpropagationNeuron";

        /// <summary>
        /// A listing of the neuron's inbound connections.
        /// </summary>
        private readonly List<ISupervisedLearnerConnection> inboundConnections;

        /// <summary>
        /// A listing of the neuron's outbound connections.
        /// </summary>
        private readonly List<ISupervisedLearnerConnection> outboundConnections;

        /// <summary>
        /// The neuron's cached error value.
        /// </summary>
        /// <remarks>
        /// This value is stored in a single-element array in order to satisfy its interface properties. 
        /// </remarks>
        private readonly double[] cachedError;

        /// <summary>
        /// The neuron's name (for logging purposes).
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The adjustment that will be applied to this neuron's bias when the network's weights are adjusted.
        /// </summary>
        private double pendingBiasAdjustment;

        /// <summary>
        /// The adjustment that was applied to the neuron's bias the last time connection weights were adjusted.
        /// </summary>
        private double previousBiasAdjustment;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackpropagationNeuron"/> class.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        public BackpropagationNeuron(IActivationFunction activationFunction)
            : base(activationFunction)
        {
            // Initialize the class name for loggin purposes.
            this.name = ClassName + "_" + this.Id;
            const string MethodName = "Ctor";
            Logger.TraceIn(this.name, MethodName);

            this.inboundConnections = new List<ISupervisedLearnerConnection>();
            this.outboundConnections = new List<ISupervisedLearnerConnection>();

            // Initialize the single-element array for the cached error value.
            // This value is stored in an array solely to satisfy its interface requirements.
            this.cachedError = new double[1];

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Gets the node's last-computed (cached) error signals.
        /// </summary>
        public double[] CachedErrors
        {
            get
            {
                return this.cachedError;
            }
        }

        /// <summary>
        /// Adds an inbound connection from another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="inboundConnection">The inbound connection to add.</param>
        public void AddInboundConnection(ISupervisedLearnerConnection inboundConnection)
        {
            const string MethodName = "AddInboundConnection";
            Logger.TraceIn(this.name, MethodName);

            this.inboundConnections.Add(inboundConnection);

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

            this.outboundConnections.Add(outboundConnection);

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Calculates the node's error from the provided error signal.
        /// </summary>
        /// <param name="errorSignal">The error signal.</param>
        /// <remarks>
        /// This method will aggregate error signals as they are reported by the outbound connections.
        /// When all connections have reported a signal, then the aggregated signal will be processed to
        /// determine this neuron's error signal.
        /// </remarks>
        public void CalculateError(double errorSignal)
        {
            const string MethodName = "CalculateError(double)";
            Logger.TraceIn(this.name, MethodName);
            
            Logger.Debug(this.name, MethodName, "Current Error Signal", this.cachedError[0]);
            this.cachedError[0] += errorSignal;
            Logger.Debug(this.name, MethodName, "New Error Signal", this.cachedError[0]);

            // If all outbound connections are reporting an error, then calculate the error.
            // (Note: This condition will be true for output nodes with no outbound connections.)
            if (this.outboundConnections.All(connection => connection.IsReportingError))
            {
                this.CalculateError();
            }
            else
            {
                Logger.Debug(this.name, MethodName, "Not all connections are reporting an error signal");
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

            // Adjuust the bias.
            // Learning rate affects how much of the new adjustment to add.
            // Momentum affects how much of the previous adjustment to re-use.
            var biasAdjustment = (learningRate * this.pendingBiasAdjustment) + (momentum * this.previousBiasAdjustment);
            Logger.Debug(this.name, MethodName, "biasAdjustment", biasAdjustment);
            
            this.Bias += biasAdjustment;
            Logger.Debug(this.name, MethodName, "New Bias", this.Bias);

            this.previousBiasAdjustment = biasAdjustment;

            // Propagate command by notifying outbound connections.
            foreach (var connection in this.outboundConnections)
            {
                connection.ApplyWeightAdjustments(learningRate, momentum);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Clear the temporary training values between input sets.
        /// </summary>
        public void ClearTempTrainingValues()
        {
            const string MethodName = "ClearTempTrainingValues";
            Logger.TraceIn(this.name, MethodName);

            this.pendingBiasAdjustment = 0.0d;
            this.cachedError[0] = 0.0d;

            // Propagate command by notifying outbound connections.
            foreach (var connection in this.outboundConnections)
            {
                connection.ClearTempTrainingValues();
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Calculates the node's error from the accumulated error signal.
        /// </summary>
        /// <remarks>
        /// This method performs a component of the Delta Rule formula used in backpropagation:
        /// <code>
        /// Delta Rule ==> weightAdjustment[i] = learningRate * (idealOutput - actualOutput) * activationDerivative(sumInputs) * input[i]
        /// </code>
        /// This method performs the
        /// <code>
        /// (idealOutput - actualOutput) * activationDerivative(sumInputs)
        /// </code>
        /// portion of the formulat.  The other portions will be peformed in the connection objects.
        /// </remarks>
        private void CalculateError()
        {
            const string MethodName = "CalculateError";
            Logger.TraceIn(this.name, MethodName);

            var errorDelta = this.cachedError[0] * this.ActivationFunction.InvokeDerivative(this.CachedOutputs[0]);
            
            foreach (var connection in this.inboundConnections)
            {
               connection.ReportError(errorDelta);
            }

            this.pendingBiasAdjustment += errorDelta;
            Logger.Debug(this.name, MethodName, "pendingBiasAdjustment", this.pendingBiasAdjustment);

            Logger.TraceOut(this.name, MethodName);
        }
    }
}
