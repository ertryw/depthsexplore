using DG.Tweening;
using UnityEngine;

public class Bomb : WaterObject
{
    [Space(10)]
    [SerializeField]
    private int energyTake;

    private void Start()
    {
        base.SetCommonSprite();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject != null)
        {
            bool isBathyscaphe = col.TryGetComponent(out Bathyscaphe bathyscaphe);

            if (isBathyscaphe)
            {
                bathyscaphe.data.energyValue -= energyTake;
                Camera.main.DOShakePosition(0.5f, 2, 10, 10, true);
                DestroyDestoryObject(0.01f, true);
            }
        }

    }
}
