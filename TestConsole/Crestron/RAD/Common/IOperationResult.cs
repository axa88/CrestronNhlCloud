// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Interfaces.ExtensionDevice
{
	/// <summary>
	/// Defines a result for an operation on an <see cref="IExtensionDevice"/>.
	/// </summary>
	public interface IOperationResult
	{
		/// <summary>
		/// Indicates the result of the operation.
		/// </summary>
		OperationResultCode ResultCode { get; }

		/// <summary>
		/// Message to be displayed to the user providing more detailed information about the result.
		/// </summary>
		string UserMessage { get; }
	}

	/// <summary>
	/// The result of the operation.
	/// </summary>
	public enum OperationResultCode
	{
		/// <summary>
		/// The result has not been initialized.
		/// </summary>
		Uninitialized,

		/// <summary>
		/// The operation has succeeded.
		/// </summary>
		Success,

		/// <summary>
		/// The operation resulted in an error.
		/// </summary>
		Error
	}
}