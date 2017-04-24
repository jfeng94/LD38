using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NeuralNetwork {
	public class Neuron {
		public List<Node> InputNodes { get; set; }
		public List<Node> OutputNodes { get; set; }
		public double Bias { get; set; }
		public double BiasDelta { get; set; }
		public double Gradient { get; set; }
		public double Value { get; set; }

		public Neuron() {
			InputNodes = new List<Node> ();
			OutputNodes = new List<Node> ();
			Bias = NeuralNet.GetRandom();
		}

		public Neuron(IEnumerable<Neuron> inputNeurons) : this() {
			foreach (var inputNeuron in inputNeurons) {
				var node = new Node (inputNeuron, this);
				inputNeuron.OutputNodes.Add (node);
				InputNodes.Add (node);
			}
		}

		public virtual double CalculateValue() {
			return Value = Sigmoid.Output (InputNodes.Sum (a => a.Weight * a.InputNeuron.Value) + Bias);
		}

		public double CalculateError(double target)
		{
			// Value should already be calculated (might want to check this is true.)
			return target - Value;
		}

		public double CalculateGradient(double? target = null) {
			if (target == null) {
				return Gradient = OutputNodes.Sum (a => a.OutputNeuron.Gradient * a.Weight) * Sigmoid.Derivative (Value);
			}

			return Gradient = CalculateError (target.Value) * Sigmoid.Derivative (Value);
		}

		public void UpdateWeights(double learnRate, double momentum) {
			var prevDelta = BiasDelta;
			BiasDelta = learnRate * Gradient;
			Bias += BiasDelta + momentum * prevDelta;

			foreach (var node in InputNodes) {
				prevDelta = node.WeightDelta;
				node.WeightDelta = learnRate * Gradient * node.InputNeuron.Value;
				node.Weight += node.WeightDelta + momentum * prevDelta;
			}
		}

	}

	public class Node {
		public Neuron InputNeuron { get; set; }
		public Neuron OutputNeuron { get; set; }
		public double Weight { get; set; }
		public double WeightDelta { get; set; }

		public Node(Neuron inputNeuron, Neuron outputNeuron) {
			InputNeuron = inputNeuron;
			OutputNeuron = outputNeuron;
			Weight = NeuralNet.GetRandom ();
		}
	}

	public static class Sigmoid {
		public static double Output(double x) {
			// Basically 1 when large. 
			return x < -40.0 ? 0.0 : x > 40.0 ? 1.0 : 1.0 / (1.0 + Mathf.Exp((float)-x));
		}

		public static double Derivative(double x) {
			return x * (1 - x);
		}
	}


	public class DataSet {
		public double[] Values { get; set; }
		public double[] Targets { get; set; }

		public DataSet(double[] values, double[] targets) {
			Values = values;
			Targets = targets;
		}
	}

}