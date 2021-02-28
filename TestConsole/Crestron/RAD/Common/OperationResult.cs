using Crestron.RAD.Common.Interfaces.ExtensionDevice;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.DeviceTypes.ExtensionDevice
{
	/// <summary>
	/// Defines a result for an operation on an <see cref="IExtensionDevice"/>.
	/// </summary>
	public class OperationResult : IOperationResult
	{
		/// <summary>
		/// Create an operation result.
		/// </summary>
		public OperationResult(OperationResultCode resultCode)
		{
			ResultCode = resultCode;
		}

		/// <summary>
		/// Create an operation result with a message.
		/// </summary>
		public OperationResult(OperationResultCode resultCode, string message)
		{
			ResultCode = resultCode;
			UserMessage = message;
		}

		/// <summary>
		/// Indicates the result of the operation.
		/// </summary>
		public OperationResultCode ResultCode { get; private set; }

		/// <summary>
		/// Message to be displayed to the user providing more detailed information about the result.
		/// </summary>
		public string UserMessage { get; private set; }
	}
}
