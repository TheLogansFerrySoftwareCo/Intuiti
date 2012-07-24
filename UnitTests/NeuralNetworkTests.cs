// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeuralNetworkTests.cs" company="The Logans Ferry Software Co.">
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
    /// A collection of unit tests for the NeuralNetwork.cs class.
    /// </summary>
    [TestFixture]
    public class NeuralNetworkTests
    {
        /// <summary>
        /// Property:   InputSize
        /// Condition:  Network has an input layer.
        /// Results:    InputSize = Sum of input layer node InputSizes
        /// </summary>
        [Test]
        public void InputSize_BasedOnSumOfInputLayerSizes()
        {
            const int InputSize1 = 2;
            const int InputSize2 = 5;
            const int Expected = InputSize1 + InputSize2;

            // Setup:  Program mock input nodes.
            var mockNode1 = new Mock<INeuralNode>();
            var mockNode2 = new Mock<INeuralNode>();
            mockNode1.SetupGet(mock => mock.InputSize).Returns(InputSize1);
            mockNode2.SetupGet(mock => mock.InputSize).Returns(InputSize2);

            // setup network
            var network = new NeuralNetwork();
            network.AddInputNode(mockNode1.Object);
            network.AddInputNode(mockNode2.Object);

            // Verify
            Assert.AreEqual(Expected, network.InputSize);
            mockNode1.VerifyGet(mock => mock.InputSize, Times.Once());
            mockNode2.VerifyGet(mock => mock.InputSize, Times.Once());
        }

        /// <summary>
        /// Property:   InputSize
        /// Condition:  Network has no input layer.
        /// Results:    InputSize = 0
        /// </summary>
        [Test]
        public void InputSize_NoExceptionWithNoInputLayer()
        {
            // setup network
            var network = new NeuralNetwork();
            
            // Verify
            Assert.AreEqual(0, network.InputSize);
        }

        /// <summary>
        /// Property:   OutputSize
        /// Condition:  Network has an output layer.
        /// Results:    OutputSize = Sum of output layer node OutputSizes
        /// </summary>
        [Test]
        public void OutputSize_BasedOnSumOfOutputLayerSizes()
        {
            const int OutputSize1 = 2;
            const int OutputSize2 = 5;
            const int Expected = OutputSize1 + OutputSize2;

            // Setup:  Program mock output nodes.
            var mockNode1 = new Mock<INeuralNode>();
            var mockNode2 = new Mock<INeuralNode>();
            mockNode1.SetupGet(mock => mock.OutputSize).Returns(OutputSize1);
            mockNode2.SetupGet(mock => mock.OutputSize).Returns(OutputSize2);

            // setup network
            var network = new NeuralNetwork();
            network.AddOutputNode(mockNode1.Object);
            network.AddOutputNode(mockNode2.Object);

            // Verify
            Assert.AreEqual(Expected, network.OutputSize);
            mockNode1.VerifyGet(mock => mock.OutputSize, Times.Once());
            mockNode2.VerifyGet(mock => mock.OutputSize, Times.Once());
        }

        /// <summary>
        /// Property:   OutputSize
        /// Condition:  Network has no output layer.
        /// Results:    OutputSize = 0
        /// </summary>
        [Test]
        public void OutputSize_NoExceptionWithNoOutputLayer()
        {
            // setup network
            var network = new NeuralNetwork();

            // Verify
            Assert.AreEqual(0, network.OutputSize);
        }

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
            var network = new NeuralNetwork();

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
            var network = new NeuralNetwork();

            // Execute/Verify
            network.AddOutboundConnection(null);
        }

        /// <summary>
        /// Method:     Fire (double)
        /// Condition:  Inbound connections fire in sequence.
        /// Result:     1) Network accumulates input signals and activates after final signal is received.
        ///             2) Activation causes network to initiate internal computations.
        ///             3) Internal computations are propogated forward to outbound connections.
        /// </summary>
        [Test]
        public void Fire_ActivatesAfterAllInboundsFire()
        {
            const double Input1 = -2.3d;
            const double DigitizedInput1 = -1.0d;
            const double Input2 = 0.3d;
            const double DigitizedInput2 = 1.0d;
            const double Output1 = -0.01d;
            const double DigitizedOutput1 = -1.0d;
            const double Output2 = 2.6d;
            const double DigitizedOutput2 = 1.0d;

            var outputs = new[] { Output1, Output2 };

            //// SETUP

            // Create 2 input and 2 output mock nodes
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();

            // Program mock nodes
            
            // Input nodes have an input size of 1
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);
            
            // Output nodes have an output size of 1
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);
            
            // Configure the output nodes to return the output constants.
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output1 });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output2 });
            
            // Create 2 inbound and 1 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // program the mock inbounds such that one is fired and one isn't
            mockInboundFired.SetupGet(mock => mock.IsFired).Returns(true);
            mockInboundUnfired.SetupGet(mock => mock.IsFired).Returns(false);
            mockInboundFired.SetupGet(mock => mock.Output).Returns(Input1);
            mockInboundUnfired.SetupGet(mock => mock.Output).Returns(Input2);

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInboundConnection(mockInboundFired.Object);
            network.AddInboundConnection(mockInboundUnfired.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // Archive the initialized output values for later verification.
            var initialOutput = network.CachedOutputs;
            
            // EXECUTION #1:  Not all inbound connections have fired.
            network.Fire(Input1);  // Fire with Input 1
            
            // VERIFICATION #1:  The fire signals were checked...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Exactly(1));
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Exactly(1));

            // ...but no activation activities occurred.
            mockInputNode1.Verify(mock => mock.Fire(It.IsAny<double[]>()), Times.Never());
            mockInputNode2.Verify(mock => mock.Fire(It.IsAny<double[]>()), Times.Never());
            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.Never());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.Never());
            mockOutbound1.Verify(mock => mock.Fire(It.IsAny<double>()), Times.Never());
            mockOutbound2.Verify(mock => mock.Fire(It.IsAny<double>()), Times.Never());
            Assert.AreEqual(initialOutput, network.CachedOutputs);

            // MODIFY SETUP: Configure inbound connections such that both are now firing.
            mockInboundUnfired.SetupGet(mock => mock.IsFired).Returns(true);

            // EXECUTION #2
            network.Fire(Input2);  // Fire with Input 2

            // VERIFICATION #2

            // The fire signals were checked again...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Exactly(2));
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Exactly(2));

            // ...and all activation activities occurred.
            mockInputNode1.Verify(mock => mock.Fire(new[] { DigitizedInput1 }), Times.Once());
            mockInputNode2.Verify(mock => mock.Fire(new[] { DigitizedInput2 }), Times.Once());
            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutbound1.Verify(mock => mock.Fire(DigitizedOutput1), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(DigitizedOutput2), Times.Once());
            Assert.AreEqual(outputs, network.CachedOutputs);
        }

        /// <summary>
        /// Method:     Fire (double[])
        /// Condition:  The overload of Fire is used.
        /// Result:     1) The network immediately activates with the provided inputs.
        ///             2) Activation causes network to initiate internal computations.
        ///             3) Internal computations are propogated forward to outbound connections.
        /// </summary>
        [Test]
        public void FireArray_ActivatesImmediately()
        {
            const double Input1 = -2.3d;
            const double DigitizedInput1 = -1.0d;
            const double Input2 = 0.3d;
            const double DigitizedInput2 = 1.0d;
            const double Output1 = -0.01d;
            const double DigitizedOutput1 = -1.0d;
            const double Output2 = 2.6d;
            const double DigitizedOutput2 = 1.0d;

            var outputs = new[] { Output1, Output2 };

            //// SETUP

            // Create 2 input and 2 output mock nodes
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();

            // Program mock nodes

            // Input nodes have an input size of 1
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);

            // Output nodes have an output size of 1
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Configure the output nodes to return the output constants.
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output1 });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output2 });

            // Create 2 inbound and 1 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInboundConnection(mockInboundFired.Object);
            network.AddInboundConnection(mockInboundUnfired.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.Fire(new[] { Input1, Input2 });

            // VERIFICATION #1

            // The inbound connections were not used...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Never());
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Never());

            // ...and the activation activities occurred.
            mockInputNode1.Verify(mock => mock.Fire(new[] { DigitizedInput1 }), Times.Once());
            mockInputNode2.Verify(mock => mock.Fire(new[] { DigitizedInput2 }), Times.Once());
            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutbound1.Verify(mock => mock.Fire(DigitizedOutput1), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(DigitizedOutput2), Times.Once());
            Assert.AreEqual(outputs, network.CachedOutputs);
        }
    }
}
