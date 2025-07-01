﻿

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
        var polls = _pollService.GetAll();
        var response=polls.Adapt<IEnumerable<PollResponse>>();
        return Ok(response);
    }


    [HttpGet("{id:int}")]
    public IActionResult GetByID([FromRoute] int id)
    {
        var poll = _pollService.Get(id);

        if (poll is null)
            return NotFound();

        var response =poll.Adapt<PollResponse>();
        
        return Ok(response);
        
    }

    [HttpPost("")]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        //var validationResult = validator.Validate(request);
        //if (!validationResult.IsValid)
        //{
        //    var modelstate = new ModelStateDictionary();
        //    validationResult.Errors.ForEach(error => modelstate.AddModelError(error.PropertyName, error.ErrorMessage));

        //    return ValidationProblem(modelstate);
        //}
        var newPoll = _pollService.Add(request.Adapt<Poll>());

        return CreatedAtAction(nameof(GetByID), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] CreatePollRequest request)
    {
        var isUpdated = _pollService.Update(id, request.Adapt<Poll>());

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var isDeleted = _pollService.Delete(id);
        if(!isDeleted)
            return NotFound();
        return NoContent();
    }


    [HttpGet("test")]
    public IActionResult Test([FromQuery] int id)
    {
        return Ok(id);
    }
}
