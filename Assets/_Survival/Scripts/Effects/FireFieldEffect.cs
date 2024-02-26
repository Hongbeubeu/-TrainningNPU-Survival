public class FireFieldEffect : Effect
{
    public override void SetSize(float radius)
    {
        var emission = _particle.emission;
        var shape = _particle.shape;
        shape.radius = radius;
        emission.rateOverTime = radius * 5;
    }
}