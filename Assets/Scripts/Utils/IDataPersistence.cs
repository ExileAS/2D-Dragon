public interface IDataPersistence {
    void SaveState(ref GameData gameData);
    void LoadState(GameData gameData);
}