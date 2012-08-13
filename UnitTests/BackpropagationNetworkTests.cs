// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackpropagationNetworkTests.cs" company="The Logans Ferry Software Co.">
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
    public class BackpropagationNetworkTests
    {
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
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);

            // Execute/Verify
            network.AddInboundConnection(null);
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
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);

            // Execute/Verify
            network.AddOutboundConnection(null);
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

            // Create 2 input nodes and 2 output nodes.
            var mockInputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockInputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode2 = new Mock<ISupervisedLearnerNode>();

            // Create the test object.
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);
            network.AddInboundConnection(mockInbound1.Object);
            network.AddInboundConnection(mockInbound2.Object);
            network.AddOutboundConnection(mockOutboundReporting.Object);
            network.AddOutboundConnection(mockOutboundNotReporting.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            const double ErrorSignal = -2.3d;
            network.CalculateError(ErrorSignal);

            // VERIFICATION:  The IsReporting signals were checked...
            mockOutboundReporting.Verify(mock => mock.IsReportingError, Times.Exactly(1));
            mockOutboundNotReporting.Verify(mock => mock.IsReportingError, Times.Exactly(1));

            // ...but no calculation activities occurred.
            mockInbound1.Verify(mock => mock.ReportError(It.IsAny<double>()), Times.Never());
            mockInbound2.Verify(mock => mock.ReportError(It.IsAny<double>()), Times.Never());
            mockOutboundReporting.Verify(mock => mock.ClearReportingFlag(), Times.Never());
            mockOutboundNotReporting.Verify(mock => mock.ClearReportingFlag(), Times.Never());
            mockInputNode1.VerifyGet(mock => mock.CachedErrors, Times.Never());
            mockInputNode2.VerifyGet(mock => mock.CachedErrors, Times.Never());
            mockOutputNode1.Verify(mock => mock.CalculateError(It.IsAny<double>()), Times.Never());
            mockOutputNode2.Verify(mock => mock.CalculateError(It.IsAny<double>()), Times.Never());
        }

        /// <summary>
        /// Method:         CalculateError
        /// Requirement:    The overall error is calculated and reported after all outbound connections are reporting errors.
        /// </summary>
        [Test]
        public void CalculateError_CalculatesAfterAllConnectionsHaveReported()
        {
            //// SETUP
            const double ErrorSignal1 = -0.5d;
            const double ErrorSignal2 = 0.5d;
            const double DigitizedErrorSignal1 = -1.0d;
            const double DigitizedErrorSignal2 = 1.0d;
            const double NetworkErrorSignal1 = -1.5d;
            const double NetworkErrorSignal2 = 1.5d;
            var expectedNetworkErrors = new[] { NetworkErrorSignal1, NetworkErrorSignal2 };

            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that one is reporting and one isn't
            mockOutbound1.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal1);
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(false);

            // Create 2 input nodes and 2 output nodes.
            var mockInputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockInputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode2 = new Mock<ISupervisedLearnerNode>();

            // Program nodes to report input/output sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Program nodes to provide values.
            mockInputNode1.SetupGet(mock => mock.CachedErrors).Returns(new[] { NetworkErrorSignal1 });
            mockInputNode2.SetupGet(mock => mock.CachedErrors).Returns(new[] { NetworkErrorSignal2 });
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            
            // Create the test object.
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);

            network.AddInboundConnection(mockInbound1.Object);
            network.AddInboundConnection(mockInbound2.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);

            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.Fire(new[] { 0.0d });
            network.CalculateError(ErrorSignal1);
            mockOutbound2.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal2);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);
            network.CalculateError(ErrorSignal2);

            // VERIFICATION:  The IsReporting signals were checked...
            mockOutbound1.Verify(mock => mock.IsReportingError, Times.Exactly(2));
            mockOutbound2.Verify(mock => mock.IsReportingError, Times.Exactly(2));

            // ...and calculation activities occurred.
            mockInbound1.Verify(mock => mock.ReportError(NetworkErrorSignal1), Times.Once());
            mockInbound2.Verify(mock => mock.ReportError(NetworkErrorSignal2), Times.Once());
            mockOutbound1.VerifyGet(mock => mock.ErrorSignal, Times.Once());
            mockOutbound1.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            mockOutbound2.VerifyGet(mock => mock.ErrorSignal, Times.Once());
            mockOutbound2.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            mockInputNode1.VerifyGet(mock => mock.CachedErrors, Times.AtLeastOnce());
            mockInputNode2.VerifyGet(mock => mock.CachedErrors, Times.AtLeastOnce());
            mockOutputNode1.Verify(mock => mock.CalculateError(DigitizedErrorSignal1), Times.Once());
            mockOutputNode2.Verify(mock => mock.CalculateError(DigitizedErrorSignal2), Times.Once());
            Assert.AreEqual(2, network.CachedErrors.Length);
            Assert.AreEqual(expectedNetworkErrors, network.CachedErrors);
        }

        /// <summary>
        /// Method:         CalculateError
        /// Requirement:    Will aggregate signals to when there are more signals than output nodes.
        /// </summary>
        [Test]
        public void CalculateError_AggregatesSignals()
        {
            //// SETUP
            const double ErrorSignal1 = -0.5d;
            const double ErrorSignal2 = -1.5d;
            const double ErrorSignal3 = -2.5d;
            const double ErrorSignal4 = 1.5d;
            const double ErrorSignal5 = 2.5d;
            const double AggregatedErrorSignal1 = -3.0d;  // After digitization on each individual signal.
            const double AggregatedErrorSignal2 = 2.0d;   // After digitization on each individual signal.
            const double NetworkErrorSignal1 = -1.0d;
            const double NetworkErrorSignal2 = 1.0d;
            var expectedNetworkErrors = new[] { NetworkErrorSignal1, NetworkErrorSignal2 };

            // Create 2 inbound and 5 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound3 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound4 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound5 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that all are reporting their respective errors
            mockOutbound1.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal1);
            mockOutbound2.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal2);
            mockOutbound3.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal3);
            mockOutbound4.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal4);
            mockOutbound5.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal5);
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound3.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound4.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound5.SetupGet(mock => mock.IsReportingError).Returns(true);

            // Create 2 input nodes and 2 output nodes.
            var mockInputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockInputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode2 = new Mock<ISupervisedLearnerNode>();

            // Program nodes to report input/output sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Program nodes to provide values.
            mockInputNode1.SetupGet(mock => mock.CachedErrors).Returns(new[] { NetworkErrorSignal1 });
            mockInputNode2.SetupGet(mock => mock.CachedErrors).Returns(new[] { NetworkErrorSignal2 });
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });

            // Create the test object.
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);

            network.AddInboundConnection(mockInbound1.Object);
            network.AddInboundConnection(mockInbound2.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddOutboundConnection(mockOutbound3.Object);
            network.AddOutboundConnection(mockOutbound4.Object);
            network.AddOutboundConnection(mockOutbound5.Object);

            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.Fire(new[] { 0.0d });
            network.CalculateError(ErrorSignal1);
            
            // VERIFICATION:  The IsReporting signals were checked...
            mockOutbound1.Verify(mock => mock.IsReportingError, Times.Exactly(1));
            mockOutbound2.Verify(mock => mock.IsReportingError, Times.Exactly(1));

            // ...and calculation activities occurred.
            mockInbound1.Verify(mock => mock.ReportError(NetworkErrorSignal1), Times.Once());
            mockInbound2.Verify(mock => mock.ReportError(NetworkErrorSignal2), Times.Once());
            mockOutbound1.VerifyGet(mock => mock.ErrorSignal, Times.Once());
            mockOutbound1.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            mockOutbound2.VerifyGet(mock => mock.ErrorSignal, Times.Once());
            mockOutbound2.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            mockInputNode1.VerifyGet(mock => mock.CachedErrors, Times.AtLeastOnce());
            mockInputNode2.VerifyGet(mock => mock.CachedErrors, Times.AtLeastOnce());
            mockOutputNode1.Verify(mock => mock.CalculateError(AggregatedErrorSignal1), Times.Once());
            mockOutputNode2.Verify(mock => mock.CalculateError(AggregatedErrorSignal2), Times.Once());
            Assert.AreEqual(2, network.CachedErrors.Length);
            Assert.AreEqual(expectedNetworkErrors, network.CachedErrors);
        }

        /// <summary>
        /// Method:         CalculateError
        /// Requirement:    Will distribute signals to when there are fewer signals than output nodes.
        /// </summary>
        [Test]
        public void CalculateError_DistributesSignals()
        {
            //// SETUP
            const double ErrorSignal1 = -0.5d;
            const double ErrorSignal2 = 1.5d;
            const double DigitizedErrorSignal1 = -1.0d;
            const double DigitizedErrorSignal2 = 1.0d;
            const double NetworkErrorSignal1 = -1.0d;
            const double NetworkErrorSignal2 = 1.0d;
            var expectedNetworkErrors = new[] { NetworkErrorSignal1, NetworkErrorSignal2 };

            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();
            
            // program the mock outbounds such that all are reporting their respective errors
            mockOutbound1.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal1);
            mockOutbound2.SetupGet(mock => mock.ErrorSignal).Returns(ErrorSignal2);
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);
            
            // Create 2 input nodes and 5 output nodes.
            var mockInputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockInputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode3 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode4 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode5 = new Mock<ISupervisedLearnerNode>();

            // Program nodes to report input/output sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode3.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode4.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode5.SetupGet(mock => mock.OutputSize).Returns(1);

            // Program nodes to provide values.
            mockInputNode1.SetupGet(mock => mock.CachedErrors).Returns(new[] { NetworkErrorSignal1 });
            mockInputNode2.SetupGet(mock => mock.CachedErrors).Returns(new[] { NetworkErrorSignal2 });
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode3.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode4.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode5.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });

            // Create the test object.
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);

            network.AddInboundConnection(mockInbound1.Object);
            network.AddInboundConnection(mockInbound2.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);
            network.AddOutputNode(mockOutputNode3.Object);
            network.AddOutputNode(mockOutputNode4.Object);
            network.AddOutputNode(mockOutputNode5.Object);

            // EXECUTION
            network.Fire(new[] { 0.0d });
            network.CalculateError(ErrorSignal1);

            // VERIFICATION:  The IsReporting signals were checked...
            mockOutbound1.Verify(mock => mock.IsReportingError, Times.Exactly(1));
            mockOutbound2.Verify(mock => mock.IsReportingError, Times.Exactly(1));

            // ...and calculation activities occurred.
            mockInbound1.Verify(mock => mock.ReportError(NetworkErrorSignal1), Times.Once());
            mockInbound2.Verify(mock => mock.ReportError(NetworkErrorSignal2), Times.Once());
            mockOutbound1.VerifyGet(mock => mock.ErrorSignal, Times.Once());
            mockOutbound1.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            mockOutbound2.VerifyGet(mock => mock.ErrorSignal, Times.Once());
            mockOutbound2.Verify(mock => mock.ClearReportingFlag(), Times.Once());
            mockInputNode1.VerifyGet(mock => mock.CachedErrors, Times.AtLeastOnce());
            mockInputNode2.VerifyGet(mock => mock.CachedErrors, Times.AtLeastOnce());
            mockOutputNode1.Verify(mock => mock.CalculateError(DigitizedErrorSignal1), Times.Once());
            mockOutputNode2.Verify(mock => mock.CalculateError(DigitizedErrorSignal1), Times.Once());
            mockOutputNode3.Verify(mock => mock.CalculateError(DigitizedErrorSignal1), Times.Once());
            mockOutputNode4.Verify(mock => mock.CalculateError(DigitizedErrorSignal2), Times.Once());
            mockOutputNode5.Verify(mock => mock.CalculateError(DigitizedErrorSignal2), Times.Once());
            Assert.AreEqual(2, network.CachedErrors.Length);
            Assert.AreEqual(expectedNetworkErrors, network.CachedErrors);
        }

        /// <summary>
        /// Method:         ApplyWeightAdjustments
        /// Requirement:    Propogates method throughout the network by invoking same method on input nodes and outgoing connections.
        /// </summary>
        [Test]
        public void ApplyWeightAdjustments_PropogatesThroughNetwork()
        {
            //// SETUP
            const double ErrorSignal = -2.3d;
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

            // Create 2 input nodes and 2 output nodes.
            var mockInputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockInputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode2 = new Mock<ISupervisedLearnerNode>();

            // Program nodes to report input/output sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Program nodes to provide values.
            mockInputNode1.SetupGet(mock => mock.CachedErrors).Returns(new[] { ErrorSignal });
            mockInputNode2.SetupGet(mock => mock.CachedErrors).Returns(new[] { ErrorSignal });
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            
            // Create the test object.
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);
            network.AddInboundConnection(mockInbound1.Object);
            network.AddInboundConnection(mockInbound2.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);
            
            network.Fire(new[] { 0.0d });
            network.CalculateError(ErrorSignal);
            
            // EXECUTION
            network.ApplyWeightAdjustments(LearningRate, Momentum);
            
            // VERIFICATION
            mockInputNode1.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Once());
            mockInputNode2.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Once());
            mockOutbound1.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Once());
            mockOutbound2.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Once());
        }

        /// <summary>
        /// Method:         ClearCachedErrors
        /// Requirement:    Propogates command through the network.
        /// </summary>
        [Test]
        public void ClearCachedErrors_PropogatesThroughNetwork()
        {
            ///// SETUP
            const double ErrorSignal = -2.3d;

            // Create 2 inbound and 2 outbound mock connections.
            var mockInbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockInbound2 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound1 = new Mock<ISupervisedLearnerConnection>();
            var mockOutbound2 = new Mock<ISupervisedLearnerConnection>();

            // program the mock outbounds such that both are reporting error signals
            mockOutbound1.SetupGet(mock => mock.IsReportingError).Returns(true);
            mockOutbound2.SetupGet(mock => mock.IsReportingError).Returns(true);

            // Create 2 input nodes and 2 output nodes.
            var mockInputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockInputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode2 = new Mock<ISupervisedLearnerNode>();

            // Program nodes to report input/output sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Program nodes to provide values.
            mockInputNode1.SetupGet(mock => mock.CachedErrors).Returns(new[] { ErrorSignal });
            mockInputNode2.SetupGet(mock => mock.CachedErrors).Returns(new[] { ErrorSignal });
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { 0.0d });

            // Create the test object.
            var network = new BackpropagationNetwork(new Mock<IErrorCalculator>().Object);
            network.AddInboundConnection(mockInbound1.Object);
            network.AddInboundConnection(mockInbound2.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.ClearCachedErrors();
            
            // VERIFICATION
            mockInputNode1.Verify(mock => mock.ClearCachedErrors(), Times.Once());
            mockInputNode2.Verify(mock => mock.ClearCachedErrors(), Times.Once());
            mockOutbound1.Verify(mock => mock.ClearCachedErrors(), Times.Once());
            mockOutbound2.Verify(mock => mock.ClearCachedErrors(), Times.Once());
        }

        /// <summary>
        /// Method:         Train
        /// Requirement:    Trains the network using the provided params.
        /// </summary>
        [Test]
        public void Train_TrainsTheNetwork()
        {
            //// SETUP

            const int NumEpochs = 3;
            const float LearningRate = 0.1f;
            const float Momentum = 0.9f;
            const double FinalError = 99.99d;

            var inputs = new[] { new[] { 1.0d, 2.0d }, new[] { 3.0d, 4.0d } };
            var ideals = new[] { new[] { 5.0d, 6.0d }, new[] { 7.0d, 8.0d } };
            var outputs = new[] { new[] { 3.5d, 5.5d }, new[] { 7.5d, 9.5d } };
            var overallErrors = new[]
                {
                    new[] { ideals[0][0] - outputs[0][0], ideals[0][1] - outputs[0][1] }, 
                    new[] { ideals[1][0] - outputs[1][0], ideals[1][1] - outputs[1][1] }
                };
            var finalErrors = new[] { new[] { 1.5d, 2.5d }, new[] { 3.5d, 4.5d } };

            var mockErrorCalc = new Mock<IErrorCalculator>();
            mockErrorCalc.Setup(mock => mock.Calculate()).Returns(FinalError);

            // Create 2 input nodes and 2 output nodes.
            var mockInputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockInputNode2 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode1 = new Mock<ISupervisedLearnerNode>();
            var mockOutputNode2 = new Mock<ISupervisedLearnerNode>();

            // Program nodes to report input/output sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Program output nodes to return output values based on inputs.
            mockInputNode1.Setup(mock => mock.Fire(new[] { inputs[0][0] })).Callback(() => mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { outputs[0][0] }));
            mockInputNode2.Setup(mock => mock.Fire(new[] { inputs[0][1] })).Callback(() => mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { outputs[0][1] }));
            mockInputNode1.Setup(mock => mock.Fire(new[] { inputs[1][0] })).Callback(() => mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { outputs[1][0] }));
            mockInputNode2.Setup(mock => mock.Fire(new[] { inputs[1][1] })).Callback(() => mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { outputs[1][1] }));

            // Program input nodes to return final error based on overal errors.
            mockOutputNode1.Setup(mock => mock.CalculateError(overallErrors[0][0])).Callback(
                () => mockInputNode1.SetupGet(mock => mock.CachedErrors).Returns(new[] { finalErrors[0][0] }));
            mockOutputNode1.Setup(mock => mock.CalculateError(overallErrors[1][0])).Callback(
                () => mockInputNode1.SetupGet(mock => mock.CachedErrors).Returns(new[] { finalErrors[1][0] }));
            mockOutputNode2.Setup(mock => mock.CalculateError(overallErrors[0][1])).Callback(
                () => mockInputNode2.SetupGet(mock => mock.CachedErrors).Returns(new[] { finalErrors[0][1] }));
            mockOutputNode2.Setup(mock => mock.CalculateError(overallErrors[1][1])).Callback(
                () => mockInputNode2.SetupGet(mock => mock.CachedErrors).Returns(new[] { finalErrors[1][1] }));

            // Create the network
            var network = new BackpropagationNetwork(mockErrorCalc.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // Execute
            var actualError = network.Train(NumEpochs, LearningRate, Momentum, inputs, ideals);

            //// Verify
            
            // Use of error calculator
            mockErrorCalc.Verify(mock => mock.Reset(), Times.Exactly(NumEpochs));
            mockErrorCalc.Verify(mock => mock.AddToErrorCalc(overallErrors[0]), Times.Exactly(NumEpochs));
            mockErrorCalc.Verify(mock => mock.AddToErrorCalc(overallErrors[1]), Times.Exactly(NumEpochs));
            mockErrorCalc.Verify(mock => mock.Calculate(), Times.Once());

            // Use of input nodes
            mockInputNode1.Verify(mock => mock.Fire(new[] { inputs[0][0] }), Times.Exactly(NumEpochs));
            mockInputNode1.Verify(mock => mock.Fire(new[] { inputs[1][0] }), Times.Exactly(NumEpochs));
            mockInputNode2.Verify(mock => mock.Fire(new[] { inputs[0][1] }), Times.Exactly(NumEpochs));
            mockInputNode2.Verify(mock => mock.Fire(new[] { inputs[1][1] }), Times.Exactly(NumEpochs));

            mockInputNode1.Verify(mock => mock.ClearCachedErrors(), Times.Exactly(NumEpochs * inputs.Length));
            mockInputNode2.Verify(mock => mock.ClearCachedErrors(), Times.Exactly(NumEpochs * inputs.Length));

            mockInputNode1.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Exactly(NumEpochs));
            mockInputNode2.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Exactly(NumEpochs));

            // Use of output nodes
            mockOutputNode1.VerifyGet(mock => mock.CachedOutputs, Times.AtLeast(NumEpochs));
            mockOutputNode2.VerifyGet(mock => mock.CachedOutputs, Times.AtLeast(NumEpochs));
            
            mockOutputNode1.Verify(mock => mock.CalculateError(overallErrors[0][0]), Times.Exactly(NumEpochs));
            mockOutputNode1.Verify(mock => mock.CalculateError(overallErrors[1][0]), Times.Exactly(NumEpochs));
            mockOutputNode2.Verify(mock => mock.CalculateError(overallErrors[0][1]), Times.Exactly(NumEpochs));
            mockOutputNode2.Verify(mock => mock.CalculateError(overallErrors[1][1]), Times.Exactly(NumEpochs));

            // Verify Error
            Assert.AreEqual(FinalError, actualError);
        }
    }
}
