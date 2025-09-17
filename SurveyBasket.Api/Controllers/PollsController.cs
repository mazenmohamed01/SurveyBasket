


using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;



    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetAllAsync(cancellationToken));
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetCurrentAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByID([FromRoute] int id,CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : result.ToProblem(); // Return 404 if poll not found or any other error
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CreatePollRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByID), new { id = result.Value.Id }, result.Value) 
            : result.ToProblem(); // Return 400 Bad Request if there was an error, duplicated title
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePollRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request,cancellationToken);

        return result.IsSuccess
            ? NoContent() // Return 204 No Content if update is successful
            : result.ToProblem(); // Return 404 if poll not found or update failed
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id,CancellationToken cancellationToken )
    {
        var result = await _pollService.DeleteAsync(id,cancellationToken);
        
        return result.IsSuccess 
            ? NoContent() // Return 204 No Content if delete is successful
            : result.ToProblem();
    }


    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess 
            ? NoContent() // Return 204 No Content if toggle is successful
            : result.ToProblem();
    }
}
