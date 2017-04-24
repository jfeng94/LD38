using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NeuralNetwork
{
	public class NeuralNet {
		public double LearnRate { get; set; }
		public double Momentum { get; set; }
		public List<Neuron> InputLayer { get; set; }
		// TODO: multiple hidden layers.
		public List<Neuron> HiddenLayer { get; set; }
		public List<Neuron> OutputLayer { get; set; }

		private static readonly System.Random Random = new System.Random();

		public NeuralNet(int inputSize, int hiddenSize, int outputSize, double? learnRate = null, double? momentum = null) {
			LearnRate = learnRate ?? .4;
			Momentum = momentum ?? .9;
			InputLayer = new List<Neuron> ();
			HiddenLayer = new List<Neuron> ();
			OutputLayer = new List<Neuron> ();

			for (var i = 0; i < inputSize; i++) {
				InputLayer.Add (new Neuron ());
			}

			for (var i = 0; i < hiddenSize; i++) {
				HiddenLayer.Add (new Neuron (InputLayer));
			}

			for (var i = 0; i < outputSize; i++) {
				OutputLayer.Add (new Neuron (HiddenLayer));
			}
		}

		// Offline batch sgd training.
		public void Train(List<DataSet> dataSets, int numEpochs) {
			for (var i = 0; i < numEpochs; i++) {
				foreach (var dataSet in dataSets) {
					ForwardPropagate (dataSet.Values);
					BackPropagate (dataSet.Targets);
				}
			}
		}

		private void ForwardPropagate(params double[] inputs) {
			var i = 0;
			InputLayer.ForEach (a => a.Value = inputs [i++]);
			HiddenLayer.ForEach (a => a.CalculateValue ());
			OutputLayer.ForEach (a => a.CalculateValue ());
		}

		private void BackPropagate(params double[] targets) {
			var i = 0;
			OutputLayer.ForEach (a => a.CalculateGradient (targets [i++]));
			HiddenLayer.ForEach (a => a.CalculateGradient ());
			HiddenLayer.ForEach (a => a.UpdateWeights (LearnRate, Momentum));
			OutputLayer.ForEach (a => a.UpdateWeights (LearnRate, Momentum));
		}

		public double[] Compute(params double[] inputs) {
			ForwardPropagate (inputs);
			return OutputLayer.Select (a => a.Value).ToArray ();

		}


		private double CalculateError(params double[] targets) {
			var i = 0;
			return OutputLayer.Sum (a => Mathf.Abs ((float)a.CalculateError (targets [i++])));
		}


		public static double GetRandom() {
			return 2 * Random.NextDouble () - 1;
		}
	}

	public enum TrainingType {
		Epoch
	}

}