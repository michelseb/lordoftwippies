using System.Collections;

public interface ICollectable  {

    IEnumerator Collecting(AdvancedTwippie twippie);
    void Collect(AdvancedTwippie twippie);
}
