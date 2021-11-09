using System;
using Microsoft.AspNetCore.Mvc;
using Models.Messages;

namespace Services.Infrastructure.Extensions
{
    public static class ControllerBaseExtension
    {
        public static BaseResponse<T> GenerateResponse<T>(this ControllerBase controllerBase, T responseObj, int statusCode = 200, string message = "")
        {
            return new BaseResponse<T>
            {
                Result = responseObj,
                StatusCode = statusCode,
                Msg = message,
            };
        }
    }
}
