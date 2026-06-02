using App;

namespace SpaceBattle.Lib;

public class AuthActionCommand : ICommand
{
    private readonly IAuthRepository _repository;
    private readonly AuthActionType _actionType;
    private readonly string _subjectId;
    private readonly string _objectId;
    private readonly string _action;

    public AuthActionCommand(IAuthRepository repository, AuthActionType actionType, string subjectId, string objectId, string action)
    {
        _repository = repository;
        _actionType = actionType;
        _subjectId = subjectId;
        _objectId = objectId;
        _action = action;
    }

    public void Execute()
    {
        switch (_actionType)
        {
            case AuthActionType.Check:
                if (!_repository.CheckPermission(_subjectId, _objectId, _action))
                {
                    throw new UnauthorizedAccessException($"Access denied: Subject '{_subjectId}' cannot perform '{_action}' on '{_objectId}'.");
                }
                break;

            case AuthActionType.Add:
                _repository.AddPermission(_subjectId, _objectId, _action);
                break;

            case AuthActionType.Remove:
                _repository.RemovePermission(_subjectId, _objectId, _action);
                break;
        }
    }
}

