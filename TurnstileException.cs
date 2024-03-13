namespace TurnstileTag;


/// <summary>
/// The <see cref="Exception"/> that is thrown when the turnstile validation fails.
/// </summary>
public class TurnstileException : Exception
{
  /// <summary>
  /// Creates a new instance of <see cref="TurnstileException"/> with the specified exception message.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public TurnstileException(string message) : base(message) { }

  /// <summary>
  /// Creates a new instance of <see cref="TurnstileException"/> with the specified exception message and inner exception.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  /// <param name="innerException">The inner <see cref="Exception"/>.</param>
  public TurnstileException(string message, Exception innerException) : base(message, innerException) { }
}