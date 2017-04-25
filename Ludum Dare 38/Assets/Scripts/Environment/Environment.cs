using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {
  public GameObject endGame;

  public Enemy boss;

  void Start() {
    endGame.SetActive(false);
  }

  void Update() {
    if (!boss.isAlive) {
      endGame.SetActive(true);
    }
  }
}