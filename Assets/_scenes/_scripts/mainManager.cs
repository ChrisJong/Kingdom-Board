using UnityEngine;
using System.Collections;

public class mainManager : MonoBehaviour {

    // change to private.
    public int numberOfPlayers = 2;

    void Awake() {
        this.numberOfPlayers = 2;
    }

    public void SetPlayerCount(int n) {
        this.numberOfPlayers = n;
    }

    public int NumberOfPlayers {
        get { return this.numberOfPlayers; }
    }
}
