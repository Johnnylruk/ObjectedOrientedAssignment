using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Helper;


public static class VoterCalculationHelper
{
  public static void ElectCandidatesSTV(Dictionary<int, int> voteCounts, List<CandidateModel> candidates, HashSet<int> electedCandidates, double quota)
{
    // Check if all seats have been filled
    if (electedCandidates.Count == candidates.Count)
    {
        return;
    }

    // Check if any candidate has reached the quota
    if (quotaReached(voteCounts, quota))
    {
        // Find the candidate(s) with the excess votes
        var candidatesWithExcessVotes = getCandidatesWithExcessVotes(voteCounts, quota);

        foreach (var candidateId in candidatesWithExcessVotes)
        {
            // Declare the candidate with excess votes as a winner
            electedCandidates.Add(candidateId);
            candidates.First(c => c.Id == candidateId).IsElected = true;

            // Recalculate vote counts after transferring excess votes
            TransferExcessVotesSTV(voteCounts, candidates, candidateId);

            // Remove the elected candidate from the list
            candidates.RemoveAll(c => c.Id == candidateId);
        }
    }
    else
    {
        // Find the candidate(s) with the fewest votes
        int minVoteCount = voteCounts.Values.Min();
        int[] candidatesWithMinVotesIds = voteCounts.Where(x => x.Value == minVoteCount).Select(x => x.Key).ToArray();

        // Implement a tie-breaking mechanism
        int candidateWithMinVotesId = TieBreaker(candidates, candidatesWithMinVotesIds);

        foreach (var candidate in candidates.ToList())
        {
            // Skip the eliminated candidate
            if (candidate.Id == candidateWithMinVotesId)
            {
                // Transfer votes from the eliminated candidate's voters to the highest-ranked remaining candidate
                foreach (var voter in candidate.Votes.ToList())
                {
                    // Find the preference of this voter for the eliminated candidate
                    var preference = voter.Preferences.FirstOrDefault(p => p.CandidateId != candidateWithMinVotesId);
                    if (preference != null)
                    {
                        // Transfer the vote to the candidate with the highest rank
                        voteCounts[preference.CandidateId]++;

                        // Deduct the vote from the eliminated candidate
                        voteCounts[candidateWithMinVotesId]--;
                        break; // Stop after transferring one vote from this voter
                    }
                }

                // Remove the eliminated candidate from the list of candidates
                candidates.RemoveAll(c => c.Id == candidateWithMinVotesId);
            }
        }
    }
}

private static bool quotaReached(Dictionary<int, int> voteCounts, double quota)
{
    return voteCounts.Values.Any(votes => votes >= quota);
}

private static List<int> getCandidatesWithExcessVotes(Dictionary<int, int> voteCounts, double quota)
{
    return voteCounts.Where(kv => kv.Value >= quota).Select(kv => kv.Key).ToList();
}

private static int TieBreaker(List<CandidateModel> candidates, int[] candidatesWithMinVotesIds)
{
    // Implement a tie-breaking mechanism
    // For simplicity, choose the candidate who was listed first
    return candidatesWithMinVotesIds.First();
}

public static void TransferExcessVotesSTV(Dictionary<int, int> voteCounts, List<CandidateModel> candidates, int electedCandidateId)
{
    // Calculate surplus votes
    int surplusVotes = voteCounts[electedCandidateId] - voteCounts.Values.Min();

    // Find the elected candidate
    var electedCandidate = candidates.FirstOrDefault(c => c.Id == electedCandidateId);

    // Transfer surplus votes proportionally to the next preferences of the voters who voted for the elected candidate
    foreach (var vote in electedCandidate.Votes)
    {
        // Check if the voter's first preference is the elected candidate
        var firstPreference = vote.Preferences.FirstOrDefault(p => p.CandidateId == electedCandidateId && p.Rank == 1);
        if (firstPreference != null)
        {
            // Find the total number of votes for this voter's preferences
            int totalVotesForPreferences = vote.Preferences.Sum(p => voteCounts.GetValueOrDefault(p.CandidateId, 0));

            // Transfer surplus votes proportionally based on subsequent preferences
            foreach (var nextPreference in vote.Preferences.Where(p => p.Rank > firstPreference.Rank))
            {
                int candidateVotes = voteCounts.GetValueOrDefault(nextPreference.CandidateId, 0);
                double transferRatio = (double)candidateVotes / totalVotesForPreferences;
                int transferredVotes = (int)Math.Floor(surplusVotes * transferRatio);

                // Ensure that transferred votes do not exceed the available votes for the next preference
                int remainingVotes = Math.Max(0, voteCounts[nextPreference.CandidateId] - voteCounts.Values.Min()); // Can't exceed the minimum vote count among all candidates
                voteCounts[nextPreference.CandidateId] += Math.Min(transferredVotes, remainingVotes);
            }
        }
    }

    // Reduce votes for the elected candidate to the quota
    voteCounts[electedCandidateId] -= surplusVotes;
}



    
    //PV Calculation helper
    public static int CalculateVotesBasedOnRank(int rank, int totalCandidates)
    {
        // Assign votes inversely proportional to rank
        return totalCandidates - rank + 1;
    }
    public static void ElectCandidatesPV(Dictionary<int, int> voteCounts, List<CandidateModel> candidates, HashSet<int> electedCandidates)
    {
        while (true)
        {
            int totalVotes = voteCounts.Values.Sum();

            // Check if voteCounts is empty
            if (!voteCounts.Any())
            {
                break;
            }

            // If there is only one candidate left, declare them as the winner
            if (candidates.Count == 1)
            {
                var lastCandidateId = candidates[0].Id;
                electedCandidates.Add(lastCandidateId);
                candidates[0].IsElected = true;
                break;
            }

            // Find the candidate(s) with the most votes
            var maxVoteCount = voteCounts.Values.Max();
            var candidatesWithMaxVotes = voteCounts.Where(kv => kv.Value == maxVoteCount).Select(kv => kv.Key).ToList();

            if (maxVoteCount > totalVotes / 2)
            {
                // Declare the candidate(s) with the simple majority as winner(s)
                foreach (var candidateId in candidatesWithMaxVotes)
                {
                    electedCandidates.Add(candidateId);
                    candidates.First(c => c.Id == candidateId).IsElected = true;
                }
                break;
            }

            // Find the candidate with the fewest votes
            var minVoteCount = voteCounts.Values.Min();
            var candidateWithMinVotes = voteCounts.Where(kv => kv.Value == minVoteCount).Select(kv => kv.Key).First();

            // Eliminate the candidate with the fewest votes
            candidates.RemoveAll(c => c.Id == candidateWithMinVotes);
            voteCounts.Remove(candidateWithMinVotes);
        }
    }






}