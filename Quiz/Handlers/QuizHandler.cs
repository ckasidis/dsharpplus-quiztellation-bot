using DSharpPlus;
using DSharpPlus.Entities;
using Quiztellation.Quiz.Questions;

namespace Quiztellation.Quiz.Handlers;

public class QuizHandler
{
    private readonly DiscordChannel _channel;
    private readonly DiscordClient _client;
    private readonly DiscordUser _user;
    private int _currentQuestion;
    private int _currentScore;
    private readonly List<IQuestion> _questions = new();
    private int _totalQuestion;
    private int _totalScore;

    public QuizHandler(DiscordClient client, DiscordChannel channel, DiscordUser user)
    {
        _client = client;
        _channel = channel;
        _user = user;
    }

    public void AddQuestion(IQuestion question)
    {
        _questions.Add(question);
        _totalQuestion += 1;
        _totalScore += question.Point;
    }

    public async Task ProcessQuiz()
    {
        foreach (var question in _questions)
        {
            _currentQuestion++;

            var quizProgressEmbed = new DiscordEmbedBuilder
            {
                Title = $"Question {_currentQuestion}/{_totalQuestion}",
                Color = DiscordColor.Azure
            };
            await _channel.SendMessageAsync(quizProgressEmbed);

            var exit = await question.ProcessQuestion(_client, _channel, _user);

            if (exit)
            {
                var quizExitEmbed = new DiscordEmbedBuilder
                {
                    Title = "Quiz Ended",
                    Description = "You have exited the Quiz",
                    Color = DiscordColor.Red
                };
                await _channel.SendMessageAsync(quizExitEmbed);

                return;
            }

            _currentScore += question.Point;
        }

        var quizResultEmbed = new DiscordEmbedBuilder
        {
            Title = "Quiz Completed",
            Color = DiscordColor.Green
        };
        quizResultEmbed.AddField("Your Score", $"{_currentScore}/{_totalScore}");
        await _channel.SendMessageAsync(quizResultEmbed);
    }
}