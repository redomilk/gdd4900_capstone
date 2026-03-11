/// Implement this on any enemy that can be stunned or slowed.
/// CoreEffects will call these automatically when effects land.
public interface IStunnable
{
    void Stun(float duration);
    void SetSpeedMultiplier(float multiplier);
}