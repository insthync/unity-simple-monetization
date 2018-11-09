using System.Collections.Generic;

[System.Serializable]
public struct MatchReward
{
    public AdsReward[] rewards;

    public AdsReward RandomReward()
    {
        var cacheRewards = new Dictionary<AdsReward, int>();
        foreach (var reward in rewards)
        {
            cacheRewards[reward] = reward.randomWeight;
        }
        var randomizer = WeightedRandomizer.From(cacheRewards);
        return randomizer.TakeOne();
    }
}
