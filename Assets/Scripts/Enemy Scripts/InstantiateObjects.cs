using UnityEngine;

public class InstantiateObjects : MonoBehaviour
{
    private struct TerrainBorders 
    {
        public float leftWidth, rightWidth, topHeight, bottomHeight;

        public TerrainBorders(float leftWidth, float rightWidth, float topHeight, float bottomHeight)
        {
            this.leftWidth = leftWidth;
            this.rightWidth = rightWidth;
            this.topHeight = topHeight;
            this.bottomHeight = bottomHeight;
        }
    }

    TerrainBorders desertBorders = new TerrainBorders(-280f, 280f, 75f, -155f);
    TerrainBorders forestBorders = new TerrainBorders(-165f, 205f, 55f, -70f);
    TerrainBorders arcticBorders = new TerrainBorders(-170f, 205f, 170f, 5f);

    [SerializeField] private GameObject[] desertEnemies, forestEnemies, arcticEnemies;
    private int maxTerrainEnemyCount = 200; //, minTerrainEnemyCount = 20;

    private void Awake()
    {
        for(int i = 0; i < maxTerrainEnemyCount; i++)
        {
            Instantiate(forestEnemies[Random.Range(0, 2)], GenerateInstantiationPosition("Forest"), Quaternion.identity);
            Instantiate(desertEnemies[Random.Range(0, 2)], GenerateInstantiationPosition("Desert"), Quaternion.identity);
            Instantiate(arcticEnemies[Random.Range(0, 2)], GenerateInstantiationPosition("Arctic"), Quaternion.identity);
        }
    }

    private Vector3 GenerateInstantiationPosition(string terrain)
    {
        float leftBorder, rightBorder, topBorder, bottomBorder;
        
        if(terrain == "Forest")
        {
            leftBorder = forestBorders.leftWidth;
            rightBorder = forestBorders.rightWidth;
            topBorder = forestBorders.topHeight;
            bottomBorder = forestBorders.bottomHeight;
        }
        else if(terrain == "Desert")
        {
            leftBorder = desertBorders.leftWidth;
            rightBorder = desertBorders.rightWidth;
            topBorder = desertBorders.topHeight;
            bottomBorder = desertBorders.bottomHeight;
        }
        else // Arctic
        {
            leftBorder = arcticBorders.leftWidth;
            rightBorder = arcticBorders.rightWidth;
            topBorder = arcticBorders.topHeight;
            bottomBorder = arcticBorders.bottomHeight;
        }

        Vector3 position = new Vector3(Random.Range(leftBorder, rightBorder),
                Random.Range(bottomBorder, topBorder), 0f);

        if (CurrentTerrainLocator.LocateTerrain(position) == terrain)
            return position;
        else
            return GenerateInstantiationPosition(terrain);
    }
}
