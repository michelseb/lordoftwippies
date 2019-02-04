public interface IConsumable {

    bool Consuming(float hunger);
    void Consume();
    void Reserve();
    void Liberate();
}
