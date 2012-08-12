// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackpropagationNeuronTests.cs" company="The Logans Ferry Software Co.">
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

namespace LogansFerry.NeuroDotNet.UnitTests
{
    using System;

    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// A collection of tests for the BackpropagationNeuron.cs class.
    /// </summary>
    [TestFixture]
    public class BackpropagationNeuronTests
    {
        /// <summary>
        /// The required precision of floating point calculations
        /// </summary>
        private const double Epsilon = 0.000000000001d;

        /// <summary>
        /// Method:     AddInboundConnection
        /// Condition:  Null argument is provided.
        /// Results:    Argument Null Exception
        /// </summary>
        [Test]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void AddInboundConnection_ValidatesArgs()
        {
            // Setup
            var neuron = new BackpropagationNeuron(0.0d, new Mock<IActivationFunction>().Object);

            // Execute/Verify
            neuron.AddInboundConnection(null);
        }

        /// <summary>
        /// Method:     AddOutboundConnection
        /// Condition:  Null argument is provided.
        /// Results:    Argument Null Exception
        /// </summary>
        [Test]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void AddOutboundConnection_ValidatesArgs()
        {
            // Setup
            var neuron = new BackpropagationNeuron(0.0d, new Mock<IActivationFunction>().Object);

            // Execute/Verify
            neuron.AddOutboundConnection(null);
        }

        /// <summary>
        /// Method:         CalculateError
        /// Requirement:    The overall error will not be calculated until all outbound connections are reporting errors.
        /// </summary>
        [Test]
        public void CalculateError_DoesNotCalculateUntilAllConnectionsReport()
        {
            //// SETUP

            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutboundReporting = new Mock<ISupervisedLearnerConnection>();
            var mockOutboundNotReporting = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that one is reporting and one isn't
            mockOutboundReporting.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutboundNotReporting.SetupGet(mock => mock.IsReportingError).Returns(false);

            // mock activation function
            var mockActivationFunction = new Mock<IActivationFunction>();

            // Create the test object.
            var neuron = new BackpropagationNeuron(0.0d, mockActivationFunction.Object);
            neuron.AddInboundConnection(mockInbound1.Object);
            neuron.AddInboundConnection(mockInbound2.Object);
            neuron.AddOutboundConnection(mockOutboundReporting.Object);
            neuron.AddOutboundConnection(mockOutboundNotReporting.Object);
            
            // EXECUTION
            const double ErrorSignal = -2.3d;
            neuron.CalculateError(ErrorSignal);

            // VERIFICATION:  The IsReporting signals were checked...
            mockOutboundReporting.Verify(mock => mock.IsReportingError, Times.Exactly(1));
            mockOutboundNotReporting.Verify(mock => mock.IsReportingError, Times.Exactly(1));

            // ...but no calculation activities occurred.
            mockActivationFunction.Verify(mock => mock.InvokeDerivative(It.IsAny<double>()), Times.Never());
            mockInbound1.Verify(mock => mock.ReportError(It.IsAny<double>()), Times.Never());
            mockInbound2.Verify(mock => mock.ReportError(It.IsAny<double>()), Times.Never());
            mockOutboundReporting.Verify(mock => mock.ClearReportingFlag(), Times.Never());
            mockOutboundNotReporting.Verify(mock => mock.ClearReportingFlag(), Times.Never());
            Assert.AreEqual(1, neuron.CachedErrors.Length);
            Assert.AreEqual(ErrorSignal, neuron.CachedErrors[0], Epsilon);
        }

        /// <summary>
        /// Method:         CalculateError
        /// Requirement:    The overall error is calculated and reported after all outbound connections are reporting errors.
        /// </summary>
        [Test]
        public void CalculateError_CalculatesAfterAllConnectionsHaveReported()
        {
            //// SETUP
            const double CachedOutput = 1.23d;
            const double ErrorSignal1 = -2.3d;
            const double ErrorSignal2 = -9.87d;
            const double AccumulatedError = ErrorSignal1 + ErrorSignal2;
            const double Derivative = 4.567d;
            const double ErrorDelta = AccumulatedError * Derivative;

            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that one is reporting and one isn't
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(false);

            // mock activation function
            var mockActivationFunction = new Mock<IActivationFunction>();
            mockActivationFunction.Setup(mock => mock.Invoke(It.IsAny<double>())).Returns(CachedOutput);
            mockActivationFunction.Setup(mock => mock.InvokeDerivative(CachedOutput)).Returns(Derivative);

            // Create the test object.
            var neuron = new BackpropagationNeuron(0.0d, mockActivationFunction.Object);
            neuron.AddInboundConnection(mockInbound1.Object);
            neuron.AddInboundConnection(mockInbound2.Object);
            neuron.AddOutboundConnection(mockOutbound1.Object);
            neuron.AddOutboundConnection(mockOutbound2.Object);

            // EXECUTION
            neuron.Fire(new[] { 0.0d });
            neuron.CalculateError(ErrorSignal1);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);
            neuron.CalculateError(ErrorSignal2);

            // VERIFICATION:  The IsReporting signals were checked...
            mockOutbound1.Verify(mock => mock.IsReportingError, Times.Exactly(2));
            mockOutbound2.Verify(mock => mock.IsReportingError, Times.Exactly(2));

            // ...and calculation activities occurred.
            mockActivationFunction.Verify(mock => mock.InvokeDerivative(CachedOutput), Times.Once());
            mockInbound1.Verify(mock => mock.ReportError(ErrorDelta), Times.Once());
            mockInbound2.Verify(mock => mock.ReportError(ErrorDelta), Times.Once());
            mockOutbound1.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            mockOutbound2.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            Assert.AreEqual(1, neuron.CachedErrors.Length);
            Assert.AreEqual(AccumulatedError, neuron.CachedErrors[0], Epsilon);
        }

        /// <summary>
        /// Method:         Apply weight adjustments.
        /// Requirement:    Uses momentum and learning rate to apply a weight adjustment.
        /// </summary>
        [Test]
        public void ApplyWeightAdjustments_UpdatesBias()
        {
            //// SETUP
            const double CachedOutput = 1.23d;
            const double ErrorSignal1A = -2.3d;
            const double ErrorSignal1B = -9.87d;
            const double AccumulatedError1 = ErrorSignal1A + ErrorSignal1B;
            const double ErrorSignal2 = 3.2345d;
            const double Derivative = 4.567d;
            const double ErrorDelta1 = AccumulatedError1 * Derivative;
            const double ErrorDelta2 = ErrorSignal2 * Derivative;
            const float Momentum = 0.9f;
            const float LearningRate = 0.1f;

            const double Expected1 = ErrorDelta1 * LearningRate;
            const double Expected2 = Expected1 + (ErrorDelta2 * LearningRate) + (Momentum * Expected1);

            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that one is reporting and one isn't
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(false);

            // mock activation function
            var mockActivationFunction = new Mock<IActivationFunction>();
            mockActivationFunction.Setup(mock => mock.Invoke(It.IsAny<double>())).Returns(CachedOutput);
            mockActivationFunction.Setup(mock => mock.InvokeDerivative(CachedOutput)).Returns(Derivative);

            // Create the test object.
            var neuron = new BackpropagationNeuron(0.0d, mockActivationFunction.Object);
            neuron.AddInboundConnection(mockInbound1.Object);
            neuron.AddInboundConnection(mockInbound2.Object);
            neuron.AddOutboundConnection(mockOutbound1.Object);
            neuron.AddOutboundConnection(mockOutbound2.Object);

            neuron.Fire(new[] { 0.0d });
            
            // EXECUTION
            neuron.CalculateError(ErrorSignal1A);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);
            neuron.CalculateError(ErrorSignal1B);
            neuron.ApplyWeightAdjustments(LearningRate, Momentum);
            var actual1 = neuron.Bias;

            neuron.ClearCachedErrors();
            neuron.CalculateError(ErrorSignal2);
            neuron.ApplyWeightAdjustments(LearningRate, Momentum);
            var actual2 = neuron.Bias;

            // VERIFICATION
            Assert.AreEqual(Expected1, actual1, Epsilon);
            Assert.AreEqual(Expected2, actual2, Epsilon);
        }

        /// <summary>
        /// Method:         ApplyWeightAdjustments
        /// Requirement:    Propogates method throughout the network by invoking same method on outgoing connections.
        /// </summary>
        [Test]
        public void ApplyWeightAdjustments_PropogatesThroughNetwork()
        {
            //// SETUP
            const double CachedOutput = 1.23d;
            const double ErrorSignal = -2.3d;
            const double Derivative = 4.567d;
            const float Momentum = 0.9f;
            const float LearningRate = 0.1f;
            
            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that both are reporting error signals
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);
            
            // mock activation function
            var mockActivationFunction = new Mock<IActivationFunction>();
            mockActivationFunction.Setup(mock => mock.Invoke(It.IsAny<double>())).Returns(CachedOutput);
            mockActivationFunction.Setup(mock => mock.InvokeDerivative(CachedOutput)).Returns(Derivative);

            // Create the test object.
            var neuron = new BackpropagationNeuron(0.0d, mockActivationFunction.Object);
            neuron.AddInboundConnection(mockInbound1.Object);
            neuron.AddInboundConnection(mockInbound2.Object);
            neuron.AddOutboundConnection(mockOutbound1.Object);
            neuron.AddOutboundConnection(mockOutbound2.Object);

            neuron.Fire(new[] { 0.0d });
            neuron.CalculateError(ErrorSignal);
            
            // EXECUTION
            neuron.ApplyWeightAdjustments(LearningRate, Momentum);
            
            // VERIFICATION
            mockOutbound1.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Once());
            mockOutbound2.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Once());
        }

        /// <summary>
        /// Method:         ClearCachedErrors
        /// Requirement:    Clears the cached error value to 0.0d
        /// </summary>
        [Test]
        public void ClearCachedErrors_ClearsCachedError()
        {
            //// SETUP
            const double CachedOutput = 1.23d;
            const double ErrorSignal = -2.3d;
            const double Derivative = 4.567d;
            
            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that both are reporting error signals
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);

            // mock activation function
            var mockActivationFunction = new Mock<IActivationFunction>();
            mockActivationFunction.Setup(mock => mock.Invoke(It.IsAny<double>())).Returns(CachedOutput);
            mockActivationFunction.Setup(mock => mock.InvokeDerivative(CachedOutput)).Returns(Derivative);

            // Create the test object.
            var neuron = new BackpropagationNeuron(0.0d, mockActivationFunction.Object);
            neuron.AddInboundConnection(mockInbound1.Object);
            neuron.AddInboundConnection(mockInbound2.Object);
            neuron.AddOutboundConnection(mockOutbound1.Object);
            neuron.AddOutboundConnection(mockOutbound2.Object);

            neuron.Fire(new[] { 0.0d });

            // EXECUTION
            neuron.CalculateError(ErrorSignal);
            var cachedError = neuron.CachedErrors[0];
            neuron.ClearCachedErrors();
            var clearedError = neuron.CachedErrors[0];
            
            // VERIFICATION
            Assert.AreEqual(1, neuron.CachedErrors.Length);
            Assert.AreNotEqual(0.0d, cachedError);
            Assert.AreEqual(0.0d, clearedError, Epsilon);
        }

        /// <summary>
        /// Method:         ClearCachedErrors
        /// Requirement:    Propogates command through the network.
        /// </summary>
        [Test]
        public void ClearCachedErrors_PropogatesThroughNetwork()
        {
            //// SETUP
            const double CachedOutput = 1.23d;
            const double Derivative = 4.567d;

            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that both are reporting error signals
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);

            // mock activation function
            var mockActivationFunction = new Mock<IActivationFunction>();
            mockActivationFunction.Setup(mock => mock.Invoke(It.IsAny<double>())).Returns(CachedOutput);
            mockActivationFunction.Setup(mock => mock.InvokeDerivative(CachedOutput)).Returns(Derivative);

            // Create the test object.
            var neuron = new BackpropagationNeuron(0.0d, mockActivationFunction.Object);
            neuron.AddInboundConnection(mockInbound1.Object);
            neuron.AddInboundConnection(mockInbound2.Object);
            neuron.AddOutboundConnection(mockOutbound1.Object);
            neuron.AddOutboundConnection(mockOutbound2.Object);

            neuron.Fire(new[] { 0.0d });

            // EXECUTION
            neuron.ClearCachedErrors();
            
            // VERIFICATION
            mockOutbound1.Verify(mock => mock.ClearCachedErrors(), Times.Once());
            mockOutbound1.Verify(mock => mock.ClearCachedErrors(), Times.Once());
        }
    }
}
