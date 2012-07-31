// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackpropagationConnection.cs" company="The Logans Ferry Software Co.">
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
    /// A weighted connection between two nodes that supports backpropagation learning.
    /// </summary>
    public class BackpropagationConnection : NeuralConnection, ISupervisedLearnerConnection
    {
        /// <summary>
        /// The connection's name (for logging purposes).
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The weight adjustment that will be applied to this connection when the network's weights are adjusted.
        /// </summary>
        private double pendingWeightAdjustment;

        /// <summary>
        /// The weight adjustment that was applied the last time connection weights were adjusted.
        /// </summary>
        private double previousWeightAdjustment;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackpropagationConnection"/> class.
        /// </summary>
        /// <param name="initialWeight">The connection's initial weight.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="targetNode">The target node.</param>
        public BackpropagationConnection(double initialWeight, ISupervisedLearnerNode sourceNode, ISupervisedLearnerNode targetNode)
            : base(initialWeight, sourceNode, targetNode)
        {
            // Initialize the class name that will appear in log files.
            this.name = sourceNode.Name + "->" + targetNode.Name;
            const string MethodName = "ctor";
            Logger.TraceIn(this.name, MethodName);

            // Ensure that the end-point nodes have a reference to this connection object.
            sourceNode.AddOutboundConnection(this);
            targetNode.AddInboundConnection(this);

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Gets the current error signal that is being reported.
        /// </summary>
        public double ErrorSignal { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this connection is currently reporting an error signal.
        /// </summary>
        /// <value>
        /// <c>true</c> if this connection is reporting an error signal; otherwise, <c>false</c>.
        /// </value>
        public bool IsReportingError { get; private set; }

        /// <summary>
        /// Gets the source node of the connection.
        /// </summary>
        /// <value>
        /// The source node.
        /// </value>
        private new ISupervisedLearnerNode SourceNode
        {
            get
            {
                return base.SourceNode as ISupervisedLearnerNode;
            }
        }

        /// <summary>
        /// Gets the target node of the connection.
        /// </summary>
        /// <value>
        /// The target node.
        /// </value>
        private new ISupervisedLearnerNode TargetNode
        {
            get
            {
                return base.TargetNode as ISupervisedLearnerNode;
            }
        }

        /// <summary>
        /// Reports the error signal to the source node.
        /// </summary>
        /// <param name="errorSignal">The error signal.</param>
        /// <remarks>
        /// <para>
        /// This method receives an error signal from the target node, modifies the signal
        /// based on the connection's weight, and then reports the signal to the source node.
        /// Note that the signal is moving from target to source because this type of connection
        /// backpropagates signals.
        /// </para>
        /// <para>
        /// This method also completes the Delta Rule for calculating weight adjustments by multiplying
        /// the reported error signal by the original input that was received during output computations.
        /// Remember that the remainder of the Delta Rule was already calculated in the Target Node.
        /// </para>
        /// </remarks>
        public void ReportError(double errorSignal)
        {
            const string MethodName = "ReportError";
            Logger.TraceIn(this.name, MethodName);

            // Complete the delta rule.
            this.pendingWeightAdjustment += errorSignal * this.CachedInput;
            Logger.Debug(this.name, MethodName, "pendingWeightAdjustment", this.pendingWeightAdjustment);

            // Modify and backpropagate the error signal.
            this.ErrorSignal = errorSignal * this.Weight;
            Logger.Debug(this.name, MethodName, "ErrorSignal", this.ErrorSignal);
            this.IsReportingError = true;
            this.SourceNode.CalculateError(this.ErrorSignal);

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Applies the pending weight adjustments to the connection.
        /// </summary>
        /// <param name="learningRate">The learning rate.</param>
        /// <param name="momentum">The momentum.</param>
        public void ApplyWeightAdjustments(float learningRate, float momentum)
        {
            const string MethodName = "ApplyWeightAdjustments";
            Logger.TraceIn(this.name, MethodName);

            // Use the learning rate to apply the pending weight adjustment.
            // Use the momentum to re-apply the previous weight adjustment.
            var weightAdjustment = (learningRate * this.pendingWeightAdjustment) + (momentum * this.previousWeightAdjustment);
            Logger.Debug(this.name, MethodName, "weightAdjustment", weightAdjustment);
            this.Weight += weightAdjustment;
            Logger.Debug(this.name, MethodName, "Weight", this.Weight);
            
            // This weight adjustment is now the previous weight adjustment.
            this.previousWeightAdjustment = weightAdjustment;
            this.pendingWeightAdjustment = 0.0d;
            
            // Propagate the command.
            this.TargetNode.ApplyWeightAdjustments(learningRate, momentum);

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Clears the IsReporting flag.
        /// </summary>
        public void ClearReportingFlag()
        {
            const string MethodName = "ClearReportingFlag";
            Logger.TraceIn(this.name, MethodName);

            this.IsReportingError = false;

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Clears the temporary training values.
        /// </summary>
        public void ClearTempTrainingValues()
        {
            const string MethodName = "ClearTempTrainingValues";
            Logger.TraceIn(this.name, MethodName);

            // Propagate the command.
            this.TargetNode.ClearTempTrainingValues();

            Logger.TraceOut(this.name, MethodName);
        }
    }
}
