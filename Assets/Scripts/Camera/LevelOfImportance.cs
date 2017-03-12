using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOfImportance : MonoBehaviour {
    [Range(1, 10)]
    [SerializeField]
    private int level = 1;

    public int getLevel()
    {
        return level;
    }

    public void setLevel(int level_t)
    {
        level = level_t;
    }
}
