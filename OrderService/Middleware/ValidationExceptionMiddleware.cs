using FluentValidation;

namespace OrderService.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ValidationExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
        }
    }
}
