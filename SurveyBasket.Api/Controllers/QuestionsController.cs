using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;


    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromRoute]int pollId, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAllAsync(pollId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }


    [HttpGet("{questionId:int}")]
    public async Task<IActionResult> GetById([FromRoute]int pollId, [FromRoute]int questionId, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAsync(pollId, questionId, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }


    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute]int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.AddAsync(pollId, request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { pollId,result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{questionId:int}")]
    public async Task<IActionResult> Update([FromRoute]int pollId, [FromRoute] int questionId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(pollId, questionId, request, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    [HttpPut("{questionId:int}/toggle-status")]
    public async Task<IActionResult> ToggleStatus([FromRoute]int pollId, [FromRoute]int questionId, CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggelStatusAsync(pollId, questionId, cancellationToken);

        return result.IsSuccess
            ? NoContent() 
            : result.ToProblem();
    }
}
