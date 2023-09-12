using CareerHub.Domain.Exceptions;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Text.Json;

namespace CareerHub.gRPC.Extensions;

public class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;
    private readonly string _correlationId;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
        _correlationId = Guid.NewGuid().ToString();
    }

    #region gRPC All Method Interceptors
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception e)
        {
            HandleError(e);
            throw;
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(requestStream, context);
        }
        catch (Exception e)
        {
            HandleError(e);
            throw;
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context);
        }
        catch (Exception e)
        {
            HandleError(e);
            throw;
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (Exception e)
        {
            HandleError(e);
            throw;
        }
    }

    #endregion

    #region gRPC Exception Handler
    private void HandleError(Exception exception)
    {
        _logger.LogError(exception, "CorrelationId: {CorrelationId} - An error occurred", _correlationId);

        var status = MapExceptionToStatus(exception);
        var trailers = CreateTrailers(exception, _correlationId);

        throw new RpcException(status, trailers);
    }

    private static Status MapExceptionToStatus(Exception exception)
    {
        return exception switch
        {
            TimeoutException => new Status(StatusCode.ResourceExhausted, "An external resource did not answer within the time limit"),
            ValidationException => new Status(StatusCode.InvalidArgument, "One or more validation failures occurred"),
            UnauthorizedAccessException => new Status(StatusCode.Unauthenticated, exception.Message ?? "You do not have access to this resource"),
            BadRequestException => new Status(StatusCode.Unavailable, exception.Message),
            NotFoundException => new Status(StatusCode.NotFound, exception.Message),
            _ => new Status(StatusCode.Internal, "Internal Server Error")
        };
    }

    private static Metadata CreateTrailers(Exception exception, string correlationId)
    {
        var trailers = new Metadata
            {
                { "CorrelationId", correlationId }
            };

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => JsonNamingPolicy.CamelCase.ConvertName(group.Key) ?? group.Key,
                    group => group.Select(e => e.ErrorMessage).ToArray());

            trailers.Add("ValidationErrors", JsonSerializer.Serialize(errors));
        }

        return trailers;
    }

    #endregion
}
