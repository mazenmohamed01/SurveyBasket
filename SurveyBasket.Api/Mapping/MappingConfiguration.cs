using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(a => new Answer { Content = a }));

        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

    }
}
