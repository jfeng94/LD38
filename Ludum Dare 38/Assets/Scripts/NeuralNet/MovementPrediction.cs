using UnityEngine;
using System.Collections;
using NeuralNetwork;
using UnityEngine.UI;
using System.Collections.Generic;

public class MovementPrediction
{
	private static NeuralNet net;
	private static List<DataSet> dataSets;

	public MovementPrediction () {
		Debug.Log ("movement prediction initialized.");
		net = new NeuralNet (3, 4, 1);
		dataSets = new List<DataSet> ();
		TestNet ();
	}

	void TestNet() {
		// Runs test on neural net.
		double[][] train_x = new double[][] {
			new double[] {0, 0, 1},
			new double[] {0, 1, 1}, 
			new double[] {1, 0, 1}, 
			new double[] {1, 1, 1},
			new double[] {1, 0, 0}};
		double[][] train_y = new double[][] { 
			new double[] { 0 }, 
			new double[] { 1 },
			new double[] { 1 },
			new double[] { 0 },
			new double[] { 1 }};
		for (int i = 0; i < 5; i++) {
			dataSets.Add(new DataSet(train_x[i], train_y[i]));
		}
		net.Train(dataSets, 800);
		for (int i = 0; i < 5; i++) {
			Debug.Log(net.Compute (train_x [i])[0].ToString());
		}
		Debug.Log(net.Compute(new double[] {0,1,0})[0].ToString());
	}



}


