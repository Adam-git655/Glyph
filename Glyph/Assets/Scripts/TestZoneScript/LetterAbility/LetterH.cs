using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/LetterH")]
public class LetterH : Ability
{
    [Header("Ladder Settings")]
    [SerializeField] private GameObject _ladderPrefab;
    [SerializeField] private float _segmentHeight = 1f;
    [SerializeField] private int _maxSegments = 20;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _platformLayer;

    [Header("Optional")]
    [SerializeField] private int _ladderLayer = -1; 

    private GameObject _ladderContainer;
    private List<GameObject> _spawnedSegments = new List<GameObject>();

    public override void Use(Vector2 characterPosition, Vector2 mousePosition)
    {
        if (_ladderContainer != null)
        {
            Destroy(_ladderContainer);
            _ladderContainer = null;
        }
        _spawnedSegments.Clear();

        Collider2D platformCollider = Physics2D.OverlapPoint(mousePosition, _platformLayer);
        if (platformCollider == null)
        {
            Debug.LogWarning("LetterH: No platform clicked.");
            return;
        }

        Bounds platBounds = platformCollider.bounds;

        float ladderWidth = 1f;
        var sr = _ladderPrefab != null ? _ladderPrefab.GetComponent<SpriteRenderer>() : null;
        if (sr != null) ladderWidth = sr.bounds.size.x;
        else Debug.LogWarning("LetterH: ladderPrefab needs a SpriteRenderer for correct width.");

        bool placeLeft = characterPosition.x < platBounds.center.x;
        float spawnX = placeLeft ? platBounds.min.x - ladderWidth * 0.5f : platBounds.max.x + ladderWidth * 0.5f;
        float platformTopY = platBounds.max.y;

        Vector2 groundRayOrigin = new Vector2(spawnX, platformTopY + 0.01f);
        RaycastHit2D groundHit = Physics2D.Raycast(groundRayOrigin, Vector2.down, Mathf.Infinity, _groundLayer);
        if (groundHit.collider == null)
        {
            Debug.LogWarning("LetterH: No ground beneath selected platform edge.");
            return;
        }
        float groundY = groundHit.point.y;
        float totalHeight = platformTopY - groundY;
        int segmentCount = Mathf.CeilToInt(totalHeight / _segmentHeight);
        segmentCount = Mathf.Clamp(segmentCount, 1, _maxSegments);

        _ladderContainer = new GameObject("Ladder");
        _ladderContainer.tag = "Ladder";
        _ladderContainer.transform.position = new Vector3(spawnX, 0f, 0f); 

        if (_ladderLayer >= 0)
            _ladderContainer.layer = _ladderLayer;

        float firstCenterY = platformTopY - (_segmentHeight * 0.5f);
        float bottomCenterY = firstCenterY - (segmentCount - 1) * _segmentHeight;
        float midY = (firstCenterY + bottomCenterY) * 0.5f;

        for (int i = 0; i < segmentCount; i++)
        {
            float centerY = firstCenterY - i * _segmentHeight;
            Vector2 spawnPos = new Vector2(spawnX, centerY);
            GameObject seg = Instantiate(_ladderPrefab, spawnPos, Quaternion.identity, _ladderContainer.transform);
            _spawnedSegments.Add(seg);
        }

        var bc = _ladderContainer.AddComponent<BoxCollider2D>();
        float totalColliderHeight = segmentCount * _segmentHeight;
        bc.size = new Vector2(ladderWidth, totalColliderHeight);

        float localMidY = midY - _ladderContainer.transform.position.y;
        bc.offset = new Vector2(0f, localMidY);
        bc.isTrigger = true;
    }
}
