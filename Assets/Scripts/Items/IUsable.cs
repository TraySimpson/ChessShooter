using UnityEngine;

public interface IUsable {
    public UsableType GetUsableType();
    public UsableSO GetStatSO();
    public bool IsActive();
    public void SetActive(bool active);
    public GameObject GetGameObject();
}

public enum UsableType {
    Weapon,
    Throwable,
    Deployable
}
