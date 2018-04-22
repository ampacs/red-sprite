using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTeamSetter : MonoBehaviour {

    private Block _block;
    private BlockTeamApplier _blockTeamApplier;

    void Start () {
        _block = GetComponent<Block>();
        _blockTeamApplier = GetComponent<BlockTeamApplier>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collision!");
        Block blockComponent = other.gameObject.GetComponent<Block>();
        if (blockComponent != null) {
            if (_block != null) {
                if (blockComponent.team != null && _block.team == null) {
                    _block.team = blockComponent.team;
                    _block.DisablePhysics();
                }
                if (blockComponent.team == null && _block.team != null) {
                    blockComponent.team = _block.team;
                    blockComponent.DisablePhysics();
                }
            } else if (_blockTeamApplier != null) {
                if (blockComponent.team != null && _blockTeamApplier.team == null) {
                    _blockTeamApplier.team = blockComponent.team;
                }
                if (blockComponent.team == null && _blockTeamApplier.team != null) {
                    blockComponent.team = _blockTeamApplier.team;
                    blockComponent.DisablePhysics();
                }
            }
        }
    }
}
