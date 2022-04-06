public class ModificationAction : UserAction
{
    protected override void Awake()
    {
        AssociatedAction = AssociatedAction.Modification;
    }
}
