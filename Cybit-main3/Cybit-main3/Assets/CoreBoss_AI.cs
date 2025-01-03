using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoreBoss_AI : MonoBehaviour
{
    [Header("Core Warden References")]
    [SerializeField] private List<GameObject> _bossPieces; // The pieces that make up the boss
    [SerializeField] private NavMeshAgent _mainNavMeshAgent; // The main NavMeshAgent to control the whole group
    [SerializeField] private Transform _livePlayerLocation; // The player's location for the attack behavior

    [Header("Pattern")]
    [SerializeField] private bool _isAttacking; // Whether the boss is currently attacking

    private List<Vector3> _originalPieceOffsets = new List<Vector3>(); // The original positions of pieces relative to the main agent

    private void Start()
    {
        // Store the original positions of the boss pieces relative to the main agent
        foreach (var piece in _bossPieces)
        {
            _originalPieceOffsets.Add(piece.transform.position - _mainNavMeshAgent.transform.position);
        }
    }

    private void Update()
    {
        if (_isAttacking)
        {
            // Move the main NavMeshAgent to the player's position when attacking
            _mainNavMeshAgent.SetDestination(_livePlayerLocation.position);
        }
        else
        {
            // Return the pieces to their original positions when not attacking
            ReturnPiecesToOriginalPositions();
        }

        // Move each piece to follow the main agent but maintain relative positions
        MovePiecesRelativeToMainAgent();

        transform.rotation = Quaternion.identity;

    }

    private void MovePiecesRelativeToMainAgent()
    {
        for (int i = 0; i < _bossPieces.Count; i++)
        {
            // Calculate the new position of each piece relative to the main agent
            Vector3 targetPosition = _mainNavMeshAgent.transform.position + _originalPieceOffsets[i];

            // Move each piece towards its target position, maintaining the relative position
            _bossPieces[i].transform.position = Vector3.MoveTowards(_bossPieces[i].transform.position, targetPosition, Time.deltaTime * 5f); // Adjust speed as needed
        }
    }

    private void ReturnPiecesToOriginalPositions()
    {
        for (int i = 0; i < _bossPieces.Count; i++)
        {
            // Calculate the original position of the piece relative to the main agent
            Vector3 targetPosition = _originalPieceOffsets[i] + _mainNavMeshAgent.transform.position;

            // Move each piece back to its original position
            _bossPieces[i].transform.position = Vector3.MoveTowards(_bossPieces[i].transform.position, targetPosition, Time.deltaTime * 2f); // Adjust speed as needed
        }
    }
}