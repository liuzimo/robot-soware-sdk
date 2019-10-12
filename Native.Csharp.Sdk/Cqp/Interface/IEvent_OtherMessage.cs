
using Native.Csharp.Sdk.Cqp.EventArgs;

namespace Native.Csharp.Sdk.Cqp.Interface
{
	/// <summary>
	/// 酷Q 其它事件处理接口
	/// </summary>
	public interface IEvent_OtherMessage
	{
		/// <summary>
		/// Type=21 在线状态消息<para/>
		/// 当在派生类中重写时, 处理收到的在线状态消息
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		void ReceiveOnlineStatusMessage (object sender, CqPrivateMessageEventArgs e);
	}
}
