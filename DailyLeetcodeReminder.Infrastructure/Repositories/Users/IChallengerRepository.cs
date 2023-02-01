﻿using DailyLeetcodeReminder.Domain.Entities;

namespace DailyLeetcodeReminder.Infrastructure.Repositories;

public interface IChallengerRepository
{
    Task<Challenger> SelectUserByTelegramIdAsync(long telegramId);
    Task<Challenger> SelectUserByLeetcodeUsernameAsync(string leetcodeUsername);
    Task<Challenger> InsertChallengerAsync(Challenger challenger);
    Task UpdateChallengerAsync(Challenger challenger);
}