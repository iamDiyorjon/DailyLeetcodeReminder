﻿using DailyLeetcodeReminder.Domain.Entities;
using DailyLeetcodeReminder.Infrastructure.Models;
using DailyLeetcodeReminder.Infrastructure.Repositories;
using DailyLeetcodeReminder.Infrastructure.Services;

namespace DailyLeetcodeReminder.Application.Services;

public class ChallengerService : IChallengerService
{
    private readonly IChallengerRepository challengerRepository;
    private readonly ILeetCodeBroker leetcodeBroker;
    private const short maxAttempts = 3;

    public ChallengerService(
        IChallengerRepository userRepository,
        ILeetCodeBroker leetcodeBroker)
    {
        this.challengerRepository = userRepository;
        this.leetcodeBroker = leetcodeBroker;
    }

    public async Task<Challenger> AddUserAsync(Challenger challenger)
    {
        challenger.Attempts = maxAttempts;

        UserProfile userProfile = await leetcodeBroker
            .GetUserProfile(challenger.LeetcodeUserName);

        SubmissionNumber? totalSubmission = userProfile
            .SubmitStatistics
            .Submissions
            .Where(s => s.Difficulty == "All")
            .FirstOrDefault();

        if(totalSubmission is null)
        {
            throw new Exception("Unable to retrieve total solved problems");
        }

        challenger.TotalSolvedProblems = totalSubmission.Count;

        Challenger insertedChallenger = await this.challengerRepository
            .InsertChallengerAsync(challenger);

        return insertedChallenger;
    }

    public async Task<Challenger> RetrieveChallengerByTelegramIdAsync(long telegramId)
    {
        Challenger storageChallenger = await this.challengerRepository
            .SelectUserByTelegramIdAsync(telegramId);

        if(storageChallenger is null)
        {
            throw new Exception("Challenger is not found");
        }

        return storageChallenger;
    }

    public async Task<Challenger> RetrieveChallengerByLeetcodeUsernameAsync(string leetcodeUsername)
    {
        Challenger storageChallenger = await this.challengerRepository
            .SelectUserByLeetcodeUsernameAsync(leetcodeUsername);

        if (storageChallenger is null)
        {
            throw new Exception("Challenger is not found");
        }

        return storageChallenger;
    }

    public async Task<Challenger> ModifyChallengerAsync(Challenger challenger)
    {
        Challenger storageChallenger = await this.challengerRepository
            .SelectUserByTelegramIdAsync(challenger.TelegramId);

        if (storageChallenger is null)
        {
            throw new Exception("Challenger not found");
        }

        storageChallenger.FirstName = challenger.FirstName;
        storageChallenger.LastName = challenger.LastName;
        storageChallenger.TotalSolvedProblems = challenger.TotalSolvedProblems;
        storageChallenger.Attempts = challenger.Attempts;
        storageChallenger.Status = challenger.Status;

        await this.challengerRepository.UpdateChallengerAsync(storageChallenger);

        return storageChallenger;
    }
}