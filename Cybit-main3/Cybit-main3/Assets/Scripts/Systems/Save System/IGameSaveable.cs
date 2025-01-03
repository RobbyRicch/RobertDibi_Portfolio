public interface IGameSaveable
{
    public void LoadData(GameData data);
    public void SaveData(ref GameData data);
}