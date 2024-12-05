﻿using ChatService.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Extensions;

public static class ErrorMappingExtensions
{
    public static IActionResult ToActionResult(this Error error)
    {
        return error.code switch
        {
            HttpResponseStatusCodes.NotFound => new NotFoundObjectResult(error),
            HttpResponseStatusCodes.BadRequest => new BadRequestObjectResult(error),
            HttpResponseStatusCodes.Conflict => new ConflictObjectResult(error),
            HttpResponseStatusCodes.InternalServerError => new ObjectResult(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            },
            _ => new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}
