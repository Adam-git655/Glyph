using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/LetterO")]
public class LetterO : Ability
{
    [Header("Portal Prefabs")]
    public GameObject _portalPrefabA;     
    public GameObject _portalPrefabB;     

    [Header("Collision Settings")]
    public LayerMask _obstacleLayer;      
    public float _portalRadius = 0.5f;    

    [Header("Distances")]
    public float _portalADistance = 4f;   
    public float _minSpawnDistance = 1f;  

    private GameObject _currentA;
    private GameObject _currentB;

    public override void Use(Vector2 characterPosition, Vector2 mousePosition)
    {
        if (_currentA != null) Destroy(_currentA);
        if (_currentB != null) Destroy(_currentB);

        Vector2 rawDir = mousePosition - characterPosition;
        if (rawDir.sqrMagnitude < 0.01f)
            rawDir = Vector2.right;
        Vector2 dir = rawDir.normalized;

        float signX = Mathf.Abs(dir.x) > 0.1f ? Mathf.Sign(dir.x) : 1f;
        Vector2 horizontal = new Vector2(signX, 0f);

        Vector2 portalAPos = characterPosition + horizontal * _portalADistance;
        portalAPos.y = characterPosition.y;

        if (!IsValidPosition(portalAPos))
        {
            for (float dx = _portalADistance; dx > 0; dx -= 0.1f)
            {
                Vector2 tryPos = characterPosition + horizontal * dx;
                tryPos.y = characterPosition.y;
                if (IsValidPosition(tryPos))
                {
                    portalAPos = tryPos;
                    break;
                }
            }
        }

        float rawDist = rawDir.magnitude;
        float spawnDist;
        if (!IsValidPosition(mousePosition))
        {
            float maxCheck = Mathf.Min(rawDist, Range);
            RaycastHit2D groundHit = Physics2D.Raycast(characterPosition, dir, maxCheck, _obstacleLayer);
            if (groundHit.collider != null)
            {
                spawnDist = Mathf.Max(groundHit.distance - _portalRadius, _minSpawnDistance);
            }
            else
            {
                spawnDist = Mathf.Clamp(rawDist, _minSpawnDistance, Range);
            }
        }
        else
        {
            spawnDist = Mathf.Clamp(rawDist, _minSpawnDistance, Range);
        }

        Vector2 portalBPos = characterPosition + dir * spawnDist;

        RaycastHit2D hit = Physics2D.Raycast(characterPosition, dir, spawnDist, _obstacleLayer);
        if (hit.collider != null)
            portalBPos = hit.point - dir * _portalRadius;

        if (!IsValidPosition(portalBPos))
        {
            for (float d = spawnDist; d > _minSpawnDistance; d -= 0.1f)
            {
                Vector2 tryPos = characterPosition + dir * d;
                if (IsValidPosition(tryPos))
                {
                    portalBPos = tryPos;
                    break;
                }
            }
        }

        float abDist = Vector2.Distance(portalAPos, portalBPos);
        if (abDist < _minSpawnDistance)
        {
            portalBPos = portalAPos + dir * _minSpawnDistance;
        }

        _currentA = Instantiate(_portalPrefabA, portalAPos, Quaternion.identity);
        _currentB = Instantiate(_portalPrefabB, portalBPos, Quaternion.identity);

        var scriptA = _currentA.GetComponent<PortalScript>();
        var scriptB = _currentB.GetComponent<PortalScript>();
        if (scriptA != null && scriptB != null)
        {
            scriptA.linkedPortal = _currentB.transform;
            scriptB.linkedPortal = _currentA.transform;
        }
    }

    private bool IsValidPosition(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, _portalRadius, _obstacleLayer) == null;
    }
}
