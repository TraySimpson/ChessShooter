public interface IUsable {
    public UsableType GetUsableType();
    public UsableSO GetStatSO();
    public bool IsActive();
}

public enum UsableType {
    Weapon,
    Throwable,
    Deployable
}
