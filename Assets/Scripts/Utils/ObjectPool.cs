using UnityEngine;

public static class ObjectPool{
    public static int NextInPool(GameObject[] fireBalls) {
        for(int i = 0; i < fireBalls.Length; i++) {
            if(!fireBalls[i].activeInHierarchy) return i;
        }
        return -1;
    }
}