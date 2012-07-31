// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeuronTests.cs" company="The Logans Ferry Software Co.">
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
    /// Unit tests for the Neuron.cs class.
    /// </summary>
    [TestFixture]
    public class NeuronTests
    {
        /// <summary>
        /// Method:     Constructor
        /// Condition:  Null argument is provided.
        /// Results:    Argument Null Exception
        /// </summary>
        [Test]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void Ctor_ValidatesArgs()
        {
            // Verify
            new Neuron(null);
        }

        /// <summary>
        /// Property:   InputSize
        /// Condition:  Neuron is an input node.
        /// Results:    InputSize = 1
        /// </summary>
        [Test]
        public void InputSize_InputNodesReturnOne()
        {
            // Setup: create an input neuron with 2 outbound and 0 inbound connections.
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);

            // Verify
            Assert.AreEqual(neuron.InputSize, 1);
        }

        /// <summary>
        /// Property:   InputSize
        /// Condition:  Nueron is not an input node.
        /// Results:    InputSize = # of inbound connections.
        /// </summary>
        [Test]
        public void InputSize_BasedOnInboundConnections()
        {
            // Setup:  create a hidden neuron with 3 inbound and 2 outbound connections.
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);

            neuron.AddInboundConnection(new Mock<INeuralConnection>().Object);
            neuron.AddInboundConnection(new Mock<INeuralConnection>().Object);
            neuron.AddInboundConnection(new Mock<INeuralConnection>().Object);

            // Verify
            Assert.AreEqual(neuron.InputSize, 3);
        }

        /// <summary>
        /// Property:   OutputSize
        /// Condition:  Nueron is an input node
        /// Results:    OutputSize = 1
        /// </summary>
        [Test]
        public void OutputSize_InputNodesReturnOne()
        {
            // Setup: create an input neuron with 2 outbound and 0 inbound connections.
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);

            // Verify
            Assert.AreEqual(neuron.OutputSize, 1);
        }

        /// <summary>
        /// Property:   OutputSize
        /// Condition:  Nueron is an output node
        /// Results:    OutputSize = 1
        /// </summary>
        [Test]
        public void OutputSize_OutputNodesReturnOne()
        {
            // Setup: create an output neuron with 2 inbound and 0 outbound connections.
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);
            neuron.AddInboundConnection(new Mock<INeuralConnection>().Object);
            neuron.AddInboundConnection(new Mock<INeuralConnection>().Object);

            // Verify
            Assert.AreEqual(neuron.OutputSize, 1);
        }

        /// <summary>
        /// Property:   Bias
        /// Condition:  Neuron is an input node
        /// Results:    Bias always = 0
        /// </summary>
        [Test]
        public void Bias_InputNodesReturnZero()
        {
            // Setup: create an input neuron with 1 outbound and 0 inbound connections.
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);
            
            // Verify Initialization
            Assert.AreEqual(neuron.Bias, 0.0d);

            // Execute
            neuron.Bias = 1.0d;

            // Verify no update
            Assert.AreEqual(neuron.Bias, 0.0d);
        }

        /// <summary>
        /// Property:   Bias
        /// Condition:  Neuron is not an input node
        /// Results:    Bias initializes and updates normally.
        /// </summary>
        [Test]
        public void Bias_NonInputNodesHaveBias()
        {
            // Setup: create a neuron with 1 outbound and 1 inbound connections.
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);
            neuron.AddInboundConnection(new Mock<INeuralConnection>().Object);
            neuron.AddOutboundConnection(new Mock<INeuralConnection>().Object);

            // Verify Initialization is not zero.
            Assert.AreNotEqual(neuron.Bias, 0.0d);
            
            // Execute
            var initialBias = neuron.Bias;
            neuron.Bias += 1.0d;
            var expected = initialBias + 1.0d;

            // Verify
            Assert.AreEqual(neuron.Bias, expected);
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
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);
            
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
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);

            // Execute/Verify
            neuron.AddOutboundConnection(null);
        }

        /// <summary>
        /// Method:     Fire (double)
        /// Condition:  Inbound connections fire in sequence.
        /// Result:     1) Neuron accumulates input signals and activates after final signal is received.
        ///             2) Activation uses activation function and propogates to outbound connections.
        /// </summary>
        [Test]
        public void Fire_ActivatesAfterAllInboundsFire()
        {
            const double Bias = 1.0d;
            const double Input1 = 2.0d;
            const double Input2 = 3.0d;
            const double BiasPlusInputs = 6.0d;
            const double Output = -1.0d;

            //// SETUP
            
            // Create 2 inbound and 1 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // program the mock inbounds such that one is fired and one isn't
            mockInboundFired.SetupGet(mock => mock.IsFired).Returns(true);
            mockInboundUnfired.SetupGet(mock => mock.IsFired).Returns(false);
            
            // Create and program an activation function to return the output value.
            var mockFunction = new Mock<IActivationFunction>();
            mockFunction.Setup(mock => mock.Invoke(BiasPlusInputs)).Returns(Output);

            // Create the test object.
            var neuron = new Neuron(mockFunction.Object);
            neuron.AddInboundConnection(mockInboundFired.Object);
            neuron.AddInboundConnection(mockInboundUnfired.Object);
            neuron.AddOutboundConnection(mockOutbound1.Object);
            neuron.AddOutboundConnection(mockOutbound2.Object);
            neuron.Bias = Bias;

            // EXECUTION #1

            // Fire with Input 1
            neuron.Fire(Input1);

            // VERIFICATION #1

            // The fire signals were checked but no activation activities occurred.
            mockInboundFired.Verify(mock => mock.IsFired, Times.Exactly(1));
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Exactly(1));
            mockInboundFired.Verify(mock => mock.ClearFire(), Times.Never());
            mockInboundUnfired.Verify(mock => mock.ClearFire(), Times.Never());
            mockOutbound1.Verify(mock => mock.Fire(It.IsAny<double>()), Times.Never());
            mockOutbound2.Verify(mock => mock.Fire(It.IsAny<double>()), Times.Never());
            mockFunction.Verify(mock => mock.Invoke(It.IsAny<float>()), Times.Never());
            Assert.AreEqual(neuron.CachedOutputs[0], 0.0d);

            // MODIFY SETUP
            mockInboundUnfired.SetupGet(mock => mock.IsFired).Returns(true);
            
            // EXECUTION #2
            neuron.Fire(Input2);

            // VERIFICATION #2

            // The fire signals were checked again, and all activation activities occurred.
            mockInboundFired.Verify(mock => mock.IsFired, Times.Exactly(2));
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Exactly(2));
            mockFunction.Verify(mock => mock.Invoke(BiasPlusInputs), Times.Once());
            mockOutbound1.Verify(mock => mock.Fire(Output), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(Output), Times.Once());
            mockInboundFired.Verify(mock => mock.ClearFire(), Times.Once());
            mockInboundUnfired.Verify(mock => mock.ClearFire(), Times.Once());
            Assert.AreEqual(neuron.CachedOutputs[0], Output);
        }

        /// <summary>
        /// Method:     Fire (double[])
        /// Condition:  The overload of Fire is used.
        /// Result:     1) Neuron immediately accumulates input signals and activates.
        ///             2) Activation uses activation function and propogates to outbound connections.
        /// </summary>
        [Test]
        public void FireArray_ActivatesImmediately()
        {
            const double Bias = 1.0d;
            const double Input1 = 2.0d;
            const double Input2 = 3.0d;
            const double BiasPlusInputs = 6.0d;
            const double Output = -1.0d;

            //// SETUP

            // Create 2 inbound and 1 outbound mock connections.
            var mockInbound1 = new Mock<INeuralConnection>();
            var mockInbound2 = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // Create and program an activation function to return the output value.
            var mockFunction = new Mock<IActivationFunction>();
            mockFunction.Setup(mock => mock.Invoke(BiasPlusInputs)).Returns(Output);

            // Create the test object.
            var neuron = new Neuron(mockFunction.Object);
            neuron.AddInboundConnection(mockInbound1.Object);
            neuron.AddInboundConnection(mockInbound2.Object);
            neuron.AddOutboundConnection(mockOutbound1.Object);
            neuron.AddOutboundConnection(mockOutbound2.Object);
            neuron.Bias = Bias;

            // EXECUTION

            // Fire with all inputs
            neuron.Fire(new[] { Input1, Input2 });

            // VERIFICATION

            // The fire signals were Not checked and activation activities occurred.
            mockInbound1.Verify(mock => mock.IsFired, Times.Never());
            mockInbound2.Verify(mock => mock.IsFired, Times.Never());
            mockInbound1.Verify(mock => mock.ClearFire(), Times.Never());
            mockInbound2.Verify(mock => mock.ClearFire(), Times.Never());
            
            mockFunction.Verify(mock => mock.Invoke(BiasPlusInputs), Times.Once());
            mockOutbound1.Verify(mock => mock.Fire(Output), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(Output), Times.Once());
            Assert.AreEqual(neuron.CachedOutputs[0], Output);
        }

        /// <summary>
        /// Method:     Fire (double[])
        /// Condition:  Null argument is provided.
        /// Results:    Argument Null Exception
        /// </summary>
        [Test]
        [ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void FireArray_ValidatesNullArgs()
        {
            // Setup
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);

            // Execute/Verify
            neuron.Fire(null);
        }

        /// <summary>
        /// Method:     Fire (double[])
        /// Condition:  Empty array is provided.
        /// Results:    Argument Exception
        /// </summary>
        [Test]
        [ExpectedExceptionAttribute(typeof(ArgumentException))]
        public void FireArray_ValidatesEmptyArray()
        {
            // Setup
            var neuron = new Neuron(new Mock<IActivationFunction>().Object);

            // Execute/Verify
            neuron.Fire(new double[0]);
        }
    }
}
