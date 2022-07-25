public interface IUsable {
    public UsableType GetUsableType();
    public UsableSO GetStatSO();
    public bool IsActive();
    public void SetActive(bool active);
}

public enum UsableType {
    Weapon,
    Throwable,
    Deployable
}
