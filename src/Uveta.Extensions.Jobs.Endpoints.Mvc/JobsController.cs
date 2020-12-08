using System;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc
{
    [Route("jobs/[controller]")]
    [ApiController]
    public class JobsController<TEndpoint, TInput, TOutput> : ControllerBase
        where TEndpoint : IEndpoint<TInput, TOutput>
        where TOutput : class
    {
        private readonly Endpoint<TEndpoint, TInput, TOutput> _jobEndpoint;
        private readonly LinkGenerator _linkGenerator;

        public JobsController(
            Endpoint<TEndpoint, TInput, TOutput> jobEndpoint,
            LinkGenerator linkGenerator)
        {
            _jobEndpoint = jobEndpoint;
            _linkGenerator = linkGenerator;
        }

        [HttpPost]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Job>> CreateJob(
            [FromBody] TInput request,
            CancellationToken cancel)
        {
            var job = await _jobEndpoint.RequestNewJobAsync(request, cancel);
            if (job is null) return BadRequest();

            string location = _linkGenerator.GetUriByAction(
                httpContext: HttpContext,
                action: nameof(GetJobStatus),
                controller: ControllerContext.ActionDescriptor.ControllerName,
                values: new { id = job.Header.Identifier.Id });

            return Accepted(location);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(303)]
        public async Task<ActionResult<Job>> GetJobStatus(
            [FromRoute(Name = "id")] string id,
            CancellationToken cancel)
        {
            var job = await _jobEndpoint.GetJobAsync(id, cancel);
            if (job is null) return NotFound();
            if (job.Header.State.IsSuccess())
            {
                return SeeOther(
                    action: nameof(GetJobOutput),
                    controller: ControllerContext.ActionDescriptor.ControllerName,
                    values: new { id = job.Header.Identifier.Id });
            }
            else
            {
                return Ok(job);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelJob(
            [FromRoute(Name = "id")] string id,
            CancellationToken cancel)
        {
            await _jobEndpoint.DeleteJobAsync(id, cancel);
            return Ok();
        }

        [HttpGet("{id}/output")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TOutput>> GetJobOutput(
            [FromRoute(Name = "id")] string id,
            CancellationToken cancel)
        {
            var output = await _jobEndpoint.GetJobOutputAsync(id, cancel);
            if (output is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(output);
            }
        }

        [HttpDelete("{id}/output")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteJobOutput(
            [FromRoute(Name = "id")] string id,
            CancellationToken cancel)
        {
            await _jobEndpoint.DeleteJobAsync(id, cancel);
            return Ok();
        }

        [NonAction]
        private ActionResult SeeOther(string action, string controller, object values)
        {
            string location = _linkGenerator.GetUriByAction(
                httpContext: HttpContext,
                action: action,
                controller: controller,
                values: values);
            HttpContext.Response.GetTypedHeaders().Location = new Uri(location);
            return StatusCode(StatusCodes.Status303SeeOther);
        }
    }
}