public class AddAction : UserAction
{
    protected override void Awake()
    {
        AssociatedAction = AssociatedAction.Add;
    }
}
