public class Feeling {

    private string _name;
    private float _intensity;
    private Feeling _opposite;

    public Feeling(string name, float intensity, Feeling opposite)
    {
        _name = name;
        _intensity = intensity;
        _opposite = opposite;
    }

}
