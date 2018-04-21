using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTeamApplier : MonoBehaviour {

    public Team team;

    private Block _block;
    private SpriteRenderer _spriteRenderer;

    void Start () {
        _block = GetComponent<Block>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (team != null) {
            ApplyTeam();
        }
    }

    // Update is called once per frame
    void Update () {
        if (_block != null && _block.team != null) {
            team = _block.team;
            _block.Disable();
        }
        if (team != null) {
            ApplyTeam();
            Disable();
        }
    }

    void Disable () {
        this.enabled = false;
    }

    public void ApplyTeam () {
        _spriteRenderer.color = team.color;
    }
}
