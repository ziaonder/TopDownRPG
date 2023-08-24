using System.Collections.Generic;
using UnityEngine;

public static class CurrentTerrainLocator
{
    public static string LocateTerrain(Transform transform)
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 0.6f);
        float rayDistance = 250f;
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Dictionary<string, int> mapColliderCount = new Dictionary<string, int>()
        {
            {"Forest", 0 },
            {"Desert", 0 },
            {"Arctic", 0 }
        };

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        
        Vector2 northEastDirection = new Vector2(1, 1).normalized;
        hit = Physics2D.Raycast(rayOrigin, northEastDirection, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        Vector2 northWestDirection = new Vector2(-1, 1).normalized;
        hit = Physics2D.Raycast(rayOrigin, northWestDirection, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        Vector2 southEastDirection = new Vector2(1, -1).normalized;
        hit = Physics2D.Raycast(rayOrigin, southEastDirection, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        Vector2 southWestDirection = new Vector2(-1, -1).normalized;
        hit = Physics2D.Raycast(rayOrigin, southWestDirection, rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);

        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Cos(45), Mathf.Sin(45)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Sin(-45), Mathf.Cos(-45)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Cos(135), Mathf.Sin(135)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Sin(-135), Mathf.Cos(-135)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);

        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Cos(270), Mathf.Sin(270)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Sin(-270), Mathf.Cos(-270)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Cos(90), Mathf.Sin(90)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);
        hit = Physics2D.Raycast(rayOrigin, new Vector2(Mathf.Sin(-90), Mathf.Cos(-90)), rayDistance, LayerMask.GetMask("Terrain"));
        hits.Add(hit);

        foreach (RaycastHit2D castHit in hits)
        {
            if(castHit.collider != null)
            {
                switch (castHit.collider.gameObject.name)
                {
                    case "Forest Tilemap":
                        mapColliderCount["Forest"]++;
                        break;
                    case "Desert Tilemap":
                        mapColliderCount["Desert"]++;
                        break;
                    case "Arctic Tilemap":
                        mapColliderCount["Arctic"]++;
                        break;
                }
            }
        }

        //Debug.Log("Forest: " + mapColliderCount["Forest"] + " --- Desert: " + mapColliderCount["Desert"] + " --- Arctic: " + mapColliderCount["Arctic"]);
        if (mapColliderCount["Forest"] > mapColliderCount["Desert"] && mapColliderCount["Forest"] > mapColliderCount["Arctic"])
            return "Forest";
        else if (mapColliderCount["Desert"] > mapColliderCount["Forest"] && mapColliderCount["Desert"] > mapColliderCount["Arctic"])
            return "Desert";
        else
            return "Arctic";
    }
}
