namespace SpaceBattle.Lib;

public enum AuthActionType
{
    Check,
    Add,
    Remove
}

public interface IAuthRepository
{
    bool CheckPermission(string subjectId, string objectId, string action);
    void AddPermission(string subjectId, string objectId, string action);
    void RemovePermission(string subjectId, string objectId, string action);
}

