public interface IDataPersistence {
    void SaveData(ref GameData gameData);
    void LoadData(GameData gameData);
}